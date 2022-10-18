using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    public static event EventHandler OnAnyActionStarted;
    
    public static event EventHandler OnAnyActionCompleted;

    public event EventHandler OnMoveActionStarted;
    public event EventHandler<OnTakeEventArgs> OnTakeActionStarted;

    public class OnTakeEventArgs : EventArgs
    {
        public GridPosition targetGridPosition;
        public Unit targetUnit;
    }

    protected Unit unit;
    protected bool isActive;
    protected Action onActionComplete;

    protected List<GridPosition> validMovementGridPositionList = new List<GridPosition>();
    protected List<GridPosition> validAttackGridPositionList = new List<GridPosition>();
    protected List<GridPosition> validAllyTargetGridPositionList = new List<GridPosition>();

    protected List<Vector3> positionList;
    protected int currentPositionIndex;

    protected Vector3 targetPostition;
    protected Vector3 initialPosition;
    protected Vector3 movementDirection;

    [SerializeField] protected float maxMoveTimer;
    [SerializeField] protected float maxAttackTimer;
    [SerializeField] protected float moveDistanceTimerAddition;

    protected float timer;
    protected float modifiedMaxTimer;
    protected bool attacking;

    [SerializeField] protected PieceBase piece;
    [SerializeField] protected AnimationCurve movePiece;
    [SerializeField] protected AnimationCurve attackPiece;
    [SerializeField] protected AudioClip takenSound;

    protected AnimationCurve animCurve;
    protected virtual void Awake()
    {
        unit = GetComponent<Unit>();
    }

    public abstract string GetActionName();

    public virtual void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        //List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(unit.GetGridPosition(), gridPosition, out int pathLength);

        //currentPositionIndex = 0;
        //this.positionList = new List<Vector3>();

        //foreach (GridPosition pathGridPosition in pathGridPositionList)
        //{
        //    positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        //}
        //OnStartMoving?.Invoke(this, EventArgs.Empty);
        
        
        initialPosition = transform.position;
        targetPostition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        GridPosition unitGridPosition = unit.GetGridPosition();

        float distance = Mathf.Sqrt( Mathf.Pow(unitGridPosition.x - gridPosition.x, 2) + Mathf.Pow(unitGridPosition.z - gridPosition.z, 2));
        //modifiedMaxTimer = maxTimer + (moveDistanceTimerAddition * distance);

        movementDirection = targetPostition - initialPosition;
        
        
        if (LevelGrid.Instance.HasAnyUnitOnGridPosition(gridPosition))
        {
            Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            if (targetUnit.IsEnemy() != this.unit.IsEnemy())
            {
                OnTakeActionStarted?.Invoke(this, new OnTakeEventArgs
                {
                    targetUnit = targetUnit,
                    targetGridPosition = gridPosition
                });
                modifiedMaxTimer = maxAttackTimer;
                animCurve = attackPiece;
            }
            else
            {
                OnMoveActionStarted?.Invoke(this, EventArgs.Empty);
                modifiedMaxTimer = maxMoveTimer;
                animCurve = movePiece;
            }
        }
        else
        {
            OnMoveActionStarted?.Invoke(this, EventArgs.Empty);
            modifiedMaxTimer = maxMoveTimer;
            animCurve = movePiece;
        }
        ActionStart(onActionComplete);
    }

    protected virtual void Update()
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
                break;
            }

            LevelGrid.Instance.SnapToGrid(unit.GetGridPosition(), unit);
            OnActionComplete();
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
        timer = 0;

        OnAnyActionStarted?.Invoke(this, EventArgs.Empty);
    }

    protected void OnActionComplete()
    {
        isActive = false;
        OnAnyActionCompleted?.Invoke(this, EventArgs.Empty);
        onActionComplete();
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
    public EnemyAIAction GetRandomEnemyAIAction(VirtualBoard virtualBoard)
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
            enemyAIActionList.Sort((EnemyAIAction a, EnemyAIAction b) => b.randomValue - a.randomValue);
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

        //Debug.Log("BaseAction.cs  testing testGridPosition: " + testGridPosition.ToString());
        ResetGridPositionLists();

        // if taking a piece
        if (virtualBoard.HasAnyUnitOnGridPosition(testGridPosition))
        {
            Unit unit = virtualBoard.GetUnitAtGridPosition(testGridPosition);
            if (!unit.IsEnemy())
            {
                if (unit.TryGetComponent<BaseAction>(out BaseAction baseAction))
                {
                    if (unit.TryGetComponent<KingAction>(out KingAction kingAction))
                    {
                        return new EnemyAIAction {
                            gridPosition = testGridPosition,
                            actionValue = 99999,
                            randomValue = UnityEngine.Random.Range(0, 100),
                        };
                    }
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
        
        //Debug.Log("BaseAction.cs  possibleAttackers Count: " + possibleAttackers.Count);
        foreach (GridPosition gp in possibleAttackers)
        {
            if (!virtualBoard.HasAnyUnitOnGridPosition(gp)) continue;

            Unit unit = virtualBoard.GetUnitAtGridPosition(gp);
            BaseAction action = unit.GetUnitAction();

            action.GetValidActionGridPositionList(unit.GetGridPosition(), virtualBoard);

            List<GridPosition> unitAttackPositions = action.GetValidAttackGridPositionList();

            if (unitAttackPositions.Contains(testGridPosition))
            {
                adjustedPointValue -= BoardAnalysis.GetPieceValue(action.GetPiece()) / 5;
            }
        }
        //Debug.Log("BaseAction.cs  adjustedPointValue, check attackers: " + adjustedPointValue);

        // Check if the piece would be defended at this position
        List<GridPosition> possibleDefenders = BoardAnalysis.GetPiecesAttackingAtGridPosition(this, testGridPosition, virtualBoard, virtualBoard.GetAllEnemyUnitsOnBoard());
        // TODO:  Possible defenders, similar to above, but do a VALUE - pieceValue, which makes it incentivse being defended by weaker pieces
        //     note: for the king, reduce the sampled value so that pieces defended by the king arent like destroying the actionValue economy
        
        //Debug.Log("BaseAction.cs  possibleDefenders Count: " + possibleDefenders.Count);
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
                adjustedPointValue += pieceValueConstant - (pieceValue / 5);
            }
        }

        int addRandomValue = UnityEngine.Random.Range(0, 100 - ((GameProgression.Instance.GetDifficultyStage() + 1) * 30));
        int totalActionValue = boardStateDiff + possibleMoves + possibleAttacks + adjustedPointValue + addRandomValue;
        //Debug.Log("BaseAction.cs  totalActionValue: " + totalActionValue + " randomValue: " + addRandomValue);

        return new EnemyAIAction{
            gridPosition = testGridPosition,
            actionValue = totalActionValue,
            randomValue = UnityEngine.Random.Range(0, 100),
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
                    if (unit.IsEnemy() != this.unit.IsEnemy())
                    {
                        validAttackGridPositionList.Add(testGridPosition);
                        validGridPositionList.Add(testGridPosition);
                    }
                    
                    if (jumpPiece) continue;
                    break; 
                }
            }

            if (onlyTake) 
            {
                validAttackGridPositionList.Add(testGridPosition);
                continue;
            }

            validMovementGridPositionList.Add(testGridPosition);
            validGridPositionList.Add(testGridPosition);
        }
        return validGridPositionList;
    }

    public void CallMoveAction()
    {
        OnMoveActionStarted?.Invoke(this, EventArgs.Empty);
    }

    void OnMouseEnter()
    {
        PieceInfoUI.Instance.UpdateDescription(piece);
    }
}