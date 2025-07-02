using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshait : MonoBehaviour
{
    public RectTransform crosshair; // Assign in Inspector (UI element)

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false; // Hide the default cursor
        Cursor.lockState = CursorLockMode.Confined ; // Lock the cursor to the center of the screen
    }

    // Update is called once per frame
    void Update()
    {
        if (crosshair != null)
        {
            crosshair.position = Input.mousePosition;
        }
    }

    public int GetCrosshairDirection(Transform player)
    {
        Vector3 playerScreenPos = Camera.main.WorldToScreenPoint(player.position);
        Vector2 diff = (Vector2)Input.mousePosition - (Vector2)playerScreenPos;
        float h = Mathf.Round(diff.x);
        float v = Mathf.Round(diff.y);

        // Normalize to -1, 0, or 1 for both axes
        h = Mathf.Abs(h) < 1f ? 0 : Mathf.Sign(h);
        v = Mathf.Abs(v) < 1f ? 0 : Mathf.Sign(v);

        // Use your direction logic
        if (v > 0 && h == 0) return 5;     // Up
        if (v > 0 && h > 0) return 6;      // UpRight
        if (v > 0 && h < 0) return 4;      // UpLeft
        if (v < 0 && h == 0) return 1;     // Down
        if (v < 0 && h > 0) return 8;      // DownRight
        if (v < 0 && h < 0) return 2;      // DownLeft
        if (v == 0 && h > 0) return 7;     // Right
        if (v == 0 && h < 0) return 3;     // Left

        return 0; // Center or no direction
    }
}
