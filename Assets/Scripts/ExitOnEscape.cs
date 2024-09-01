using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitOnEscape : MonoBehaviour
{
    void Update()
    {
        // Check if the Escape key is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }
    }

    void ExitGame()
    {
        // If running in the Unity Editor
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
            // If running as a standalone build
            Application.Quit();
#endif
    }
}

