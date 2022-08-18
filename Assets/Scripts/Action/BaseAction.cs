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

    [SerializeField] protected PieceDataBase pieceData;
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

    public abstract List<GridPosition> GetValidActionGridPositionList(GridPosition gridPosition);

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

    public EnemyAIAction GetBestEnemyAIAction()
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        List<GridPosition> validActionGridPositionList = GetValidActionGridPositionList();

        foreach (GridPosition gridPosition in validActionGridPositionList)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
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

    public virtual EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        // Get Board State value for action
        int currentBoardState = BoardAnalysis.Instance.GetBoardState();
        int possibleBoardState = currentBoardState;

        Debug.Log("BaseAction.cs  pre analysis possibleBoardState: " + possibleBoardState);
        ResetGridPositionLists();

        // if taking a piece
        if (LevelGrid.Instance.HasAnyUnitOnGridPosition(gridPosition))
        {
            Unit unit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
            if (!unit.IsEnemy())
            {
                if (unit.TryGetComponent<BaseAction>(out BaseAction baseAction))
                {
                    possibleBoardState += BoardAnalysis.GetPieceValue(baseAction.GetPieceData());
                }
            }
        }

        Debug.Log("BaseAction.cs  post possibleBoardState: " + possibleBoardState);

        // calculate possible moves

        // calculate possible takes from position

        return new EnemyAIAction{
            gridPosition = gridPosition,
            actionValue = possibleBoardState -currentBoardState,
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

    public PieceDataBase GetPieceData()
    {
        return pieceData;
    }
}