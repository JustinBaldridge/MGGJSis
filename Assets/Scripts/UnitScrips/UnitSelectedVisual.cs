using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelectedVisual : MonoBehaviour
{
    [SerializeField] Unit unit;

    [SerializeField] ParticleSystem particles; 
    [SerializeField] ParticleSystemRenderer particleRenderer;
    [SerializeField] SpriteRenderer spriteRenderer;
    Color setColor;

    // Start is called before the first frame update
    void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystem_OnSelectedUnitChanged;
        PieceBase piece = unit.GetUnitAction().GetPiece();
        
        setColor = piece.PrimaryColor;
        particleRenderer.material.SetColor("_Color", setColor); 
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        particleRenderer.gameObject.SetActive(UnitActionSystem.Instance.GetSelectedUnit() == unit);
        spriteRenderer.sortingOrder = (UnitActionSystem.Instance.GetSelectedUnit() == unit) ? 6 : 5;
        /*
        if (UnitActionSystem.Instance.GetSelectedUnit() == unit)
        {   
            particles.Play();
        }
        else
        {
            particles.Stop();
        }*/
    }

    private void OnDestroy()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystem_OnSelectedUnitChanged;
    }

    private void UnitActionSystem_OnSelectedUnitChanged(object sender, EventArgs e)
    {
        UpdateVisual();
    }

}
