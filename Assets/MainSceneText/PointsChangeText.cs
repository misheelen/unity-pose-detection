using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointsChangeText : MonoBehaviour
{
    public GameObject LHMarker;
    public GameObject RHMarker;
    public GameObject FMarker;
    //それぞれの点のX,Y座標
    public Vector3 LHxy;
    public Vector3 RHxy;
    public Vector3 Fxy;
    public Transform LHM_transform;
    public Transform RHM_transform;
    public Transform FM_transform;
    public int LHColor;
    public int RHColor;
    public int FColor;
    public static PointsChangeText instance;

    public float[,] bolt_xy;
    public float[,] mike_xy;
    public float[,] satu_xy;
    public int count;
    

    // Start is called before the first frame update
    void Start()
    {
        if(instance == null)
        {
            instance = this;
        }    

        LHM_transform = LHMarker.GetComponent<Transform>();
        RHM_transform = RHMarker.GetComponent<Transform>();
        FM_transform = FMarker.GetComponent<Transform>();

        LHColor = 0;
        RHColor = 0;
        FColor = 0;


        // LHxy = new Vector3(Random.Range(-5.0F,5.0F),Random.Range(-5.0F,5.0F),100);
        // RHxy = new Vector3(Random.Range(-5.0F,5.0F),Random.Range(-5.0F,5.0F),100);
        // Fxy = new Vector3(Random.Range(-5.0F,5.0F),Random.Range(-5.0F,5.0F),100);
        // Debug.Log(LHxy);
        // Debug.Log(RHxy);
        LHxy = new Vector3(3.5F,0.0F,100);
        RHxy = new Vector3(-3.5F,0.0F,100);
        Fxy = new Vector3(0.0F,3.5F,100);
    
        LHM_transform.position = LHxy;
        RHM_transform.position = RHxy;
        FM_transform.position = Fxy;
        
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("Markers");
        // Debug.Log(LHxy);
        // Debug.Log(RHxy);
        // Debug.Log(Fxy);
        
        if(CountDown.instance.CountDownFlag == 1)
        {

            // LHxy = new Vector3(Random.Range(-5.0F,5.0F),Random.Range(-5.0F,5.0F),100);
            // RHxy = new Vector3(Random.Range(-5.0F,5.0F),Random.Range(-5.0F,5.0F),100);
            // Fxy = new Vector3(Random.Range(-5.0F,5.0F),Random.Range(-5.0F,5.0F),100);
    
            LHM_transform.position = LHxy;
            RHM_transform.position = RHxy;
            FM_transform.position = Fxy;

            // Reset color 
            LHColor = 0;
            RHColor = 0;
            FColor = 0;
            
        }
        
    }
}
