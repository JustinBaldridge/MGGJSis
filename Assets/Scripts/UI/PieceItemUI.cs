using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class PieceItemUI : MonoBehaviour, IPointerEnterHandler
{
    public static event EventHandler OnAnyPieceItemSelected;

    [SerializeField] Image pieceImage;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI pieceTitle;
    [SerializeField] List<Image> starIndicators;
    [SerializeField] AudioClip selectedSound;

    PieceBase piece;

    void Awake()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() => {
            SFXPlayer.PlaySound(selectedSound);
            OnAnyPieceItemSelected?.Invoke(this, EventArgs.Empty);
        });
    }

    public void SetPiece(PieceBase piece)
    {
        this.piece = piece;
        pieceImage.sprite = piece.Sprite;
        characterName.text = piece.Name;
        pieceTitle.text = piece.Title;

        for (int i = 0; i < starIndicators.Count; i++)
        {
            if (i < piece.Tier)
            {
                starIndicators[i].enabled = true;
                continue;
            }
            starIndicators[i].enabled = false;
        }
    }

    public PieceBase GetPiece()
    {
        return piece;
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        PieceInfoUI.Instance.UpdateDescription(piece);
    }
}
