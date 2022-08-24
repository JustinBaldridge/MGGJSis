using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServantAction : BaseAction
{
    //protected List<Vector3> positionList;
    //protected int currentPositionIndex;

    bool hasPerformedKingSwap;

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        GridPosition oldGridPosition = unit.GetGridPosition();
        currentPositionIndex = 0;
        this.positionList = new List<Vector3>();
        if (LevelGrid.Instance.HasAnyUnitOnGridPosition(gridPosition))
        {
            Unit unit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            if (unit.TryGetComponent<KingAction>(out KingAction kingAction))
            {
                positionList.Add(LevelGrid.Instance.GetWorldPosition(gridPosition));
                kingAction.ServantActionSwap(oldGridPosition);
                ActionStart(onActionComplete);
                return;
            }
        }
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

        foreach (GridPosition pathGridPosition in pathGridPositionList)
        {
            positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }
        //OnStartMoving?.Invoke(this, EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition, VirtualBoard virtualBoard)
    {
        int maxMoveDistance = 2;
        int maxAttackDistance = 1;
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        ResetGridPositionLists();
        
        // Left 
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(-1, 0), maxMoveDistance, 1, false));

        // Right
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(1, 0), maxMoveDistance, 1, false));

        // Down
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(0, -1), maxMoveDistance, 1, false));

        // Up
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(0, 1), maxMoveDistance, 1, false));

        // Up Left
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(-1, 1), maxAttackDistance, 1, true, true));

        // Up Right
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(1, 1), maxAttackDistance, 1, true, true));

        // Down Left
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(-1, -1), maxAttackDistance, 1, true, true));

        // Down Right
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(1, -1), maxAttackDistance, 1, true, true));

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Servant";
    }
}


