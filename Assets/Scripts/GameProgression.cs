using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameProgression : MonoBehaviour
{
    public static GameProgression Instance;

    private int levelsCompleted = 0;
    private int bonusStars = 0; 

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
    }

    private void UnitManager_OnAllEnemiesDefeated(object sender, EventArgs e)
    {
        levelsCompleted++;
    }

    public int GetLevelsCompleted()
    {
        return levelsCompleted;
    }

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
}