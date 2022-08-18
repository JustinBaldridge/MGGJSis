using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KnightAction : BaseAction
{
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

    public override List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition)
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

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition))
                {
                    continue;
                }

                if (gridPosition == testGridPosition)
                {
                    // Same Grid Position where the unit is already at
                    continue;
                }

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
                {
                    // Grid position already occupied with another unit
                    Unit _unit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

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

