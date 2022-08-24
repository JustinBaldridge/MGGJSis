using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryAction : BaseAction
{
    //protected List<Vector3> positionList;
    //protected int currentPositionIndex;

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

        currentPositionIndex = 0;
        this.positionList = new List<Vector3>();

        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }
        //OnStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition, VirtualBoard virtualBoard)
    {
        int maxMoveDistance = 3;
        int minMoveDistance = 2;
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        ResetGridPositionLists();
        
        // Left 
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(-1, 0), maxMoveDistance, minMoveDistance, true, false, true));

        // Right
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(1, 0), maxMoveDistance, minMoveDistance, true, false, true));

        // Down
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(0, -1), maxMoveDistance, minMoveDistance, true, false, true));

        // Up
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(0, 1), maxMoveDistance, minMoveDistance, true, false, true));

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Artillery";
    }
}


