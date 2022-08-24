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
        SpawnCards();
        UnitSelectionUI.OnAnyPieceAddSelected += UnitSelectionUI_OnAnyPieceAddSelected;
    }

    void SpawnCards()
    {
        for (int i = 0; i < maxCardCount; i++)
        {
            GameObject unitSelectionUIGameObject = Instantiate(unitSelectionUIPrefab, this.transform);
            unitSelectionUIRectTransform.Add(unitSelectionUIGameObject.GetComponent<RectTransform>());
            
            unitSelectionUIRectTransform[i].anchoredPosition = new Vector2(-300, 100 - (100 * i));
            
            UnitSelectionUI unitSelectionUI = unitSelectionUIRectTransform[i].GetComponent<UnitSelectionUI>();
            unitSelectionUI.UpdatePiece(PieceDatabase.Instance.GetRandomPieceOfTier(i + 1));
        }
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
                for (int i = 0; i < maxCardCount; i++)
                {
                    if (timer - buffer * i < maxTimer)
                    {
                        timer += Time.deltaTime;
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
                for (int i = 0; i < maxCardCount; i++)
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
                        timer += Time.deltaTime;
                        unitSelectionUIRectTransform[i].anchoredPosition = new Vector2(
                            initalPositions.x + (-initalPositions.x * animCurve.Evaluate((maxTimer - (timer - buffer * i)) / maxTimer)), 
                            unitSelectionUIRectTransform[i].anchoredPosition.y);
                    }
                }
                if (timer - buffer * (maxCardCount - 1) > maxTimer)
                {
                    OnUnitSelectionFinished?.Invoke(this, EventArgs.Empty);
                    state = State.Inactive;
                }
                break;

        }
    }

    private void UnitSelectionUI_OnAnyPieceAddSelected(object sender, EventArgs e)
    {
        UnitSelectionUI unitSelectionUI = sender as UnitSelectionUI;
        RectTransform rectTransform = unitSelectionUI.GetComponent<RectTransform>();

        index = unitSelectionUIRectTransform.IndexOf(rectTransform);
        state = State.Ending; 
        timer = 0;
    }
}
