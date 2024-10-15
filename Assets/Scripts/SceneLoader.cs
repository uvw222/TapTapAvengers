using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.Rendering;

public class SceneLoader : MonoBehaviour
{
    public AudioSource musicSource;
    private SongData chosenSong=null;
    private string chosenLevel=null;
    public float delayBeforeLoad = 0.5f;

    public void chooseLevel(string level)
    {
        chosenLevel = level;
    }

    public void chooseSong(SongData songData)
    {
        chosenSong=songData;
    }
    public void LoadGame()
    {
        if (musicSource != null)
        {
            musicSource.Stop();
        }

        StartCoroutine(LoadSceneWithDelay(chosenSong, chosenLevel));
    }
    IEnumerator LoadSceneWithDelay(SongData songData, string level)
    {
        yield return new WaitForSeconds(delayBeforeLoad);
        GameManager2.Instance.SetSongData(songData); 
        GameManager2.Instance.SetLevel(level);
        SceneManager.LoadScene("GameScene");

    }
}
