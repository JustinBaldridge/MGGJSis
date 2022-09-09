using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeController : MonoBehaviour
{
    public static TimeController Instance;

    public static event EventHandler OnAnyTimeControllerFreezeFrame;

    float timer;
    float timeStopDuration;
    bool isActive;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one TimeController! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        if (timer < timeStopDuration)
        {
            timer += Time.unscaledDeltaTime;
        }
        else
        {
            Time.timeScale = 1f;
            isActive = false;
        }
    }

    public void HitStop(float duration = 0.1f)
    {
        timer = 0;
        timeStopDuration = duration;
        Time.timeScale = 0;
        isActive = true;
        OnAnyTimeControllerFreezeFrame?.Invoke(this, EventArgs.Empty);
    }
}
