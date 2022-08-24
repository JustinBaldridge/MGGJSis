using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PieceDatabase : MonoBehaviour
{
    public static PieceDatabase Instance;

    [SerializeField] List<PieceBase> tierOnePieces = new List<PieceBase>();
    [SerializeField] List<PieceBase> tierTwoPieces;
    [SerializeField] List<PieceBase> tierThreePieces;
    [SerializeField] List<PieceBase> tierFourPieces;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one PieceDatabase! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public PieceBase GetRandomPieceOfTier(int tier)
    {
        switch (tier)
        {
            default:
            case 1:
                return tierOnePieces[Random.Range(0, tierOnePieces.Count)];
            case 2:
                return tierTwoPieces[Random.Range(0, tierTwoPieces.Count)];
            case 3:
                return tierThreePieces[Random.Range(0, tierThreePieces.Count)];
            case 4:
                return tierFourPieces[Random.Range(0, tierFourPieces.Count)];
        }
    }
    
}
