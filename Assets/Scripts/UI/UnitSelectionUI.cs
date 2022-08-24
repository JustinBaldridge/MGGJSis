using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UnitSelectionUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public static event EventHandler OnAnyPieceAddSelected;

    [SerializeField] PieceBase piece;

    [SerializeField] Button unitSelectionButton;
    [SerializeField] RectTransform graphicTransform;  

    [SerializeField] Sprite starSprite;

    [Header ("Visual UI Elements")]
    [SerializeField] Image pieceSprite;
    [SerializeField] TextMeshProUGUI nameText;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] List<Image> tierStarSprites;

    [SerializeField] AnimationCurve animCurve;

    Vector2 targetPosition;

    float timer;
    float timerMax = .4f;
    float speed = 8.5f;

    // Start is called before the first frame update
    void Start()
    {
        unitSelectionButton.interactable = false;
        UpdatePiece(piece);
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (timer < timerMax)
        {
            timer += Time.deltaTime;
            graphicTransform.anchoredPosition = Vector2.Lerp(graphicTransform.anchoredPosition, targetPosition, Time.deltaTime * speed);
        }
        else
        {
            graphicTransform.anchoredPosition = targetPosition;
        }
        
    }

    public void UpdatePiece(PieceBase piece)
    {
        pieceSprite.sprite = piece.Sprite;
        nameText.text = piece.Name;
        titleText.text = piece.Title;

        for (int i = 0; i < piece.Tier; i++)
        {
            tierStarSprites[i].sprite = starSprite;
        }
        this.piece = piece;
    }

    public void SelectPiece()
    {
        PieceInventory.Instance.AddPiece(piece);
        OnAnyPieceAddSelected?.Invoke(this, EventArgs.Empty);
    }

    public PieceBase GetPiece()
    {
        return piece;
    }

    public void ActivateButton(Action onButtonClick)
    {
        unitSelectionButton.interactable = true;
        unitSelectionButton.onClick.AddListener(() => onButtonClick?.Invoke());
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        targetPosition = new Vector2(20, 0);
        timer = 0f;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        targetPosition = new Vector2(0, 0);
        timer = 0f;
    }
}
