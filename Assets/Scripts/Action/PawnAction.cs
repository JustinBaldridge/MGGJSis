using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor.Animations;

public class PawnAction : BaseAction
{
    [SerializeField] GameObject bishopPrefab;
    [SerializeField] GameObject knightPrefab;
    [SerializeField] GameObject rookPrefab;
    [SerializeField] GameObject queenPrefab;

    bool hasMoved = false;

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        base.TakeAction(gridPosition, onActionComplete);
        hasMoved = true;
    }

    public override List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition, VirtualBoard virtualBoard)
    {
        int maxMoveDistance = (hasMoved)? 1 : 2;
        int maxAttackDistance = 1;
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        int forward = (this.unit.IsEnemy())? -1 : 1;

        ResetGridPositionLists();

        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(0, forward), maxMoveDistance, 1, false));

        // Up Left
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(-1, forward), maxAttackDistance, 1, true, true));

        // Up Right
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(1, forward), maxAttackDistance, 1, true, true));
        

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Pawn";
    }
}


