using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : MonoBehaviour
{
    public MagicType currentMagicType = MagicType.None;
    public MagicState currentMagicState = MagicState.Idle;

    public bool Fire;
    public bool Ice;
    public bool Lightning;
    public bool Force;

    public bool Moving;

    public Rigidbody Gripped;
    public GameObject beamParticlePrefab;
    public GameObject bladeParticlePrefab;
    public GameObject forceFieldParticlePrefab;
    public GameObject bladeParticle2Prefab;
    public GameObject forceFieldParticle2Prefab;
    public GameObject leftHand;
    public GameObject rightHand;
    public ParticleSystem FireBeam;
    public ParticleSystem IceBeam;
    public ParticleSystem LightningBeam;
    public ParticleSystem ForceBeam;
    public ParticleSystem FireGather;
    public ParticleSystem IceGather;
    public ParticleSystem LightningGather;
    public ParticleSystem ForceGather;

    private GameObject activeBeam;//
    private GameObject activeBlade;
    private GameObject activeForceField;
    private GameObject activeBeam2;
    private GameObject activeBlade2;
    private GameObject activeForceField2;

    private Rigidbody telekinesisObject;
    private Transform RtelekinesisPoint;
    private Transform LtelekinesisPoint;
    public GameObject TelekinesisPointer;
    public GameObject TelekinesisPointer2;

    public string Gripper;

    public float forceMultiplier = 10f; // Adjust the force applied to the player
    private Rigidbody rb;
    private float timer;

    void Start()
    {
        gameObject.name = "Player";
        rb = gameObject.GetComponent<Rigidbody>();
        Fire = false;
        Ice = false;
        Lightning = false;
        Force = false;
        FireBeam.Stop();
        IceBeam.Stop();
        LightningBeam.Stop();
        ForceBeam.Stop();
        FireGather.Stop();
        IceGather.Stop();
        LightningGather.Stop();
        ForceGather.Stop();
    }

    void Update()
    {
        HandleMagicGathering();
        HandleMagicInteractions();
        IceBeam.transform.position = rightHand.transform.position;
       IceBeam.transform.rotation = rightHand.transform.rotation;
        FireBeam.transform.position = rightHand.transform.position;
        FireBeam.transform.rotation = rightHand.transform.rotation;
        FireGather.transform.position = rightHand.transform.position;
        FireGather.transform.rotation = rightHand.transform.rotation;
        IceGather.transform.position = rightHand.transform.position;
        IceGather.transform.rotation = rightHand.transform.rotation;

        LightningBeam.transform.position = leftHand.transform.position;
        LightningBeam.transform.rotation = leftHand.transform.rotation;
        LightningGather.transform.position = leftHand.transform.position;
        LightningGather.transform.rotation = leftHand.transform.rotation;
        ForceBeam.transform.position = leftHand.transform.position;
        ForceBeam.transform.rotation = leftHand.transform.rotation;
        ForceGather.transform.position = leftHand.transform.position;
        ForceGather.transform.rotation = leftHand.transform.rotation;
        TelekinesisPointer.transform.position = rightHand.transform.position;
        TelekinesisPointer.transform.rotation = rightHand.transform.rotation;
        TelekinesisPointer2.transform.position = leftHand.transform.position;
        TelekinesisPointer2.transform.rotation = leftHand.transform.rotation;

        if (!OVRInput.Get(OVRInput.Button.One) && !OVRInput.Get(OVRInput.Button.Two) && !OVRInput.Get(OVRInput.Button.Three) && !OVRInput.Get(OVRInput.Button.Four))
        {
            currentMagicType = MagicType.None;
            FireGather.Stop();
            IceGather.Stop();
            LightningGather.Stop();
            ForceGather.Stop();
        }


    }

    void FixedUpdate()
    {
        //Player movement
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        Vector3 forceDirection = transform.forward * input.y + transform.right * input.x;
        rb.AddForce(forceDirection.normalized * forceMultiplier, ForceMode.Force);
        if(forceDirection.magnitude > 0)
        {
            Moving = true;
        } 
        else
        {
            Moving = false;
        }

    }

    private void HandleMagicGathering()
    {
        if (OVRInput.GetDown(OVRInput.Button.One))
        {
           // currentMagicType = MagicType.Fire;
            FireGather.Play();
            Fire = true;
        } 
        else if (OVRInput.GetUp(OVRInput.Button.One))
        {
            FireGather.Stop();
            Fire = false;
        }
        if (OVRInput.GetDown(OVRInput.Button.Two))
        {
           // currentMagicType = MagicType.Ice;
            IceGather.Play();
            Ice = true;
        } 
        else if (OVRInput.GetUp(OVRInput.Button.Two))
        {
            IceGather.Stop();
            Ice = false;
        }
        if (OVRInput.GetDown(OVRInput.Button.Three))
        {
           // currentMagicType = MagicType.Lightning;
            LightningGather.Play();
            Lightning = true;
        }
        else if (OVRInput.GetUp(OVRInput.Button.Three))
        {
            LightningGather.Stop();
            Lightning = false;
        }
        
        if (OVRInput.GetDown(OVRInput.Button.Four))
        {
            //currentMagicType = MagicType.Force;
            ForceGather.Play();
            Force = true;
        }
        else if (OVRInput.GetUp(OVRInput.Button.Four))
        {
            ForceGather.Stop();
            Force = false;
        }

        if (Fire == true || Ice == true || Lightning == true || Force == true)
        {
            currentMagicState = MagicState.Gathering;
        } 
        else 
        {
            currentMagicState = MagicState.Idle;
        }
    }

    private void HandleMagicInteractions()
    {
        HandleTelekinesisLogic();
        HandleTeleportationLogic();
        HandleLevitationLogic();

        //if (currentMagicState == MagicState.Idle) return;

        HandleBeamBladeForceFieldLogic();
        
    }



    private void HandleBeamBladeForceFieldLogic()
    {
        if (currentMagicState != MagicState.Gathering)
        {
           FireBeam.Stop();
           IceBeam.Stop();
           LightningBeam.Stop();
           ForceBeam.Stop();
           // if (activeBlade != null) Destroy(activeBlade);
           // if (activeForceField != null) Destroy(activeForceField);
            return;
        }

        // Left hand: Beam logic
        if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            if (Fire == true)
            {
                FireBeam.Play();
            }
            else
            {
                FireBeam.Stop();
            }
            if (Ice == true)
            {
                IceBeam.Play();
            }
            else
            {
                IceBeam.Stop();
            }
        }
        if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && !OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
        { 
            if (Lightning == true)
            {
                LightningBeam.Play();
            }
            else
            {
                LightningBeam.Stop();
            }
            if (Force == true)
            {
                ForceBeam.Play();
            }
            else
            {
                ForceBeam.Stop();
            }
            //activeBeam = Instantiate(beamParticlePrefab, leftHand.transform.position, leftHand.transform.rotation, leftHand.transform);
        }
        if (!OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            //Destroy(activeBeam);
            LightningBeam.Stop();
            ForceBeam.Stop();
        }
        if (!OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
        {
            FireBeam.Stop();
            IceBeam.Stop();
        }

        //Blade is a prefab that is trained to try to follow the player's hand. It self-destructs on contact with anything in a layer that is not the Player layer. For ease, it should be in a Spells layer.
        // Right hand: Blade and ForceField logic
        if (OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
        {
           // if (activeBlade == null) activeBlade = Instantiate(bladeParticlePrefab, rightHand.transform.position, rightHand.transform.rotation);
            if (activeForceField == null) activeForceField = Instantiate(forceFieldParticlePrefab, rightHand.transform.position, rightHand.transform.rotation);
        }
        else if (OVRInput.GetUp(OVRInput.Button.SecondaryHandTrigger))
        {
            //if (activeBlade != null) Destroy(activeBlade);
           // if (activeForceField != null) Destroy(activeForceField);
        }

        // Left hand
        if (OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
        {
           //if (activeBlade2 == null) activeBlade2 = Instantiate(bladeParticle2Prefab, leftHand.transform.position, leftHand.transform.rotation);
            if (activeForceField2 == null) activeForceField2 = Instantiate(forceFieldParticle2Prefab, leftHand.transform.position, leftHand.transform.rotation);
        }
        else if (OVRInput.GetUp(OVRInput.Button.PrimaryHandTrigger))
        {
          //  if (activeBlade2 != null) Destroy(activeBlade2);
           // if (activeForceField2 != null) Destroy(activeForceField2);
        }
    }

    private void HandleTelekinesisLogic()
    {
        if (currentMagicState != MagicState.Idle && currentMagicState != MagicState.Telekinesing) return;
        LayerMask objectmask = LayerMask.GetMask("Objects", "Magic");
        // For the right hand
        Ray rayRight = new Ray(rightHand.transform.position, rightHand.transform.forward);
        RaycastHit hitRight;

        // For the left hand
        Ray rayLeft = new Ray(leftHand.transform.position, leftHand.transform.forward);
        RaycastHit hitLeft;

        // Process for the right hand
        if (Physics.Raycast(rayRight, out hitRight, Mathf.Infinity, objectmask))
        {
            Debug.DrawLine(rightHand.transform.position, hitRight.point, Color.red);
            Gripper = "right";
            Gripped = hitRight.collider.GetComponent<Rigidbody>();
            currentMagicState = MagicState.Telekinesing;
        }
            ProcessTelekinesisLogic(hitRight, Gripped);
        
         //Process for the left hand
       if (Physics.Raycast(rayLeft, out hitLeft, Mathf.Infinity, objectmask))
       {
          Debug.DrawLine(leftHand.transform.position, hitLeft.point, Color.green);
          Gripper = "left";
          Gripped = hitLeft.collider.GetComponent<Rigidbody>();
          currentMagicState = MagicState.Telekinesing;

       }
           ProcessTelekinesisLogic(hitLeft, Gripped);
    }

    private void ProcessTelekinesisLogic(RaycastHit hit, Rigidbody rb)
    {
       // if (Gripped != null)
       // {
          //  Rigidbody rb = Gripped;
       // } 
        // Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
        if (rb)
        {

            if (Gripper == "right")
            {
                Vector3 direction;
                if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
                {
                    direction = (rightHand.transform.position - rb.transform.position).normalized;
                    rb.AddForce(direction * 5f, ForceMode.Force);
                    if (RtelekinesisPoint == null)
                    {
                        RtelekinesisPoint = new GameObject("TelekinesisPoint").transform;
                        RtelekinesisPoint.SetParent(rightHand.transform);
                        RtelekinesisPoint.position = rb.transform.position;
                    } 
                    else 
                    {
                        RtelekinesisPoint.position = rb.transform.position;
                    }
                }
                if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
                {
                    direction = (rb.transform.position - rightHand.transform.position).normalized;
                    rb.AddForce(direction * 5f, ForceMode.Force);
                    if (RtelekinesisPoint == null)
                    {
                        RtelekinesisPoint = new GameObject("TelekinesisPoint").transform;
                        RtelekinesisPoint.SetParent(rightHand.transform);
                        RtelekinesisPoint.position = rb.transform.position;
                    }
                    else
                    {
                        RtelekinesisPoint.position = rb.transform.position;
                    }
                }
                if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
                {
                    rb.useGravity = false;
                    //rb.velocity = Vector3.zero;
                   // rb.angularVelocity = Vector3.zero;

                    if (RtelekinesisPoint == null)
                    {
                        RtelekinesisPoint = new GameObject("TelekinesisPoint").transform;
                        RtelekinesisPoint.SetParent(rightHand.transform);
                        RtelekinesisPoint.position = rb.transform.position;
                    }
                    direction = (RtelekinesisPoint.position - rb.transform.position);
                    rb.AddForce(direction * 5f, ForceMode.Force);
                }
                if (RtelekinesisPoint && !OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
                {
                    rb.useGravity = true;
                    Gripped = null;
                    currentMagicState = MagicState.Idle;
                    Destroy(RtelekinesisPoint.gameObject);
                }
            }
            if (Gripper == "left")
            {
                Vector3 direction2;
                if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && !OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
                {
                    direction2 = (leftHand.transform.position - rb.transform.position).normalized;
                    rb.AddForce(direction2 * 5f, ForceMode.Force);
                    if (LtelekinesisPoint == null)
                    {
                        LtelekinesisPoint = new GameObject("TelekinesisPoint2").transform;
                        LtelekinesisPoint.SetParent(leftHand.transform);
                        LtelekinesisPoint.position = rb.transform.position;
                    }
                    else
                    {
                        LtelekinesisPoint.position = rb.transform.position;
                    }
                }
                if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
                {
                    direction2 = (rb.transform.position - leftHand.transform.position).normalized;
                    rb.AddForce(direction2 * 5f, ForceMode.Force);
                    if (LtelekinesisPoint == null)
                    {
                        LtelekinesisPoint = new GameObject("TelekinesisPoint2").transform;
                        LtelekinesisPoint.SetParent(leftHand.transform);
                        LtelekinesisPoint.position = rb.transform.position;
                    }
                    else
                    {
                        LtelekinesisPoint.position = rb.transform.position;
                    }
                }
                if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
                {
                    rb.useGravity = false;
                    //rb.velocity = Vector3.zero;
                   // rb.angularVelocity = Vector3.zero;
                    //
                    if (LtelekinesisPoint == null)
                    {
                        LtelekinesisPoint = new GameObject("TelekinesisPoint2").transform;
                        LtelekinesisPoint.SetParent(leftHand.transform);
                        LtelekinesisPoint.position = rb.transform.position;
                    }
                    direction2 = (LtelekinesisPoint.position - rb.transform.position);
                    rb.AddForce(direction2 * 5f, ForceMode.Force);
                }
                if (LtelekinesisPoint && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && !OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
                {
                    rb.useGravity = true;
                    Gripped = null;
                    currentMagicState = MagicState.Idle;
                    Destroy(LtelekinesisPoint.gameObject);
                }
            }
        }
    }

    private void HandleLevitationLogic()
    {
        //if (currentMagicState == MagicState.Idle) return;

        // For the right hand
        LayerMask mask = LayerMask.GetMask("Ground");
        Ray rayRight = new Ray(rightHand.transform.position, rightHand.transform.forward);
        RaycastHit hitRight;

        // For the left hand
        Ray rayLeft = new Ray(leftHand.transform.position, leftHand.transform.forward);
        RaycastHit hitLeft;

        // Process for the right hand
        if (Physics.Raycast(rayRight, out hitRight, Mathf.Infinity, mask))
        {
            if (RtelekinesisPoint == null && Gripped == null)
            {
                ProcessLevitationLogic(hitRight);
            }
        }

        // Process for the left hand
        if (Physics.Raycast(rayLeft, out hitLeft, Mathf.Infinity, mask))
        {
            if (LtelekinesisPoint == null && Gripped == null)
            {
                ProcessLevitationLogic(hitLeft);
            }
        }
    }

    private void HandleTeleportationLogic()
    {
        if (timer <= 0f)
        {
            LayerMask mask = LayerMask.GetMask("Ground");
            if (OVRInput.Get(OVRInput.Button.One) && OVRInput.Get(OVRInput.Button.Two) && OVRInput.Get(OVRInput.Button.Three) && OVRInput.Get(OVRInput.Button.Four))
            {
                Ray TeleportRay = new Ray(rightHand.transform.position, rightHand.transform.forward);
                RaycastHit hit;
                if (Physics.Raycast(TeleportRay, out hit, Mathf.Infinity, mask))
                {
                    Teleport(hit.point);
                }
            }
            timer = 10f;
        }
        timer -= 1f * Time.deltaTime;
    }

    private void Teleport(Vector3 landingspot)
    {
            gameObject.transform.position = landingspot + new Vector3(0f, 1f, 0f);
 
    }

    private void ProcessLevitationLogic(RaycastHit hit)
    {
        if (currentMagicState != MagicState.Idle) return;

        Rigidbody playerRb = GetComponent<Rigidbody>();
        Vector3 direction;
        //Left Hand Levitation
        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            direction = (playerRb.transform.position - hit.point).normalized;
            playerRb.AddForce(direction*10f, ForceMode.Force);
            playerRb.useGravity = false;
        }
        else if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && !OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
        {
            direction = (hit.point - playerRb.transform.position).normalized;
            playerRb.AddForce(direction * 10f, ForceMode.Force);
            playerRb.useGravity = false;
        }
        else if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
        {
            playerRb.useGravity = false;
            if (Moving == false)
            {
                playerRb.velocity = Vector3.zero;
            }
        }
        else if (!OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && !OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
        {
           // playerRb.velocity = Vector3.zero;
            playerRb.useGravity = true;
        }
        //Right Hand Levitation
        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
        {
            direction = (playerRb.transform.position - hit.point).normalized;
            playerRb.AddForce(direction * 10f, ForceMode.Force);
            playerRb.useGravity = false;
        }
        else if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            direction = (hit.point - playerRb.transform.position).normalized;
            playerRb.AddForce(direction * 10f, ForceMode.Force);
            playerRb.useGravity = false;
        }
        else if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            playerRb.useGravity = false;
            if (Moving == false)
            {
                playerRb.velocity = Vector3.zero;
            }
        }
        else if (!OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            // playerRb.velocity = Vector3.zero;
            playerRb.useGravity = true;
        }

    }
}

public enum MagicType
{
    Fire,
    Ice,
    Lightning,
    Force,
    None
}

public enum MagicState
{
    Idle,
    Gathering,
    Infusing,
    Telekinesing
}
