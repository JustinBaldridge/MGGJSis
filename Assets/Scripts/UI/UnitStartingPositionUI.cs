using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStartingPositionUI : MonoBehaviour
{
    public static UnitStartingPositionUI Instance;

    [SerializeField] List<UnitStartingPlacementIndicatorUI> startingPlacements;

    int baseStarCount = 8;
    int maxStars;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one UnitStartingPositionUI! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public List<UnitStartingPlacement> GetStartingPlacements()
    {
        int width = 8;
        //int height = 2;

        List<UnitStartingPlacement> placementList = new List<UnitStartingPlacement>();

        for (int i = 0; i < startingPlacements.Count; i++)
        {
            int x = i % width;
            int y = i / width;

            PieceBase testPiece = startingPlacements[i].GetPiece();

            if (testPiece == null) continue;

            placementList.Add(new UnitStartingPlacement(testPiece, 
                new GridPosition(x , y)));
        }

        return placementList;
    }
}

public class UnitStartingPlacement
{
    public PieceBase piece;
    public GridPosition gridPosition;    

    public UnitStartingPlacement(PieceBase piece, GridPosition gridPosition)
    {
        this.piece = piece;
        this.gridPosition = gridPosition;
    }
}