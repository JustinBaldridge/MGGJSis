using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelTracker : MonoBehaviour
{
    [SerializeField] List<RectTransform> levelTrackerImage;
    [SerializeField] RectTransform currentLevelIcon;
    
    RectTransform rectTransform;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }
    private void Start()
    {
        UnitSelectionSpawner.Instance.OnUnitSelectionFinished += UnitSelectionSpawner_OnUnitSelectionFinished;
    }

    private void UnitSelectionSpawner_OnUnitSelectionFinished(object sender, EventArgs e)
    {
        int levelsProgressed = GameProgression.Instance.GetLevelsCompleted();

        levelsProgressed = Mathf.Clamp(levelsProgressed, 0, levelTrackerImage.Count);

        currentLevelIcon.anchoredPosition = levelTrackerImage[levelsProgressed - 1].anchoredPosition;
    }
}
