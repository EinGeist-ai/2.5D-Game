using System.Collections;
using UnityEngine;

public class EnemyHit : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;
    public int attackDamage = 10;
    public float attackRange = 1f;
    public float attackCooldown = 1f;

    private float lastAttackTime = -Mathf.Infinity;

    void Update()
    {
        if (player == null || enemy == null) return;

        float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
        if (distance <= 0.6f && Time.time >= lastAttackTime + attackCooldown)
        {
            Vector3 direction = (player.transform.position - enemy.transform.position).normalized;
            RaycastHit hitDistance;
            if (Physics.Raycast(enemy.transform.position, direction, out hitDistance, attackRange))
            {
                if (hitDistance.collider.gameObject == player)
                {
                    StartCoroutine(EnemyMeele());
                    lastAttackTime = Time.time;
                }
            }
        }
    }

    public IEnumerator EnemyMeele()
    {
        float[] angles = { 0f, 22.5f, -22.5f, 45f, -45f };
        bool hitSomething = false;

        for (int i = 0; i < angles.Length; i++)
        {
            Vector3 attackDirection = (player.transform.position - enemy.transform.position).normalized;
            Vector3 dir = Quaternion.AngleAxis(angles[i], Vector3.up) * attackDirection;
            RaycastHit hit;
            if (Physics.Raycast(enemy.transform.position, dir, out hit, attackRange))
            {
                PlayerHealth playerHealth = hit.collider.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.TakeDamage(attackDamage);
                    Debug.Log("Hit " + hit.collider.name + " for " + attackDamage + " damage. (Ray " + i + " angle " + angles[i] + ")");
                    hitSomething = true;
                    break;
                }
                else
                {
                    Debug.Log("Hit " + hit.collider.name + ", but it has no PlayerHealth component. (Ray " + i + " angle " + angles[i] + ")");
                }
            }
        }
        yield return new WaitForSeconds(1f);
    }
}


