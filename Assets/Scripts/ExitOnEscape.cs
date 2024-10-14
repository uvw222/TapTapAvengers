using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitOnEscape : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ExitGame();
        }
    }

    void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
            // If running as a standalone build
            Application.Quit();
        #endif
    }
}

