using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 10.0f; // Range of the attack
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
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown && !animator.GetBool("IsRolling"))
        {
            StartCoroutine(Attack());
            lastAttackTime = Time.time;
        }
    }

    private IEnumerator Attack()
    {
        animator.SetBool("Attacking", true);
        animator.SetTrigger("Meele");
        yield return new WaitForSeconds(0.20825f); // Wait for animation timing

        Vector3 attackDirection = transform.forward;
        if (movement != null && movement.CurrentDirection.sqrMagnitude > 0.01f)
            attackDirection = movement.CurrentDirection;
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        // Define angles for the rays: center, ±22.5°, ±45°
        float[] angles = { 0f, 22.5f, -22.5f, 45f, -45f };
        bool hitSomething = false;

        for (int i = 0; i < angles.Length; i++)
        {
            if (hitSomething) break; // Stop if already hit
            Vector3 dir = Quaternion.AngleAxis(angles[i], Vector3.up) * attackDirection;
            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, attackRange))
            {
                EnemyHealth enemy = hit.collider.GetComponent<EnemyHealth>();
                if (enemy != null)
                {
                    enemy.TakeDamage(attackDamage);
                    Debug.Log("Hit " + hit.collider.name + " for " + attackDamage + " damage. (Ray " + i + " angle " + angles[i] + ")");
                    hitSomething = true;
                    break;
                }
                else
                {
                    Debug.Log("Hit " + hit.collider.name + ", but it has no EnemyHealth component. (Ray " + i + " angle " + angles[i] + ")");
                }
            }
        }
        yield return new WaitForSeconds(0.20825f);

        if (animator != null)
        {
            animator.SetBool("Attacking", false);
        }
    }

    void OnDrawGizmosSelected()
    {
        Vector3 attackDirection = transform.forward;
        EightDirectionMovement movement = GetComponent<EightDirectionMovement>();
        if (movement != null && movement.CurrentDirection.sqrMagnitude > 0.01f)
            attackDirection = movement.CurrentDirection;

        Gizmos.color = Color.red;
        Vector3 origin = transform.position + Vector3.up * 0.5f;

        float[] angles = { 0f, 22.5f, -22.5f, 45f, -45f };
        foreach (float angle in angles)
        {
            Vector3 dir = Quaternion.AngleAxis(angle, Vector3.up) * attackDirection;
            Gizmos.DrawRay(origin, dir.normalized * attackRange);
        }
    }
}
