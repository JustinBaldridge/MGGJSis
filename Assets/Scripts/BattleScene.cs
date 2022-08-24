using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleScene : MonoBehaviour
{
    [SerializeField] AnimationCurve animCurve;

    //Vector3 standardPosition;
    [SerializeField] Vector3 targetPosition;
    float timer;
    float timeDuration = 2.5f;

    // Start is called before the first frame update
    void Start()
    {
        //standardPosition = Vector3.zero;
        timer = 0f;
    }

    void Update()
    {
        if (timer < 2.5f)
        {
            timer += Time.deltaTime;
            transform.position = targetPosition * animCurve.Evaluate(timer / timeDuration);//Vector3.Lerp(transform.position, targetPosition, Time.deltaTime);
        }
    }
}
