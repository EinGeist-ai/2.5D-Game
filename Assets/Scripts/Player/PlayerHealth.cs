using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHealth = 100;
    public int currentHealth;
    public int healthRegenRate = 1; // Health points regenerated per second
    public float healthRegenDelay = 5f; // Delay before health regeneration starts

    private Coroutine regenCoroutine;

    void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (currentHealth < 0)
        {
            currentHealth = 0;
            Debug.Log("Player has died.");
        }

        if (regenCoroutine != null)
            StopCoroutine(regenCoroutine);

        regenCoroutine = StartCoroutine(HealthRegen());
    }
    public void Heal(int heal)
    {
        if (currentHealth < maxHealth)
        {
            currentHealth += heal;
        }
        if (currentHealth > maxHealth)
        {
            currentHealth = maxHealth;
        }
        
        
    }


    private IEnumerator HealthRegen()
    {
        yield return new WaitForSeconds(healthRegenDelay);

        while (currentHealth < maxHealth)
        {
            currentHealth += healthRegenRate;
            if (currentHealth > maxHealth)
                currentHealth = maxHealth;

            yield return new WaitForSeconds(1f);
        }
        regenCoroutine = null;
    }
}
