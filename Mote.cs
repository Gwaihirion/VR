using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mote : MonoBehaviour
{
    public float moveSpeed = 5f; // Speed at which the object moves towards the Player.
    public string lootType; // Public string to set the loot type in the editor.

    private Rigidbody rb;
    private GameObject player;

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Cache the Rigidbody component.
        player = GameObject.FindGameObjectWithTag("Player"); // Find the Player object by tag.
    }

    void FixedUpdate()
    {
        moveSpeed += 1.5f*Time.deltaTime;
        MoveTowardsPlayer(); // Continuously move towards the Player.
    }

    void MoveTowardsPlayer()
    {
        if (player != null)
        {
            Vector3 direction = (player.transform.position - transform.position).normalized; // Calculate direction to the Player.
            rb.AddForce(direction * moveSpeed, ForceMode.VelocityChange); // Add force towards the Player's direction.
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject == player) // Check for collision with the Player.
        {
            player.SendMessage("Loot", lootType); // Send message to trigger the Loot method.
            Destroy(gameObject);
        }
    }
}
