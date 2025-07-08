using UnityEngine;

public class Spell2 : MonoBehaviour
{
    public int damage = 10;
    private ParticleSystem ps;
    public EnemyMovement2 enemyMovement;


    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        if (ps == null)
        {
            Debug.LogWarning("ParticleSystem component not found on Spell1 GameObject.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyMovement = other.GetComponent<EnemyMovement2>();
                enemyHealth.TakeDamage(damage);
                Debug.Log($"Spell1 hit {other.name} for {damage} damage.");
                enemyMovement.StartSlow(1f, 0.2f);
            }
            else
            {
                Debug.LogWarning($"Enemy component not found on {other.name}. Cannot apply damage.");
            }
        }
        else
        {
            Debug.Log($"Spell1 collided with non-enemy object: {other.name}");
        }

        
    }
}
