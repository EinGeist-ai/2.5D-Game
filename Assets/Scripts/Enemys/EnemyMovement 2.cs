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
}
