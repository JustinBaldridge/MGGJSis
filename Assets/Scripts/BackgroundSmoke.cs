using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundSmoke : MonoBehaviour
{
    public static BackgroundSmoke Instance;
    
    public event EventHandler OnWinScreenEntered;
    [SerializeField] float moveSpeed;
    RectTransform rectTransform; 

    bool isActive;

    Vector2 targetPosition;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one BackgroundSmoke! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        rectTransform = GetComponent<RectTransform>();
    }
    // Start is called before the first frame update
    void Start()
    {
        CameraController.Instance.OnFinaleCinimaticProgression += CameraController_OnFinaleCinimaticProgression;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;
        if (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) < .5f)
        {
            isActive = false;
            rectTransform.anchoredPosition = targetPosition;
            OnWinScreenEntered?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    private void CameraController_OnFinaleCinimaticProgression(object sender, EventArgs e)
    {
        isActive = true;
        targetPosition = Vector2.down * 1050;
    }

    public void Restart()
    {
        rectTransform.anchoredPosition = Vector2.zero;
    }
}
