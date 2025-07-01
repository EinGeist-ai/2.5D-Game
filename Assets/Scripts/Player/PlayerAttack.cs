using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 100.0f; // Range of the attack
    public float attackCooldown = 1.0f; // Time between attacks
    public int attackDamage = 10; // Damage dealt per attack
    private float lastAttackTime = 0f; // Time when the last attack was made

    public Animator animator; // Assign in Inspector or get in Start()

    private EightDirectionMovement movement;

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
        movement = GetComponent<EightDirectionMovement>();
        if (movement == null)
            Debug.LogWarning("EightDirectionMovement not found on Player!");
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(Attack());
            lastAttackTime = Time.time;
        }
    }

    private IEnumerator Attack()
    {
        // Play attack animation
        if (animator != null)
        {
            animator.SetBool("Attacking", true);
            animator.SetTrigger("Meele");
        }

        yield return new WaitForSeconds(0.20825f); // Wait for animation timing

        Vector3 attackDirection = transform.forward;
        if (movement != null && movement.CurrentDirection.sqrMagnitude > 0.01f)
            attackDirection = movement.CurrentDirection;

        Vector3 origin = transform.position + Vector3.up * 0.5f;
        float spacing = 0.35f; // Adjust for how far apart the rays are

        // Calculate left and right offsets
        Vector3 right = Vector3.Cross(Vector3.up, attackDirection).normalized;
        Vector3[] origins = new Vector3[]
        {
            origin, // center
            origin + right * spacing, // right
            origin - right * spacing  // left
        };

        bool hitSomething = false;
        for (int i = 0; i < origins.Length; i++)
        {
            if (hitSomething) break; // Stop if already hit 

            RaycastHit hit;
            if (Physics.Raycast(origins[i], attackDirection, out hit, attackRange))
            {
                EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage);
                    Debug.Log("Hit " + hit.collider.name + " for " + attackDamage + " damage. (Ray " + i + ")");
                    hitSomething = true;
                    break; // Stop checking other rays
                }
                else
                {
                    Debug.Log("Hit " + hit.collider.name + ", but it has no EnemyHealth component. (Ray " + i + ")");
                }
            }
        }

        yield return new WaitForSeconds(0.20825f); // Wait for animation timing

        if (animator != null)
            animator.SetBool("Attacking", false);
    }

    void OnDrawGizmosSelected()
    {
        Vector3 attackDirection = transform.forward;
        EightDirectionMovement movement = GetComponent<EightDirectionMovement>();
        if (movement != null && movement.CurrentDirection.sqrMagnitude > 0.01f)
            attackDirection = movement.CurrentDirection;

        Gizmos.color = Color.red;
        Vector3 origin = transform.position + Vector3.up * 0.5f;
        float spacing = 0.35f;
        Vector3 right = Vector3.Cross(Vector3.up, attackDirection).normalized;

        // Draw three rays: center, right, left
        Gizmos.DrawRay(origin, attackDirection.normalized * attackRange);
        Gizmos.DrawRay(origin + right * spacing, attackDirection.normalized * attackRange);
        Gizmos.DrawRay(origin - right * spacing, attackDirection.normalized * attackRange);
    }
}
