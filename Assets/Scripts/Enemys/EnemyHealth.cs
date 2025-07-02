using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public int maxHealth = 100;
    private int currentHealth;

    public float lastDeathTime;

    public GameObject healthBar;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar = GameObject.Find("HealthPivot");
        lastDeathTime = Time.time;
        
        
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        if (healthBar != null)
        {
            // Update health bar UI
            float healthPercentage = (float)currentHealth / maxHealth;
            healthBar.transform.localScale = new Vector3(1f, 1f, healthPercentage);
        }


        if (currentHealth <= 0)
        {
            float deathTime = Time.time - lastDeathTime;
            Debug.Log("Enemy died after: " + deathTime + " seconds since last death.");

            lastDeathTime = Time.time;
            currentHealth = maxHealth; // Reset health for respawn
            healthBar.transform.localScale = new Vector3(1f, 1f, 1f); // Reset health bar
        }
    }
    
    private void Die()
    {
        // Handle enemy death (e.g., play animation, drop loot, etc.) 
        Destroy(gameObject);
    }

    public int GetCurrentHealth()
    {
        return currentHealth;
    }

    public int GetMaxHealth()
    {
        return maxHealth;
    }
}
