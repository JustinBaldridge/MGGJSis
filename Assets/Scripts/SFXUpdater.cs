using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXUpdater : MonoBehaviour
{
    AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        if (audioSource != null)
        {
            audioSource.volume = StartMenu.Instance.GetSFXValue();
            StartMenu.Instance.OnUpdateSFXVolume += StartMenu_OnUpdateSFXVolume;
        }
    }

    private void StartMenu_OnUpdateSFXVolume(object sender, StartMenu.SFXEventArgs e)
    {
        audioSource.volume = e.value;
    }
}
