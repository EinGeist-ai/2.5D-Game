using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSystem : MonoBehaviour
{
    public SpellIds spellIds;
    public float[] lastCastTimes; // Last cast times for each spell

    // This dictionary should be set by another script (e.g., SpellKeyBindingManager)
    public Dictionary<KeyCode, int> keyToSpellID = new Dictionary<KeyCode, int>();

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
        if (spellIds == null)
            spellIds = FindObjectOfType<SpellIds>();

        int spellCount = spellIds != null && spellIds.spellData != null ? spellIds.spellData.Length : 0;
        lastCastTimes = new float[spellCount];
        for (int i = 0; i < spellCount; i++)
        {
            lastCastTimes[i] = -spellIds.spellData[i].cooldown;
        }
    }

    void Update()
    {
        if (spellIds == null || spellIds.spellData == null)
            return;

        foreach (var pair in keyToSpellID)
        {
            if (Input.GetKeyDown(pair.Key))
            {
                int spellID = pair.Value;
                SpellData spell = spellIds.GetSpellDataById(spellID);
                if (spell != null && spell.isUnlocked)
                {
                    int spellIndex = GetSpellIndexById(spellID);
                    if (spellIndex >= 0 && Time.time - lastCastTimes[spellIndex] >= spell.cooldown)
                    {
                        CastSpell(spellIndex);
                        lastCastTimes[spellIndex] = Time.time;
                    }
                }
            }
        }
    }

    int GetSpellIndexById(int spellID)
    {
        for (int i = 0; i < spellIds.spellData.Length; i++)
        {
            if (spellIds.spellData[i] != null && spellIds.spellData[i].spellID == spellID)
                return i;
        }
        return -1;
    }

    public void CastSpell(int slot)
    {
        SpellData spellData = spellIds.spellData[slot];
        if (spellData == null || !spellData.isUnlocked)
        {
            Debug.LogWarning("Spell in slot " + slot + " is not unlocked or does not exist.");
            return;
        }

        Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
        Quaternion rotation = Quaternion.identity;

        switch (spellData.spellType)
        {
            case SpellType.AreaOfEffect:
                spawnPos = GetMouseWorldPosition();
                break;
            case SpellType.Projectile:
                spawnPos = transform.position + Vector3.up * 0.5f;
                rotation = Quaternion.LookRotation(GetMouseWorldPosition() - spawnPos);
                break;
            case SpellType.SelfCast:
                spawnPos = transform.position + Vector3.up * 0.5f;
                break;
            case SpellType.Buff:
                // Buff logic here
                break;
        }

        if (spellData.spellPrefab != null)
        {
            animator.SetTrigger("Cast");
            animator.SetBool("Casting", true);
            GameObject spellEffect = Instantiate(spellData.spellPrefab, spawnPos, rotation);
            if (spellData.spellType == SpellType.Projectile)
            {
                Rigidbody rb = spellEffect.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 dir = (GetMouseWorldPosition() - spawnPos).normalized;
                    rb.velocity = dir * spellData.spellSpeed;
                }
            }
            StartCoroutine(StopCasting());
            Destroy(spellEffect, spellData.spellDuration > 0 ? spellData.spellDuration : 5f);
        }
        else
        {
            Debug.LogWarning("No prefab assigned for spell: " + spellData.spellName);
        }
    }

    public Vector3 GetMouseWorldPosition()
    {
        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("No main camera found for mouse world position.");
            return Vector3.zero;
        }
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        Plane groundPlane = new Plane(Vector3.up, Vector3.zero);
        float enter;
        if (groundPlane.Raycast(ray, out enter))
            return ray.GetPoint(enter);
        Debug.LogWarning("Mouse ray did not hit the ground plane.");
        return Vector3.zero;
    }

    public IEnumerator StopCasting()
    {
        yield return new WaitForSeconds(0.3834f);
        animator.SetBool("Casting", false);
    }
}
