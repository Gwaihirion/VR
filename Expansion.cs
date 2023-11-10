using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Expansion : MonoBehaviour
{

    private Vector3 ScaleChange;
    public float ScaleRate;
    private float Timer;
    public float MaxSize;
    private float TrueMaxSize;

    private Vector3 NewSize;
    // Start is called before the first frame update


    void Start()
    {
        TrueMaxSize = MaxSize * transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        Timer++; 
        if(transform.localScale.x < TrueMaxSize && Timer < 20f)
        {
            ScaleRate+=1f*Time.deltaTime;
        } 
        else
        {
            ScaleRate-=1f*Time.deltaTime;
        }
        if(gameObject.transform.localScale.x <= 0f)
        {
            Destroy(gameObject);
        }
        
        ScaleChange = new Vector3(ScaleRate, ScaleRate, ScaleRate);
        NewSize += ScaleChange/10;
        transform.localScale = NewSize;
    }
}
