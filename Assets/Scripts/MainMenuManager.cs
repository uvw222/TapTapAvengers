using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager2 : MonoBehaviour
{
    public AudioSource audioSource;  
    public AudioClip song1;  
    public AudioClip song2;  
    public AudioClip song3;  

    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }

    private void LoadGameScene()
    {
        SceneManager.LoadScene("GameScene");  
    }
}

