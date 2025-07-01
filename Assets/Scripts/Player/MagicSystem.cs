using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSystem : MonoBehaviour
{
    public KeyCode[] spellKeys = { KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4, KeyCode.X, KeyCode.C, KeyCode.V, KeyCode.E, KeyCode.Q };

    private Animator animator;

    void Start()
    {
        animator = GetComponent<Animator>();
    }

    void Update()
    {
        for (int i = 0; i < spellKeys.Length; i++)
        {
            if (Input.GetKeyDown(spellKeys[i]))
            {
                CastSpell(i);
                Debug.Log("Spell cast with key: " + spellKeys[i]);
            }
        }
    }

    public void CastSpell(int spellIndex)
    {
        switch (spellIndex)
        {
            case 0:
                Debug.Log("Casting spell 1");
                animator.SetBool("Casting", true);
                animator.SetTrigger("Cast");
                break;
            case 1:
                Debug.Log("Casting spell 2");
                animator.SetBool("Casting", true);
                animator.SetTrigger("Cast");
                break;
            case 2:
                Debug.Log("Casting spell 3");
                animator.SetBool("Casting", true);
                animator.SetTrigger("Cast");
                break;
            case 3:
                Debug.Log("Casting spell 4");
                animator.SetBool("Casting", true);
                animator.SetTrigger("Cast");
                break;
            case 4:
                Debug.Log("Casting spell 5");
                animator.SetBool("Casting", true);
                animator.SetTrigger("Cast");
                break;
            case 5:
                Debug.Log("Casting spell 6");
                animator.SetBool("Casting", true);
                animator.SetTrigger("Cast");
                break;
            case 6:
                Debug.Log("Casting spell 7");
                animator.SetBool("Casting", true);
                animator.SetTrigger("Cast");
                break;
            case 7:
                Debug.Log("Casting spell 8");
                animator.SetTrigger("Cast");
                break;
            case 8:
                Debug.Log("Casting spell 9");
                animator.SetBool("Casting", true);
                animator.SetTrigger("Cast");
                break;
            default:
                Debug.Log("Unknown spell");
                break;
        }
    }
}
