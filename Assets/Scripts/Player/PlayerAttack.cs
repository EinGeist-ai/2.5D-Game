using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour
{
    public float attackRange = 1.0f; // Range of the attack
    public float attackCooldown = 1.0f; // Time between attacks
    private float lastAttackTime = 0f; // Time when the last attack was made

    public Animator animator; // Assign in Inspector or get in Start()

    void Start()
    {
        if (animator == null)
            animator = GetComponent<Animator>();
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0) && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(Attack());
            lastAttackTime = Time.time;
        }
    }

    private IEnumerator  Attack()
    {
        // Play attack animation
        if (animator != null)
            animator.SetBool("Attacking", true);
            animator.SetTrigger("Meele");
        yield return new WaitForSeconds(0.4165f); // Wait for half a second to simulate attack duration
            animator.SetBool("Attacking", false);

        // Visualize attack range in Scene view (for debugging)
        Debug.DrawRay(transform.position, transform.forward * attackRange, Color.red, 0.2f);
    }
}
