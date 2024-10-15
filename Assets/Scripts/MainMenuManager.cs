using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager2 : MonoBehaviour
{
    public static GameManager2 Instance;
    private SongData currentSongData;
    private string currentLevel;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void SetSongData(SongData songData)
    {
        currentSongData = songData;
    }

    public void SetLevel(string level)
    {
        currentLevel = level;
    }

    public string GetLevel()
    {
        return currentLevel;
    }
    public SongData GetSongData()
    {
        return currentSongData;
    }
}
