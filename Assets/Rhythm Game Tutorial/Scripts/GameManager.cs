using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    private float totalNotes, normalHits, goodHits, perfectHits, missedHits;
    public GameObject resultsScreen;
    public AudioSource theMusic;
    public bool startPlaying;
    public BeatScroller theBS;
    public static GameManager instance;
    public Text scoreText, multiText, percentHitText, normalsText, goodsText, perfectsText, missesText, rankText, finalScoreText;
    public int currentScore, scorePerNote = 100, scorePerGoodNote = 125, scorePerPerfectNote = 150;
    public int currentMultiplier, multiplierTracker;
    public int[] multiplierThresholds;
    /// <summary>
    /// //////////////////////////
    /// </summary>
    /// // Background and music switching
    public Sprite[] backgrounds;       // Array of background images
    public Image backgroundImage;      // UI Image for the background
    public AudioClip[] songs;          // Array of songs
    private int currentBackgroundIndex = 0;
    private int currentSongIndex = 0;

    void Start()
    {   

        instance = this;
        scoreText.text = "Score: 0";
        currentMultiplier = 1;
        totalNotes = FindObjectsOfType<NoteObject>().Length;
        resultsScreen.SetActive(false);

        // Set the initial background and song
        SetBackground(0);  // Use the first background
        PlaySong(0);       // Play the first song
    }

    // Update is called once per frame
    void Update()
    {
        if(!startPlaying)
        {
            if(Input.anyKeyDown)
            {
                startPlaying = true;
                theBS.hasStarted = true;
                theMusic.Play();
            }
        }
        else
        {
            if(!(theMusic.isPlaying)  && !(resultsScreen.activeInHierarchy) && startPlaying)
            {
                ShowResults();
            } 
        }
    }
    void ShowResults()
    {
        resultsScreen.SetActive(true);
        normalsText.text = normalHits.ToString();
        goodsText.text = goodHits.ToString();
        perfectsText.text = perfectHits.ToString();
        missesText.text = missedHits.ToString();
        float totalHit = normalHits + goodHits + perfectHits;
        float percentHit = (totalHit / totalNotes) * 100f;
        percentHitText.text = percentHit.ToString("F1") + "%";

        string rankVal;
        if(percentHit > 95)
        {
            rankVal = "A+";
        }
        else if(percentHit > 85)
        {
            rankVal = "A";
        }
        else if(percentHit > 70)
        {
            rankVal = "B";
        }
        else if(percentHit > 55)
        {
            rankVal = "C";
        }
        else if(percentHit > 40)
        {
            rankVal = "D";
        }
        else
        {
            rankVal = "F";
        }
        rankText.text = rankVal;
        finalScoreText.text = currentScore.ToString();
    }
    public void NoteHit()
    {
        Debug.Log("Hit In Time");

        if(currentMultiplier - 1 < multiplierThresholds.Length)
        {
            multiplierTracker++;
            if (multiplierThresholds[currentMultiplier - 1] <= multiplierTracker)
            {
                multiplierTracker = 0;
                currentMultiplier++;
            }
        }
        multiText.text = "Multiplier: x" + currentMultiplier;
        //currentScore += scorePerNote * currentMultiplier;
        scoreText.text = "Score: " + currentScore;
    }
    public void NormalHit()
    {
        currentScore += scorePerNote * currentMultiplier;
        NoteHit();
        normalHits++;
    }
    public void GoodHit()
    {
        currentScore += scorePerGoodNote * currentMultiplier;
        NoteHit();
        goodHits++;
    }
    public void PerfectHit()
    {
        currentScore += scorePerPerfectNote * currentMultiplier;
        NoteHit();
        perfectHits++;
    }
    public void NoteMissed()
    {
        Debug.Log("Missed Note");

        currentMultiplier = 1;
        multiplierTracker = 0;
        multiText.text = "Multiplier: x" + currentMultiplier;
        missedHits++;
    }
    // Change Background
    public void SetBackground(int backgroundIndex)
    {
        if (backgroundIndex >= 0 && backgroundIndex < backgrounds.Length)
        {
            backgroundImage.sprite = backgrounds[backgroundIndex];
            currentBackgroundIndex = backgroundIndex;
        }
    }

    // Play a song
    public void PlaySong(int songIndex)
    {
        if (songIndex >= 0 && songIndex < songs.Length)
        {
            theMusic.Stop();
            theMusic.clip = songs[songIndex];
            theMusic.Play();
            currentSongIndex = songIndex;
        }
    }

    // Method to change both background and song together
    public void ChangeBackgroundAndSong(int backgroundIndex, int songIndex)
    {
        SetBackground(backgroundIndex);
        PlaySong(songIndex);
    }

    // Methods to cycle through songs or backgrounds (optional)
    public void NextBackground()
    {
        currentBackgroundIndex = (currentBackgroundIndex + 1) % backgrounds.Length;
        SetBackground(currentBackgroundIndex);
    }

    public void NextSong()
    {
        currentSongIndex = (currentSongIndex + 1) % songs.Length;
        PlaySong(currentSongIndex);
    }
}

