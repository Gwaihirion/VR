using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chest : MonoBehaviour
{
    public HingeJoint springJoint;
    public GameObject Lid;
    public Vector3 LastPosition;
    public JointSpring spring;
    // Start is called before the first frame update
    void Start()
    {
        LastPosition = transform.position;
        springJoint = Lid.GetComponent<HingeJoint>();
        spring = springJoint.spring;
        spring.targetPosition = 90f;
    }

    // Update is called once per frame
    void Update()
    {
        if (LastPosition != transform.position)
        {
            spring.targetPosition = 90f;
            float LidTimer = 0f;
            LidTimer += 1*Time.deltaTime;
            if (LidTimer > 10f) {
                LastPosition = transform.position;
            }
        }
        else
        {
            if (spring.targetPosition > -90)
            {
                spring.targetPosition-= Time.deltaTime;
            }
        }
    }
}
