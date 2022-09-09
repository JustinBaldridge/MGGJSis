using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StarIndicatorUI : MonoBehaviour
{
    [SerializeField] Image starImage;
    [SerializeField] Sprite blankStar;
    [SerializeField] Sprite fullStar;

    public void Show()
    {
        starImage.sprite = fullStar;
    }

    public void Hide()
    {
        starImage.sprite = blankStar;
    }
}
