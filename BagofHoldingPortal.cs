using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BagofHoldingPortal : MonoBehaviour
{
    public GameObject PortalMouth;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Enemy") || other.collider.CompareTag("Item") || other.collider.CompareTag("Target"))
        {
            other.collider.gameObject.transform.position = PortalMouth.transform.position;
            Debug.Log("Yoink!");
        }
    }

}
