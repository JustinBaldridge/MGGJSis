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

    public override List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition)
    {
        int maxMoveDistance = 4;
        int minMoveDistance = 2;
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        ResetGridPositionLists();

        // Up Left
        for (int d = 0; d <= maxMoveDistance; d++)
        {
            GridPosition offsetGridPosition = new GridPosition(d, -d);
            GridPosition testGridPosition = gridPosition + offsetGridPosition;

            if (Mathf.Abs(d) < minMoveDistance) continue;

            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; }

            // Same Grid Position where the unit is already at
            if (gridPosition == testGridPosition) { continue; }

            if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
            {
                // Grid position already occupied with another unit
                Unit unit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (unit.IsEnemy() != this.unit.IsEnemy())
                {
                    validAttackGridPositionList.Add(testGridPosition);
                    validGridPositionList.Add(testGridPosition);
                }
                continue; 
            }

            validMovementGridPositionList.Add(testGridPosition);
            validGridPositionList.Add(testGridPosition);
        }

        // Up Right
        for (int d = 0; d <= maxMoveDistance; d++)
        {
            GridPosition offsetGridPosition = new GridPosition(d, d);
            GridPosition testGridPosition = gridPosition + offsetGridPosition;

            if (Mathf.Abs(d) < minMoveDistance) continue;

            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; }

            // Same Grid Position where the unit is already at
            if (gridPosition == testGridPosition) { continue; }

            if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
            {
                // Grid position already occupied with another unit
                Unit unit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (unit.IsEnemy() != this.unit.IsEnemy())
                {
                    validAttackGridPositionList.Add(testGridPosition);
                    validGridPositionList.Add(testGridPosition);
                }
                continue; 
            }

            validMovementGridPositionList.Add(testGridPosition);
            validGridPositionList.Add(testGridPosition);
        }

        // Down Left
        for (int d = 0; d <= maxMoveDistance; d++)
        {
            GridPosition offsetGridPosition = new GridPosition(-d, -d);
            GridPosition testGridPosition = gridPosition + offsetGridPosition;

            if (Mathf.Abs(d) < minMoveDistance) continue;

            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; }

            // Same Grid Position where the unit is already at
            if (gridPosition == testGridPosition) { continue; }

            if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
            {
                // Grid position already occupied with another unit
                Unit unit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (unit.IsEnemy() != this.unit.IsEnemy())
                {
                    validAttackGridPositionList.Add(testGridPosition);
                    validGridPositionList.Add(testGridPosition);
                }
                continue; 
            }

            validMovementGridPositionList.Add(testGridPosition);
            validGridPositionList.Add(testGridPosition);
        }

        // Down Right
        for (int d = 0; d <= maxMoveDistance; d++)
        {
            GridPosition offsetGridPosition = new GridPosition(-d, d);
            GridPosition testGridPosition = gridPosition + offsetGridPosition;

            if (Mathf.Abs(d) < minMoveDistance) continue;

            if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) { continue; }

            // Same Grid Position where the unit is already at
            if (gridPosition == testGridPosition) { continue; }

            if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition))
            {
                // Grid position already occupied with another unit
                Unit unit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if (unit.IsEnemy() != this.unit.IsEnemy())
                {
                    validAttackGridPositionList.Add(testGridPosition);
                    validGridPositionList.Add(testGridPosition);
                }
                continue; 
            }

            validMovementGridPositionList.Add(testGridPosition);
            validGridPositionList.Add(testGridPosition);
        }

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Fairy";
    }
}


