using UnityEngine;

public class Spell1 : MonoBehaviour
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

    private void OnParticleCollision(GameObject other)
    {
        if (other.CompareTag("Enemy"))
        {
            enemyMovement = other.GetComponent<EnemyMovement2>();
            EnemyHealth enemyHealth = other.GetComponent<EnemyHealth>();
            if (enemyHealth != null)
            {
                enemyHealth.TakeDamage(damage);
                Debug.Log($"Spell1 hit {other.name} for {damage} damage.");
                enemyMovement.StartSlow(2f, 0.8f); // Example usage with 2 seconds slow duration and 0.5 slow factor
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
