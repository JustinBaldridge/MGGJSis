using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    public static event EventHandler OnAnyActionStarted;
    
    public static event EventHandler OnAnyActionCompleted;
    protected Unit unit;
    protected bool isActive;
    protected Action onActionComplete;

    protected List<GridPosition> validMovementGridPositionList = new List<GridPosition>();
    protected List<GridPosition> validAttackGridPositionList = new List<GridPosition>();
    protected List<GridPosition> validAllyTargetGridPositionList = new List<GridPosition>();

    protected List<Vector3> positionList;
    protected int currentPositionIndex;

    [SerializeField] protected PieceBase piece;
    [SerializeField] protected AnimationCurve movePiece;

    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public abstract string GetActionName();

    public abstract void TakeAction(GridPosition gridPosition, Action onActionComplete);

    protected virtual void Update()
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
                    break;
                }

                LevelGrid.Instance.SnapToGrid(unit.GetGridPosition(), unit);
                OnActionComplete();
            }
        }
    }
    public virtual bool IsValidActionGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public virtual List<GridPosition> GetValidActionGridPositionList()
    {
        return GetValidActionGridPositionList(this.unit.GetGridPosition());
    }

    public virtual List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition, BoardAnalysis.Instance.GetCurrentBoard());
    }

    public abstract List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition, VirtualBoard virtualBoard);

    public virtual int GetActionPointsCost()
    {
        return 1;
    }

    protected void ActionStart(Action onActionComplete)
    {
        isActive = true;
        this.onActionComplete = onActionComplete;

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void OnActionComplete()
    {
        isActive = false;
        onActionComplete();
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
    }

    public Unit GetUnit()
    {
        return unit;
    }

    public EnemyAIAction GetBestEnemyAIAction(VirtualBoard virtualBoard)
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        List<GridPosition> validActionGridPositionList = GetValidActionGridPositionList();

        foreach (GridPosition testGridPosition in validActionGridPositionList)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(unit.GetGridPosition(), testGridPosition, new VirtualBoard(virtualBoard));
            enemyAIActionList.Add(enemyAIAction);
        }

        if (enemyAIActionList.Count > 0)
        {
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.actionValue - a.actionValue);
            return enemyAIActionList[0];
        }
        else
        {
            // No possible Enemy AI Action
            return null;
        }
        
    }

    public virtual EnemyAIAction GetEnemyAIAction(GridPosition startGridPosition, GridPosition testGridPosition, VirtualBoard virtualBoard)
    {
        // Get Board State value for action
        int currentBoardState = BoardAnalysis.Instance.GetBoardState();
        int possibleBoardState = currentBoardState;

        Debug.Log("BaseAction.cs  testing testGridPosition: " + testGridPosition.ToString());
        ResetGridPositionLists();

        // if taking a piece
        if (virtualBoard.HasAnyUnitOnGridPosition(testGridPosition))
        {
            Unit unit = virtualBoard.GetUnitAtGridPosition(testGridPosition);
            if (!unit.IsEnemy())
            {
                if (unit.TryGetComponent<BaseAction>(out BaseAction baseAction))
                {
                    possibleBoardState += BoardAnalysis.GetPieceValue(baseAction.GetPiece());
                }
            }
        }

        int boardStateDiff = possibleBoardState - currentBoardState; 

        // calculate possible moves
        int possibleMoves = BoardAnalysis.GetPieceMovesAtGridPosition(this, testGridPosition, virtualBoard);

        // calculate possible takes from position
        int possibleAttacks = BoardAnalysis.GetPieceAttacksAtGridPosition(this, testGridPosition, virtualBoard);

        int adjustedPointValue = 0;
        // Check if the piece would be taken at this position
        virtualBoard.MovePiece(startGridPosition, testGridPosition);
        List<GridPosition> possibleAttackers = BoardAnalysis.GetPiecesAttackingAtGridPosition(this, testGridPosition, virtualBoard, virtualBoard.GetAllAllyUnitsOnBoard());
        
        Debug.Log("BaseAction.cs  possibleAttackers Count: " + possibleAttackers.Count);
        foreach (GridPosition gp in possibleAttackers)
        {
            if (!virtualBoard.HasAnyUnitOnGridPosition(gp)) continue;

            Unit unit = virtualBoard.GetUnitAtGridPosition(gp);
            BaseAction action = unit.GetUnitAction();

            action.GetValidActionGridPositionList(unit.GetGridPosition(), virtualBoard);

            List<GridPosition> unitAttackPositions = action.GetValidAttackGridPositionList();

            if (unitAttackPositions.Contains(testGridPosition))
            {
                adjustedPointValue -= BoardAnalysis.GetPieceValue(action.GetPiece());
            }
        }
        Debug.Log("BaseAction.cs  adjustedPointValue, check attackers: " + adjustedPointValue);

        // Check if the piece would be defended at this position
        List<GridPosition> possibleDefenders = BoardAnalysis.GetPiecesAttackingAtGridPosition(this, testGridPosition, virtualBoard, virtualBoard.GetAllEnemyUnitsOnBoard());
        // TODO:  Possible defenders, similar to above, but do a VALUE - pieceValue, which makes it incentivse being defended by weaker pieces
        //     note: for the king, reduce the sampled value so that pieces defended by the king arent like destroying the actionValue economy
        Debug.Log("BaseAction.cs  possibleDefenders Count: " + possibleDefenders.Count);
        foreach (GridPosition gp in possibleDefenders)
        {
            if (!virtualBoard.HasAnyUnitOnGridPosition(gp)) continue;

            Unit unit = virtualBoard.GetUnitAtGridPosition(gp);
            BaseAction action = unit.GetUnitAction();

            action.GetValidActionGridPositionList(unit.GetGridPosition(), virtualBoard);

            List<GridPosition> unitAttackPositions = action.GetValidAttackGridPositionList();

            if (unitAttackPositions.Contains(testGridPosition))
            {
                int pieceValueConstant = 100;
                PieceBase piece = action.GetPiece();
                int pieceValue = BoardAnalysis.GetPieceValue(piece);

                if (piece.Tier == 5)
                {
                    pieceValue = 10;
                }
                adjustedPointValue += pieceValueConstant - pieceValue;
            }
        }

        int totalActionValue = boardStateDiff + possibleMoves + possibleAttacks + adjustedPointValue;
        //Debug.Log("BaseAction.cs  totalActionValue: " + totalActionValue);

        return new EnemyAIAction{
            gridPosition = testGridPosition,
            actionValue = totalActionValue,
        };
    }

    public List<GridPosition> GetValidMovementGridPositionList()
    {
        return validMovementGridPositionList;
    }
    
    public List<GridPosition> GetValidAttackGridPositionList()
    {
        return validAttackGridPositionList;
    }
    
    public List<GridPosition> GetValidAllyTargetGridPositionList()
    {
        return validAllyTargetGridPositionList;
    }

    public void ResetGridPositionLists()
    {
        validMovementGridPositionList.Clear();
        validAttackGridPositionList.Clear();
        validAllyTargetGridPositionList.Clear();
    }

    public PieceBase GetPiece()
    {
        return piece;
    }

    protected List<GridPosition> GetValidGridPositionsInDirection(VirtualBoard virtualBoard, GridPosition gridPosition, Vector2 direction, int distance, 
        int minDistance = 1, bool takeOnMove = true, bool onlyTake = false, bool jumpPiece = false)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();
        for (int d = minDistance; d <= distance; d++)
        {
            GridPosition offsetGridPosition = new GridPosition((int) direction.x * d, (int) direction.y * d);
            GridPosition testGridPosition = gridPosition + offsetGridPosition;

            if (!virtualBoard.IsValidGridPosition(testGridPosition)) { continue; }

            // Same Grid Position where the unit is already at
            //if (gridPosition == testGridPosition) { continue; }

            if (virtualBoard.HasAnyUnitOnGridPosition(testGridPosition))
            {
                if (!takeOnMove)
                {   
                    if (jumpPiece) { continue; }
                    break;
                }

                // Grid position already occupied with another unit
                Unit unit = virtualBoard.GetUnitAtGridPosition(testGridPosition);
                if (testGridPosition != this.unit.GetGridPosition())
                {
                    validAttackGridPositionList.Add(testGridPosition);
                    if (unit.IsEnemy() != this.unit.IsEnemy())
                    {
                        validGridPositionList.Add(testGridPosition);
                    }
                    
                    if (jumpPiece) continue;
                    break; 
                }
            }

            if (onlyTake) continue;
            
            validMovementGridPositionList.Add(testGridPosition);
            validGridPositionList.Add(testGridPosition);
        }
        return validGridPositionList;
    }
}