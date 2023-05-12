using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance {get; private set;}

    public event EventHandler OnCombatSceneEnter;
    public event EventHandler OnInventorySceneEnter;
    public event EventHandler OnFinaleCinimaticStart;
    public event EventHandler OnFinaleCinimaticProgression;
    
    [SerializeField] AnimationCurve animCurve;
    [SerializeField] AnimationCurve lostGameCurve;
    [SerializeField] Vector3 offScreenTargetPosition;

    [SerializeField] AudioClip loseGameSound;
    AnimationCurve usingCurve;
    
    enum TransationState 
    {
        ToBoard,
        FromBoard,
    }
    
    private event Action onCallEvent;

    Vector3 initialPosition;
    Vector3 targetPosition;
    float timer;
    [SerializeField] float timeDuration = 1.5f;
    float difference;
    bool isActive;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one CameraController! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        //UnitManager.Instance.OnAllEnemiesDefeated += UnitManager_OnAllEnemiesDefeated;
        TransitionAnimation.Instance.OnEndingAnimationFinished += TransitionAnimation_OnEndingAnimationFinished; 
        TransitionAnimation.Instance.OnGameFinished += TransitionAnimation_OnGameFinished; 
        ContinueArrow.OnAnyContinue += ContinueArrow_OnAnyContinue; 
        StartMenu.Instance.OnStartGame += StartMenu_OnStartGame;
        UnitManager.Instance.OnKingTaken += UnitManager_OnKingTaken;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        if (timer < timeDuration)
        {
            timer += Time.deltaTime;
            transform.position = new Vector3(0, initialPosition.y + (difference * usingCurve.Evaluate(timer / timeDuration)), 0);//Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
        }
        else
        {
            isActive = false;
            onCallEvent?.Invoke();
        }
    }

    private void TransitionAnimation_OnEndingAnimationFinished(object sender, EventArgs e)
    {
        Debug.Log("CameraController.cs  All Enemies Defeated");
        isActive = true;
        initialPosition = transform.position;
        difference = offScreenTargetPosition.y;
        timer = 0;
        usingCurve = animCurve;
        onCallEvent = new Action(() => {
            transform.position = -transform.position;
            OnInventorySceneEnter?.Invoke(this, EventArgs.Empty);
        });
    }

    private void TransitionAnimation_OnGameFinished(object sender, EventArgs e)
    {
        isActive = true;
        initialPosition = transform.position;
        difference = offScreenTargetPosition.y;
        timer = 0;
        usingCurve = animCurve;
        onCallEvent = new Action(() => {
            isActive = true;
            timer = 0;
            initialPosition = transform.position;
            difference = offScreenTargetPosition.y / 2;
            usingCurve = lostGameCurve;
            onCallEvent = new Action(() => {
                OnFinaleCinimaticProgression?.Invoke(this, EventArgs.Empty);
            });
            OnFinaleCinimaticStart?.Invoke(this, EventArgs.Empty);
        });
    }

    private void ContinueArrow_OnAnyContinue(object sender, EventArgs e)
    {
        isActive = true;
        targetPosition = Vector3.zero;
        initialPosition = transform.position;
        difference = offScreenTargetPosition.y;
        timer = 0;
        usingCurve = animCurve;
        onCallEvent = new Action(() => {
            //transform.position = -transform.position;
            OnCombatSceneEnter?.Invoke(this, EventArgs.Empty);
        });
    }

    private void StartMenu_OnStartGame(object sender, EventArgs e)
    {
        isActive = true;
        targetPosition = Vector3.zero;
        initialPosition = transform.position;
        difference = offScreenTargetPosition.y;
        timer = 0;
        usingCurve = animCurve;
        onCallEvent = new Action(() => {
            //transform.position = -transform.position;
            OnCombatSceneEnter?.Invoke(this, EventArgs.Empty);
        });
    }

    private void UnitManager_OnKingTaken(object sender, EventArgs e)
    {
        isActive = true;
        targetPosition = Vector3.zero;
        initialPosition = transform.position;
        difference = -offScreenTargetPosition.y;
        timer = 0;
        usingCurve = lostGameCurve;
        SFXPlayer.PlaySound(loseGameSound);
        onCallEvent = null;
    }
}
