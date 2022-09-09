using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PieceInfoUI : MonoBehaviour
{
    public static PieceInfoUI Instance;

    [SerializeField] float targetX;
    [SerializeField] Image exampleImage;
    [SerializeField] TextMeshProUGUI titleText;
    [SerializeField] TextMeshProUGUI descriptionText;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one PieceInfoUI! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void UpdateDescription(PieceBase piece)
    {
        exampleImage.sprite = piece.MovementExample;
        titleText.text = piece.Title;
        descriptionText.text = piece.Description;
    }
}
