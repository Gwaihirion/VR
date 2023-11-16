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
    public float checkRadius = 5f;
    public int numDirections = 32;

    private Collider mainCollider;
    private Rigidbody rb; // Rigidbody component for movement
    private Vector3 flatDirection;
    public Vector3 colliderCenter;
    public bool Obstacled;

    void Start()
    {//
        mainCollider = gameObject.GetComponent<Collider>();
        colliderCenter = mainCollider.bounds.center;
        maxVelocity = StartingMoveSpeed;
        Game = FindObjectOfType<EnemyWaveManager>();
        animator.SetInteger("VictoryStyle", Random.Range(1, 4));
        animator.SetInteger("RunStyle", Random.Range(1, 3));
        moveSpeed = StartingMoveSpeed;
        rb = GetComponent<Rigidbody>(); // Get the Rigidbody component attached to this villager
    }

    void FixedUpdate()
    {
        colliderCenter = mainCollider.bounds.center;
        Vector3 currentVelocity = rb.velocity;
        if (currentVelocity.magnitude > maxVelocity)
        {
            rb.velocity = currentVelocity.normalized * maxVelocity;
        }
        Collider[] obstacles = CheckForObstacles();
        if (obstacles.Length > 0)
        {
            Obstacled = true;
        } else { Obstacled = false; }
    }

Collider[] CheckForObstacles()
{
        int layerMask = LayerMask.GetMask("Walls", "Objects", "Enemies");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, checkRadius, layerMask);
        hitColliders = System.Array.FindAll(hitColliders, collider => collider != GetComponent<Collider>());

        return hitColliders;
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
            if (moveDirection.magnitude > 0.5f) // Avoid LookRotation with a zero vector
            {
                Quaternion toRotation = Quaternion.LookRotation(new Vector3(rb.velocity.normalized.x, 0f, rb.velocity.normalized.z), Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, 360 * Time.deltaTime);
            }

            // 3) Raytrace in the direction of the Villager's current movement
            if (Obstacled)
            {
                flatDirection = FindLeastObstructedDirection();
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
            else { animator.SetBool("Running", false); }
        }
    }

    Vector3 FindLeastObstructedDirection()
    {
        Collider[] obstacles = CheckForObstacles();
        float[] totalDistancePerDirection = new float[numDirections];
        bool[] hasEnemyInDirection = new bool[numDirections];
        Vector3 leastObstructedDirection = Vector3.forward;
        float maxTotalDistance = 0f;

        for (int i = 0; i < numDirections; i++)
        {
            float angle = i * 360f / numDirections;
            Vector3 direction = Quaternion.Euler(0, angle, 0) * Vector3.forward;

            foreach (var obstacle in obstacles)
            {
                if (IsInDirection(obstacle, direction))
                {
                    float distance = Vector3.Distance(transform.position, obstacle.transform.position);
                    totalDistancePerDirection[i] += distance;

                    if (obstacle.gameObject.layer == LayerMask.NameToLayer("Enemies")) // Assuming enemies are on the "Enemies" layer
                    {
                        hasEnemyInDirection[i] = true;
                    }
                }
            }

            if (!hasEnemyInDirection[i] && totalDistancePerDirection[i] > maxTotalDistance)
            {
                maxTotalDistance = totalDistancePerDirection[i];
                leastObstructedDirection = direction;
            }
        }

        return leastObstructedDirection;
    }
    bool IsInDirection(Collider obstacle, Vector3 direction)
    {
        Vector3 toObstacle = obstacle.transform.position - transform.position;
        float angleToObstacle = Vector3.Angle(direction, toObstacle);
        return angleToObstacle <= (360f / numDirections) / 2;
    }

    GameObject FindClosestEnemyWithTag(string tag)
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(tag);
        GameObject closest = null;
        float closestDistance = raycastLength;
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
