using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public static StartMenu Instance;

    public event EventHandler OnStartGame;
    public event EventHandler<SFXEventArgs> OnUpdateSFXVolume;

    public class SFXEventArgs : EventArgs
    {
        public float value;
    }

    [Header ("Main Menu")]
    [SerializeField] GameObject mainMenu;
    [Header ("Options")]
    [SerializeField] GameObject optionsMenu;
    [SerializeField] Slider musicSlider;
    [SerializeField] Slider sfxSlider;
    [SerializeField] Toggle screenShakeToggle;
    [Header ("Credits")]
    [SerializeField] GameObject creditsMenu;
    [Header ("Victory")]
    [SerializeField] GameObject victoryMenu;
    [Header ("How To Play")]
    [SerializeField] GameObject howToPlayMenu;
    [Header ("Game Over")]
    [SerializeField] GameObject gameOverMenu;
    
    [SerializeField] AudioClip gameStartSound;
    [SerializeField] AudioClip menuClickSound;
    [SerializeField] float moveSpeed;

    RectTransform rectTransform;
    Vector2 initialPosition; 
    Vector2 targetPosition;
    bool isActive;
    bool startingGame;
    float savedSFXValue;

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
        initialPosition = rectTransform.anchoredPosition;
    }

    // Start is called before the first frame update
    void Start()
    {  
        UnitManager.Instance.OnKingTaken += UnitManager_OnKingTaken;
        BackgroundSmoke.Instance.OnWinScreenEntered += BackgroundSmoke_OnWinScreenEntered;

        musicSlider.onValueChanged.AddListener(delegate {
                SetMusicVolume(musicSlider.value / 10f);
            });
        sfxSlider.onValueChanged.AddListener(delegate {
                SetSFXVolume(sfxSlider.value / 10f);
            });
        screenShakeToggle.onValueChanged.AddListener(delegate {
                SFXPlayer.PlaySound(menuClickSound);
                SetScreenShake(screenShakeToggle.isOn);
            });
        SetMusicVolume(musicSlider.value);
        SetSFXVolume(sfxSlider.value);
        SetScreenShake(screenShakeToggle.isOn);
        RestartGame();
    }

    void RestartGame()
    {
        targetPosition = Vector2.zero; 
        BackgroundSmoke.Instance.Restart();
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;
        if (Vector2.Distance(rectTransform.anchoredPosition, targetPosition) < .5f)
        {
            if (startingGame) OnStartGame?.Invoke(this, EventArgs.Empty);
            isActive = false;
        }
        else
        {
            rectTransform.anchoredPosition = Vector2.MoveTowards(rectTransform.anchoredPosition, targetPosition, moveSpeed * Time.deltaTime);
        }
    }

    public void StartGame()
    {
        isActive = true;
        targetPosition = rectTransform.anchoredPosition + (Vector2.down * 450);
        startingGame = true;
        SFXPlayer.PlaySound(gameStartSound);
    }
    
    public void OpenOptions()
    {
        SFXPlayer.PlaySound(menuClickSound);
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void OpenCredits()
    {
        SFXPlayer.PlaySound(menuClickSound);
        mainMenu.SetActive(false);
        creditsMenu.SetActive(true);
    }

    public void CloseOptions()
    {
        SFXPlayer.PlaySound(menuClickSound);
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }

    public void CloseCredits()
    {
        SFXPlayer.PlaySound(menuClickSound);
        mainMenu.SetActive(true);
        creditsMenu.SetActive(false);
    }

    public void OpenHowToPlay()
    {
        SFXPlayer.PlaySound(menuClickSound);
        mainMenu.SetActive(false);
        howToPlayMenu.SetActive(true);
    }

    public void CloseHowToPlay()
    {
        SFXPlayer.PlaySound(menuClickSound);
        mainMenu.SetActive(true);
        howToPlayMenu.SetActive(false);
    }

    public void QuitGame()
    {
        SFXPlayer.PlaySound(menuClickSound);
        Application.Quit();
    }

    public void OpenPage(string url)
    {
        SFXPlayer.PlaySound(menuClickSound);
        Application.OpenURL(url);
    }

    public void SetMusicVolume(float value)
    {
        MusicPlayer.Instance.SetMusicVolume(value);
    }

    public void SetSFXVolume(float value)
    {
        SFXPlayer.Instance.SetSFXVolume(value);
        OnUpdateSFXVolume?.Invoke(this, new SFXEventArgs {
            value = value
        });
    }

    public void SetScreenShake(bool value)
    {
        ScreenShake.Instance.SetScreenShake(value);
    }

    public float GetSFXValue()
    {
        return savedSFXValue;
    }

    public void CloseGameOver()
    {
        SFXPlayer.PlaySound(menuClickSound);
        rectTransform.anchoredPosition = Vector2.zero;
        BackgroundSmoke.Instance.Restart();
        victoryMenu.SetActive(false);
        gameOverMenu.SetActive(false);
        mainMenu.SetActive(true);
        startingGame = false;
    }

    private void UnitManager_OnKingTaken(object sender, EventArgs e)
    {
        isActive = true;
        targetPosition = initialPosition;
        startingGame = false;
        mainMenu.SetActive(false);
        gameOverMenu.SetActive(true);
    }

    void BackgroundSmoke_OnWinScreenEntered(object sender, EventArgs e)
    {
        rectTransform.anchoredPosition = Vector2.zero;
        mainMenu.SetActive(false);
        victoryMenu.SetActive(true);
    }
}
