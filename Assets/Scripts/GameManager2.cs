using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager2 : MonoBehaviour
{
    public AudioSource audioSource;  // This should be assigned in the Inspector
    public AudioClip song1;  // This field should be visible in the Inspector
    public AudioClip song2;  // This field should be visible in the Inspector
    public AudioClip song3;  // This field should be visible in the Inspector

    private void Start()
    {
        DontDestroyOnLoad(gameObject);  // Keep this GameObject when loading new scenes
    }

    public void PlaySong1()
    {
        audioSource.clip = song1;
        audioSource.Play();
        LoadGameScene();
    }

    public void PlaySong2()
    {
        audioSource.clip = song2;
        audioSource.Play();
        LoadGameScene();
    }

    public void PlaySong3()
    {
        audioSource.clip = song3;
        audioSource.Play();
        LoadGameScene();
    }

    private void LoadGameScene()
    {
        // Load the scene where the game is played
        SceneManager.LoadScene("GameScene");  // Replace with the name of your game scene
    }
}

