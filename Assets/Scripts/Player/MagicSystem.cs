using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSystem : MonoBehaviour
{
    public KeyCode[] spellKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.E, KeyCode.Q };

    private Animator animator;


    public ParticleSystem magicCircle; // Assign in Inspector (magic circle prefab)

    public GameObject crosshair; // Assign in Inspector (UI element)

    public GameObject[] spellPrefabs;

    public float[] spellCooldowns = new float[9]; // Cooldowns for each spell slot
    public float[] lastCastTimes = new float[9]; // Last cast times for each spell slot

    // Each slot holds a spell ID (e.g., 1 = Spell1, 2 = Spell2, etc.)
    public int[] spellSlotIDs = new int[9];

    // Maps spell IDs to their corresponding methods
    private Dictionary<int, System.Action> spellActions;

    void Start()
    {
        animator = GetComponent<Animator>();
        magicCircle = GetComponentInChildren<ParticleSystem>();

        if (magicCircle != null)
        {
            magicCircle.Stop();
        }

        // Initialize spell slot IDs (default: 1, 2, 0, 0, ...)
        spellSlotIDs[0] = 1; // Slot 0: Spell1
        spellSlotIDs[1] = 2; // Slot 1: Spell2
        spellSlotIDs[2] = 3; // Slot 2: Empty
        spellSlotIDs[3] = 4; // Slot 3: Empty
        spellSlotIDs[4] = 5; // Slot 4: Empty
        spellSlotIDs[5] = 6; // Slot 5: Empty
        spellSlotIDs[6] = 7; // Slot 6: Empty
        spellSlotIDs[7] = 8; // Slot 7: Empty
        spellSlotIDs[8] = 9; // Slot 8: Empty
        // ...initialize others as needed


        for (int i = 0; i < spellSlotIDs.Length; i++)
        {
            lastCastTimes[i] = -Mathf.Infinity; // Initialize last cast times to allow immediate casting
        }
        // Map spell IDs to methods
        spellActions = new Dictionary<int, System.Action>
        {
            { 1, Spell1 },
            { 2, Spell2 },
            { 3, Spell3 },
            { 4, Spell4 },
            { 5, Spell5 },
            { 6, Spell6 },
            { 7, Spell7 },
            { 8, Spell8 },
            { 9, Spell9 }
            // Add more spells here
        };
    }

    void Update()
    {
        for (int i = 0; i < spellKeys.Length; i++)
        {
            if (Input.GetKeyDown(spellKeys[i]) && !animator.GetBool("IsRolling") && !animator.GetBool("Attacking"))
            {
                // Check cooldown
                if (Time.time - lastCastTimes[i] >= spellCooldowns[i])
                {
                    CastSpell(i);
                    lastCastTimes[i] = Time.time; // Update last cast time
                    Debug.Log("Spell cast with key: " + spellKeys[i]);
                }
                else
                {
                    float timeLeft = spellCooldowns[i] - (Time.time - lastCastTimes[i]);
                    Debug.Log($"Spell {i + 1} is on cooldown for {timeLeft:F1} more seconds.");
                }
            }
        }
    }

    public void CastSpell(int spellSlot)
    {
        int spellID = spellSlotIDs[spellSlot];
        if (spellActions.ContainsKey(spellID) && !animator.GetBool("Attacking") && GlobalVariables.isGamePaused == false)
        {
            spellActions[spellID].Invoke();
        }
        else
        {
            Debug.Log("No spell assigned to slot " + spellSlot);
        }
    }

    public IEnumerator StopCasting()
    {
        yield return new WaitForSeconds(0.3834f);
        animator.SetBool("Casting", false);
        if (magicCircle != null)
        {
            magicCircle.Stop();
        }
    }

    // Assign a spell to a slot at runtime
    public void AssignSpellToSlot(int slot, int spellID)
    {
        if (slot >= 0 && slot < spellSlotIDs.Length)
        {
            spellSlotIDs[slot] = spellID;
            Debug.Log($"Assigned spell ID {spellID} to slot {slot}");
        }
        else
        {
            Debug.LogWarning("Invalid slot index");
        }
    }

    // Example spell methods
    public Vector3 GetMouseWorldPosition()
    {
        Camera cam = GetComponentInChildren<Camera>();
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);

        // Ground plane at y = 0
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float enter;

        if (groundPlane.Raycast(ray, out enter))
        {
            return ray.GetPoint(enter);
        }
        else
        {
            Debug.LogWarning("Mouse ray did not hit the ground plane.");
            return Vector3.zero;
        }
    }

    public void Spell1()
    {
        animator.SetBool("Casting", true);
        animator.SetTrigger("Cast");
        if (magicCircle != null)
        {
            magicCircle.Play();
        }

        Vector3 spawnPos = GetMouseWorldPosition();

        GameObject spawned = Instantiate(spellPrefabs[0], spawnPos, Quaternion.identity);
        Destroy(spawned, 5f);

        StartCoroutine(StopCasting());
    }

    public void Spell2()
    {
        animator.SetBool("Casting", true);
        animator.SetTrigger("Cast");
        if (magicCircle != null)
        {
            magicCircle.Play();
        }

        // Start at the player position
        Vector3 spawnPos = transform.position;
        spawnPos.y = 0.5f; // Ensure it's on the ground

        // Direction from player to mouse world position (on ground)
        Vector3 targetPos = GetMouseWorldPosition();
        Vector3 direction = (targetPos - spawnPos).normalized;

        // Optional: Ignore vertical difference for a flat shot
        direction.y = 0f;
        direction = direction.normalized;

        GameObject spawned = Instantiate(spellPrefabs[1], spawnPos, Quaternion.LookRotation(direction));
        Rigidbody rb = spawned.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = direction * 10f; // Example speed
        }
        Destroy(spawned, 1.2f);

        StartCoroutine(StopCasting());
    }
    public void Spell3()
    {
        animator.SetBool("Casting", true);
        animator.SetTrigger("CastSelf");
        if (magicCircle != null)
        {
            magicCircle.Play();
        }

        Vector3 spawnPos = transform.position;
        spawnPos.y = 0.5f; // Ensure it's on the ground

        GameObject spawned = Instantiate(spellPrefabs[2], spawnPos, Quaternion.identity);
        Destroy(spawned, 4f);

        StartCoroutine(StopCasting());
    }
    public void Spell4()
    {
        // Implement Spell4 logic here
        Debug.Log("Spell4 cast!");
    }
    public void Spell5()
    {
        // Implement Spell5 logic here
        Debug.Log("Spell5 cast!");
    }
    public void Spell6()
    {
        // Implement Spell6 logic here
        Debug.Log("Spell6 cast!");
    }
    public void Spell7()
    {
        // Implement Spell7 logic here
        Debug.Log("Spell7 cast!");
    }
    public void Spell8()
    {
        // Implement Spell8 logic here
        Debug.Log("Spell8 cast!");
    }
    public void Spell9()
    {
        // Implement Spell9 logic here
        Debug.Log("Spell9 cast!");
    }

}
