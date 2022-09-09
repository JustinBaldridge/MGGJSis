using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreCombatMenuUI : MonoBehaviour
{
    public static PreCombatMenuUI Instance;
    [SerializeField] AnimationCurve entranceAnimCurve;
    [SerializeField] AnimationCurve exitAnimCurve;
    [SerializeField] float maxTimer;
    RectTransform rectTransform;
    Vector2 onScreenPosition = new Vector2(400, 0);
    Vector2 offScreenPosition = new Vector2(-100, 0);
    Vector2 targetPosition;

    AnimationCurve animCurve;

    bool isActive;
    Vector2 distance;
    Vector2 currentPosition; 
    float timer;
    
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one PreCombatMenuUI! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        rectTransform = GetComponent<RectTransform>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        ContinueArrow.OnAnyContinue += ContinueArrow_OnAnyContinue;
        UnitSelectionSpawner.Instance.OnUnitSelectionFinished += UnitSelectionSpawner_OnUnitSelectionFinished;
    }

    void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        rectTransform.anchoredPosition = new Vector2(currentPosition.x - (distance.x * animCurve.Evaluate(timer / maxTimer)), 0); //Vector2.Lerp(rectTransform.anchoredPosition, targetPosition, Time.deltaTime);

        if (timer > maxTimer)
        {
            rectTransform.anchoredPosition = targetPosition;
            isActive = false;
        }
    }
    void ContinueArrow_OnAnyContinue(object sender, EventArgs e)
    {
        isActive = true;
        targetPosition = offScreenPosition;
        animCurve = exitAnimCurve;
        currentPosition = rectTransform.anchoredPosition;
        distance = -(targetPosition - rectTransform.anchoredPosition);
        timer = 0;
    }

    void UnitSelectionSpawner_OnUnitSelectionFinished(object sender, EventArgs e)
    {  
        isActive = true;
        targetPosition = onScreenPosition;
        animCurve = entranceAnimCurve;
        currentPosition = rectTransform.anchoredPosition;
        distance = -(targetPosition - rectTransform.anchoredPosition);
        timer = 0;
    }
}
