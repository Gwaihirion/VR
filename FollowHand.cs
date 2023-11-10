using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowHand : MonoBehaviour
{
    public GameObject TargetHand;
    public GameObject Lorb;
    public GameObject Rorb;
    public Rigidbody Rb;
    public GameObject Blast;
    private GameObject Explosion;
    public List<GameObject> BlastTypes;
    public List<GameObject> OrbTypes;
    private Vector3 BigSize;
    private Vector3 scaleChange;
    private Vector3 distance;
    public string TargetHandName;
    private int Fire;
    private int Ice;
    private int Lightning;
    private int Force;
    // Start is called before the first frame update
    void Start()
    {
        Fire = 0;
        Ice = 0;
        Lightning = 0;
        Force = 0;

        if (TargetHandName == "right")
        {
            TargetHand = GameObject.FindWithTag("RightHand");
        }
        if (TargetHandName == "left")
        {
            TargetHand = GameObject.FindWithTag("LeftHand");
        }
    }

    // Update is called once per frame
    void Update()
    {
        int[] MagicType = { Fire, Ice, Lightning, Force };
        var max = Mathf.Max(MagicType);

        if (max == Fire)
        {
            Blast = BlastTypes[1];
            OrbTypes[1].SetActive(true);
            OrbTypes[2].SetActive(false);
            OrbTypes[3].SetActive(false);
            OrbTypes[4].SetActive(false);
            OrbTypes[0].SetActive(false);
        }
        if(max == Ice)
        {
            Blast = BlastTypes[2];
            OrbTypes[1].SetActive(false);
            OrbTypes[2].SetActive(true);
            OrbTypes[3].SetActive(false);
            OrbTypes[4].SetActive(false);
            OrbTypes[0].SetActive(false);
        }
        if(max == Lightning)
        {
            Blast = BlastTypes[3];
            OrbTypes[1].SetActive(false);
            OrbTypes[2].SetActive(false);
            OrbTypes[3].SetActive(true);
            OrbTypes[4].SetActive(false);
            OrbTypes[0].SetActive(false);
        }
        if(max == Force)
        {
            Blast = BlastTypes[4];
            OrbTypes[1].SetActive(false);
            OrbTypes[2].SetActive(false);
            OrbTypes[3].SetActive(false);
            OrbTypes[4].SetActive(true);
            OrbTypes[0].SetActive(false);
        }
        scaleChange = new Vector3(0.002f, 0.002f, 0.002f);
        BigSize += scaleChange;
        transform.localScale = BigSize;
        if ((TargetHandName == "right" && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger)) || (TargetHandName == "left" && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger)))
        {
            if (gameObject.name != "Lorb" && gameObject.name != "Rorb")
            {
                Vector3 direction = (TargetHand.transform.position - Rb.transform.position).normalized;
                Vector3 distance = (TargetHand.transform.position - Rb.transform.position);
                Rb.AddForce(distance, ForceMode.VelocityChange);

                if (TargetHandName == "right")
                {
                    if (OVRInput.Get(OVRInput.Button.One))
                    {
                        Fire++;
                    }
                    if (OVRInput.Get(OVRInput.Button.Two))
                    {
                        Ice++;
                    }
                }
                else
                {
                    if (OVRInput.Get(OVRInput.Button.Three))
                    {
                        Lightning++;
                    }
                    if (OVRInput.Get(OVRInput.Button.Four))
                    {
                        Force++;
                    }
                }
            }
        } 
        else
        {
            if (TargetHandName == "left" && OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                Destroy(gameObject, 20);
                Ray LeftRay = new Ray(TargetHand.transform.position, TargetHand.transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(LeftRay, out hit, Mathf.Infinity))
                {
                    Vector3 direction = (hit.point - TargetHand.transform.position).normalized;
                    gameObject.GetComponent<Rigidbody>().AddForce(direction*20f, ForceMode.Force);
                }
                return;
            }
            if (TargetHandName == "right" && OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
            {
                Destroy(gameObject, 20);
                Ray RightRay = new Ray(TargetHand.transform.position, TargetHand.transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(RightRay, out hit, Mathf.Infinity))
                {
                    Vector3 direction = (hit.point - TargetHand.transform.position).normalized;
                    gameObject.GetComponent<Rigidbody>().AddForce(direction * 20f, ForceMode.Force);
                }
                return;
            }
            Destroy(gameObject, 10);
            var currentSpeed = gameObject.GetComponent<Rigidbody>().velocity;
            gameObject.GetComponent<Rigidbody>().AddForce(currentSpeed*1.5f, ForceMode.Force);
            if (TargetHandName == "left")
            {
                gameObject.name = "Lorb";
                Lorb = GameObject.Find("Rorb");
                if (Lorb != null)
                {
                    Vector3 direction = (Lorb.transform.position - Rb.transform.position).normalized;
                    Rb.AddForce(direction * 20, ForceMode.Force);
                }
            }
            if (TargetHandName == "right")
            {
                gameObject.name = "Rorb";
                Rorb = GameObject.Find("Lorb");
                if (Rorb != null)
                {
                    Vector3 direction = (Rorb.transform.position - Rb.transform.position).normalized;
                    Rb.AddForce(direction * 20, ForceMode.Force);
                }
            }
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name != "Rorb" && collision.gameObject.name != "Lorb")
        {
            Explosion = Instantiate(Blast, transform.position, transform.rotation);
            Explosion.transform.localScale = BigSize;
            var rb = Explosion.GetComponent<Rigidbody>();
            rb.velocity = gameObject.GetComponent<Rigidbody>().velocity;
            Destroy(Explosion, 10);
            Destroy(gameObject);
        }
        else
        {
            Explosion = Instantiate(Blast, transform.position, transform.rotation);
            Explosion.transform.localScale = BigSize*2f; 
            Destroy(Explosion, 10);
            Destroy(gameObject);
        }
    }
}
