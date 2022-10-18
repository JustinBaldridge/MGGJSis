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
    [SerializeField] Image lockedSprite;

    [SerializeField] Sprite emptyStar;
    [SerializeField] Sprite fullStar;
    [SerializeField] int maxTierForTile;

    [SerializeField] Color unavailableColor;

    [SerializeField] bool locked;
    [SerializeField] PieceBase initialPiece;
    [SerializeField] AudioClip placedSound;
    [SerializeField] AudioClip errorSound;
    PieceBase piece;
    float alpha = 1f;
    float minAlpha = .25f;
    bool alphaDirection = false;
    bool previewing;
    bool mouseExited; 
    float timer;
    [SerializeField] float previewTimer;
    // Start is called before the first frame update
    void Start()
    {
        ContinueArrow.OnAnyContinue += ContinueArrow_OnAnyContinue;
        StartMenu.Instance.OnStartGame += StartMenu_OnStartGame;

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

        if (initialPiece != null)
        {
            piece = initialPiece;
        }

        lockedSprite.enabled = locked;
    }

    void Update()
    {
        if (!previewing) return;

        timer += Time.deltaTime;

        if (!pieceSprite.sprite.Equals(piece?.Sprite))
        {
            float alphaSpeed = 0.5f;
            if (alphaDirection)
            {
                alpha += alphaSpeed * Time.deltaTime;
                if (alpha >= .9f)
                {
                    alphaDirection = false;
                    alpha = 1;
                }
            }
            else
            {
                alpha -= alphaSpeed * Time.deltaTime;
                if (alpha <= minAlpha)
                {
                    alphaDirection = true;
                    alpha = minAlpha;
                }
            }
            pieceSprite.color = new Color(1f, 1f, 1f, alpha);
        }
    }

    void UpdatePlacementIndicator()
    {
        if (piece != null)
        {
            pieceSprite.enabled = true;
            pieceSprite.sprite = piece.Sprite;
            pieceSprite.color = Color.white;

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

    public PieceBase GetPiece()
    {
        return piece;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        PieceBase heldPiece = PieceAuraParticles.Instance.GetHeldPiece();

        mouseExited = false;
        if (heldPiece == null)
        {
            if (piece == null) return;
            PieceInfoUI.Instance.UpdateDescription(piece);
            return;
        } 
        PieceInfoUI.Instance.UpdateDescription(heldPiece);
        previewing = true;
        PreviewPiece(heldPiece);
        timer = 0;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UpdatePlacementIndicator();
        previewing = false;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (locked)
        {  
            SFXPlayer.PlaySound(errorSound);
            return;
        } 
        PieceBase testPiece = PieceAuraParticles.Instance.GetHeldPiece();

        if (testPiece == null)
        {
            piece = null;
            SFXPlayer.PlaySound(placedSound);
            UpdatePlacementIndicator();
            OnAnyStartingPlacementPlaced?.Invoke(this, EventArgs.Empty);
            return;
        }

        if (piece != null)
        {
            Debug.Log("UnitStartingPlacementIndicatorUI.cs  testPiece: " + testPiece.Title + ", currentPiece: " + piece.Title); //+ " are == " + testPiece == piece + ", or are Equals " + testPiece.Equals(piece));
            if (testPiece.Title.Equals(piece.Title))
            {
                //Remove Piece
                piece = null;
                SFXPlayer.PlaySound(placedSound);
                UpdatePlacementIndicator();
                OnAnyStartingPlacementPlaced?.Invoke(this, EventArgs.Empty);
                return;
            }
        }
        
        
        int currentStars = TeamTierUI.Instance.GetCurrentStars();
        int maxStars = TeamTierUI.Instance.GetMaxStars();

        if (testPiece.Tier <= maxTierForTile && currentStars + testPiece.Tier <= maxStars)
        {   
            piece = testPiece;
            Debug.Log("UnitStartingPlacementIndicatorUI.cs  piece Tier: " + piece.Tier + ", maxTierForTile: " + maxTierForTile);
            Debug.Log("UnitStartingPlacementIndicatorUI.cs  piece Tier: " + piece.Tier + ", maxStars: " + maxStars + ", currentStars: " + currentStars);
            SFXPlayer.PlaySound(placedSound);
            UpdatePlacementIndicator();
            OnAnyStartingPlacementPlaced?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            // Feedback
            SFXPlayer.PlaySound(errorSound);
        }
    }

    private void ContinueArrow_OnAnyContinue(object sender, EventArgs e)
    {
        UpdatePlacementIndicator();
    }

    private void StartMenu_OnStartGame(object sender, EventArgs e)
    {
        if (!locked) piece = null;
        previewing = false;
        UpdatePlacementIndicator();
    }
}
