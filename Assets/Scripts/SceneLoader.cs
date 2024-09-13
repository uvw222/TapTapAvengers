using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public AudioSource musicSource;  // Reference to the AudioSource playing the music
    public float delayBeforeLoad = 0.5f;  // Delay duration in seconds

    public void LoadScene(string sceneName)
    {
        if (musicSource != null)
        {
            musicSource.Stop();  // Stop the main menu music
        }

        // Start the coroutine to delay the scene transition
        StartCoroutine(LoadSceneWithDelay(sceneName));
    }

    IEnumerator LoadSceneWithDelay(string sceneName)
    {
        // Wait for the delay to complete (to allow the button sound to play)
        yield return new WaitForSeconds(delayBeforeLoad);

        // Now load the new scene
        SceneManager.LoadScene(sceneName);
    }
}
