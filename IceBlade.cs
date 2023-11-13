using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBlade : MonoBehaviour
{
    public ParticleSystem myParticleSystem;
    private ParticleSystem.EmissionModule emissionModule;
    private ParticleSystem.MainModule mainModule;

    // Start is called before the first frame update
    void Start()
    {
        emissionModule = myParticleSystem.emission;
        mainModule = myParticleSystem.main;
        mainModule.startLifetime = 3f;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        emissionModule.rateOverTime = HandMotionTracker.RightHandSpeed*Magic.IceCountPublic;
        mainModule.startLifetime = HandMotionTracker.RightHandSpeed;
        mainModule.startSize = HandMotionTracker.RightHandSpeed*0.65f;
    }
}
