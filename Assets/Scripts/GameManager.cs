using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{
    private float totalNotes, normalHits, goodHits, perfectHits, missedHits;
    public GameObject resultsScreen;
    public AudioSource theMusic;   // AudioSource for the song
    public AudioSource countdownMusicSource;  // Separate AudioSource for countdown music
    public bool startPlaying;
    public BeatScroller theBS;
    public static GameManager instance;
    public Text scoreText, multiText, percentHitText, normalsText, goodsText, perfectsText, missesText, rankText, finalScoreText, countdownText;
    public int currentScore, scorePerNote = 100, scorePerGoodNote = 125, scorePerPerfectNote = 150;
    public int currentMultiplier, multiplierTracker;
    public int[] multiplierThresholds;
    public Button tryAgainButton;
    public Button quitButton;

    private SongData currentSongData;  // The current song's data
    public GameObject buttonsObject;

    // New field for the Bg SpriteRenderer object
    public SpriteRenderer bg;

    // Note holder for spawning arrows
    public Transform noteHolder;

    void Start()
    {
        tryAgainButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        instance = this;
        scoreText.text = "Score: 0";
        currentMultiplier = 1;
        totalNotes = FindObjectsOfType<NoteObject>().Length;
        resultsScreen.SetActive(false);

        // Disable buttons at the start
        if (buttonsObject != null)
        {
            buttonsObject.SetActive(false);
        }

        // Fetch the SongData from GameManager2
        currentSongData = GameManager2.Instance.GetSongData();

        if (currentSongData != null)
        {
            SetBackground(currentSongData.backgroundImage);  // Set the background from SongData
            SpawnArrows(currentSongData.arrowsPrefab);       // Spawn arrows from SongData
            StartCoroutine(PlayCountdownAndStartSong());     // Play countdown music first, then the actual song
        }
        else
        {
            Debug.LogError("No SongData provided to GameManager.");
        }
    }

    void SetBackground(Sprite backgroundSprite)
    {
        if (bg == null)
        {
            return;
        }

        if (backgroundSprite != null)
        {
            bg.sprite = backgroundSprite;  // Set the sprite for the background image
        }
    }

    /// <summary>
    /// Spawns the arrows from the prefab in the SongData.
    /// </summary>
    /// <param name="arrowsPrefab">The prefab containing the arrows for the current song.</param>
    void SpawnArrows(GameObject arrowsPrefab)
    {
        if (arrowsPrefab == null)
        {
            Debug.LogError("No arrows prefab assigned in SongData.");
            return;
        }

        // Find or create the NoteHolder parent object
        GameObject noteHolder = GameObject.Find("NoteHolder");
        if (noteHolder == null)
        {
            noteHolder = new GameObject("NoteHolder");
        }

        // Instantiate the arrows prefab as a child of NoteHolder
        GameObject arrowsInstance = Instantiate(arrowsPrefab, noteHolder.transform);
    }

    IEnumerator PlayCountdownAndStartSong()
    {
        // Assuming you have a Text UI element in the scene assigned to this
        countdownText.gameObject.SetActive(true);

        // Start countdown from 3 down to 0
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();  // Update the text with the countdown number
            yield return new WaitForSeconds(1f); // Wait for 1 second
        }

        // After the countdown, set text to "Go!" and then hide it
        countdownText.text = "Go!";
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);

        // Enable the buttons after the countdown
        if (buttonsObject != null)
        {
            buttonsObject.SetActive(true);
        }

        // Start playing the actual song after the countdown
        PlayGameMusic();
        startPlaying = true;

        // Start the BeatScroller to move the arrows
        StartBeatScroller();
    }

    void PlayGameMusic()
    {
        // Play the selected song from SongData
        if (currentSongData != null && currentSongData.songClip != null)
        {
            theMusic.clip = currentSongData.songClip;  // Assign the song from SongData
            theMusic.Play();  // Play the game music
        }
        else
        {
            Debug.LogError("No songClip assigned in the current SongData.");
        }
    }

    // New function to start the BeatScroller
    void StartBeatScroller()
    {
        // Try to find the BeatScroller component on the instantiated NoteHolder object
        BeatScroller beatScroller = noteHolder.GetComponentInChildren<BeatScroller>();

        if (beatScroller != null)
        {
            beatScroller.hasStarted = true;
        }
    }


    void Update()
    {
        if (startPlaying)
        {
            if (!(theMusic.isPlaying) && !(resultsScreen.activeInHierarchy))
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
        if (percentHit > 95)
        {
            rankVal = "A+";
        }
        else if (percentHit > 85)
        {
            rankVal = "A";
        }
        else if (percentHit > 70)
        {
            rankVal = "B";
        }
        else if (percentHit > 55)
        {
            rankVal = "C";
        }
        else if (percentHit > 40)
        {
            rankVal = "D";
        }
        else
        {
            rankVal = "F";
        }
        rankText.text = rankVal;
        finalScoreText.text = currentScore.ToString();
        tryAgainButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        SaveScoreToFile(totalHit, percentHit);
    }

    void SaveScoreToFile(float totalHit, float percentHit)
    {
        string filePath = Application.persistentDataPath + "/scores.txt";
        string songTitle = GetSongTitle();

        string newScoreEntry = songTitle + ": Total Hits: " + totalHit.ToString("F0") + ", Percent: " + percentHit.ToString("F1") + "%, Score: " + currentScore;

        List<string> lines = new List<string>();
        if (File.Exists(filePath))
        {
            lines = File.ReadAllLines(filePath).ToList();
        }
        else
        {
            CreateInitialFile(filePath);
            lines = File.ReadAllLines(filePath).ToList();
        }

        lines = UpdateSongScoresInFile(lines, songTitle, newScoreEntry);

        File.WriteAllLines(filePath, lines);
    }

    void CreateInitialFile(string filePath)
    {
        string initialContent = "Score Records\n\nFISHER Records:\n\nBaby Shark Records:\n\nNANANA Records:\n\n";
        File.WriteAllText(filePath, initialContent);
    }

    List<string> UpdateSongScoresInFile(List<string> lines, string songTitle, string newScoreEntry)
    {
        int songHeaderIndex = lines.FindIndex(line => line.Contains(songTitle + " Records:"));

        if (songHeaderIndex != -1)
        {
            int nextSectionIndex = lines.FindIndex(songHeaderIndex + 1, line => line.Contains("Records:") || string.IsNullOrWhiteSpace(line));
            if (nextSectionIndex == -1) nextSectionIndex = lines.Count;
            List<string> songScores = lines.Skip(songHeaderIndex + 1).Take(nextSectionIndex - songHeaderIndex - 1).ToList();
            songScores.Add(newScoreEntry);
            songScores = songScores.OrderByDescending(GetScoreFromEntry).Take(3).ToList();
            lines.RemoveRange(songHeaderIndex + 1, nextSectionIndex - songHeaderIndex - 1);
            lines.InsertRange(songHeaderIndex + 1, songScores);
            lines.Insert(songHeaderIndex + 1 + songScores.Count, "");
        }

        return lines;
    }

    int GetScoreFromEntry(string scoreEntry)
    {
        string[] parts = scoreEntry.Split(new string[] { "Score: " }, System.StringSplitOptions.None);
        if (parts.Length > 1)
        {
            if (int.TryParse(parts[1], out int score))
            {
                return score;
            }
        }
        return 0;
    }
    string GetSongTitle()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        switch (sceneName)
        {
            case "FISHER":
                return "FISHER";
            case "BabyShark":
                return "Baby Shark";
            case "NANANA":
                return "NANANA";
            default:
                return "Unknown Song";
        }
    }

    public void NoteHit()
    {
        if (currentMultiplier - 1 < multiplierThresholds.Length)
        {
            multiplierTracker++;
            if (multiplierThresholds[currentMultiplier - 1] <= multiplierTracker)
            {
                multiplierTracker = 0;
                currentMultiplier++;
            }
        }
        multiText.text = "Multiplier: x" + currentMultiplier;
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
        currentMultiplier = 1;
        multiplierTracker = 0;
        multiText.text = "Multiplier: x" + currentMultiplier;
        missedHits++;
    }

    public void TryAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}


