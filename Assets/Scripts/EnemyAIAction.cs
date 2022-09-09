using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAIAction
{
    public GridPosition gridPosition;
    public int actionValue;
}

public class EnemyAIKeyValuePair
{
    public int randomness;
    public Unit enemyAIUnit;
    public EnemyAIAction enemyAIAction;
} 