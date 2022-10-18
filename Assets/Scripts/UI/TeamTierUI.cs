using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamTierUI : MonoBehaviour
{
    public static TeamTierUI Instance;

    [SerializeField] GameObject starIndicatorPrefab;
    [SerializeField] int baseStarCount;

    [SerializeField] Transform starGridParent;
    int maxStars;
    int currentStars;
    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one TeamTierUI! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UnitStartingPlacementIndicatorUI.OnAnyStartingPlacementPlaced += UnitStartingPlacementIndicatorUI_OnAnyStartingPlacementPlaced;
        UnitManager.Instance.OnAllEnemiesDefeated += UnitManager_OnAllEnemiesDefeated;
        UnitSelectionSpawner.Instance.OnUnitSelectionFinished += UnitSelectionSpawner_OnUnitSelectionFinished;

        UpdateShownStars();
    }

    private void UnitStartingPlacementIndicatorUI_OnAnyStartingPlacementPlaced(object sender, EventArgs e)
    {
        UpdateShownStars();
    }

    private void UnitManager_OnAllEnemiesDefeated(object sender , EventArgs e)
    {
        maxStars = baseStarCount + GameProgression.Instance.GetLevelsCompleted() + GameProgression.Instance.GetBonusStars();
        UpdateStarCount();
        UpdateShownStars();
    }

    private void UnitSelectionSpawner_OnUnitSelectionFinished(object sender , EventArgs e)
    {
        //maxStars = baseStarCount + GameProgression.Instance.GetLevelsCompleted() + GameProgression.Instance.GetBonusStars();
        //UpdateStarCount();
        UpdateShownStars();
    }

    void UpdateShownStars()
    {
        List<UnitStartingPlacement> startingPlacements = UnitStartingPositionUI.Instance.GetStartingPlacements();
        
        int totalStarCount = 0;

        foreach (UnitStartingPlacement usp in startingPlacements)
        {
            totalStarCount += usp.piece.Tier;
        }
        

        List<StarIndicatorUI> starIndicatorList = new List<StarIndicatorUI>();

        foreach (Transform child in starGridParent)
        {
            StarIndicatorUI starIndicatorUI = child.GetComponent<StarIndicatorUI>();
            starIndicatorList.Add(starIndicatorUI);
        }

        for (int i = 0; i < starIndicatorList.Count; i++)
        {
            if (i < totalStarCount)
            {
                starIndicatorList[i].Show();
                continue;
            }
            starIndicatorList[i].Hide();
        }
        Debug.Log("TeamTierUI.cs  Updating Shown Stars, totalStarCount: " + totalStarCount + ", startingPlacement Count: " + startingPlacements.Count 
            + " starIndicatorList Count: " + starIndicatorList.Count);
        currentStars = totalStarCount;
    }

    void UpdateStarCount()
    {
        // Clear existing items
        foreach (Transform child in starGridParent)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < maxStars; i++)
        {
            GameObject starIndicatorTransform = Instantiate(starIndicatorPrefab, transform.position, Quaternion.identity, starGridParent);
        }

        UpdateShownStars();
    }

    public int GetMaxStars()
    {
        return maxStars;
    }

    public int GetCurrentStars()
    {
        return currentStars;
    }
}
