using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class SceneLoader : MonoBehaviour
{
    public AudioSource musicSource;
    public float delayBeforeLoad = 0.5f;

    public void LoadGame(SongData songData)
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }

        StartCoroutine(LoadSceneWithDelay(songData));
    }

    IEnumerator LoadSceneWithDelay(SongData songData)
    {
        yield return new WaitForSeconds(delayBeforeLoad);

        GameManager2.Instance.SetSongData(songData); 
        //SceneManager.LoadScene("FISHER");
        SceneManager.LoadScene("GameScene");
    }
}
