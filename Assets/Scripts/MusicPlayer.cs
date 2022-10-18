using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicPlayer : MonoBehaviour
{
    public static MusicPlayer Instance;

    float volume;
    AudioSource audioSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("There's more than one MusicPlayer! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;

        audioSource = GetComponent<AudioSource>();
    }

    public void SetMusicVolume(float volume)
    {
        audioSource.volume = volume;
    }
}
