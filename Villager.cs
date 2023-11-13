using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Villager : MonoBehaviour
{
    public float moveSpeed = 5.0f; // Movement speed of the villager
    public float StartingMoveSpeed;
    public float maxVelocity;
    public float raycastLength = 10.0f; // Length of the raycast
    public Animator animator;
    public Wall MyHealth;
    public EnemyWaveManager Game;


    private Rigidbody rb; // Rigidbody component for movement
    private Vector3 flatDirection;

    void Start()
    {//
        maxVelocity = StartingMoveSpeed;
        Game = FindObjectOfType<EnemyWaveManager>();
        animator.SetInteger("VictoryStyle", Random.Range(1, 4));
        animator.SetInteger("RunStyle", Random.Range(1, 3));
        moveSpeed = StartingMoveSpeed;
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to this villager
    }

    void FixedUpdate()
    {
        rb.velocity = Vector3.ClampMagnitude(rb.velocity, maxVelocity);
    }

    void Update()
    {
       
        if(MyHealth.currentHealth < MyHealth.maxHealth/2)
        {
            animator.SetBool("Wounded", true);
            moveSpeed = StartingMoveSpeed / 2;
            maxVelocity = StartingMoveSpeed / 2;
        }
        if(MyHealth.currentHealth < MyHealth.maxHealth/4)
        {
            animator.SetBool("Crawling", true);
            moveSpeed = StartingMoveSpeed / 4;
            maxVelocity = StartingMoveSpeed / 4;
        }
        // 1) Find the closest enemy object
        GameObject closestEnemy = FindClosestEnemyWithTag("Enemy");

        if (closestEnemy != null)
        {
            // 2) Move away from the closest enemy
            Vector3 moveDirection = (transform.position - closestEnemy.transform.position).normalized;
            animator.SetBool("Running", true);
           // animator.SetInteger("RunStyle", Random.Range(1, 2));
            animator.SetBool("Victory", false);

            // 4) Keep the villager always rotated to point in the direction of its current movement
            if (moveDirection != Vector3.zero) // Avoid LookRotation with a zero vector
            {
                Quaternion toRotation = Quaternion.LookRotation(new Vector3(rb.velocity.normalized.x, 0f, rb.velocity.normalized.z), Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 360 * Time.deltaTime);
            }

            // 3) Raytrace in the direction of the Villager's current movement
            RaycastHit hit;
            float radius = GetComponent<CapsuleCollider>().radius/2;
            if (Physics.SphereCast(transform.position, radius, moveDirection, out hit, raycastLength, LayerMask.GetMask("Walls")))
            {
                // 3a) If the ray intersects with anything on the "Walls" layer
                Vector3 perpDirection = Vector3.Cross(hit.normal, Vector3.up).normalized;
                flatDirection = new Vector3(perpDirection.x, 0f, perpDirection.z).normalized;
               // Vector3 Grav = new Vector3(0f, -1f, 0f);

                // Make the Villager move perpendicular to the contact point's normal
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Turn to Run"))
                {
                    rb.AddForce(flatDirection * moveSpeed, ForceMode.Force);
                }
            }
            else
            {
                // If no wall is hit, continue moving away from the enemy
                flatDirection = new Vector3(moveDirection.x, 0f, moveDirection.z).normalized;
                //Vector3 Grav = new Vector3(0f, -1f, 0f);
                if (!animator.GetCurrentAnimatorStateInfo(0).IsName("Turn to Run"))
                {
                    rb.AddForce(flatDirection * moveSpeed, ForceMode.Force);
                }
            }
        }
        else
        {
            if (Game.Victory == true)
            {
                animator.applyRootMotion = true;
                animator.SetBool("Victory", true);
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
