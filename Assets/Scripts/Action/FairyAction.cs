using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FairyAction : BaseAction
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
        int maxMoveDistance = 4;
        int minMoveDistance = 2;
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        ResetGridPositionLists();

        // Up Left
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(-1, 1), maxMoveDistance, minMoveDistance, true, false, true));

        // Up Right
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(1, 1), maxMoveDistance, minMoveDistance, true, false, true));

        // Down Left
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(-1, -1), maxMoveDistance, minMoveDistance, true, false, true));

        // Down Right
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(1, -1), maxMoveDistance, minMoveDistance, true, false, true));

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Fairy";
    }
}


