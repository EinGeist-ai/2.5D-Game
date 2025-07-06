using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public GameObject optionsMenu; // Reference to the options menu GameObject

    void Start()
    {
        Time.timeScale = 0f; // Pause the game at the start
        Cursor.lockState = CursorLockMode.None; // Unlock the cursor
        Cursor.visible = true; // Make the cursor visible
        gameObject.SetActive(true); // Show the start menu
        GlobalVariables.isGamePaused = true; // Set game paused state
        if (optionsMenu != null)
            optionsMenu.SetActive(false); // Hide options menu at start
    }

    public void StartGame()
    {
        Time.timeScale = 1f; // Resume the game
        Cursor.lockState = CursorLockMode.Confined; // Lock the cursor
        Cursor.visible = false; // Hide the cursor
        gameObject.SetActive(false); // Hide the start menu
        GlobalVariables.isGamePaused = false; // Set game paused state to false
    }

    public void OpenOptions()
    {
        if (optionsMenu != null)
            optionsMenu.SetActive(true); // Show the options menu
    }

    public void CloseOptions()
    {
        if (optionsMenu != null)
            optionsMenu.SetActive(false); // Hide the options menu
    }

    public void QuitGame()
    {
        Application.Quit(); // Quit the application
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false; // Stop playing in the editor
        #endif
    }
}
