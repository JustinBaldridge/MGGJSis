using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitManager : MonoBehaviour
{
    public static UnitManager Instance {get; private set;}

    public event EventHandler OnAllEnemiesDefeated;
    private List<Unit> unitList;
    private List<Unit> friendlyUnitList;
    private List<Unit> enemyUnitList;

    [SerializeField] List<GameObject> enemySpawnPrefab;

    bool friendlyUnitTaken = false; 

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one UnitManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        unitList = new List<Unit>();
        friendlyUnitList = new List<Unit>();
        enemyUnitList = new List<Unit>();
    }

    private void Start()
    {
        Unit.OnAnyUnitSpawned += Unit_OnAnyUnitSpawned;
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
        CameraController.Instance.OnInventorySceneEnter += CameraController_OnInventorySceneEnter;
        ContinueArrow.OnAnyContinue += ContinueArrow_OnAnyContinue; 
        
        StartMenu.Instance.OnStartGame += StartMenu_OnStartGame;
    }

    private void Unit_OnAnyUnitSpawned(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;

        Debug.Log(unit + " spawned");
        unitList.Add(unit);
        if (unit.IsEnemy())
        {
            enemyUnitList.Add(unit);
        }
        else
        {
            friendlyUnitList.Add(unit);
        }
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;
        
        Debug.Log(unit + " spawned");
        unitList.Remove(unit);
        if (unit.IsEnemy())
        {
            enemyUnitList.Remove(unit);
            if (enemyUnitList.Count <= 0)
            {
                if (!friendlyUnitTaken) GameProgression.Instance.AddBonusStar();
                OnAllEnemiesDefeated?.Invoke(this, EventArgs.Empty);
            }
        }
        else
        {
            friendlyUnitTaken = true;
            friendlyUnitList.Remove(unit);
        }
    }

    private void CameraController_OnInventorySceneEnter(object sender, EventArgs e)
    {
        //List<Unit> unitsToRemove = new List<Unit>();
        // Clear ALl Units
        //foreach (Unit unit in friendlyUnitList)
        //{
            //unitsToRemove.Add(unit);
        //    friendlyUnitList.Remove(unit);
        //    Destroy(unit.gameObject);
        //}
        while (unitList.Count > 0)
        {
            Unit unit = unitList[0];

            unitList.Remove(unit);
            if (friendlyUnitList.Contains(unit))
            {
                friendlyUnitList.Remove(unit);
            }

            if (enemyUnitList.Contains(unit))
            {
                enemyUnitList.Remove(unit);
            }
            LevelGrid.Instance.RemoveUnitAtGridPosition(unit.GetGridPosition(), unit);
            Destroy(unit.gameObject);
        }
    }

    private void SpawnEnemies()
    {
        Instantiate(enemySpawnPrefab[UnityEngine.Random.Range(0, enemySpawnPrefab.Count)]);
    }

    private void ContinueArrow_OnAnyContinue(object sender, EventArgs e)
    {
        friendlyUnitTaken = false;
        SpawnEnemies();
    }

    private void StartMenu_OnStartGame(object sender, EventArgs e)
    {
        SpawnEnemies();
    }

    public List<Unit> GetUnitList()
    {
        return unitList;
    }
    
    public List<Unit> GetFriendlyUnitList()
    {
        return friendlyUnitList;
    }

    public List<Unit> GetEnemyUnitList()
    {
        return enemyUnitList;
    }
}
