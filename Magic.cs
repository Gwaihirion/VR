using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Magic : MonoBehaviour
{
    public Camera portalA;
    public Transform portalB;
    public LineRenderer lineRenderer;

    public MagicType currentMagicType = MagicType.None;
    public MagicState currentMagicState = MagicState.Idle;

    public static bool isFire { get; private set; }
    public static bool isIce { get; private set; }
    public static bool isLightning { get; private set; }
    public static bool isForce { get; private set; }
    public static float IceCountPublic { get; private set; }

    public bool Fire;
    public bool Ice;
    public bool Lightning;
    public bool Force;

    public bool Moving;

    public Rigidbody GrippedR = null;
    public Rigidbody GrippedL = null;
    public GameObject HeldR = null;
    public GameObject HeldL = null;
    public GameObject beamParticlePrefab;
    public GameObject bladeParticlePrefab;
    public GameObject forceFieldParticlePrefab;
    public GameObject bladeParticle2Prefab;
    public GameObject forceFieldParticle2Prefab;
    public GameObject leftHand;
    public GameObject rightHand;
    public GameObject CenterEyeTracker;
    public ParticleSystem FireBeam;
    public ParticleSystem IceBeam;
    public ParticleSystem LightningBeam;
    public ParticleSystem ForceBeam;
    public ParticleSystem FireGather;
    public ParticleSystem IceGather;
    public ParticleSystem LightningGather;
    public ParticleSystem ForceGather;

    public ParticleSystemForceField LeftField;
    public ParticleSystemForceField RightField;

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
    public GameObject NosePointer;
    public GameObject TelekinesisParticleSpawner;
    public ParticleSystem TelekinesisParticles;

    public float FireCount;
    public float LightningCount;
    public float IceCount;
    public float ForceCount;
    public float MaxFire;
    public float MaxLightning;
    public float MaxIce;
    public float MaxForce;

    public GameObject LightningBlast;
    public GameObject FireBlast;
    public GameObject IceSpike;
    public GameObject IceWall;
    private float LightningTimer = 0f;

    public Ray rayRight;
    public Ray rayLeft;

    public float maxControllerDistance = 10f;
    public float LightningStrokeSpeed;

    public string Gripper;

    public float forceMultiplier = 10f; // Adjust the force applied to the player
    public float flightForceMultiplier = 20f;
    private Rigidbody rb;
    private float timer;

   /* private Vector3 previousLeftHandPosition;
    private Vector3 previousRightHandPosition;*/

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
        LeftField.gravity = 0f;
        RightField.gravity = 0f;

        FireCount = 10f;
        IceCount = 10f;
        LightningCount = 10f;
        ForceCount = 10f;

        /*previousLeftHandPosition = leftHand.transform.position;
        previousRightHandPosition = rightHand.transform.position;*/
    }

    void Update()
    {
        if(LtelekinesisPoint)
        {
            TelekinesisParticleSpawner.transform.position = LtelekinesisPoint.transform.position;
            TelekinesisParticles.Play();
        }
        if(RtelekinesisPoint)
        {
            TelekinesisParticleSpawner.transform.position = RtelekinesisPoint.transform.position;
            TelekinesisParticles.Play();
        }
        if(!RtelekinesisPoint && !LtelekinesisPoint)
        {
            TelekinesisParticleSpawner.transform.position = transform.position;
            TelekinesisParticles.Stop();
        }
        /*Vector3 LeftHandVelocity = (leftHand.transform.position - previousLeftHandPosition) / Time.deltaTime;
        LeftHandSpeed = LeftHandVelocity.magnitude;
        previousLeftHandPosition = LeftHand.transform.position;

        Vector3 RightHandVelocity = (rightHand.transform.position - previousRightHandPosition) / Time.deltaTime;
        RightHandSpeed = RightHandVelocity.magnitude;
        previousRightHandPosition = rightHand.transform.position*/;

        // Retrieve the positions of both controllers
        Vector3 leftControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.LTouch);
        Vector3 rightControllerPosition = OVRInput.GetLocalControllerPosition(OVRInput.Controller.RTouch);
        Vector3 centerPosition = CenterEyeTracker.transform.localPosition;

        // Compute the distance between the controllers
        float distance = Vector3.Distance(leftControllerPosition, rightControllerPosition);
        float LeftCenterDistance = Vector3.Distance(leftControllerPosition, centerPosition);
        float RightCenterDistance = Vector3.Distance(rightControllerPosition, centerPosition);

        LeftField.directionZ = MapDistanceToForce(LeftCenterDistance);
        RightField.directionZ = MapDistanceToForce(RightCenterDistance);

        // Map the distance to gravity strength (modify this mapping as needed)
        if (Lightning || Force)
        {
            //Debug.Log("Current Left Hand Distance = " + LeftCenterDistance);
            
            LeftField.gravity = MapDistanceToGravity(distance, 5f);
            LeftField.endRange = distance*30f;
            RightField.endRange = distance * 30f;
        } else { LeftField.gravity = 0f; }

        if (Fire || Ice)
        {
            RightField.gravity = MapDistanceToGravity(distance, 5f);
            RightField.endRange = 2f + distance*30f;
            LeftField.endRange = 2f + distance*30f;
        } else { RightField.gravity = 0f; }

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
        if (!OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger)) 
        {
            GrippedL = null;
            HeldL = null;
            if(LtelekinesisPoint)
            {
                Destroy(LtelekinesisPoint.gameObject);
            }    
        }
        if (!OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
        {
            GrippedR = null;
            HeldR = null;
            if (RtelekinesisPoint)
            {
                Destroy(RtelekinesisPoint.gameObject);
            }
        }


    }

    float MapDistanceToGravity(float distance, float max)
    {
        distance = Mathf.Clamp(distance, 0f, maxControllerDistance);

        // Invert the distance to map it to gravity: Close distance yields high gravity, far distance yields negative gravity
        return Mathf.Lerp(max, 0f, distance / maxControllerDistance);
    }
    float MapDistanceToForce(float distance)
    {
        distance = Mathf.Clamp(distance, 0f, maxControllerDistance);

        // Invert the distance to map it to gravity: Close distance yields high gravity, far distance yields negative gravity
        return Mathf.Lerp(-5f, 7f, distance / maxControllerDistance);
    }

    void FixedUpdate()
    {
        //Track mana amounts for different spells, alter emission rates for each
        FireCount = Mathf.Clamp(FireCount, 0, MaxFire);
        IceCount = Mathf.Clamp(IceCount, 0, MaxIce);
        LightningCount = Mathf.Clamp(LightningCount, 0, MaxLightning);
        ForceCount = Mathf.Clamp(ForceCount, 0, MaxForce);
        IceCountPublic = IceCount;

        if (Fire == true)
        {
            FireCount -= 1 * Time.deltaTime;
            var FireG = FireGather.emission;
            var FireB = FireBeam.emission;

            FireG.rateOverTime = FireCount;
            FireB.rateOverTime = FireCount;
        }
        if (Ice == true)
        {
            IceCount -= 1 * Time.deltaTime;
            var IceG = IceGather.emission;
            var IceB = IceBeam.emission;

            IceG.rateOverTime = IceCount;
            IceB.rateOverTime = IceCount;
        }
        if (Lightning == true)
        {
            LightningCount -= 1 * Time.deltaTime;
            var LightningG = LightningGather.emission;
            var LightningB = LightningBeam.emission;

            LightningG.rateOverTime = LightningCount;
            LightningB.rateOverTime = LightningCount;
        }
        if (Force == true)
        {
            ForceCount -= 1 * Time.deltaTime;
            var ForceG = ForceGather.emission;
            var ForceB = ForceBeam.emission;

            ForceG.rateOverTime = ForceCount;
            ForceB.rateOverTime = ForceCount;
        }
        //Broadcast what magic I'm currently using, for powertowers;
        isFire = Fire;
        isIce = Ice;
        isLightning = Lightning;
        isForce = Force;

        if(GrippedL != null && GrippedR == null) { ProcessTelekinesisLogic(null, GrippedL); }
        if(GrippedR != null && GrippedL == null) { ProcessTelekinesisLogic(GrippedR, null); }
        if(GrippedL && GrippedR) { ProcessTelekinesisLogic(GrippedR, GrippedL); }
        LayerMask EyeMask = LayerMask.GetMask("Ground", "Enemies", "Walls", "Default");
        Ray EyeRays = new Ray(CenterEyeTracker.transform.position, CenterEyeTracker.transform.forward);
        RaycastHit hit1;
        if (Physics.Raycast(EyeRays, out hit1, Mathf.Infinity, EyeMask))
        {
            NosePointer.transform.position = hit1.point;
        }

        //Player movement
        Vector2 input = OVRInput.Get(OVRInput.Axis2D.PrimaryThumbstick);
        Vector3 forceDirection = transform.forward * input.y + transform.right * input.x;
        rb.AddForce(forceDirection.normalized * forceMultiplier, ForceMode.Force);

        Vector2 input2 = OVRInput.Get(OVRInput.Axis2D.SecondaryThumbstick);
        float scalingFactor = Mathf.Abs(input2.y); // Use absolute value for scaling regardless of direction

        // Optional: For more refined control, you can use a non-linear scale, like a quadratic scale
        scalingFactor = scalingFactor * scalingFactor;

        Vector3 forceDirection2 = transform.up * input2.y;
        rb.AddForce(forceDirection2.normalized * flightForceMultiplier * scalingFactor, ForceMode.Force);

        if (forceDirection.magnitude > 0)
        {
            Moving = true;
        } 
        else
        {
            Moving = false;
        }

        if(Lightning && HandMotionTracker.LeftHandSpeed > LightningStrokeSpeed && HandMotionTracker.SimplifiedLeftHandDirection.Contains("Down") )
        {
            LayerMask mask = LayerMask.GetMask("Ground", "Enemies", "Walls");
            Ray LightningBlastRay = new Ray(CenterEyeTracker.transform.position, CenterEyeTracker.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(LightningBlastRay, out hit, Mathf.Infinity, mask) && LightningTimer < 0f)
            {
                GameObject Stroke;
                Stroke = Instantiate(LightningBlast, hit.point, Quaternion.identity);
                LightningTimer = 1f;
            }
            
        }
        if (Fire && HandMotionTracker.RightHandSpeed > LightningStrokeSpeed && HandMotionTracker.SimplifiedRightHandDirection.Contains("Up"))
        {
            LayerMask mask = LayerMask.GetMask("Ground", "Enemies", "Walls");
            Ray FireBlastRay = new Ray(CenterEyeTracker.transform.position, CenterEyeTracker.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(FireBlastRay, out hit, Mathf.Infinity, mask) && LightningTimer < 0f)
            {
                GameObject Blammo;
                Blammo = Instantiate(FireBlast, hit.point, Quaternion.identity);
                LightningTimer = 1f;
            }

        }
        if (Ice && HandMotionTracker.RightHandSpeed > LightningStrokeSpeed && HandMotionTracker.SimplifiedRightHandDirection.Contains("Down"))
        {
            //New concept; you "strike" and an icicle appears, hovering in the air. You can drag it telekinetically, and it drops wall spikes everywhere you drag it (but only if there is no wall spike below it).
            LayerMask mask = LayerMask.GetMask("Ground");
            Ray IceSpikeRay = new Ray(CenterEyeTracker.transform.position, CenterEyeTracker.transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(IceSpikeRay, out hit, Mathf.Infinity, mask) && LightningTimer < 0f)
            {
                GameObject Blammo;
                Blammo = Instantiate(IceSpike, hit.point + new Vector3(0f,1f,0f), Quaternion.identity);
                Blammo.GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
                LightningTimer = 1f;
            }

        }
        /*        if (Ice && HandMotionTracker.RightHandSpeed*2f > LightningStrokeSpeed && HandMotionTracker.SimplifiedRightHandDirection.Contains("Right"))
                {
                    LayerMask mask = LayerMask.GetMask("Ground");
                    Ray IceWallRay = new Ray(CenterEyeTracker.transform.position, CenterEyeTracker.transform.forward);
                    RaycastHit hit;
                    if (Physics.Raycast(IceWallRay, out hit, Mathf.Infinity, mask) && LightningTimer < 1f)
                    {
                        GameObject Wallo;
                        Wallo = Instantiate(IceWall, hit.point, transform.rotation);
                        Wallo.GetComponent<AutoPlace>().IsRight = true;
                        LightningTimer = 1f;
                    }

                }
                if (Ice && HandMotionTracker.RightHandSpeed * 2f > LightningStrokeSpeed && HandMotionTracker.SimplifiedRightHandDirection.Contains("Left"))
                {
                    LayerMask mask = LayerMask.GetMask("Ground");
                    Ray IceWallRay = new Ray(CenterEyeTracker.transform.position, CenterEyeTracker.transform.forward);
                    RaycastHit hit;
                    if (Physics.Raycast(IceWallRay, out hit, Mathf.Infinity, mask) && LightningTimer < 1f)
                    {
                        GameObject Wallo;
                        Wallo = Instantiate(IceWall, hit.point, transform.rotation);
                        Wallo.GetComponent<AutoPlace>().IsRight = false;
                        LightningTimer = 1f;
                    }

                }*/
        LightningTimer -= 1f * Time.deltaTime;

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
        //if (currentMagicState != MagicState.Idle && currentMagicState != MagicState.Telekinesing) return;
        // if (GrippedL ) return;
        if (Fire || Ice || Lightning || Force) return;
        LayerMask objectmask = LayerMask.GetMask("Objects", "Magic", "Default", "Enemies");
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
            // currentMagicState = MagicState.Telekinesing;    
            if (!GrippedR)
            {
                GrippedR = hitRight.collider.GetComponent<Rigidbody>();
            }
        }
        
         //Process for the left hand
       if (Physics.Raycast(rayLeft, out hitLeft, Mathf.Infinity, objectmask))
       {

                Debug.DrawLine(leftHand.transform.position, hitLeft.point, Color.green);
                Gripper = "left";
            //currentMagicState = MagicState.Telekinesing;
            if (!GrippedL)
            {
                GrippedL = hitLeft.collider.GetComponent<Rigidbody>();
            }
       }
    }


    public void ProcessTelekinesisLogic(Rigidbody rb, Rigidbody lb)
    {
        // if (Gripped != null)
        // {
        //  Rigidbody rb = Gripped;
        // } 
        // Rigidbody rb = hit.collider.GetComponent<Rigidbody>();
        if (rb)
        {//
            if (rb.gameObject.GetComponent<EnemyController>())
            {
                rb.gameObject.GetComponent<EnemyController>().GoRagdoll();
                rb.gameObject.GetComponent<EnemyController>().downtime = 10f;
                rb = rb.gameObject.GetComponent<EnemyController>().secondaryRigidbody;
            }
        }
        if (lb)
        {
            if (lb.gameObject.GetComponent<EnemyController>())
            {
                lb.gameObject.GetComponent<EnemyController>().GoRagdoll();
                lb.gameObject.GetComponent<EnemyController>().downtime = 10f;
                lb = lb.gameObject.GetComponent<EnemyController>().secondaryRigidbody;
            }
        }
        if (GrippedR)
        {
            Vector3 HandGripPoint = rightHand.transform.position + (rightHand.transform.forward*rb.transform.gameObject.GetComponent<Collider>().bounds.extents.magnitude);
            Vector3 direction;
            float speed;
            if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            {
                if (!HeldR) HeldR = rb.transform.gameObject;
                speed = 10f;
                if (RtelekinesisPoint == null)
                {
                    RtelekinesisPoint = new GameObject("TelekinesisPoint").transform;
                    RtelekinesisPoint.SetParent(rightHand.transform);

                    RtelekinesisPoint.position = HandGripPoint + (rightHand.transform.forward * ((rb.transform.position - rightHand.transform.position).magnitude / 2));
                }
                else
                {
                    RtelekinesisPoint.position = HandGripPoint + (rightHand.transform.forward * ((rb.transform.position - rightHand.transform.position).magnitude / 2));
                }
                direction = (RtelekinesisPoint.position - rb.transform.position);

                rb.AddForce(direction.normalized * Mathf.Clamp(direction.magnitude, 0, 10), ForceMode.VelocityChange);
                rb.drag = Mathf.Clamp(rb.drag, 10, 0 * (1 - (direction.magnitude / 100f)));
            }
            if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
            {
                if (!HeldR) HeldR = rb.transform.gameObject;
                speed = 10f;
                if (RtelekinesisPoint == null)
                {
                    RtelekinesisPoint = new GameObject("TelekinesisPoint").transform;
                    RtelekinesisPoint.SetParent(rightHand.transform);
                    RtelekinesisPoint.position = rb.transform.position + (rightHand.transform.forward * speed);
                }
                else
                {
                    RtelekinesisPoint.position = rb.transform.position + (rightHand.transform.forward * speed);
                }
                direction = (RtelekinesisPoint.position - rb.transform.position);
                rb.AddForce(direction.normalized * Mathf.Clamp(direction.magnitude, 0, 10), ForceMode.VelocityChange);
                rb.drag = Mathf.Clamp(rb.drag, 10, 0 * (1 - (direction.magnitude / 100f)));

            }
            if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            {
                if (!HeldR) HeldR = rb.transform.gameObject;
                rb.useGravity = false;
                //rb.velocity = Vector3.zero;
                // rb.angularVelocity = Vector3.zero;

                if (RtelekinesisPoint == null || OVRInput.GetDown(OVRInput.Button.SecondaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.SecondaryHandTrigger))
                {
                    RtelekinesisPoint = new GameObject("TelekinesisPoint").transform;
                    RtelekinesisPoint.SetParent(rightHand.transform);
                    RtelekinesisPoint.position = rightHand.transform.position + (rightHand.transform.forward * (rb.transform.position - rightHand.transform.position).magnitude);
                }

                direction = (RtelekinesisPoint.position - rb.transform.position);
                rb.AddForce(direction.normalized * Mathf.Clamp(direction.magnitude, 0, 10), ForceMode.VelocityChange);
                rb.drag = Mathf.Clamp(rb.drag, 10, 0 * (1 - (direction.magnitude / 100f)));
                LayerMask objectmask = LayerMask.GetMask("Enemies");
                // For the right hand
                Ray rayRight = new Ray(rightHand.transform.position, rightHand.transform.forward);
                RaycastHit hitRight;
                if (Physics.Raycast(rayRight, out hitRight, Mathf.Infinity, objectmask) && hitRight.collider.GetComponent<Rigidbody>() != rb)
                {
                    RtelekinesisPoint.position = hitRight.point;
                    rb.AddForce(direction.normalized * Mathf.Clamp(direction.magnitude, 0, 10), ForceMode.VelocityChange);
                }
            }
            if (RtelekinesisPoint && !OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
            {
                if (HeldR) HeldR = null;
                rb.useGravity = true;
                rb.drag = 0f;
                GrippedR = null;
                currentMagicState = MagicState.Idle;
                Destroy(RtelekinesisPoint.gameObject);
            }
        }
        if (GrippedL)
        {
            Vector3 direction2;
            float speed;
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && !OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
            {
                if (!HeldL) HeldL = lb.transform.gameObject;
                speed = 10f;
                if (LtelekinesisPoint == null)
                {
                    LtelekinesisPoint = new GameObject("TelekinesisPoint").transform;
                    LtelekinesisPoint.SetParent(leftHand.transform);
                    LtelekinesisPoint.position = leftHand.transform.position + (leftHand.transform.forward * ((lb.transform.position - leftHand.transform.position).magnitude / 2));
                }
                else
                {
                    LtelekinesisPoint.position = leftHand.transform.position + (leftHand.transform.forward * ((lb.transform.position - leftHand.transform.position).magnitude / 2));
                }
                direction2 = (LtelekinesisPoint.position - lb.transform.position);

                lb.AddForce(direction2.normalized * Mathf.Clamp(direction2.magnitude, 0, 10), ForceMode.VelocityChange);
                lb.drag = Mathf.Clamp(lb.drag, 10, 0 * (1 - (direction2.magnitude / 100f)));
            }
            if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
            {
                if (!HeldL) HeldL = lb.transform.gameObject;
                speed = 10f;
                if (LtelekinesisPoint == null)
                {
                    LtelekinesisPoint = new GameObject("TelekinesisPoint").transform;
                    LtelekinesisPoint.SetParent(leftHand.transform);
                    LtelekinesisPoint.position = lb.transform.position + (leftHand.transform.forward * speed);
                }
                else
                {
                    LtelekinesisPoint.position = lb.transform.position + (leftHand.transform.forward * speed);
                }
                direction2 = (LtelekinesisPoint.position - lb.transform.position);
                lb.AddForce(direction2.normalized * Mathf.Clamp(direction2.magnitude, 0, 10), ForceMode.VelocityChange);
                lb.drag = Mathf.Clamp(lb.drag, 10, 0 * (1 - (direction2.magnitude / 100f)));

            }
            if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
            {
                if (!HeldL) HeldL = lb.transform.gameObject;
                lb.useGravity = false;
                //lb.velocity = Vector3.zero;
                // lb.angularVelocity = Vector3.zero;

                if (LtelekinesisPoint == null || OVRInput.GetDown(OVRInput.Button.PrimaryIndexTrigger) || OVRInput.GetDown(OVRInput.Button.PrimaryHandTrigger))
                {
                    LtelekinesisPoint = new GameObject("TelekinesisPoint").transform;
                    LtelekinesisPoint.SetParent(leftHand.transform);
                    LtelekinesisPoint.position = leftHand.transform.position + (leftHand.transform.forward * (lb.transform.position - leftHand.transform.position).magnitude);
                }

                direction2 = (LtelekinesisPoint.position - lb.transform.position);
                lb.AddForce(direction2.normalized * Mathf.Clamp(direction2.magnitude, 0, 10), ForceMode.VelocityChange);
                lb.drag = Mathf.Clamp(lb.drag, 10, 0 * (1 - (direction2.magnitude / 100f)));
                LayerMask objectmask = LayerMask.GetMask("Enemies");
                // For the left hand
                Ray rayleft = new Ray(leftHand.transform.position, leftHand.transform.forward);
                RaycastHit hitleft;
                if (Physics.Raycast(rayleft, out hitleft, Mathf.Infinity, objectmask) && hitleft.collider.GetComponent<Rigidbody>() != rb)
                {
                    LtelekinesisPoint.position = hitleft.point;
                    lb.AddForce(direction2.normalized * Mathf.Clamp(direction2.magnitude, 0, 10), ForceMode.VelocityChange);
                }
            }
            if (LtelekinesisPoint && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && !OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
            {
                if (HeldL) HeldL = null;
                lb.useGravity = true;
                lb.drag = 0f;
                GrippedL = null;
                currentMagicState = MagicState.Idle;
                Destroy(LtelekinesisPoint.gameObject);
            }
        }
    }
    

    private void HandleLevitationLogic()
    {
        //if (currentMagicState == MagicState.Idle) return;

        // For the right hand
        LayerMask mask = LayerMask.GetMask("Ground");
        rayRight = new Ray(rightHand.transform.position, rightHand.transform.forward);
        RaycastHit hitRight;

        // For the left hand
        rayLeft = new Ray(leftHand.transform.position, leftHand.transform.forward);
        RaycastHit hitLeft;

        // Process for the right hand
        if (Physics.Raycast(rayRight, out hitRight, Mathf.Infinity, mask))
        {
            if (RtelekinesisPoint == null && GrippedR == null)
            {
                ProcessRLevitationLogic(hitRight);
            }
        }

        // Process for the left hand
        if (Physics.Raycast(rayLeft, out hitLeft, Mathf.Infinity, mask))
        {
            if (LtelekinesisPoint == null && GrippedL == null)
            {
                ProcessLLevitationLogic(hitLeft);
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

    private void ProcessRLevitationLogic(RaycastHit Rhit)
    {
        if (currentMagicState != MagicState.Idle) return;

        Rigidbody playerRb = GetComponent<Rigidbody>();
        Vector3 direction;
        //Right Hand Levitation
        if (OVRInput.Get(OVRInput.Button.SecondaryHandTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger))
        {
            direction = (playerRb.transform.position - Rhit.point).normalized;
            playerRb.AddForce(direction * 10f, ForceMode.Force);
            playerRb.useGravity = false;
        }
        else if (OVRInput.Get(OVRInput.Button.SecondaryIndexTrigger) && !OVRInput.Get(OVRInput.Button.SecondaryHandTrigger))
        {
            direction = (Rhit.point - playerRb.transform.position).normalized;
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
    private void ProcessLLevitationLogic(RaycastHit Lhit)
    {
        if (currentMagicState != MagicState.Idle) return;

        Rigidbody playerRb = GetComponent<Rigidbody>();
        Vector3 direction;
        //Left Hand Levitation
        if (OVRInput.Get(OVRInput.Button.PrimaryHandTrigger) && !OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger))
        {
            direction = (playerRb.transform.position - Lhit.point).normalized;
            playerRb.AddForce(direction * 10f, ForceMode.Force);
            playerRb.useGravity = false;
        }
        else if (OVRInput.Get(OVRInput.Button.PrimaryIndexTrigger) && !OVRInput.Get(OVRInput.Button.PrimaryHandTrigger))
        {
            direction = (Lhit.point - playerRb.transform.position).normalized;
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

    }
    public void Loot(string LootType)
    {
        if (LootType == "Firemote")
        {
            FireCount += 10f;
        }
        if (LootType == "IceMote")
        {
            IceCount += 10f;
        }
        if (LootType == "LightningMote")
        {
            LightningCount += 10f;
        }
        if (LootType == "ForceMote")
        {
            ForceCount += 10f;
        }
    }

    private void OnParticleCollision(GameObject other)
    {
            // Check the tag of the particles and call the respective reaction function
            if (other.CompareTag("TowerFire"))
            {
            FireCount++;
            }
            else if (other.CompareTag("TowerIce"))
            {
            IceCount++;
            }
            else if (other.CompareTag("TowerLightning"))
            {
            LightningCount++;
            }
            else if (other.CompareTag("TowerForce"))
            {
            ForceCount++;
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

