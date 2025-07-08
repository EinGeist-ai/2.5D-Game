using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SpellBook : MonoBehaviour
{
    [SerializeField] float pageSpeed = 0.5f;
    [SerializeField] List<Transform> pages;
    int index = -1;

    public void RotateForward()
    {
        index++;
        float angel = 180f;
        StartCoroutine(Rotate(angel, true));
    }
    public void RotateBack()
    {
        float angel = 0f;
        StartCoroutine(Rotate(angel, false));
    }


    IEnumerator Rotate(float angle, bool forward)
    {
        float value = 0f;
        while (true)
        {
            Quaternion targetRotation = Quaternion.Euler(0, angle, 0);
            value += Time.deltaTime * pageSpeed;
            pages[index].rotation = Quaternion.Slerp(pages[index].rotation, targetRotation, value);
            float currentAngle = Quaternion.Angle(pages[index].rotation, targetRotation);
            if (currentAngle < 0.1f)
            {
                if (forward == false)
                {
                    index--;
                }
                else
                {
                    index++;
                }
                yield return null;
            }
        }
    }

}