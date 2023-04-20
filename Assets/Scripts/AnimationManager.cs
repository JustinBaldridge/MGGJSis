using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    public static AnimationManager Instance;

    [SerializeField] float defaultMoveTimer; 
    [SerializeField] AnimationCurve defaultMoveAnimationCurve;

    bool animating;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one AnimationManager! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    public void SetAnimating(bool value)
    {
        animating = value;
    }

    public bool GetAnimating()
    {
        return animating;
    }

    public float GetDefaultMoveTimer()
    {
        return defaultMoveTimer;
    }

    public AnimationCurve GetDefaultMoveAnimationCurve()
    {
        return defaultMoveAnimationCurve;
    }
}
