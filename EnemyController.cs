using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float health = 100f;
    public int Damage = 10;
    public float downtime;
    public List<GameObject> lootItems;
    public GameObject player;
    public LayerMask wallLayer;
    public float attackRange = 2f; // Adjustable attack range
    public float moveSpeed = 2f;
    public float detourDistance = 20f; // Distance to detour from the obstacle
    private GameObject currentTarget;
    public float ragdollVelocityThreshold = 10f;
    public string MonsterType;

    [Header("Magic Vulnerabilities and Resistances")]
    public float FireDamage;
    public float IceDamage;
    public float LightningDamage;
    public float ForceDamage;

    [Header("Component References")]
    public Rigidbody rb;
    public Animator animator;
    public Collider mainCollider;
    public List<Rigidbody> ragdollRigidbodies;
    public List<Collider> ragdollColliders;
    public Vector3 colliderCenter;

    private bool isRagdoll = false;
    private bool Obstacled = false;

    public void Start()
    {
        player = GameObject.FindWithTag("Player");
        /*        player = GameObject.FindWithTag("Target");
                if (player == null)
                {
                    player = GameObject.FindWithTag("Player"); // It's safer to use tags to find the player
                }*/
        // Initialize the ragdollRigidbodies list and disable all inner Rigidbodies initially
        mainCollider.enabled = true;
        colliderCenter = mainCollider.bounds.center;
        foreach (Rigidbody childRigidbody in GetComponentsInChildren<Rigidbody>())
        {
            if (childRigidbody != rb) // Ignore the main Rigidbody
            {
                ragdollRigidbodies.Add(childRigidbody);
                childRigidbody.isKinematic = true; // Disable physics
            }
        }
        foreach (Collider childCollider in GetComponentsInChildren<Collider>())
        {
            if (childCollider != mainCollider)
            {
                ragdollColliders.Add(childCollider);
                childCollider.enabled = false;
            }
        }    
    }

    private void FixedUpdate()
    {
        colliderCenter = mainCollider.bounds.center;
        FindAndSetTarget();

        if (currentTarget != null)
        {
            MoveTowardsTargetOrObstacle();
        }
        if (currentTarget == null)
        {
            currentTarget = player;
        }
        if (isRagdoll == true && health > 0f)
        {
            downtime -= 1f * Time.deltaTime;
            if (downtime < 0f)
            {
                UnRagdoll();
            }
        }
        if (health <= 0 || isRagdoll) return; // If dead or ragdolled, no further action.
/*
        var directionToPlayer = player.transform.position - transform.position;
        bool wallBetween = Physics.Raycast(colliderCenter, directionToPlayer.normalized, directionToPlayer.magnitude, wallLayer);

        if ((!wallBetween || ((1 << player.layer) & wallLayer) != 0) && directionToPlayer.magnitude < attackRange)
        {
            Attack();
        }
        else if ((!wallBetween || ((1 << player.layer) & wallLayer) != 0))
        {
            MoveTowardsPlayer(directionToPlayer);
        }
        if ((wallBetween && (((1 << player.layer) & wallLayer) == 0)) || player == null)
        {
            Ray rayForward = new Ray(colliderCenter, directionToPlayer);
            RaycastHit hit;
            // Consider using a specific range instead of Mathf.Infinity
            float maxDistance = 100f; // Example max distance
            if (Physics.Raycast(rayForward, out hit, maxDistance, wallLayer))
            {
               // Debug.Log("I see a wall: " + hit.collider.name); // To ensure we hit the right object
                player = hit.collider.gameObject;
            }
            else
            {
                player = GameObject.FindWithTag("Target");
                // Debug.Log("Raycast did not hit a wall");
            }
            Debug.DrawRay(rayForward.origin, rayForward.direction * maxDistance, Color.red, 1.0f); // Visualize the Raycast
        }
*/

    }
    
   // private void MoveTowardsAndAttack(Vector3 target)
   // {
     //   if ((transform.position - target).magnitude > attackRange)
       // {
         //   animator.SetBool("IsWalking", true);
           // animator.SetBool("Attack", false);
            //if (MonsterType == "Skeleton")
            //{
              //  animator.SetFloat("speedh", 1f);
                //animator.SetFloat("speedv", 1f);
            //}
            //rb.AddForce(transform.forward * moveSpeed, ForceMode.Force);
           // rb.MovePosition(transform.position + (transform.forward * moveSpeed * Time.fixedDeltaTime)) ;
       // } else
        //{
          //  Debug.Log("I'm Attacking an Obstacle");
            //animator.SetBool("IsWalking", false);
            //animator.SetBool("Attack", true);
            //if (MonsterType == "Skeleton")
            //{
              //  animator.SetFloat("speedh", 0f);
                //animator.SetFloat("speedv", 0f);
            //}
        //}
        //Obstacled = true;
    //}

    private void FindAndSetTarget()
    {
        GameObject[] targets = GameObject.FindGameObjectsWithTag("Target");
        if (targets.Length > 0)
        {
            currentTarget = GetClosestTarget(targets);
        } 
        else
        {
            currentTarget = player;
        }
    }

    GameObject GetClosestTarget(GameObject[] targets)
    {
        return targets
            .OrderBy(t => (t.transform.position - transform.position).sqrMagnitude)
            .FirstOrDefault();
    }

    void MoveTowardsTargetOrObstacle()
    {
        Vector3 directionToTarget = currentTarget.transform.position - transform.position;
        RaycastHit hit;

        if (Physics.Raycast(colliderCenter, directionToTarget.normalized, out hit, 10f, wallLayer))
        {
            if (hit.collider.CompareTag("Destructible"))
            {
                AttackOrMoveToDestructible(hit.collider.gameObject);
            }
            else
            {
               // Debug.Log("Need to Move Around");
                NavigateAroundObstacle(hit.normal);
            }
        }
        else
        {
            MoveTowards(currentTarget.transform.position);
            if(directionToTarget.magnitude <= attackRange)
            {
                Attack(currentTarget);
            }
        }
    }

    void AttackOrMoveToDestructible(GameObject destructible)
    {
        // var directionToPlayer = currentTarget.transform.position - transform.position;
        Ray rayForward = new Ray(colliderCenter, transform.forward);
        //Debug.DrawRay(rayForward.origin, transform.forward * attackRange, Color.green, 1.0f);
        RaycastHit hit;
        // Consider using a specific range instead of Mathf.Infinity
        float maxDistance = attackRange; // Example max distance
        if (Physics.Raycast(rayForward, out hit, maxDistance, wallLayer))
        {
            Attack(destructible);
        }
        else
        {
            MoveTowards(destructible.transform.position);
        }
    }

    void MoveTowards(Vector3 targetPosition)
    {
        Vector3 direction = targetPosition - transform.position;
        Vector3 flatDirection = new Vector3(direction.x, 0f, direction.z).normalized;

        animator.SetBool("IsWalking", true);
        animator.SetBool("Attack", false);

        if (MonsterType == "Skeleton")
        {
            animator.SetFloat("speedh", 1f);
            animator.SetFloat("speedv", 1f);
        }
        RotateTowardsPlayer(direction);
        rb.AddForce(flatDirection * moveSpeed, ForceMode.Force);

        //  Vector3 newPosition = Vector3.MoveTowards(rb.position, targetPosition, moveSpeed * Time.deltaTime);
        // rb.MovePosition(newPosition);

        /*     
              animator.SetBool("IsWalking", true);
              animator.SetBool("Attack", false);
              if (MonsterType == "Skeleton")
              {
                  animator.SetFloat("speedh", 1f);
                  animator.SetFloat("speedv", 1f);
              }
              RotateTowardsPlayer(direction);
              rb.AddForce(flatDirection * moveSpeed, ForceMode.Force);
              //  rb.MovePosition(transform.position + (direction.normalized * moveSpeed * Time.fixedDeltaTime));
      */
    }

    void NavigateAroundObstacle(Vector3 hitNormal)
    {
        //Debug.Log("Navigating Around Obstacle");
        // Determine detour direction
        Vector3 detourDirection = Vector3.Cross(hitNormal, Vector3.up).normalized;
        Vector3 detourPoint = transform.position + detourDirection * detourDistance;
        Debug.DrawRay(gameObject.transform.position, detourDirection * detourDistance, Color.red, 1.0f);
        // Move towards the detour point
        MoveTowards(detourPoint);
    }

    void Attack(GameObject destructible)
    {
        // Implement the attack logic specific to your game
        animator.SetBool("IsWalking", false);
        if (MonsterType == "Skeleton")
        {
            animator.SetFloat("speedh", 0f);
            animator.SetFloat("speedv", 0f);
        }
        animator.SetBool("Attack", true);
        RotateTowardsPlayer(currentTarget.transform.position - transform.position);
        // Implement additional logic here to deal damage to the player if needed
    }

    private void RotateTowardsPlayer(Vector3 direction)
    {
        // Create a new direction vector with a zeroed y-axis to keep the enemy upright
        Vector3 directionWithYZero = new Vector3(direction.x, 0.0f, direction.z);
        // Create a rotation from the current forward vector towards the target direction vector
        Quaternion targetRotation = Quaternion.LookRotation(directionWithYZero);
        // Smoothly rotate towards the target point over time
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, moveSpeed * Time.fixedDeltaTime);
    }



    

    private void Die()
    {        
        GoRagdoll();
        animator.SetTrigger("Die");

        DropLoot();
        Destroy(gameObject, 10);
    }

    private void GoRagdoll()
    {
        animator.enabled = false;
        isRagdoll = true;
        // Deactivate main collider and Rigidbody
        mainCollider.enabled = false;
        rb.isKinematic = true;
        // Activate all inner Rigidbodies
        foreach (Rigidbody childRigidbody in ragdollRigidbodies)
        {
            childRigidbody.isKinematic = false;
        }
        foreach (Collider childCollider in ragdollColliders)
        {
            childCollider.enabled = true;
        }
        // Deactivate the Animator to enable the ragdoll effect

    }
    private void UnRagdoll()
    {
        isRagdoll = false;
        // Deactivate main collider and Rigidbody
        mainCollider.enabled = true;
        rb.isKinematic = false;
        // Activate all inner Rigidbodies
        foreach (Rigidbody childRigidbody in ragdollRigidbodies)
        {
            childRigidbody.isKinematic = true;
        }
        foreach (Collider childCollider in ragdollColliders)
        {
            childCollider.enabled = false;
        }
        // Deactivate the Animator to enable the ragdoll effect
        animator.enabled = true;
    }
        

    private void CheckForRagdoll()
    {
        if (rb.velocity.magnitude > ragdollVelocityThreshold && !isRagdoll)
        {
            GoRagdoll();
        }
    }

    private void DropLoot()
    {
        if (lootItems.Count == 0) return;

        int index = Random.Range(0, lootItems.Count);
        Instantiate(lootItems[index], transform.position, Quaternion.identity);
    }

    public void TakeDamage(float damageAmount)
    {
        if (isRagdoll == true) return;
        if (health <= 0)
        {
            Die();
        }
        health -= damageAmount;

    }

    private void OnCollisionEnter(Collision collision)
    {
       /* if (Obstacled == true)
        {
           // Attack();
        }*/
       if(collision.collider.CompareTag("Destructible"))
        {
            Attack(collision.collider.gameObject);
        }
        if (collision.relativeVelocity.magnitude > ragdollVelocityThreshold && !isRagdoll)
        {
            GoRagdoll();
            TakeDamage(collision.relativeVelocity.magnitude);
            downtime = collision.relativeVelocity.magnitude;
        }
    }

    private void OnParticleCollision(GameObject other)
    {
       // LayerMask mask = LayerMask.GetMask("Ground");
        // Check if the particles are on the Magic layer
        if (other.layer == LayerMask.NameToLayer("Magic"))
        {
            // Check the tag of the particles and call the respective reaction function
            if (other.CompareTag("Fire"))
            {
                ReactToFire();
                TakeDamage(FireDamage);
            }
            else if (other.CompareTag("Ice"))
            {
                ReactToIce();
                TakeDamage(IceDamage);
            }
            else if (other.CompareTag("Lightning"))
            {
                ReactToLightning();
                TakeDamage(LightningDamage);
            }
            else if (other.CompareTag("Force"))
            {
                ReactToForce();
                TakeDamage(ForceDamage);
            }
        }
    }

    // Placeholder methods for reactions to each element type
    private void ReactToFire()
    {

        // Implement fire reaction logic here
    }

    private void ReactToIce()
    {
        // Implement ice reaction logic here
    }

    private void ReactToLightning()
    {
        // Implement lightning reaction logic here
    }

    private void ReactToForce()
    {
        // Implement force reaction logic here
    }
    public void DealDamage()
    {
        
       // var directionToPlayer = currentTarget.transform.position - transform.position;
        Ray rayForward = new Ray(colliderCenter, transform.forward);
        Debug.DrawRay(rayForward.origin, transform.forward * attackRange, Color.green, 1.0f);
        RaycastHit hit;
        // Consider using a specific range instead of Mathf.Infinity
        float maxDistance = attackRange; // Example max distance
        if (Physics.Raycast(rayForward, out hit, maxDistance) && (hit.collider.CompareTag("Destructible") || hit.collider.CompareTag("Target")))
        {
            hit.collider.gameObject.SendMessage("TakeDamage", Damage);
            Debug.Log("Skelly Dealing Damage");
        }
    }

}
