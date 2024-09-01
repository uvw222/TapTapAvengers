using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public AudioSource musicSource;  // Reference to the AudioSource playing the music

    public void LoadScene(string sceneName)
    {
        if (musicSource != null)
        {
            musicSource.Stop();  // Stop the music
        }
        SceneManager.LoadScene(sceneName);
    }
}


