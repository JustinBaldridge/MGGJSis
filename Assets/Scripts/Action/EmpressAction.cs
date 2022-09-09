using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmpressAction : BaseAction
{

    public override List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition, VirtualBoard virtualBoard)
    {
        int maxMoveDistance = 2;
        int maxOrthoDistance = 8;
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        ResetGridPositionLists();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = gridPosition + offsetGridPosition;

                if (!((Mathf.Abs(x) == 2 && Mathf.Abs(z) == 1) || (Mathf.Abs(x) == 1 && Mathf.Abs(z) == 2)))
                {
                    continue;
                }

                if (!virtualBoard.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (gridPosition == testGridPosition)
                {
                    // Same Grid Position where the unit is already at
                    continue;
                }

                if (virtualBoard.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    // Grid position already occupied with another unit
                    Unit _unit = virtualBoard.GetUnitAtGridPosition(testGridPosition);

                    if (_unit.IsEnemy() != this.unit.IsEnemy())
                    {
                        validAttackGridPositionList.Add(testGridPosition);
                        validGridPositionList.Add(testGridPosition);
                    }
                    continue;
                }

                validMovementGridPositionList.Add(testGridPosition);
                validGridPositionList.Add(testGridPosition);
            }
        }

        // Left 
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(-1, 0), maxOrthoDistance));

        // Right
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(1, 0), maxOrthoDistance));

        // Down
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(0, -1), maxOrthoDistance));

        // Up
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(0, 1), maxOrthoDistance));

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Empress";
    }
}


