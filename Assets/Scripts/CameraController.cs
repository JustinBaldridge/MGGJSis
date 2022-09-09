using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance {get; private set;}

    public event EventHandler OnCombatSceneEnter;
    public event EventHandler OnInventorySceneEnter;
    
    [SerializeField] AnimationCurve animCurve;
    [SerializeField] Vector3 offScreenTargetPosition;
    
    enum TransationState 
    {
        ToBoard,
        FromBoard,
    }
    
    private event Action onCallEvent;

    Vector3 initialPosition;
    Vector3 targetPosition;
    float timer;
    float timeDuration = 3.5f;
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
        ContinueArrow.OnAnyContinue += ContinueArrow_OnAnyContinue; 
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        if (timer < timeDuration)
        {
            timer += Time.deltaTime;
            transform.position = new Vector3(0, initialPosition.y + (difference * animCurve.Evaluate(timer / timeDuration)), 0);//Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
        }
        else
        {
            isActive = false;
            transform.position = -transform.position;
            onCallEvent();
        }
    }

    private void TransitionAnimation_OnEndingAnimationFinished(object sender, EventArgs e)
    {
        Debug.Log("CameraController.cs  All Enemies Defeated");
        isActive = true;
        initialPosition = transform.position;
        difference = offScreenTargetPosition.y;
        timer = 0;
        onCallEvent = new Action(() => {
            OnInventorySceneEnter?.Invoke(this, EventArgs.Empty);
        });
    }

    private void ContinueArrow_OnAnyContinue(object sender, EventArgs e)
    {
        isActive = true;
        targetPosition = Vector3.zero;
        initialPosition = transform.position;
        difference = offScreenTargetPosition.y;
        timer = 0;
        onCallEvent = new Action(() => {
            OnCombatSceneEnter?.Invoke(this, EventArgs.Empty);
        });
    }
}
