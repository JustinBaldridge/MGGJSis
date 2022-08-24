using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] AnimationCurve animCurve;
    [SerializeField] Vector3 targetPosition;
    float timer;
    float timeDuration = 2f;

    bool isActive = true;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!isActive) return;

        if (timer < timeDuration)
        {
            timer += Time.deltaTime;
            transform.position = targetPosition * animCurve.Evaluate(timer / timeDuration);//Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
        }
        else
        {
            isActive = false;
            transform.position = -transform.position;
        }
    }
}
