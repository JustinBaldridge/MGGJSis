using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitDespawner : MonoBehaviour
{
    public static UnitDespawner Instance;

    public event EventHandler OnDespawningFinished;

    [SerializeField] GameObject lightBeamPrefab;
    [SerializeField] AudioClip lightBeamSound;

    [SerializeField] float cooldownTimer;
    [SerializeField] float maxTimer;
    enum State 
    {
        Inactive,
        Idle,
        Despawning
    }

    State state = State.Inactive;
    bool isActive; 

    float timer;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one UnitDespawner! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UnitManager.Instance.OnAllEnemiesDefeated += UnitManager_OnAllEnemiesDefeated;
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
                    state = State.Despawning;
                    SFXPlayer.PlaySound(lightBeamSound);
                    //maxTimer = startingPlacementsList.Count * unitSpawnTimeAddition;
                    //index = 0;
                }
                break;
            case State.Despawning:
                if (timer > maxTimer)
                {
                    List<Unit> aliveUnits = UnitManager.Instance.GetFriendlyUnitList();

                    foreach (Unit unit in aliveUnits)
                    {
                        PieceBase unitPiece = unit.GetUnitAction().GetPiece();
                        Vector3 spawnPosition = LevelGrid.Instance.GetWorldPosition(unit.GetGridPosition());

                        GameObject spawnBeamGameObject = Instantiate(lightBeamPrefab, spawnPosition, Quaternion.identity);
                        SpawningLightBeam lightBeam = spawnBeamGameObject.GetComponent<SpawningLightBeam>();

                        lightBeam.Initialize(unitPiece, new Action(() => {
                            //Destroy(unit.gameObject);
                            unit.gameObject.SetActive(false);
                        }));
                    }

                    state = State.Inactive;
                    isActive = false;
                    OnDespawningFinished?.Invoke(this, EventArgs.Empty);
                }
                break;
        }
    }

    private void UnitManager_OnAllEnemiesDefeated(object sender, EventArgs e)
    {
        state = State.Idle;
        timer = 0;
        isActive = true;
    }
}
