using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovement2 : MonoBehaviour
{
    public GameObject player;
    public NavMeshAgent enemy;

    // Update is called once per frame
    void Update()
    {
       Vector3 playerPos = player.transform.position;

        enemy.SetDestination(playerPos);
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
