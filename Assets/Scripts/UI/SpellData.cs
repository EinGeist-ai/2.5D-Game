using UnityEngine;

[CreateAssetMenu(fileName = "NewSpellData", menuName = "Spells/Spell Data")]
public class SpellData : ScriptableObject
{
    public string spellName; // Name of the spell
    public string description; // Description of the spell
    public int spellID; // Unique identifier for the spell
    public int manaCost; // Mana cost to cast the spell
    public GameObject spellPrefab; // Prefab for the spell effect
    public Sprite icon; // Icon for the spell UI

    public float cooldown; // Cooldown time in seconds
    public bool isUnlocked; // Whether the spell is unlocked for use

    public SpellType spellType; // Use enum for clarity

    public float spellDuration; // Duration of the spell effect (if applicable)
    public int spellDamage; // Damage dealt by the spell (if applicable)
    public float spellSpeed; // Speed of the spell projectile (if applicable)
}
public enum SpellType
{
    AreaOfEffect = 1,
    Projectile = 2,
    SelfCast = 3,
    Buff = 4
}


