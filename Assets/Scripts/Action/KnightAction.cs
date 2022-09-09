using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightAction : BaseAction
{

    public override List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition, VirtualBoard virtualBoard)
    {
        int maxMoveDistance = 2;
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
        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Knight";
    }
}


