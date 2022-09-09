using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    private void Start()
    {
        TimeController.OnAnyTimeControllerFreezeFrame += TimeController_OnAnyTimeControllerFreezeFrame;
    }

    private void TimeController_OnAnyTimeControllerFreezeFrame(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(.25f);
    }
}
