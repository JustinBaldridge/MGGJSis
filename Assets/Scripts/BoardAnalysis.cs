using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardAnalysis : MonoBehaviour
{
    public static BoardAnalysis Instance {get; private set; }

    int boardState;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one BoardAnalysis! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        Unit.OnAnyUnitDead += Unit_OnAnyUnitDead;
    }

    public int GetBoardState()
    {
        return boardState;
    }

    private void Unit_OnAnyUnitDead(object sender, EventArgs e)
    {
        Unit unit = sender as Unit;

        if (unit.TryGetComponent<BaseAction>(out BaseAction baseAction))
        {
            int pointValue = GetPieceValue(baseAction.GetPieceData());

            if (unit.IsEnemy())
            {
                boardState += pointValue;
            }
            else
            {
                boardState -= pointValue;
            }
        }
    }

    public static int GetPieceValue(PieceDataBase pieceData)
    {
        switch (pieceData.Tier)
        {
            case 1:
                return 10;
            case 2:
                return 30;
            case 3:
                return 50;
            case 4:
                return 90;
            case 5:
                return 999;
        }

        return 0;
    } 
}
