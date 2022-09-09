using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileAttackEffect : AttackEffect
{
    [SerializeField] float speed;
    [SerializeField] bool rotateOnSpawn = true;
    void Start()
    {
        Debug.Log("ProjectileAttackEffect.cs  Beginnign");
    }

    public override void Setup(Vector3 origin, Vector3 target, Unit targetUnit)
    {
        base.Setup(origin, target, targetUnit);

        if (rotateOnSpawn)
        {
            transform.right = Vector3.RotateTowards(origin, target, 50, 0f);
        }
    }

    protected override void Update()
    {
        // Skips if not is active
        base.Update();

        float stoppingDistance = 0.5f;
        Debug.Log("ProjectileAttackEffect.cs  Distance:  " + Vector3.Distance(transform.position, target));

        if (Vector3.Distance(transform.position, target) > stoppingDistance)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        }
        else
        {
            targetUnit.TakeDamage();
            TimeController.Instance.HitStop();
            Destroy(this.gameObject);
        }
    }
}
