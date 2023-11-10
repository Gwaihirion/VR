using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    public int maxHealth = 100; // Set the max health of the object.
    public int currentHealth;
    private Renderer objRenderer;

    void Start()
    {
        currentHealth = maxHealth; // Initialize health.
        objRenderer = GetComponent<Renderer>(); // Get the Renderer component.
        UpdateTint(); // Initialize the color.
    }

    void UpdateTint()
    {
        float healthRatio = currentHealth / (float)maxHealth; // Calculate the health ratio.
        Color newColor = new Color(1f, healthRatio, healthRatio, 1f); // Interpolate the color.
        objRenderer.material.color = newColor; // Apply the color to the object's material.
    }

    // Call this method to update the object's health.
    public void ModifyHealth(int healthChange)
    {
        Debug.Log("Wall Losing Health");
        currentHealth += healthChange; // Modify the health value.
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth); // Ensure health stays within bounds.
        if(currentHealth == 0)
        {
            Destroy(gameObject);
        }
        UpdateTint(); // Update the color tint based on the new health.
    }

    // Example method that reacts to damage.
    public void TakeDamage(int damageAmount)
    {
        ModifyHealth(-damageAmount);// Decrease health and update the tint.
    }

    // Example method that heals the object.
    public void Heal(int healAmount)
    {
        ModifyHealth(healAmount); // Increase health and update the tint.
    }
}
