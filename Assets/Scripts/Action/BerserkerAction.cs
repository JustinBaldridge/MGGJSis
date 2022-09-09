using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BerserkerAction : BaseAction
{
    //protected List<Vector3> positionList;
    //protected int currentPositionIndex;
    [SerializeField] bool tookPieceLastTurn; 
    int turnCounter;

    protected void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }
    protected override void Update()
    {
        if (!isActive) return;

        float stoppingDistance = .1f;
        if (timer < modifiedMaxTimer)
        {
            timer += Time.deltaTime;
            transform.position = initialPosition + (movementDirection * animCurve.Evaluate(timer / modifiedMaxTimer));
        }
        else
        {
            List<Unit> unitsAtPosition = LevelGrid.Instance.GetUnitListAtGridPosition(unit.GetGridPosition());
            foreach (Unit _unit in unitsAtPosition)
            {
                // Unit is this unit
                if (_unit == unit) {continue;}

                // Take Enemy Unit (basic function)
                _unit.TakePiece();
                tookPieceLastTurn = true;
                turnCounter = 0;
                break;
            }

            LevelGrid.Instance.SnapToGrid(unit.GetGridPosition(), unit);
            OnActionComplete();
        }
    }

    public override List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition, VirtualBoard virtualBoard)
    {
        int maxMoveDistance = (tookPieceLastTurn)? 8 : 2;
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        ResetGridPositionLists();
        
        // Left 
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(-1, 0), maxMoveDistance));

        // Right
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(1, 0), maxMoveDistance));

        // Down
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(0, -1), maxMoveDistance));

        // Up
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(0, 1), maxMoveDistance));

        // Up Left
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(-1, 1), maxMoveDistance));

        // Up Right
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(1, 1), maxMoveDistance));

        // Down Left
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(-1, -1), maxMoveDistance));

        // Down Right
        validGridPositionList.AddRange(GetValidGridPositionsInDirection(virtualBoard, gridPosition, new Vector2(1, -1), maxMoveDistance));

        return validGridPositionList;
    }

    public override string GetActionName()
    {
        return "Berserker";
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (turnCounter > 2)
        {
            tookPieceLastTurn = false;
        }
        turnCounter++;
    }
}


