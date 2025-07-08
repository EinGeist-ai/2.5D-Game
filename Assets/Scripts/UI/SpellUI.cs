using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellUI : MonoBehaviour
{
    public GameObject[] SpellSlots; // UI elements for spell slots (e.g., cooldown overlays)
    public MagicSystem magicSystem; // Reference to the MagicSystem script

    void Update()
    {
        if (magicSystem == null || magicSystem.spellIds == null || magicSystem.spellIds.spellData == null)
            return;

        int spellCount = Mathf.Min(
            SpellSlots.Length,
            magicSystem.keyToSpellID.Count
        );

        int slotIndex = 0;
        foreach (var pair in magicSystem.keyToSpellID)
        {
            if (slotIndex >= spellCount)
                break;

            int spellID = pair.Value;
            int spellIndex = GetSpellIndexById(spellID);
            if (spellIndex < 0)
            {
                slotIndex++;
                continue;
            }

            SpellData spell = magicSystem.spellIds.spellData[spellIndex];
            if (spell == null)
            {
                slotIndex++;
                continue;
            }

            float cooldown = spell.cooldown;
            float lastCast = magicSystem.lastCastTimes[spellIndex];
            float timeSinceCast = Time.time - lastCast;

            float scale = 0f;
            if (timeSinceCast < cooldown)
            {
                scale = Mathf.Clamp01(1f - (timeSinceCast / cooldown));
            }

            if (SpellSlots[slotIndex] != null)
            {
                SpellSlots[slotIndex].transform.localScale = new Vector3(scale, 0.04f, 1);
            }

            slotIndex++;
        }
    }

    // Helper to find the index of a spell by its ID
    int GetSpellIndexById(int spellID)
    {
        if (magicSystem == null || magicSystem.spellIds == null || magicSystem.spellIds.spellData == null)
            return -1;

        for (int i = 0; i < magicSystem.spellIds.spellData.Length; i++)
        {
            if (magicSystem.spellIds.spellData[i] != null && magicSystem.spellIds.spellData[i].spellID == spellID)
                return i;
        }
        return -1;
    }
}
