using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawningLightBeam : MonoBehaviour
{
    [SerializeField] SpriteRenderer unitSprite;
    [SerializeField] SpriteRenderer lightBase;
    [SerializeField] LineRenderer lightLineRenderer;
    [SerializeField] SpriteRenderer lightOutline;
    [SerializeField] Renderer particleRenderer;
    [SerializeField] Animator animator;

    [SerializeField] float startPosition;
    [SerializeField] float endPosition;

    [SerializeField] bool destroying; 

    Action createUnit;

    public void Initialize(PieceBase piece, Action createUnit)
    {
        lightBase.color = piece.SecondaryColor;
        lightLineRenderer.startColor = piece.SecondaryColor;
        lightOutline.color = piece.PrimaryColor;
        unitSprite.sprite = piece.Sprite;
        particleRenderer.material.SetColor("_Color", piece.PrimaryColor);

        if (piece.Title == "King")
        {
            unitSprite.enabled = false;
            lightBase.enabled = false;
            lightLineRenderer.enabled = false;
            lightOutline.enabled = false;
            particleRenderer.enabled = false;
        }

        if (destroying)
        {
            animator.CrossFade("LightBeamExit", 0);
        }
        this.createUnit = createUnit;
    }

    public void SpawnUnit()
    {
        createUnit();
    }

    public void SelfDestruct()
    {
        Destroy(this.gameObject);
    }
    // Update is called once per frame
    void Update()
    {
        lightLineRenderer.SetPosition(0, new Vector3(0, startPosition, 0));
        lightLineRenderer.SetPosition(1, new Vector3(0, endPosition, 0));
    }
}
