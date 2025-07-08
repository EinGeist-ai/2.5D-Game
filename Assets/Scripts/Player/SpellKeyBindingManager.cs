using System.Collections.Generic;
using UnityEngine;

public class SpellKeyBindingManager : MonoBehaviour
{
    public MagicSystem magicSystem;

    [System.Serializable]
    public struct KeySpellPair
    {
        public KeyCode key;
        public int spellID;
    }

    public KeySpellPair[] bindings; // Set up in Inspector

    void Awake()
    {
        if (magicSystem == null)
            magicSystem = FindObjectOfType<MagicSystem>();

        // Clear and set up the mapping
        magicSystem.keyToSpellID = new Dictionary<KeyCode, int>();
        foreach (var pair in bindings)
        {
            magicSystem.keyToSpellID[pair.key] = pair.spellID;
        }
    }
}