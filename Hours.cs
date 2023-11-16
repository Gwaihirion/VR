using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hours : MonoBehaviour
{
    public float Speed;
    private Vector3 RotationChange;

    // Start is called before the first frame update
    void Start()
    {
       // RotationChange = new Vector3(Speed, 0f, 0f);    
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Speed * Time.deltaTime, 0f, 0f);
    }
}
