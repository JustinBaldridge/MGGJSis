using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AssassinAction : BaseAction
{
    public override List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition, VirtualBoard virtualBoard)
    {
        int maxMoveDistance = 1;
        int maxAttackDistance = 2;
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        int forward = (this.unit.IsEnemy())? -1 : 1;

        ResetGridPositionLists();

        // Left 
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(-1, 0), maxMoveDistance, 1, false));

        // Right
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(1, 0), maxMoveDistance, 1, false));

        // Backwards
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(0, -1 * forward), maxMoveDistance, 1, false));

        // Forward
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(0, 1 * forward), maxMoveDistance, 1, false));

        // Attack Forward
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(0, 1), maxAttackDistance, maxAttackDistance, true, true));
        

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Assassin";
    }
}


