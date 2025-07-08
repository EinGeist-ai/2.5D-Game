using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpellIds : MonoBehaviour
{
    public SpellData[] spellData; // Assign SpellData assets in Inspector

    public SpellData GetSpellDataById(int spellID)
    {
        foreach (var data in spellData)
        {
            if (data != null && data.spellID == spellID)
                return data;
        }
        return null;
    }
}
