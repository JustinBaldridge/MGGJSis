using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceInventoryUI : MonoBehaviour
{
    [SerializeField] Transform pieceInventoryItemPrefab;

    PieceInventory pieceInventory;

    // Start is called before the first frame update
    void Start()
    {
        PieceInventory.Instance.OnInventoryUpdated += PieceInventory_OnInventoryUpdated;
        UpdateInventoryUI();
    }

    void UpdateInventoryUI()
    {
        // Clear existing items
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        List<PieceBase> inventory = PieceInventory.Instance.GetOwnedPieces();
        foreach (PieceBase pieceBase in inventory)
        {
            Transform pieceInventoryItemTransform = Instantiate(pieceInventoryItemPrefab, this.transform);

            PieceItemUI pieceItemUI = pieceInventoryItemTransform.GetComponent<PieceItemUI>();
            pieceItemUI.SetPiece(pieceBase);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void PieceInventory_OnInventoryUpdated(object sender, EventArgs e)
    {
        UpdateInventoryUI();
    }

    
}
