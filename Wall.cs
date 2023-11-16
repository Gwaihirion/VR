using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public int maxHealth = 100; // Set the max health of the object.
    public int currentHealth;
    private Renderer objRenderer;
    public List<Renderer> objRenderers;
    public ParticleSystem Blood;
    public float BloodTicker;
    public GameObject Corpse;

    void Start()
    {
        if(Blood) 
        { 
            Blood.Stop();
            BloodTicker = -1f;
        }
        foreach (Renderer childRenderer in gameObject.GetComponentsInChildren<Renderer>())
        {
            objRenderers.Add(childRenderer);// Disable physics
        }
        currentHealth = maxHealth; // Initialize health.
       // objRenderer = GetComponent<Renderer>(); // Get the Renderer component.
        UpdateTint(); // Initialize the color.

    }

    void UpdateTint()
    {
        float healthRatio = currentHealth / (float)maxHealth; // Calculate the health ratio.
        Color newColor = new Color(1f, healthRatio, healthRatio, 1f); // Interpolate the color.

            //objRenderer.material.color = newColor; // Apply the color to the object's material.

            foreach (Renderer bodRenderer in objRenderers)
            {
                bodRenderer.material.color = newColor;
            }
        
        
    }

    // Call this method to update the object's health.
    public void ModifyHealth(int healthChange)
    {
        Debug.Log("Wall Losing Health");
        currentHealth += healthChange; // Modify the health value.
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health stays within bounds.
        if(currentHealth == 0)
        {
            if (!Corpse)
            {
                Destroy(gameObject);
            } else
            {
                GameObject Destructicus;
                Destructicus = Instantiate(Corpse, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
        UpdateTint(); // Update the color tint based on the new health.

    }

    // Example method that reacts to damage.
    public void TakeDamage(int damageAmount)
    {
        ModifyHealth(-damageAmount);// Decrease health and update the tint.
        if (Blood)
        { 
            Blood.Play();
            BloodTicker = damageAmount;
        }
    }

    public void Update()
    {
        if (Blood)
        {
            if (BloodTicker > 0)
            {
                BloodTicker -= 1 * Time.deltaTime;
            }
            if (BloodTicker < 0f)
            {
                Blood.Stop();
                BloodTicker = 0f;
            }
        }
    }
    // Example method that heals the object.
    public void Heal(int healAmount)
    {
        ModifyHealth(healAmount); // Increase health and update the tint.
    }
}
