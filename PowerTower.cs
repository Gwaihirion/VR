using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerTower : MonoBehaviour
{
    public string Type;
    public ParticleSystem FireParticles;
    public ParticleSystem IceParticles;
    public ParticleSystem LightningParticles;
    public ParticleSystem ForceParticles;
    
    // Start is called before the first frame update
    void Start()
    {
     if(Type == "Fire")
        {
            FireParticles.Play();
            IceParticles.Stop();
            LightningParticles.Stop();
            ForceParticles.Stop();
        }
        if (Type == "Ice")
        {
            FireParticles.Stop();
            IceParticles.Play();
            LightningParticles.Stop();
            ForceParticles.Stop();
        }
        if (Type == "Lightning")
        {
            FireParticles.Stop();
            IceParticles.Stop();
            LightningParticles.Play();
            ForceParticles.Stop();
        }
        if (Type == "Force")
        {
            FireParticles.Stop();
            IceParticles.Stop();
            LightningParticles.Stop();
            ForceParticles.Play();
        }
    }

    // Update is called once per frame
    void Update()
    {
     if(Type == "Fire" && Magic.isFire)
        {
            var ex = FireParticles.externalForces;
            ex.enabled = true;
        }
        else
        {
            var ex = FireParticles.externalForces;
            ex.enabled = false;
        }
        if (Type == "Ice" && Magic.isIce)
        {
            var ex = IceParticles.externalForces;
            ex.enabled = true;
        }
        else
        {
            var ex = IceParticles.externalForces;
            ex.enabled = false;
        }
        if (Type == "Lightning" && Magic.isLightning)
        {
            var ex = LightningParticles.externalForces;
            ex.enabled = true;
        }
        else
        {
            var ex = LightningParticles.externalForces;
            ex.enabled = false;
        }
        if (Type == "Force" && Magic.isForce)
        {
            var ex = ForceParticles.externalForces;
            ex.enabled = true;
        }
        else
        {
            var ex = ForceParticles.externalForces;
            ex.enabled = false;
        }
    }
}
