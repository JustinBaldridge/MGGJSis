using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionAnimation : MonoBehaviour
{
    public static TransitionAnimation Instance;

    public event EventHandler OnStartingAnimationFinished;
    public event EventHandler OnEndingAnimationFinished;

    [SerializeField] Transform mayaTransform;
    [SerializeField] Transform mizukiTransform;

    [SerializeField] Animator mayaAnimator;
    [SerializeField] Animator mizukiAnimator;

    [SerializeField] Vector3 offScreenPosition; 
    [SerializeField] Vector3 mayaOffScreenPosition; 
    [SerializeField] Vector3 mizukiOffScreenPosition; 

    [SerializeField] float moveSpeed; 

    enum State
    {
        Idle, 
        MovingTowards,
        MeetPause,
        MovingOffscreen,
        MovingOnscreen,
        FinishingOnscreen
    }

    State state = State.Idle; 

    Vector3 mayaPosition;
    Vector3 mizukiPosition;
    Vector3 midpoint;

    bool isActive;
    bool startEndAnim;

    float timer;
    float subTimer;
    float maxTimer; 

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one TransitionAnimation! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        UnitManager.Instance.OnAllEnemiesDefeated += UnitManager_OnAllEnemiesDefeated;
        UnitDespawner.Instance.OnDespawningFinished += UnitDespawner_OnDespawningFinished;   
        CameraController.Instance.OnCombatSceneEnter += CameraController_OnCombatSceneEnter;
        UnitSpawner.OnAnySpawningStarted += UnitSpawner_OnAnySpawningStarted;

        mayaTransform.gameObject.SetActive(false);
        mizukiTransform.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        switch (state)
        {
            case State.Idle:
                if (timer > maxTimer)
                {
                    timer = 0;
                    maxTimer = 0;
                    state = State.MovingTowards;
                }
                break;
            case State.MovingTowards:
                mayaTransform.localPosition = Vector3.MoveTowards(mayaTransform.localPosition, Vector3.zero, moveSpeed * Time.deltaTime);
                mizukiTransform.localPosition = Vector3.MoveTowards(mizukiTransform.localPosition, Vector3.zero, moveSpeed * Time.deltaTime);

                bool bothAtZero = true;
                float stoppingDistance = 0.1f;

                // Maya Animating
                if (Vector3.Distance(mayaTransform.localPosition, Vector3.zero) < stoppingDistance)
                {
                    mayaAnimator.CrossFade("Idle", 0);
                    mayaTransform.localPosition = Vector3.zero; 
                }
                else
                {
                    mayaAnimator.CrossFade("Running", 0);
                    bothAtZero = false;
                }

                // Mizuki Animating
                if (Vector3.Distance(mizukiTransform.localPosition, Vector3.zero) < stoppingDistance)
                {
                    mizukiAnimator.CrossFade("Idle", 0);
                    mizukiTransform.localPosition = Vector3.zero;
                }
                else
                {
                    mizukiAnimator.CrossFade("Running", 0);
                    bothAtZero = false;
                }

                if (bothAtZero)
                {
                    timer = 0;
                    maxTimer = .75f;
                    state = State.MeetPause;
                }
                break;
            case State.MeetPause:
                if (timer > maxTimer)
                {
                    timer = 0;
                    subTimer = 1.25f;
                    maxTimer = 3f;
                    state = State.MovingOffscreen;
                    mayaAnimator.CrossFade("RunningHoldHands", 0);
                    mizukiAnimator.CrossFade("RunningHoldHands", 0);
                }
                break;
            case State.MovingOffscreen:
                float speedMultipler = 1.1f;
                mayaTransform.localPosition += Vector3.up * (speedMultipler * moveSpeed) * Time.deltaTime;
                mizukiTransform.localPosition += Vector3.up * (speedMultipler * moveSpeed) * Time.deltaTime;

                if (timer > subTimer)
                {
                    if (!startEndAnim)
                    {
                        startEndAnim = true;
                        OnEndingAnimationFinished?.Invoke(this, EventArgs.Empty);
                    }
                }
                if (timer > maxTimer)
                {
                    //isActive = false; 
                    mayaTransform.gameObject.SetActive(false);
                    mizukiTransform.gameObject.SetActive(false);
                }
                break;
            case State.MovingOnscreen:
                mayaTransform.localPosition += Vector3.up * moveSpeed * Time.deltaTime;
                mizukiTransform.localPosition += Vector3.up * moveSpeed * Time.deltaTime;

                if (timer > maxTimer)
                {
                    mizukiAnimator.CrossFade("BattlePrep", 0);
                    mayaAnimator.CrossFade("Idle", 0);
                    state = State.FinishingOnscreen;
                    timer = 0;
                    maxTimer = .75f;
                }
                break;
            case State.FinishingOnscreen:
                if (timer > maxTimer)
                {
                    mizukiAnimator.CrossFade("BattleFrame", 0);
                    OnStartingAnimationFinished?.Invoke(this, EventArgs.Empty);
                    isActive = false;
                }
                break;
        }
    }

    private void UnitManager_OnAllEnemiesDefeated(object sender, EventArgs e)
    {
        List<Unit> aliveUnits = UnitManager.Instance.GetFriendlyUnitList();

        bool foundPrincess = false;
        foreach (Unit unit in aliveUnits)
        {
            if (unit.TryGetComponent<KingAction>(out KingAction kingAction))
            {
                mayaPosition = LevelGrid.Instance.GetWorldPosition(unit.GetGridPosition());

                if (foundPrincess) break;

                continue;
            }

            if (foundPrincess) continue;

            if (unit.TryGetComponent<PrincessAction>(out PrincessAction princessAction))
            {
                mizukiPosition = LevelGrid.Instance.GetWorldPosition(unit.GetGridPosition());
                foundPrincess = true;
            }
        }

        if (!foundPrincess)
        {
            mizukiPosition = offScreenPosition;
        }

        //midpoint = (mayaPosition + mizukiPosition) / 2f;  
        midpoint = new Vector3(7f, 7f, 0f);
        transform.position = midpoint;
    }

    private void UnitDespawner_OnDespawningFinished(object sender, EventArgs e)
    {
        isActive = true;
        timer = 0;
        maxTimer = 1.5f;
        mayaTransform.position = mayaPosition;
        mizukiTransform.position = mizukiPosition;
        mayaTransform.gameObject.SetActive(true);
        mizukiTransform.gameObject.SetActive(true);
        mayaAnimator.CrossFade("Idle", 0);
        mizukiAnimator.CrossFade("BattleFrame", 0);
        startEndAnim = false;
        state = State.Idle;
    }

    private void CameraController_OnCombatSceneEnter(object sender, EventArgs e)
    {
        isActive = true;
        timer = 0;
        maxTimer = .8f;
        mizukiTransform.position = mizukiOffScreenPosition;
        mayaTransform.position = mayaOffScreenPosition;
        mayaAnimator.CrossFade("Running", 0);
        mizukiAnimator.CrossFade("Running", 0);
        mayaTransform.gameObject.SetActive(true);
        mizukiTransform.gameObject.SetActive(true);
        state = State.MovingOnscreen;
    }

    private void UnitSpawner_OnAnySpawningStarted(object sender, UnitSpawner.SpawnEventArgs e)
    {
        if (e.piece.Title == "King")
        {
            mayaTransform.gameObject.SetActive(false);
        }
        
        if (e.piece.Title == "Princess")
        {
            mizukiTransform.gameObject.SetActive(false);
        }
    }
}
