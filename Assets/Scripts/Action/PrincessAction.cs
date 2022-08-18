using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrincessAction : BaseAction
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

    public override List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition)
    {
        int maxMoveDistance = 3;
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        ResetGridPositionLists();

        // Left 
        for (int x = 0; x >= -maxMoveDistance; x--)
        {
            GridPosition offsetGridPosition = new GridPosition(x, 0);
            GridPosition testGridPosition = gridPosition + offsetGridPosition;

            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; }

            // Same Grid Position where the unit is already at
            if (gridPosition == testGridPosition) { continue; }

            if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
            {
                // Grid position already occupied with another unit
                Unit _unit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (_unit.IsEnemy() != this.unit.IsEnemy())
                {
                    validAttackGridPositionList.Add(testGridPosition);
                    validGridPositionList.Add(testGridPosition);
                }
                break;
            }

            validMovementGridPositionList.Add(testGridPosition);
            validGridPositionList.Add(testGridPosition);
        }

        // Right
        for (int x = 0; x <= maxMoveDistance; x++)
        {
            GridPosition offsetGridPosition = new GridPosition(x, 0);
            GridPosition testGridPosition = gridPosition + offsetGridPosition;

            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; }

            // Same Grid Position where the unit is already at
            if (gridPosition == testGridPosition) { continue; }

            if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
            {
                // Grid position already occupied with another unit
                Unit unit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (unit.IsEnemy())
                {
                    validAttackGridPositionList.Add(testGridPosition);
                    validGridPositionList.Add(testGridPosition);
                }
                break;
            }

            validMovementGridPositionList.Add(testGridPosition);
            validGridPositionList.Add(testGridPosition);
        }

        // Down
        for (int z = 0; z >= -maxMoveDistance; z--)
        {
            GridPosition offsetGridPosition = new GridPosition(0, z);
            GridPosition testGridPosition = gridPosition + offsetGridPosition;

            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; }

            // Same Grid Position where the unit is already at
            if (gridPosition == testGridPosition) { continue; }

            if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
            {
                // Grid position already occupied with another unit
                Unit unit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (unit.IsEnemy())
                {
                    validAttackGridPositionList.Add(testGridPosition);
                    validGridPositionList.Add(testGridPosition);
                }
                break;
            }

            validMovementGridPositionList.Add(testGridPosition);
            validGridPositionList.Add(testGridPosition);
        }

        // Up
        for (int z = 0; z <= maxMoveDistance; z++)
        {
            GridPosition offsetGridPosition = new GridPosition(0, z);
            GridPosition testGridPosition = gridPosition + offsetGridPosition;

            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; }

            // Same Grid Position where the unit is already at
            if (gridPosition == testGridPosition) { continue; }

            if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
            {
                // Grid position already occupied with another unit
                Unit unit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (unit.IsEnemy())
                {
                    validAttackGridPositionList.Add(testGridPosition);
                    validGridPositionList.Add(testGridPosition);
                }
                break;
            }

            validMovementGridPositionList.Add(testGridPosition);
            validGridPositionList.Add(testGridPosition);
        }

        // Up Left
        for (int d = 0; d <= maxMoveDistance; d++)
        {
            GridPosition offsetGridPosition = new GridPosition(d, -d);
            GridPosition testGridPosition = gridPosition + offsetGridPosition;

            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; }

            // Same Grid Position where the unit is already at
            if (gridPosition == testGridPosition) { continue; }

            if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
            {
                // Grid position already occupied with another unit
                Unit unit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (unit.IsEnemy())
                {
                    validAttackGridPositionList.Add(testGridPosition);
                    validGridPositionList.Add(testGridPosition);
                }
                break;
            }

            validMovementGridPositionList.Add(testGridPosition);
            validGridPositionList.Add(testGridPosition);
        }

        // Up Right
        for (int d = 0; d <= maxMoveDistance; d++)
        {
            GridPosition offsetGridPosition = new GridPosition(d, d);
            GridPosition testGridPosition = gridPosition + offsetGridPosition;

            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; }

            // Same Grid Position where the unit is already at
            if (gridPosition == testGridPosition) { continue; }

            if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
            {
                // Grid position already occupied with another unit
                Unit unit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (unit.IsEnemy())
                {
                    validAttackGridPositionList.Add(testGridPosition);
                    validGridPositionList.Add(testGridPosition);
                }
                break;
            }

            validMovementGridPositionList.Add(testGridPosition);
            validGridPositionList.Add(testGridPosition);
        }

        // Down Left
        for (int d = 0; d <= maxMoveDistance; d++)
        {
            GridPosition offsetGridPosition = new GridPosition(-d, -d);
            GridPosition testGridPosition = gridPosition + offsetGridPosition;

            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; }

            // Same Grid Position where the unit is already at
            if (gridPosition == testGridPosition) { continue; }

            if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
            {
                // Grid position already occupied with another unit
                Unit unit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (unit.IsEnemy())
                {
                    validAttackGridPositionList.Add(testGridPosition);
                    validGridPositionList.Add(testGridPosition);
                }
                break;
            }

            validMovementGridPositionList.Add(testGridPosition);
            validGridPositionList.Add(testGridPosition);
        }

        // Down Right
        for (int d = 0; d <= maxMoveDistance; d++)
        {
            GridPosition offsetGridPosition = new GridPosition(-d, d);
            GridPosition testGridPosition = gridPosition + offsetGridPosition;

            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; }

            // Same Grid Position where the unit is already at
            if (gridPosition == testGridPosition) { continue; }

            if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
            {
                // Grid position already occupied with another unit
                Unit unit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (unit.IsEnemy())
                {
                    validAttackGridPositionList.Add(testGridPosition);
                    validGridPositionList.Add(testGridPosition);
                }
                break;
            }

            validMovementGridPositionList.Add(testGridPosition);
            validGridPositionList.Add(testGridPosition);
        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "King";
    }
}


