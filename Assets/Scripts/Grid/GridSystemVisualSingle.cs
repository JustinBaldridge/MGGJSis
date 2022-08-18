using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridSystemVisualSingle : MonoBehaviour
{
    [SerializeField] private SpriteRenderer spriteRenderer;

    public void Show(Color color)
    {
        spriteRenderer.enabled = true;
        spriteRenderer.color = color;
    }

    public void Hide()
    {
        
        spriteRenderer.enabled = false;
    }
}
