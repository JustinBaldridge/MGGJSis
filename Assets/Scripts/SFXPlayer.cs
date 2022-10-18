using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer : MonoBehaviour
{
    public static SFXPlayer Instance;

    float volume;
    AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one SFXPlayer! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    public static void PlaySound(AudioClip clip)
    {
        Instance.audioSource.PlayOneShot(clip);
    }

    public void SetSFXVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
