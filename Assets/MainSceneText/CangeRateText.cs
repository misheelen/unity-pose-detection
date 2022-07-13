using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CangeRateText : MonoBehaviour
{
    public Text text;
    public int RateNum;
    public int FPScount;
    public GameObject RateTex;
    // Start is called before the first frame update
    void Start()
    {
        RateNum = 0;
        RateTex.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    { 
        if(FPScount == 500)
        {
            RateTex.gameObject.SetActive(false);
            FPScount = 0;            
        }
        else if(FPScount != 0)
        {
            FPScount += 1;
        }

        
        if(CountDown.instance.CountDownFlag == 1)
        {
            
            RateNum = Random.Range(0, 3);
            
            if(RateNum == 0)
            {
                text.text = "Bad";
            }
            else if(RateNum == 1)
            {
                text.text = "Good";
            }
            else if(RateNum == 2)
            {
                text.text = "Excellent";
            }
            
            RateTex.gameObject.SetActive(true);
            FPScount = 1;
        }
        
    }
}
