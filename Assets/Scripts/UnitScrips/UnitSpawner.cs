using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSpawner : MonoBehaviour
{
    public static UnitSpawner Instance;

    public static event EventHandler<SpawnEventArgs> OnAnySpawningStarted; 
    public event EventHandler OnSpawningFinished;

    [System.Serializable]
    public struct UnitPrefabPair
    {
        public GameObject unitPrefab;
        public PieceBase piece;
    }

    enum State
    {
        Inactive,
        Idle,
        Spawning, // The pawn row
    }

    public class SpawnEventArgs : EventArgs
    {
        public PieceBase piece;
    }

    [SerializeField] List<UnitPrefabPair> unitPrefabPairs;
    [SerializeField] GameObject lightBeamPrefab;
    
    [SerializeField] AudioClip lightBeamSound;

    [SerializeField] float cooldownTimer;
    [SerializeField] float unitSpawnTimeAddition;
    [SerializeField] float firstRowTimer;
    [SerializeField] float firstRowBuffer;

    bool isActive;
    float timer;
    float maxTimer;
    float buffer;
    int index;
    int finishedSpawningCounter;

    List<UnitStartingPlacement> startingPlacementsList = new List<UnitStartingPlacement>();

    State state = State.Inactive;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one UnitSpawner! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        //CameraController.Instance.OnCombatSceneEnter += CameraController_OnCombatSceneEnter;
        TransitionAnimation.Instance.OnStartingAnimationFinished += TransitionAnimation_OnStartingAnimationFinished;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return; 

        timer += Time.deltaTime;
        switch (state)
        {
            case State.Inactive:
                break;
            case State.Idle:
                if (timer > cooldownTimer)
                {
                    state = State.Spawning;
                    maxTimer = startingPlacementsList.Count * unitSpawnTimeAddition;
                    buffer = firstRowBuffer;
                    index = 0;
                }
                break;
            case State.Spawning:
                if (timer - (unitSpawnTimeAddition * index) > maxTimer)
                {
                    if (index < startingPlacementsList.Count)
                    {
                        PieceBase unitPiece = startingPlacementsList[index].piece;
                        GameObject newUnitPrefab = GetPiecePrefab(unitPiece);
                        Vector3 spawnPosition = LevelGrid.Instance.GetWorldPosition(startingPlacementsList[index].gridPosition);

                        GameObject spawnBeamGameObject = Instantiate(lightBeamPrefab, spawnPosition, Quaternion.identity);
                        SpawningLightBeam lightBeam = spawnBeamGameObject.GetComponent<SpawningLightBeam>();

                        SFXPlayer.PlaySound(lightBeamSound);
                        lightBeam.Initialize(unitPiece, new Action(() => {
                            Instantiate(newUnitPrefab, spawnPosition, Quaternion.identity);
                            OnAnySpawningStarted?.Invoke(this, new SpawnEventArgs{
                                piece = unitPiece
                            });
                            finishedSpawningCounter++;
                        }));
                        
                        index++;
                    }
                }
                if (finishedSpawningCounter >= startingPlacementsList.Count)
                {
                    state = State.Inactive;
                    isActive = false;
                    OnSpawningFinished(this, EventArgs.Empty);
                }
                break;
        }
    }

    private void HandleLightBeamSpawn(List<UnitStartingPlacement> uspList)
    {
        if (timer > (maxTimer - (buffer * (uspList.Count - index))))
        {
            PieceBase unitPiece = uspList[index].piece;
            GameObject newUnitPrefab = GetPiecePrefab(unitPiece);
            Vector3 spawnPosition = LevelGrid.Instance.GetWorldPosition(uspList[index].gridPosition);

            GameObject spawnBeamGameObject = Instantiate(lightBeamPrefab, spawnPosition, Quaternion.identity);
            SpawningLightBeam lightBeam = spawnBeamGameObject.GetComponent<SpawningLightBeam>();

            lightBeam.Initialize(unitPiece, new Action(() => {
                Instantiate(newUnitPrefab, spawnPosition, Quaternion.identity);
            }));
            index++;
        }
    }

    GameObject GetPiecePrefab(PieceBase piece)
    {
        foreach (UnitPrefabPair upp in unitPrefabPairs)
        {
            if (upp.piece == piece)
            {
                return upp.unitPrefab;
            }
        }
        return null;
    }

    void BeginSpawning()
    {
        List<UnitStartingPlacement> unitStartingPlacements = UnitStartingPositionUI.Instance.GetStartingPlacements();
        List<UnitStartingPlacement> firstSetStartingPlacements = new List<UnitStartingPlacement>();
        List<UnitStartingPlacement> secondSetStartingPlacements = new List<UnitStartingPlacement>();

        foreach (UnitStartingPlacement usp in unitStartingPlacements)
        {
            if (usp.gridPosition.z == 1)
            {
                firstSetStartingPlacements.Add(usp);
            }
            else
            {
                secondSetStartingPlacements.Add(usp);
            }
        }

        startingPlacementsList.Clear();

        for (int i = 0; i < firstSetStartingPlacements.Count; i++)
        {
            startingPlacementsList.Add(firstSetStartingPlacements[i]);
        }

        for (int j = 0; j < secondSetStartingPlacements.Count; j++)
        {
            startingPlacementsList.Add(secondSetStartingPlacements[j]);
        }

        timer = 0;
        finishedSpawningCounter = 0;
        isActive = true;
        state = State.Idle;
    }

    void CameraController_OnCombatSceneEnter(object sender, EventArgs e)
    {
        BeginSpawning();
    }

    void TransitionAnimation_OnStartingAnimationFinished(object sender, EventArgs e)
    {
        Debug.Log("UnitSpawner.cs  startingAnimationFinished");
        BeginSpawning();
    }
}
