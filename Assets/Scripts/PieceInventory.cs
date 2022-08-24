using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceInventory : MonoBehaviour
{
    public static PieceInventory Instance;

    public event EventHandler OnInventoryUpdated;

    [SerializeField] PieceBase kingPiece;
    [SerializeField] PieceBase princessPiece;

    List<PieceBase> ownedPieces;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one PieceInventory! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        ownedPieces = new List<PieceBase>();
        ownedPieces.Add(kingPiece);
        ownedPieces.Add(princessPiece);   
    }

    private void Start()
    {
        UnitSelectionUI.OnAnyPieceAddSelected += UnitSelectionUI_OnAnyPieceAddSelected;
    }

    public List<PieceBase> GetOwnedPieces()
    {
        return ownedPieces;
    }

    public void AddPiece(PieceBase piece)
    {
        ownedPieces.Add(piece);
        OnInventoryUpdated?.Invoke(this, EventArgs.Empty);
    }

    private void UnitSelectionUI_OnAnyPieceAddSelected(object sender, EventArgs e)
    {
        foreach (PieceBase p in ownedPieces)
        {
            Debug.Log("PieceInventory.cs  " + p.Name);
        }
    }
}
