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

        Vector3 targetPostition = positionList[currentPositionIndex];
        Vector3 moveDirection = (targetPostition - transform.position).normalized;

        float stoppingDistance = .1f;
        if (Vector3.Distance(transform.position, targetPostition) > stoppingDistance)
        {
            float moveSpeed = 10f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else
        {
            currentPositionIndex++;
            if (currentPositionIndex >= positionList.Count)
            {
                //OnStopMoving?.Invoke(this, EventArgs.Empty);
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
    }

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


