using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance;

    [Serializable]
    public struct GridVisualTypeColor
    {
        public GridVisualType gridVisualType;
        public Color color;
    }

    public enum GridVisualType
    {
        White,
        Blue,
        Red,
        RedSoft,
        Yellow
    }
    
    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeColor> gridVisualTypeColorList;

    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one GridSystemVisual! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    private void Start()
    {
        gridSystemVisualSingleArray = new GridSystemVisualSingle[
            LevelGrid.Instance.GetWidth(),
            LevelGrid.Instance.GetHeight()
        ];
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z <LevelGrid.Instance.GetHeight(); z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform gridSystemVisualSingleTransform = 
                    Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);

                gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }

        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMovedGridPosition += LevelGrid_OnAnyUnitMovedGridPosition;
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        UpdateGridVisual();
    }

    public void HideAllGridPosition()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z <LevelGrid.Instance.GetHeight(); z++)
            {
                gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range)
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }    

    private void ShowGridPositionRangeSquare(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                gridPositionList.Add(testGridPosition);
            }
        }

        ShowGridPositionList(gridPositionList, gridVisualType);
    }    

    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].
                Show(GetGridVisualTypeColor(gridVisualType));
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPosition();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = selectedUnit.GetUnitAction();

        GridVisualType gridVisualType;
        switch (selectedAction)
        {
            default:
                gridVisualType = GridVisualType.White;
                break;
        }

        List<GridPosition> validActions = selectedAction.GetValidActionGridPositionList();
        ShowGridPositionList(
            validActions, gridVisualType);

        ShowGridPositionList(
            selectedAction.GetValidMovementGridPositionList(), GridVisualType.White);

        List<GridPosition> validAttacks = new List<GridPosition>();
        List<GridPosition> invalidAttacks = new List<GridPosition>();
        List<GridPosition> allAttacks = selectedAction.GetValidAttackGridPositionList();
        foreach (GridPosition gp in allAttacks)
        {
            if (validActions.Contains(gp))
            {
                validAttacks.Add(gp);
            }
            else
            {
                invalidAttacks.Add(gp);
            }
        }
        ShowGridPositionList(
            validAttacks, GridVisualType.Red);
        ShowGridPositionList(
            invalidAttacks, GridVisualType.RedSoft);
        ShowGridPositionList(
            selectedAction.GetValidAllyTargetGridPositionList(), GridVisualType.Blue);
    }

    private void UnitActionSystem_OnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private void LevelGrid_OnAnyUnitMovedGridPosition(object sender, EventArgs e)
    {
        //UpdateGridVisual();
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        HideAllGridPosition();
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (UnitActionSystem.Instance.GetOffline()) 
        {
            HideAllGridPosition();
            return;
        }
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            UpdateGridVisual();
        }
    }

    private Color GetGridVisualTypeColor(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeColor gridVisualTypeColor in gridVisualTypeColorList)
        {
            if (gridVisualTypeColor.gridVisualType == gridVisualType)
            {
                return gridVisualTypeColor.color;
            }
        }
        Debug.LogError("Could not find GridVisualTypeColor for GridVisualType " + gridVisualType);
        return Color.white;
    }
}
