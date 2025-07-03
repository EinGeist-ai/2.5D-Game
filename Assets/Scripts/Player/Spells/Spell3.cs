using UnityEngine;

public class Spell3 : MonoBehaviour
{
    public int healing = 10;
    private ParticleSystem ps;

    public float healCooldown = 1;
    private float lastHealTime = 0f;

    public GameObject player; // Reference to the player GameObject 

    private bool inRadius = false;

    private void Awake()
    {
        ps = GetComponent<ParticleSystem>();
        if (ps == null)
        {
            Debug.LogWarning("ParticleSystem component not found on Spell1 GameObject.");
        }
    }

    void Update()
    {
        if (inRadius && Time.time >= lastHealTime + healCooldown)
        {

            lastHealTime = Time.time;
            if (player != null)
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null)
                {
                    playerHealth.Heal(healing);
                    Debug.Log($"Spell3 healed {player.name} for {healing} health.");
                }
                else
                {
                    Debug.LogWarning($"PlayerHealth component not found on {player.name}. Cannot apply healing.");
                }
            }
            else
            {
                Debug.LogWarning("Player GameObject is not assigned or found.");
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRadius = true;
            player = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            inRadius = false;
            player = null;
        }
    }

    
    

}
