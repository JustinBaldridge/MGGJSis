using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UnitSelectionSpawner : MonoBehaviour
{
    public static UnitSelectionSpawner Instance;

    public event EventHandler OnUnitSelectionFinished;

    [SerializeField] GameObject unitSelectionUIPrefab;
    [SerializeField] AnimationCurve animCurve;

    [SerializeField] float buffer = 0.5f;
    [SerializeField] float maxTimer = 4f;
    List<RectTransform> unitSelectionUIRectTransform = new List<RectTransform>();
    float maxCardCount = 3;

    bool isActive = true;
    float timer;
    float altTimer;

    int index;

    Vector2 initalPositions = new Vector2(-300, 100);

    enum State {
        Inactive,
        Begin,
        ActivateCards,
        Ending,
    }

    State state = State.Inactive;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one UnitSelectionSpawner! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    
    void Start()
    {
        //SpawnCards();
        UnitSelectionUI.OnAnyPieceAddSelected += UnitSelectionUI_OnAnyPieceAddSelected;
        CameraController.Instance.OnInventorySceneEnter += CameraController_OnInventorySceneEnter;
    }

    public void SpawnCards()
    {
        // Get the highest tier
        int baseHighest = 2;
        int highestTier = baseHighest + GameProgression.Instance.GetDifficultyStage();

        // Get pieces to filter out
        List<PieceBase> ownedPieces = PieceInventory.Instance.GetOwnedPieces();

        List<PieceBase> spawningPiece = new List<PieceBase>();
        for (int i = 0; i < maxCardCount; i++)
        {
            GameObject unitSelectionUIGameObject = Instantiate(unitSelectionUIPrefab, this.transform);
            unitSelectionUIRectTransform.Add(unitSelectionUIGameObject.GetComponent<RectTransform>());
            
            unitSelectionUIRectTransform[i].anchoredPosition = new Vector2(-300, 100 - (100 * i));
            
            UnitSelectionUI unitSelectionUI = unitSelectionUIRectTransform[i].GetComponent<UnitSelectionUI>();

            PieceBase spawnPiece = PieceDatabase.Instance.GetRandomPieceOfTier(UnityEngine.Random.Range(0, highestTier + 1));

            while (ownedPieces.Contains(spawnPiece) || spawningPiece.Contains(spawnPiece))
            {
                spawnPiece = PieceDatabase.Instance.GetRandomPieceOfTier(UnityEngine.Random.Range(0, highestTier + 1));
            }
            spawningPiece.Add(spawnPiece);
            unitSelectionUI.UpdatePiece(spawnPiece);
        }
        timer = 0;
        state = State.Begin;
    }

    void Update()
    {
        switch (state)
        {
            case State.Inactive:
                return;
                //break;
            case State.Begin:
                timer += Time.deltaTime;
                for (int i = 0; i < maxCardCount; i++)
                {
                    if (timer - buffer * i < maxTimer)
                    {
                        
                        unitSelectionUIRectTransform[i].anchoredPosition = new Vector2(
                            initalPositions.x + (-initalPositions.x * animCurve.Evaluate((timer - buffer * i) / maxTimer)), 
                            unitSelectionUIRectTransform[i].anchoredPosition.y);
                    }
                }

                if (timer - buffer * (maxCardCount - 1) > maxTimer) state = State.ActivateCards;
                break;
            case State.ActivateCards:
                foreach (RectTransform rectTransform in unitSelectionUIRectTransform)
                {
                    UnitSelectionUI unitSelectionUI = rectTransform.GetComponent<UnitSelectionUI>();
                    unitSelectionUI.ActivateButton(() => {
                        //Debug.Log("UnitSelectionSpawner.cs  Taking " + unitSelectionUI.GetPiece().Name);
                        unitSelectionUI.SelectPiece();
                    });
                }
                state = State.Inactive;
                break;
            case State.Ending:
                timer += Time.deltaTime;
                for (int i = 0; i < unitSelectionUIRectTransform.Count; i++)
                {
                    if (i == index) 
                    {
                        //Image image = unitSelectionUIRectTransform[i].GetComponent<Image>();

                        float timerInt = (float) (int) timer;
                        //image.enabled = 
                        unitSelectionUIRectTransform[i].gameObject.SetActive((timer - timerInt) >= .35f);
                        continue;
                    }
                    if (timer - buffer * i < maxTimer)
                    {
                        
                        unitSelectionUIRectTransform[i].anchoredPosition = new Vector2(
                            initalPositions.x + (-initalPositions.x * animCurve.Evaluate((maxTimer - (timer - buffer * i)) / maxTimer)), 
                            unitSelectionUIRectTransform[i].anchoredPosition.y);
                    }
                }
                if (timer - buffer * (maxCardCount - 1) > maxTimer)
                {
                    OnUnitSelectionFinished?.Invoke(this, EventArgs.Empty);
                    foreach (RectTransform rt in unitSelectionUIRectTransform)
                    {
                        Destroy(rt.gameObject);
                    }
                    unitSelectionUIRectTransform.Clear();
                    state = State.Inactive;
                }
                break;

        }
    }

    private void UnitSelectionUI_OnAnyPieceAddSelected(object sender, EventArgs e)
    {
        UnitSelectionUI unitSelectionUI = sender as UnitSelectionUI;
        RectTransform rectTransform = unitSelectionUI.GetComponent<RectTransform>();

        for (int i = 0; i < unitSelectionUIRectTransform.Count; i++)
        {
            UnitSelectionUI allUnitSelectionUI = unitSelectionUIRectTransform[i].GetComponent<UnitSelectionUI>();
            //allUnitSelectionUI.DisableButtons();
        }

        index = unitSelectionUIRectTransform.IndexOf(rectTransform);
        state = State.Ending; 
        timer = 0;
    }

    private void CameraController_OnInventorySceneEnter(object sender, EventArgs e)
    {
        SpawnCards();
    }
}
