using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    private enum State {
        WaitingForEnemyTurn,
        TakingTurn,
        Busy,
    }

    private State state;
    private float timer;

    private void Awake()
    {
        state = State.WaitingForEnemyTurn;
    }
    private void Start()
    {
        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
    }

    private void Update()
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            return;
        }

        switch (state)
        {
            case State.WaitingForEnemyTurn:
                break;
            case State.TakingTurn:
                timer -= Time.deltaTime;
                if (timer <= 0f)
                {
                    if (TryTakeEnemyAIAction(SetStateTakingTurn))
                    {
                        state = State.Busy;
                    }
                    else
                    {
                        // No more enemies have actions they can take, end enemy turn;
                        TurnSystem.Instance.NextTurn();
                    }
                }
                break;
            case State.Busy:
                break;
        }
    }
    
    private void SetStateTakingTurn()
    {
        timer = 0.5f;
        state = State.TakingTurn;
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e)
    {
        if (!TurnSystem.Instance.IsPlayerTurn())
        {
            state = State.TakingTurn;
            timer = 2f;
        }
    }

    private bool TryTakeEnemyAIAction(Action onEnemyAIActionComplete)
    {
        // Random Selection
        List<Unit> enemyUnits = UnitManager.Instance.GetEnemyUnitList();

        List<EnemyAIKeyValuePair> enemyAIBestActionsList = new List<EnemyAIKeyValuePair>();
        //Unit enemyUnit = enemyUnits[UnityEngine.Random.Range(0, enemyUnits.Count)];
        foreach (Unit enemyUnit in UnitManager.Instance.GetEnemyUnitList())
        {
            BaseAction bestBaseAction = enemyUnit.GetUnitAction();
            EnemyAIAction bestEnemyAIAction = bestBaseAction.GetBestEnemyAIAction(new VirtualBoard(BoardAnalysis.Instance.GetCurrentBoard()));

            if (bestEnemyAIAction != null)
            {
                enemyAIBestActionsList.Add(new EnemyAIKeyValuePair
                {
                    enemyAIUnit = enemyUnit,
                    enemyAIAction = bestEnemyAIAction,
                });
            }
        }

        if (enemyAIBestActionsList.Count == 0) return false;

        EnemyAIKeyValuePair kvp = enemyAIBestActionsList[0];
        foreach (EnemyAIKeyValuePair enemyAIkvp in enemyAIBestActionsList)
        {
            Debug.Log("EnemyAI.cs  Key Value Pair - Unit: " + enemyAIkvp.enemyAIUnit + " targetGridPosition: " + enemyAIkvp.enemyAIAction.gridPosition + " actionValue: " + enemyAIkvp.enemyAIAction.actionValue);
            if (enemyAIkvp.enemyAIAction.actionValue > kvp.enemyAIAction.actionValue)
            {
                kvp = enemyAIkvp;
            }
        }

        if (kvp != null)
        {
            kvp.enemyAIUnit.GetUnitAction().TakeAction(kvp.enemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        return false;
    }

    /*
    private bool TryTakeEnemyAIAction(Unit enemyUnit, Action onEnemyAIActionComplete)
    {
        //Debug.Log("EnemyAI.cs  TryTakeEnemyAIAction Start");
        BaseAction bestBaseAction = enemyUnit.GetUnitAction();
        EnemyAIAction bestEnemyAIAction = bestBaseAction.GetBestEnemyAIAction();
        /*foreach (BaseAction baseAction in enemyUnit.GetBaseActionArray())
        {
            if (!enemyUnit.CanSpendActionPointsToTakeAction(baseAction))
            {
                // Enemy cannot affor this action
                continue;
            }

            if (bestEnemyAIAction == null)
            {
                bestEnemyAIAction = baseAction.GetBestEnemyAIAction();
                bestBaseAction = baseAction;
            }
            else
            {
                EnemyAIAction testEnemyAIAction = baseAction.GetBestEnemyAIAction();
                if (testEnemyAIAction != null && testEnemyAIAction.actionValue > bestEnemyAIAction.actionValue)
                {
                    bestEnemyAIAction = testEnemyAIAction;
                    bestBaseAction = baseAction;
                }
            }
        }* /

        if (bestEnemyAIAction != null /*&& enemyUnit.TrySpendActionPointsToTakeAction(bestBaseAction)* /)
        {
            bestBaseAction.TakeAction(bestEnemyAIAction.gridPosition, onEnemyAIActionComplete);
            return true;
        }
        else
        {
            return false;
        }
    }*/
}
