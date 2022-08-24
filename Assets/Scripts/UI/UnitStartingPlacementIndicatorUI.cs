using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UnitStartingPlacementIndicatorUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private const int MAX_TIERS = 5;

    public static event EventHandler OnAnyStartingPlacementPlaced; 

    [SerializeField] Image pieceSprite;
    [SerializeField] List<Image> starIndicator;

    [SerializeField] Sprite emptyStar;
    [SerializeField] Sprite fullStar;
    [SerializeField] int maxTierForTile;

    [SerializeField] Color unavailableColor;

    PieceBase piece;
    // Start is called before the first frame update
    void Start()
    {
        Initialize();
        UpdatePlacementIndicator();
    }
    void Initialize()
    {
        for (int i = 0; i < starIndicator.Count; i++)
        {
            if (i >= maxTierForTile)
            {
                starIndicator[i].enabled = false;
                continue;
            }

            starIndicator[i].sprite = emptyStar;
        }
    }

    void UpdatePlacementIndicator()
    {
        if (piece != null)
        {
            pieceSprite.enabled = true;
            pieceSprite.sprite = piece.Sprite;

            for (int i = 0; i < starIndicator.Count; i++)
            {
                if (i >= maxTierForTile)
                {
                    starIndicator[i].enabled = false;
                }
                if (i < piece.Tier)
                {
                    starIndicator[i].sprite = fullStar;
                    continue;
                }
                starIndicator[i].sprite = emptyStar;
            }
        }
        else
        {
            pieceSprite.enabled = false;

            for (int i = 0; i < starIndicator.Count; i++)
            {
                if (i >= maxTierForTile)
                {
                    starIndicator[i].enabled = false;
                }
                starIndicator[i].sprite = emptyStar;
            }
        }
        
    }

    void PreviewPiece(PieceBase piece)
    {
        pieceSprite.enabled = true;
        pieceSprite.sprite = piece.Sprite;

        for (int i = 0; i < starIndicator.Count; i++)
        {
            if (i < piece.Tier)
            {
                starIndicator[i].sprite = fullStar;

                if (i >= maxTierForTile)
                {
                    starIndicator[i].enabled = true;
                    starIndicator[i].color = unavailableColor;
                }
                continue;
            }
            starIndicator[i].sprite = emptyStar;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PieceBase piece = PieceAuraParticles.Instance.GetHeldPiece();

        if (piece == null) return;
        PreviewPiece(PieceAuraParticles.Instance.GetHeldPiece());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (eventData.delta.magnitude > .5f)
        {
            UpdatePlacementIndicator();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        PieceBase testPiece = PieceAuraParticles.Instance.GetHeldPiece();

        if (testPiece == piece)
        {
            //Remove Piece
            piece = null;
            UpdatePlacementIndicator();
            OnAnyStartingPlacementPlaced?.Invoke(this, EventArgs.Empty);
        }
        if (testPiece.Tier <= maxTierForTile)
        {   
            piece = testPiece;
            Debug.Log("UnitStartingPlacementIndicatorUI.cs  piece Tier: " + piece.Tier + ", maxTierForTile: " + maxTierForTile);
            UpdatePlacementIndicator();
            OnAnyStartingPlacementPlaced?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            // Feedback
        }
    }
}
