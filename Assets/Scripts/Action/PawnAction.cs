using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PawnAction : BaseAction
{
    bool hasMoved = false;

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

        hasMoved = true;

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
        int maxMoveDistance = (hasMoved)? 1 : 2;
        int maxAttackDistance = 1;
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        int forward = (this.unit.IsEnemy())? -1 : 1;

        ResetGridPositionLists();

        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(0, forward), maxMoveDistance, 0, false));

        // Up Left
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(-1, forward), maxAttackDistance, 0, true, true));

        // Up Right
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(1, forward), maxAttackDistance, 0, true, true));
        

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Pawn";
    }
}


