using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackEffect : MonoBehaviour
{
    [SerializeField] protected AnimationCurve animCurve;
    protected Vector3 origin;
    protected Vector3 target;
    protected Unit targetUnit;

    protected bool isActive;

    public virtual void Setup(Vector3 origin, Vector3 target, Unit targetUnit)
    {
        this.origin = origin;
        this.target = target;
        this.targetUnit = targetUnit;
        
        isActive = true;
    }

    protected virtual void Update()
    {
        if (!isActive) return;
    }
}
