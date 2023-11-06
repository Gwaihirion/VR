using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    [Header("Enemy Settings")]
    public float health = 100f;
    public float downtime;
    public List<GameObject> lootItems;
    public GameObject player;
    public LayerMask wallLayer;
    public float attackRange = 2f; // Adjustable attack range
    public float moveSpeed = 2f;
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

    private bool isRagdoll = false;
    private bool Obstacled = false;

    public void Start()
    {
        player = GameObject.FindWithTag("Player"); // It's safer to use tags to find the player
        // Initialize the ragdollRigidbodies list and disable all inner Rigidbodies initially
        mainCollider.enabled = true;
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
        if (isRagdoll == true && health > 0f)
        {
            downtime -= 1f * Time.deltaTime;
            if (downtime < 0f)
            {
                UnRagdoll();
            }
        }
        if (health <= 0 || isRagdoll) return; // If dead or ragdolled, no further action.

        var directionToPlayer = player.transform.position - transform.position;
        bool wallBetween = Physics.Raycast(transform.position, directionToPlayer.normalized, directionToPlayer.magnitude, wallLayer);

        if (!wallBetween && directionToPlayer.magnitude < attackRange)
        {
            Attack();
        }
        else if (!wallBetween)
        {
            MoveTowardsPlayer(directionToPlayer);
        }
        if(wallBetween)
        {
            //   Ray rayLeft = new Ray(leftHand.transform.position, leftHand.transform.forward);
            //RaycastHit hitLeft;

            //if (Physics.Raycast(rayRight, out hitRight, Mathf.Infinity, objectmask))
            Ray rayForward = new Ray(transform.position, transform.forward);
            RaycastHit hit;
            if (Physics.Raycast(rayForward, out hit, Mathf.Infinity, wallLayer))
            {
                MoveTowardsAndAttack(hit.point);
            }
        }

    }
    
    private void MoveTowardsAndAttack(Vector3 target)
    {
        if ((transform.position - target).magnitude > attackRange)
        {
            animator.SetBool("IsWalking", true);
            animator.SetBool("Attack", false);
            if (MonsterType == "Skeleton")
            {
                animator.SetFloat("speedh", 1f);
                animator.SetFloat("speedv", 1f);
            }
            rb.MovePosition(transform.position + (transform.forward * moveSpeed * Time.fixedDeltaTime));
        } else
        {
            Debug.Log("I'm Attacking an Obstacle");
            animator.SetBool("IsWalking", false);
            animator.SetBool("Attack", true);
            if (MonsterType == "Skeleton")
            {
                animator.SetFloat("speedh", 0f);
                animator.SetFloat("speedv", 0f);
            }
        }
        Obstacled = true;
    }
    private void MoveTowardsPlayer(Vector3 direction)
    {
        animator.SetBool("IsWalking", true);
        animator.SetBool("Attack", false);
        if(MonsterType == "Skeleton")
        {
            animator.SetFloat("speedh", 1f);
            animator.SetFloat("speedv", 1f);
        }
        RotateTowardsPlayer(direction);
        rb.MovePosition(transform.position + (direction.normalized * moveSpeed * Time.fixedDeltaTime));
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

    private void Attack()
    {
        animator.SetBool("IsWalking", false);
        if (MonsterType == "Skeleton")
        {
            animator.SetFloat("speedh", 0f);
            animator.SetFloat("speedv", 0f);
        }
        animator.SetBool("Attack", true);
        RotateTowardsPlayer(player.transform.position - transform.position);
        // Implement additional logic here to deal damage to the player if needed
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
        Debug.Log("Standing up now.");
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
        if (Obstacled == true)
        {
            Attack();
        }
        if (collision.relativeVelocity.magnitude > ragdollVelocityThreshold && !isRagdoll)
        {
            GoRagdoll();
            TakeDamage(collision.relativeVelocity.magnitude);
            Debug.Log("Damage taken: " + collision.relativeVelocity.magnitude);
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
        Debug.Log("Burning");

        // Implement fire reaction logic here
    }

    private void ReactToIce()
    {
        Debug.Log("Freezing");
        // Implement ice reaction logic here
    }

    private void ReactToLightning()
    {
        Debug.Log("Shocked");
        // Implement lightning reaction logic here
    }

    private void ReactToForce()
    {
        Debug.Log("Forced");
        // Implement force reaction logic here
    }

}
