using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public AudioSource musicSource; 
    public float delayBeforeLoad = 0.5f;  

    public void LoadScene(string sceneName)
    {
        if (musicSource != null)
        {
            musicSource.Stop(); 
        }

        StartCoroutine(LoadSceneWithDelay(sceneName));
    }

    IEnumerator LoadSceneWithDelay(string sceneName)
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        SceneManager.LoadScene(sceneName);
    }
}
