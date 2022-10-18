using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgression : MonoBehaviour
{
    public static GameProgression Instance;

    private int levelsCompleted = 0;
    private int bonusStars = 0; 

    [SerializeField] int maxLevels;
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one GameProgession! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }
    // Start is called before the first frame update
    void Start()
    {
        UnitManager.Instance.OnAllEnemiesDefeated += UnitManager_OnAllEnemiesDefeated;
        StartMenu.Instance.OnStartGame += StartMenu_OnStartGame;
    }

    private void UnitManager_OnAllEnemiesDefeated(object sender, EventArgs e)
    {
        levelsCompleted++;
    }

    private void StartMenu_OnStartGame(object sender, EventArgs e)
    {
        levelsCompleted = 0;
        bonusStars = 0;
    }

    public int GetLevelsCompleted()
    {
        return levelsCompleted;
    }

    // Starts at 0, goes up to 2.
    public int GetDifficultyStage()
    { 
        return (int) Mathf.Floor(((float) levelsCompleted - 1f) / 2f);
    }

    public void AddBonusStar(int addedStars = 1)
    {
        bonusStars += addedStars;
        Debug.Log("GameProgression.cs  bonusStars: " + bonusStars);
    }

    public int GetBonusStars()
    {
        return bonusStars;
    }

    public bool IsGameComplete()
    {
        return levelsCompleted >= maxLevels;
    }
}