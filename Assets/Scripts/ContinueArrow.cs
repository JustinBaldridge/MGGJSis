using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ContinueArrow : MonoBehaviour//, IPointerEnterHandler, IPointerExitHandler
{
    public static event EventHandler OnAnyContinue;

    [SerializeField] Button button;
    [SerializeField] AnimationCurve animCurve;
    [SerializeField] AudioClip selectedSound; 

    [SerializeField] float heightAmplitude;
    [SerializeField] float duration;
    
    RectTransform rectTransform;
    float timer = 0;
    bool isActive = true;
    Vector2 basePosition;
    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        basePosition = rectTransform.anchoredPosition;
        button.onClick.AddListener(() => {
            SFXPlayer.PlaySound(selectedSound);
            OnAnyContinue?.Invoke(this, EventArgs.Empty);
        });
    }


    void Update()
    {
        if (!isActive) return;

        timer += Time.deltaTime;
        float timerDivided = timer / duration;
        float timerDecimal = timerDivided - ((int) timerDivided);
        rectTransform.anchoredPosition = basePosition  + new Vector2(0, heightAmplitude * animCurve.Evaluate(timerDecimal)); 
    }

    /*
    void OnPointerEnter(PointerEventData eventData)
    {

    }

    void OnPointerExit(PointerEventData eventData)
    {

    }*/
}
