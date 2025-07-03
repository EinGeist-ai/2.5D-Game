using System.Collections;
using System.Collections.Generic;
using System.Net.Security;
using UnityEngine;

public class EnemyMove : MonoBehaviour
{
    public float EnemyMoveSpeed;

   public bool isPursuing;

    public GameObject player;

    public Vector3 playerPos;

    public Vector3 enemyPos;

    public Vector3 enemyDirection;

    public GameObject enemy;

    private float eX;

    private float eY;

    private float eZ;

    private float offset;

    public Rigidbody rb;

    public bool isSlowed;

    void Start()
    {
        isPursuing = false;
        rb = GetComponent<Rigidbody>();
    }

    void Update()
    {
        if (player != null)
        {
            playerPos = player.transform.position;

            enemyPos = enemy.transform.position;

            enemyDirection = playerPos - enemyPos;

            eX = enemyDirection.x;
            eY = enemyDirection.y;
            eZ = enemyDirection.z;

            offset = Mathf.Sqrt(eX * eX + eY * eY + eZ * eZ);

            if (isSlowed)
              

            rb.MovePosition(rb.position + enemyDirection * EnemyMoveSpeed * 1/offset * Time.deltaTime);
        }
    }
}
