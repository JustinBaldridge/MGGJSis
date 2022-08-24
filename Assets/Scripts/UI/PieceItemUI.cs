using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PieceItemUI : MonoBehaviour
{
    public static event EventHandler OnAnyPieceItemSelected;

    [SerializeField] Image pieceImage;
    [SerializeField] TextMeshProUGUI characterName;
    [SerializeField] TextMeshProUGUI pieceTitle;
    [SerializeField] List<Image> starIndicators;

    PieceBase piece;

    void Awake()
    {
        Button btn = GetComponent<Button>();
        btn.onClick.AddListener(() => {
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
}
