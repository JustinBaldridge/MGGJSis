using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BeamAttackEffect : AttackEffect
{
    [SerializeField] LineRenderer lineRenderer;
    [SerializeField] float reachDuration;
    [SerializeField] float hitDuration; 
    
    [SerializeField] float hitStopDuration;
    enum State {
        StartingBeam,
        Beaming,
        EndingBeam, 

    }
    Vector3 direction;
    float timer;
    bool hitstopFinished;

    State state = State.StartingBeam;

    void Start()
    {
        Debug.Log("ProjectileAttackEffect.cs  Beginnign");
    }

    public override void Setup(Vector3 origin, Vector3 target, Unit targetUnit)
    {
        base.Setup(origin, target, targetUnit);

        direction = target - origin;
        
        //transform.right = Vector3.RotateTowards(origin, target, 50, 0f);
        lineRenderer.SetPosition(0, origin);
        lineRenderer.SetPosition(1, origin);
    }

    protected override void Update()
    {
        // Skips if not is active
        base.Update();
        
        timer += Time.deltaTime;

        switch (state)
        {
            case State.StartingBeam:

                if (timer < reachDuration)
                {
                    lineRenderer.SetPosition(1, origin + direction * (timer / reachDuration));
                }
                else
                {
                    lineRenderer.SetPosition(1, target);
                    state = State.Beaming;
                    timer = 0;
                }
                break;
            case State.Beaming:
                if (timer > hitDuration)
                {
                    state = State.EndingBeam;
                    timer = 0;
                    TimeController.Instance.HitStop(hitStopDuration);
                    targetUnit.TakeDamage();
                }
                break;
            case State.EndingBeam:
                if (timer < reachDuration)
                {
                    lineRenderer.SetPosition(0, origin + direction * (timer / reachDuration));
                }
                else
                {
                    
                    Destroy(gameObject);
                }
                break;
        }
    }
}
