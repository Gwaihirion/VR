using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour
{
    public float moveSpeed = 5.0f; // Movement speed of the villager
    public float raycastLength = 10.0f; // Length of the raycast

    private Rigidbody rb; // Rigidbody component for movement

    void Start()
    {
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to this villager
    }

    void Update()
    {
        // 1) Find the closest enemy object
        GameObject closestEnemy = FindClosestEnemyWithTag("Enemy");

        if (closestEnemy != null)
        {
            // 2) Move away from the closest enemy
            Vector3 moveDirection = (transform.position - closestEnemy.transform.position).normalized;

            // 4) Keep the villager always rotated to point in the direction of its current movement
            if (moveDirection != Vector3.zero) // Avoid LookRotation with a zero vector
            {
                Quaternion toRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 360 * Time.deltaTime);
            }

            // 3) Raytrace in the direction of the Villager's current movement
            RaycastHit hit;
            if (Physics.Raycast(transform.position, moveDirection, out hit, raycastLength, LayerMask.GetMask("Walls")))
            {
                // 3a) If the ray intersects with anything on the "Walls" layer
                Vector3 perpDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;

                // Make the Villager move perpendicular to the contact point's normal
                rb.velocity = perpDirection * moveSpeed;
            }
            else
            {
                // If no wall is hit, continue moving away from the enemy
                rb.velocity = moveDirection * moveSpeed;
            }
        }
    }

    GameObject FindClosestEnemyWithTag(string tag)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float closestDistance = Mathf.Infinity;
        Vector3 position = transform.position;

        foreach (GameObject enemy in enemies)
        {
            Vector3 diff = enemy.transform.position - position;
            float curDistance = diff.sqrMagnitude;
            if (curDistance < closestDistance)
            {
                closest = enemy;
                closestDistance = curDistance;
            }
        }
        return closest;
    }
}
