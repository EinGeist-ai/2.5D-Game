using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellUI : MonoBehaviour
{
    public GameObject[] SpellSlots;
    public MagicSystem magicSystem; // Reference to the MagicSystem script

    void Update()
    {
        for (int i = 0; i < SpellSlots.Length; i++)
        {
            float cooldown = magicSystem.spellCooldowns[i];
            float lastCast = magicSystem.lastCastTimes[i];
            float timeSinceCast = Time.time - lastCast;

            float scale = 1f;
            if (timeSinceCast < cooldown)
            {
                scale = Mathf.Clamp01(1f - (timeSinceCast / cooldown));
            }
            else
            {
                scale = 0f;
            }

            if (SpellSlots[i] != null)
            {
                SpellSlots[i].transform.localScale = new Vector3(scale, 0.04f, 1);
            }
        }
    }
}
