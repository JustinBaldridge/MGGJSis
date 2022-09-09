using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartMenu : MonoBehaviour
{
    public static StartMenu Instance;

    public event EventHandler OnStartGame;

    [SerializeField] float moveSpeed;

    RectTransform rectTransform;

    Vector2 targetPosition;
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one StartMenu! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        rectTransform = GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start()
    {  
        RestartGame();
    }

    void RestartGame()
    {
        targetPosition = Vector2.zero; 
    }

    // Update is called once per frame
    void Update()
    {
        if (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) < .5f)
        {
            OnStartGame?.Invoke(this, EventArgs.Empty);
        }
        else
        {
            rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    public void StartGame()
    {
        targetPosition = rectTransform.anchoredPosition + (Vector2.down * 450);
    }
}
