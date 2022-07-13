using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
public class ScoreText : MonoBehaviour
{
    public static int Score;
    public static ScoreText scoretxt; 
    public Text TextScore;
    
    // Start is called before the first frame update
    void Start()
    {
        if(scoretxt == null)
        {
            scoretxt = this;
        }    
        Score = 0;                
    }

    // Update is called once per frame
    void Update()
    {
        TextScore.text = String.Format("{0}", Score);

        // if(CountDown.instance.CountDownFlag == 1)
        // {
        //     Score += 10;
        // }
    }
}
