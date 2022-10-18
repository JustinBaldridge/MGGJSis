using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceAuraParticles : MonoBehaviour
{
    public static PieceAuraParticles Instance;

    [SerializeField] GameObject auraGameObject;
    [SerializeField] List<Renderer> renderers;

    PieceBase heldPiece;
    bool isActive;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one PieceAuraParticles! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        Hide();
    }
    // Start is called before the first frame update
    void Start()
    {
        PieceItemUI.OnAnyPieceItemSelected += PieceItemUI_OnAnyPieceItemSelected;
        UnitStartingPlacementIndicatorUI.OnAnyStartingPlacementPlaced += UnitStartingPlacementIndicatorUI_OnAnyStartingPlacementPlaced;
        ContinueArrow.OnAnyContinue += ContinueArrow_OnAnyContinue;
        CameraController.Instance.OnInventorySceneEnter += CameraController_OnInventorySceneEnter;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return; 

        Vector3 mousePos = Input.mousePosition;
        Vector3 worldPoint = Camera.main.ScreenToWorldPoint(mousePos);
        worldPoint.z = 0;
        transform.position = worldPoint;
    }

    void Show()
    {
        isActive = true;
        auraGameObject.SetActive(true);
    }
    void Hide()
    {
        isActive = false;
        auraGameObject.SetActive(false);
    }

    public PieceBase GetHeldPiece()
    {
        return heldPiece;
    }

    private void PieceItemUI_OnAnyPieceItemSelected(object sender, EventArgs e)
    {
        PieceItemUI pieceItemUi = sender as PieceItemUI;
        PieceBase testPiece = pieceItemUi.GetPiece();

        if (heldPiece != null)
        {
            if (testPiece.Title.Equals(heldPiece.Title))
            {
                heldPiece = null;
                Hide();
                return;
            }
        }
        
        heldPiece = testPiece;

        Color color = heldPiece.PrimaryColor;
        Color secondaryColor = heldPiece.SecondaryColor;

        Show();

        foreach (Renderer renderer in renderers)
        {
            if (renderer is TrailRenderer trailRenderer)
            {
                renderer.material.SetColor("_Color", secondaryColor);
                continue;
            }
            renderer.material.SetColor("_Color", color);
        }
    }

    private void UnitStartingPlacementIndicatorUI_OnAnyStartingPlacementPlaced(object sender, EventArgs e)
    {
        //Hide();
    }

    private void ContinueArrow_OnAnyContinue(object sender, EventArgs e)
    {
        Hide();
    }

    private void CameraController_OnInventorySceneEnter(object sender, EventArgs e)
    {
        heldPiece = null;
        Hide();
    }
}
