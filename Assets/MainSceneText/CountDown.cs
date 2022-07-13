using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class CountDown : MonoBehaviour
{
    public static float CountDownTime;
    public Text TextCountDown;
    public int CountDownFlag;
    public bool TakeScreenshot;
    public static CountDown instance;

    void Start()
    {
        CountDownTime = 5.0F;
        CountDownFlag = 0;
        TakeScreenshot = false;

        //CountDownFlagをとるための処理
        if(instance == null)
        {
            instance = this;
        }        
    }

    // Update is called once per frame
    void Update()
    {
        TextCountDown.text = String.Format("{0:00.00}", CountDownTime);
        CountDownTime -= Time.deltaTime;
        CountDownFlag = 0;

        if(CountDownTime < 0.0F)
        {
            CountDownTime = 5.0F;
            CountDownFlag = 1;
            TakeScreenshot = true;
        }
    }
}
