using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VirtualBoard
{
    private GridSystem<GridObject> gridSystem;

    public VirtualBoard()
    {
        gridSystem = new GridSystem<GridObject>(LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight(), 1, 
                (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));

        List<Unit> allUnits = UnitManager.Instance.GetUnitList();

        foreach (Unit unit in allUnits)
        {
            GridPosition gridPosition = unit.GetGridPosition();
            GridObject gridObject = gridSystem.GetGridObject(gridPosition);
            gridObject.AddUnit(unit);
        }
    }

    public VirtualBoard(VirtualBoard virtualBoard)
    {
        //gridSystem = virtualBoard.GetGridSystem();

        
        GridSystem<GridObject> copyGridSystem = virtualBoard.GetGridSystem();

        int width = copyGridSystem.GetWidth();
        int height = copyGridSystem.GetHeight();
        gridSystem = new GridSystem<GridObject>(width, height, 1,
            (GridSystem<GridObject> g, GridPosition gridPosition) => new GridObject(g, gridPosition));

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridPosition gridPosition = new GridPosition(x, y);
                GridObject gridObject = gridSystem.GetGridObject(gridPosition);

                if (!copyGridSystem.GetGridObject(gridPosition).HasAnyUnit()) continue;

                gridObject.AddUnit(copyGridSystem.GetGridObject(gridPosition).GetUnit());
            }
        }
    }

    public void MovePiece(GridPosition startGridPosition, GridPosition targetGridPosition)
    {
        if (!gridSystem.GetGridObject(startGridPosition).HasAnyUnit()) { Debug.Log("VirtualBoard.cs  No Unit at " + startGridPosition.ToString()); return; }

        if (gridSystem.GetGridObject(targetGridPosition).HasAnyUnit())
        {
            Unit targetUnit = gridSystem.GetGridObject(targetGridPosition).GetUnit();
            gridSystem.GetGridObject(targetGridPosition).RemoveUnit(targetUnit);
        }

        Unit startUnit = gridSystem.GetGridObject(startGridPosition).GetUnit();
        
        GridObject startGridObject = gridSystem.GetGridObject(startGridPosition);
        startGridObject.RemoveUnit(startUnit);

        GridObject endGridObject = gridSystem.GetGridObject(targetGridPosition);
        endGridObject.AddUnit(startUnit);

        Debug.Log("VirtualBoard.cs  piece at " + startGridObject.ToString() + " moved to " + endGridObject.ToString() );
    }
    
    public override string ToString()
    {
        int width = gridSystem.GetWidth();
        int height = gridSystem.GetHeight();
        string gridString = "";
        
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                string protoString = "empty";
                GridPosition testGridPosition = new GridPosition(x, (height - 1) - y);
                if (gridSystem.GetGridObject(testGridPosition).HasAnyUnit())
                {
                    Unit unit = gridSystem.GetGridObject(testGridPosition).GetUnit();
                    BaseAction action = unit.GetUnitAction();

                    protoString = action.GetPiece().Title;
                }

                int maxStringLength = 12;
                while (protoString.Length < maxStringLength)
                {
                    protoString = protoString + " ";
                }
                
                gridString = gridString + "( " + x + ", " + ((height - 1) - y) + ") = " + protoString + "| ";
            }
            gridString = gridString + "\n";
        }

        return gridString;
    }

    public GridSystem<GridObject> GetGridSystem()
    {
        return gridSystem;
    }

    public bool IsValidGridPosition(GridPosition gridPosition) => gridSystem.IsValidGridPosition(gridPosition);

    public int GetWidth() => gridSystem.GetWidth();
    public int GetHeight() => gridSystem.GetHeight();

    public bool HasAnyUnitOnGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.HasAnyUnit();
    }

    public Unit GetUnitAtGridPosition(GridPosition gridPosition)
    {
        GridObject gridObject = gridSystem.GetGridObject(gridPosition);
        return gridObject.GetUnit();
    }

    public List<Unit> GetAllUnitsOnBoard()
    {
        int width = gridSystem.GetWidth();
        int height = gridSystem.GetHeight();
        List<Unit> unitsOnBoard = new List<Unit>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridPosition testGridPosition = new GridPosition(x, y);
                if (gridSystem.GetGridObject(testGridPosition).HasAnyUnit())
                {
                    unitsOnBoard.Add(gridSystem.GetGridObject(testGridPosition).GetUnit());
                }
            }
        }
        return unitsOnBoard;
    }

    public List<Unit> GetAllEnemyUnitsOnBoard()
    {
        int width = gridSystem.GetWidth();
        int height = gridSystem.GetHeight();
        List<Unit> unitsOnBoard = new List<Unit>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridPosition testGridPosition = new GridPosition(x, y);
                if (gridSystem.GetGridObject(testGridPosition).HasAnyUnit())
                {   
                    Unit unit = gridSystem.GetGridObject(testGridPosition).GetUnit();
                    if (unit.IsEnemy()) { unitsOnBoard.Add(unit); }
                }
            }
        }
        return unitsOnBoard;
    }

    public List<Unit> GetAllAllyUnitsOnBoard()
    {
        int width = gridSystem.GetWidth();
        int height = gridSystem.GetHeight();
        List<Unit> unitsOnBoard = new List<Unit>();

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                GridPosition testGridPosition = new GridPosition(x, y);
                if (gridSystem.GetGridObject(testGridPosition).HasAnyUnit())
                {
                    Unit unit = gridSystem.GetGridObject(testGridPosition).GetUnit();
                    if (!unit.IsEnemy()) { unitsOnBoard.Add(unit); }
                }
            }
        }
        Debug.Log("VirtualBoard.cs/GetAllAllyUnits  allyUnitCount = " + unitsOnBoard.Count );
        return unitsOnBoard;
    }
}
