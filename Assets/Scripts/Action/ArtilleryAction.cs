using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArtilleryAction : BaseAction
{
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


