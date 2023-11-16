using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceSpike : MonoBehaviour
{
    public Rigidbody rb;
    public LayerMask Ground;
    public GameObject IcePillar;
    public float MaxTime;
    private float iceCounter;
    public float iceNumber;
    public float maxCount;
    public GameObject IceBlast;
    public ParticleSystem IceSplash;
    // Start is called before the first frame update
    void Start()
    {
        rb.velocity = new Vector3(0f, 0f, 0f);
        Destroy(gameObject, MaxTime);
        rb = gameObject.GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        iceCounter -= Time.deltaTime;
        RaycastHit hit;
        Ray IceRay = new Ray(transform.position, -transform.up);

            if (Physics.SphereCast(IceRay, 2f, out hit, Mathf.Infinity, Ground) && iceNumber < maxCount && !hit.collider.gameObject.CompareTag("Destructible"))
            {
                Instantiate(IcePillar, hit.point, Quaternion.identity);
            iceNumber++;
            }
    }
    void OnCollisionEnter()
    {
        if(rb.velocity.magnitude > 5f)
        {
            var radius = rb.velocity.magnitude * .25f;
            Debug.Log("Radius = " + radius);
            for (int i = 0; i < rb.velocity.magnitude; i++)
            {
                float angle = i * Mathf.PI * 2 / rb.velocity.magnitude;
                Vector3 position = new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle)) * radius;
                position += transform.position; // Center of the circle
                Vector3 Syzygy = new Vector3(position.x, 0f, position.z);
                RaycastHit hit;
                if (Physics.Raycast(Syzygy + Vector3.up * 1000, Vector3.down, out hit, Mathf.Infinity))
                {
                    position.y = hit.point.y; // Set y to the hit point's y coordinate
                }

                Instantiate(IcePillar, position, Quaternion.identity);
            }
            GameObject IceBlow = Instantiate(IceBlast, transform.position, Quaternion.identity);
            IceSplash = IceBlow.GetComponent<ParticleSystem>();
            var IceG = IceSplash.emission;
            var IceMain = IceSplash.main;
            IceMain.startSpeed = radius;
            IceMain.startLifetime = 1f;
            IceG.rateOverTime = radius*10f;
            IceSplash.Play();
            Destroy(gameObject);
        }
    }
}
