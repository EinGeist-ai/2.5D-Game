 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class E : MonoBehaviour
{
    public GameObject player;
    public GameObject enemy;
    public int damage;
    
    RaycastHit hitDistance;
    RaycastHit hitCheck;
    bool hitSomething;
    float angles;
    public int attackDamage = 10;
    public float attackRange = 1;
    void Update()
    {
        float distance = Vector3.Distance(player.transform.position, enemy.transform.position);
        if (distance <= 0.6 );
        {
            Physics.Raycast(player.transform.position, enemy.transform.position, out hitDistance);
            if (distance == hitDistance.distance) 
            {
                StartCoroutine(EnemyMeele());
            }
        }
    }
    public IEnumerator  EnemyMeele()
    {
        float[] angles = { 0f, 22.5f, -22.5f, 45f, -45f };
        bool hitSomething = false;

        for (int i = 0; i < angles.Length; i++)
        {
            Vector3 attackDirection = enemy.transform.position - player.transform.position;
            if (hitSomething) break; // Stop if already hit
            Vector3 dir = Quaternion.AngleAxis(angles[i], Vector3.up) * attackDirection;
            RaycastHit hit;
            if (Physics.Raycast(enemy.transform.position, dir, out hit, attackRange))
            {
                PlayerHealth player = hitCheck.collider.GetComponent<PlayerHealth>();
                if (player != null)
                {
                    player.TakeDamage(attackDamage);
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
        yield return new WaitForSeconds(1f);
    }

    




}


