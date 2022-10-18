using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PieceInfoUI : MonoBehaviour
{
    public static PieceInfoUI Instance;

    [SerializeField] float targetX;
    [SerializeField] Image exampleImage;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;

    [SerializeField] float maxTimer;
    [SerializeField] Vector2 onScreenPosition;  
    [SerializeField] AnimationCurve animCurve; 

    RectTransform rectTransform;
    Vector2 currentPosition;
    Vector2 targetPosition;

    float timer;
    
    Vector2 distance;
    bool isActive;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one PieceInfoUI! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        rectTransform = GetComponent<RectTransform>();
    }

    void Start()
    {
        StartMenu.Instance.OnStartGame += StartMenu_OnStartGame;
        UnitManager.Instance.OnKingTaken += UnitManager_OnKingTaken;
        CameraController.Instance.OnFinaleCinimaticStart += CameraController_OnFinaleCinimaticStart;
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

    void StartMenu_OnStartGame(object sender, EventArgs e)
    {
        isActive = true;
        targetPosition = onScreenPosition;
        currentPosition = rectTransform.anchoredPosition;
        distance = -(targetPosition - rectTransform.anchoredPosition);
        timer = 0;
    }

    void UnitManager_OnKingTaken(object sender, EventArgs e)
    {
        isActive = true;
        targetPosition = Vector2.zero;
        currentPosition = rectTransform.anchoredPosition;
        distance = -(targetPosition - rectTransform.anchoredPosition);
        timer = 0;
    }

    void CameraController_OnFinaleCinimaticStart(object sender, EventArgs e)
    {
        isActive = true;
        targetPosition = Vector2.zero;
        currentPosition = rectTransform.anchoredPosition;
        distance = -(targetPosition - rectTransform.anchoredPosition);
        timer = 0;
    }

    public void UpdateDescription(PieceBase piece)
    {
        exampleImage.sprite = piece.MovementExample;
        titleText.text = piece.Title;
        descriptionText.text = piece.Description;
    }
}
