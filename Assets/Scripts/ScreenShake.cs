using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ScreenShake : MonoBehaviour
{
    
    public static ScreenShake Instance {get; private set;}
    private CinemachineImpulseSource cinemachineImpulseSource;

    bool doScreenShake = true;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one ScreenShake! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }

    public void Shake(float intensity = 1f)
    {
        if (doScreenShake)
        {
            cinemachineImpulseSource.GenerateImpulse(intensity);
        }   
    }

    public void SetScreenShake(bool value)
    {
        doScreenShake = value;
    }
}
