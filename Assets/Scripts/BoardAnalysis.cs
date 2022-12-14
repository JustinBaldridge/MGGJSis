using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardAnalysis : MonoBehaviour
{
    public static BoardAnalysis Instance {get; private set; }

    public event EventHandler OnBoardUpdated;

    int boardState;
    VirtualBoard currentBoard;
    GridPosition startingGridPosition;
    GridPosition endingGridPosition;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one BoardAnalysis! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        currentBoard = new VirtualBoard();
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
        BaseAction.OnAnyActionStarted += BaseAction_OnAnyActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
        UnitSpawner.Instance.OnSpawningFinished += UnitSpawner_OnSpawningFinished;
    }

    public int GetBoardState()
    {
        return boardState;
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;

        if (unit.TryGetComponent<BaseAction>(out BaseAction baseAction))
        {
            int pointValue = GetPieceValue(baseAction.GetPiece());

            if (unit.IsEnemy())
            {
                boardState += pointValue;
            }
            else
            {
                boardState -= pointValue;
            }
        }
    }

    private void BaseAction_OnAnyActionStarted(object sender, EventArgs e)
    {
        BaseAction action = sender as BaseAction;
        startingGridPosition = action.GetUnit().GetGridPosition();

        //Debug.Log("BoardAnalusis.cs OnMoveStart");
        //Debug.Log(currentBoard.ToString());
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        BaseAction action = sender as BaseAction;
        endingGridPosition = action.GetUnit().GetGridPosition();

        //Debug.Log("BoardAnalusis.cs OnMoveStart");
        //Debug.Log(currentBoard.ToString());
        currentBoard.MovePiece(startingGridPosition, endingGridPosition);
        OnBoardUpdated?.Invoke(this, EventArgs.Empty);
    }

    private void UnitSpawner_OnSpawningFinished(object sender, EventArgs e)
    {
        currentBoard = new VirtualBoard();
        OnBoardUpdated?.Invoke(this, EventArgs.Empty);
        //Debug.Log("BoardAnalusis.cs  OnSpawningFinished");
        //Debug.Log(currentBoard.ToString());
    }

    public static int GetPieceValue(PieceBase piece)
    {
        switch (piece.Tier)
        {
            case 1:
                return 10;
            case 2:
                return 30;
            case 3:
                return 50;
            case 4:
                return 90;
            case 5:
                return 900;
        }

        return 0;
    } 

    public VirtualBoard GetCurrentBoard()
    {
        return currentBoard;
    }

    public static int GetPieceMovesAtGridPosition(BaseAction baseAction, GridPosition gridPosition, VirtualBoard virtualBoard)
    {
        baseAction.GetValidActionGridPositionList(gridPosition, virtualBoard);

        int movementCount = baseAction.GetValidMovementGridPositionList().Count;
        
        baseAction.ResetGridPositionLists();

        return movementCount;
    }

    public static int GetPieceAttacksAtGridPosition(BaseAction baseAction, GridPosition gridPosition, VirtualBoard virtualBoard)
    {
        List<GridPosition> validPositions = baseAction.GetValidActionGridPositionList(gridPosition, virtualBoard);
        List<GridPosition> positionList = baseAction.GetValidAttackGridPositionList();

        List<GridPosition> validAttacks = new List<GridPosition>();
        foreach (GridPosition position in positionList)
        {
            if (validPositions.Contains(position))
            {
                validAttacks.Add(position);
            }
        }
        int totalPieceValue = 0;
        foreach (GridPosition position in validAttacks)
        {
            Unit unit = virtualBoard.GetUnitAtGridPosition(position);
            BaseAction action = unit.GetUnitAction();
            PieceBase piece = action.GetPiece();
            totalPieceValue += GetPieceValue(piece);
        }

        int attackCount = positionList.Count;
        baseAction.ResetGridPositionLists();

        return attackCount + (totalPieceValue / 10);
    }

    public static List<GridPosition> GetPiecesAttackingAtGridPosition(BaseAction baseAction, GridPosition gridPosition, VirtualBoard virtualBoard, List<Unit> unitsToEvaluate)
    {
        List<GridPosition> possibleAttackPositions = new List<GridPosition>();
        foreach (Unit unit in unitsToEvaluate)
        {
            BaseAction opposingUnitAction = unit.GetUnitAction();
            List<GridPosition> validPositions = opposingUnitAction.GetValidActionGridPositionList(unit.GetGridPosition(), virtualBoard);
            List<GridPosition> attackingPositions = opposingUnitAction.GetValidAttackGridPositionList();
            if (validPositions.Contains(gridPosition))
            {
                if (attackingPositions.Contains(gridPosition))
                {
                    possibleAttackPositions.Add(unit.GetGridPosition());
                }
            }
        }

        return possibleAttackPositions;
    }
}

