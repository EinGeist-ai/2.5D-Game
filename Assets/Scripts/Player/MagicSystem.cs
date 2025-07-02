using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSystem : MonoBehaviour
{
    public KeyCode[] spellKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.E, KeyCode.Q };

    private Animator animator;


    public GameObject[] spellPrefabs;

    // Each slot holds a spell ID (e.g., 1 = Spell1, 2 = Spell2, etc.)
    public int[] spellSlotIDs = new int[9];

    // Maps spell IDs to their corresponding methods
    private Dictionary<int, System.Action> spellActions;

    void Start()
    {
        animator = GetComponent<Animator>();

        // Initialize spell slot IDs (default: 1, 2, 0, 0, ...)
        spellSlotIDs[0] = 1; // Slot 0: Spell1
        spellSlotIDs[1] = 2; // Slot 1: Spell2
        // ...initialize others as needed

        // Map spell IDs to methods
        spellActions = new Dictionary<int, System.Action>
        {
            { 1, Spell1 },
            { 2, Spell2 }
            // Add more spells here
        };
    }

    void Update()
    {
        for (int i = 0; i < spellKeys.Length; i++)
        {
            if (Input.GetKeyDown(spellKeys[i]) && !animator.GetBool("IsRolling"))
            {
                CastSpell(i);
                Debug.Log("Spell cast with key: " + spellKeys[i]);
            }
        }
    }

    public void CastSpell(int spellSlot)
    {
        int spellID = spellSlotIDs[spellSlot];
        if (spellActions.ContainsKey(spellID))
        {
            animator.SetBool("Casting", true);
            animator.SetTrigger("Cast"); // Always use the same trigger
            StartCoroutine(StopCasting());
            spellActions[spellID].Invoke();
        }
        else
        {
            Debug.Log("No spell assigned to slot " + spellSlot);
        }
    }

    public IEnumerator StopCasting()
    {
        yield return new WaitForSeconds(0.7668f);
        animator.SetBool("Casting", false);
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
    public void Spell1()
    {
        Vector3 spawnPos = transform.position;
        spawnPos.y = 0f;
        GameObject spawned = Instantiate(spellPrefabs[0], spawnPos, Quaternion.identity);
        Destroy(spawned, 5f); // Destroy the spawned instance after 5 seconds
        Debug.Log("Casting Spell 1 (ID 1)");
    }
    public void Spell2()
    {
        Debug.Log("Casting Spell 2 (ID 2)");
        // No animator.SetTrigger here!
    }
}
