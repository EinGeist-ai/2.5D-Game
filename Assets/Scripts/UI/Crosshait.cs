using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshait : MonoBehaviour
{
    public RectTransform crosshair; // Assign in Inspector (UI element)
    public Transform player;        // Assign in Inspector
    public Animator playerAnimator; // Assign in Inspector

    void Start()
    {
        Cursor.visible = false; // Hide the default cursor
        Cursor.lockState = CursorLockMode.Confined; // Lock the cursor to the center of the screen
    }

    void Update()
    {
        if (crosshair != null)
        {
            crosshair.position = Input.mousePosition;
        }

        if (player != null && playerAnimator != null)
        {
            int dir = GetCrosshairDirection(player);
            playerAnimator.SetInteger("CastDirection", dir);
        }
    }

    public int GetCrosshairDirection(Transform player)
    {
        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(player.position);
        Vector2 diff = (Vector2)Input.mousePosition - (Vector2)playerScreenPos;

        float angle = Mathf.Atan2(diff.y, diff.x) * Mathf.Rad2Deg;
        // Angle is in degrees, 0 is right, 90 is up, -90 is down

        // 8 directions, clockwise, starting from up (5)
        if (angle >= -22.5f && angle < 22.5f) return 7;         // Right
        if (angle >= 22.5f && angle < 67.5f) return 6;          // UpRight
        if (angle >= 67.5f && angle < 112.5f) return 5;         // Up
        if (angle >= 112.5f && angle < 157.5f) return 4;        // UpLeft
        if (angle >= 157.5f || angle < -157.5f) return 3;       // Left
        if (angle >= -157.5f && angle < -112.5f) return 2;      // DownLeft
        if (angle >= -112.5f && angle < -67.5f) return 1;       // Down
        if (angle >= -67.5f && angle < -22.5f) return 8;        // DownRight

        return 0; // Center or no direction
    }
}
