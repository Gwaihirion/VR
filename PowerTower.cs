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
            //IceParticles.Stop();
            //LightningParticles.Stop();
            //ForceParticles.Stop();
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
    }
}
