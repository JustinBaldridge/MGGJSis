using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    [SerializeField] TrailRenderer trailRenderer;
    [SerializeField] Transform projectileSpawnPoint;

    [SerializeField] AudioClip impactSound;
    [SerializeField] AudioClip projectileSpawnSound;
    Unit unit;
    Animator animator;
    BaseAction baseAction;

    int animIdle;
    int animMoving;
    int animAttacking;
    int animDamaged;
    
    Unit saveTakeTarget;
    Vector3 attackEffectTarget;
    
    void Awake()
    {
        unit = GetComponent<Unit>();
        animator = GetComponent<Animator>();
        baseAction = GetComponent<BaseAction>();
        animIdle = Animator.StringToHash("Idle");
        animMoving = Animator.StringToHash("Moving");
        animAttacking = Animator.StringToHash("Attacking");
        animDamaged = Animator.StringToHash("Hurt");
    }

    // Start is called before the first frame update
    void Start()
    {
        unit.OnUnitDamaged += Unit_OnUnitDamaged;
        baseAction.OnMoveActionStarted += BaseAction_OnMoveActionStarted;
        baseAction.OnTakeActionStarted += BaseAction_OnTakeActionStarted;
        BaseAction.OnAnyActionCompleted += BaseAction_OnAnyActionCompleted;
        trailRenderer.material.color = (unit.IsEnemy())? Color.black : baseAction.GetPiece().PrimaryColor;
    }
    
    public void FreezeFrame(float duration)
    {
        SFXPlayer.PlaySound(impactSound);
        saveTakeTarget.TakeDamage();
        TimeController.Instance.HitStop(duration);
    }

    public void SpawnProjectile(GameObject attackEffectPrefab)
    {
        SFXPlayer.PlaySound(projectileSpawnSound);
        Debug.Log("UnitAnimator.cs  Spawned projectile");
        GameObject attackEffectObject = Instantiate(attackEffectPrefab, projectileSpawnPoint.position, Quaternion.identity);
        AttackEffect attackEffect = attackEffectObject.GetComponent<AttackEffect>();
        attackEffect.Setup(projectileSpawnPoint.position, attackEffectTarget, saveTakeTarget);
    }

    private void Unit_OnUnitDamaged(object sender, EventArgs e)
    {
        animator.CrossFade(animDamaged, 0);

        TimeController.Instance.ResetScale();
    }

    private void BaseAction_OnMoveActionStarted(object sender, EventArgs e)
    {
        animator.CrossFade(animMoving, 0);
    }

    private void BaseAction_OnTakeActionStarted(object sender, BaseAction.OnTakeEventArgs e)
    {
        animator.CrossFade(animAttacking, 0);
        saveTakeTarget = e.targetUnit;
        attackEffectTarget = LevelGrid.Instance.GetWorldPosition(e.targetGridPosition);

        if (saveTakeTarget.TryGetComponent<KingAction>(out KingAction kingAction))
        {
            TimeController.Instance.TakingKing();
        }
    }

    private void BaseAction_OnAnyActionCompleted(object sender, EventArgs e)
    {
        BaseAction usedAction = sender as BaseAction;

        if (usedAction == this.baseAction)
        {
            animator.CrossFade(animIdle, 0);
        }
    }
}
