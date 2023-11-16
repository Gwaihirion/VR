using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grower : MonoBehaviour
{
    public float MaxSize;
    private Vector3 Growth;
    public float GrowRate;
    public bool Growing;
    public float GrowTimer;
    // Start is called before the first frame update
    void Start()
    {
        Grower[] growerObjects = FindObjectsOfType<Grower>();

        // Check if the count of Grower objects exceeds 25
        if (growerObjects.Length > 25)
        {
            // Destroy the game object this script is attached to
            Destroy(gameObject);
        }
        GrowTimer = 60f;
        Growing = true;
        transform.rotation = Quaternion.Euler(0f, 0f, 90f);
        transform.localScale = new Vector3(.1f, .1f, .1f); 

    }

    // Update is called once per frame
    void Update()
    {
        Growth = new Vector3(GrowRate, GrowRate, GrowRate);

        if (Growing == true)
        { 
            GrowRate += Time.deltaTime; 
        }

        if (transform.localScale.x < MaxSize)
        {
            transform.localScale = Growth;
        }
        else
        { 
            Growing = false;
        }

        if(Growing == false)
        {
            GrowTimer -= Time.deltaTime;
            if (GrowTimer < 0f)
            {
                GrowRate -= Time.deltaTime;
                transform.localScale = Growth;
            }
        }
        if(transform.localScale.x < 0)
        {
            Destroy(gameObject);
        }

    }
    void OnParticleCollision(GameObject other)
    {
        if(other.CompareTag("Fire") || other.CompareTag("TowerFire"))
        {
            Growing = false;
            GrowTimer = 0f;
        }
        if(other.CompareTag("Ice") || other.CompareTag("TowerIce"))
        {
            Growing = true;
            GrowTimer = 60f;
            MaxSize+= 5f*Time.deltaTime;
        }
    }
}
