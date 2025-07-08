using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement2 : MonoBehaviour
{
    public GameObject player;
    public NavMeshAgent enemy;
    public GameObject enemyG;
    public bool isAgitaed = false;
    RaycastHit hit;
    public float detectionRange = 10f;
    
    // Update is called once per frame
    void Update()
    {
        Vector3 direction = player.transform.position - enemyG.transform.position;
    float distance = Vector3.Distance(player.transform.position, enemyG.transform.position);
        if (distance <= detectionRange)
        {
            Debug.DrawRay(enemyG.transform.position, direction, Color.red);
            if (Physics.Raycast(enemyG.transform.position,  direction, out hit, detectionRange))
            {
                isAgitaed = true;
                Debug.Log("Enemy is agitated and can see the player.");
                Debug.DrawRay(enemyG.transform.position, player.transform.position, Color.green);
            }
        }
        if (isAgitaed == true)
        {
            Vector3 playerPos = player.transform.position;

            enemy.SetDestination(playerPos);
        }
    }

   public IEnumerator EnemySlow(float slowDuration, float slowFactor)
    {
        enemy.speed *= slowFactor; // Slow down the enemy
        yield return new WaitForSeconds(slowDuration);
        enemy.speed /= slowFactor; // Restore original speed
    }
    public void StartSlow(float duration, float factor)
    {
        StartCoroutine(EnemySlow(duration, factor)); // Example usage with 2 seconds slow duration and 0.5 slow factor
    }




}
