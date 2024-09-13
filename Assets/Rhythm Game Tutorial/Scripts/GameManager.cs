using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Collections.Generic;
using System.Linq;


public class GameManager : MonoBehaviour
{
    private float totalNotes, normalHits, goodHits, perfectHits, missedHits;
    public GameObject resultsScreen;
    public AudioSource theMusic;
    public bool startPlaying;
    public BeatScroller theBS;
    public static GameManager instance;
    public Text scoreText, multiText, percentHitText, normalsText, goodsText, perfectsText, missesText, rankText, finalScoreText;
    // New AudioClip for countdown
    public AudioClip countdownMusic;

    // Game music
    public AudioClip gameMusic;
    // Countdown timer
    public Text countdownText;  // UI text for the countdown
    public int countdownTime = 3;  // Countdown duration in seconds

    public int currentScore, scorePerNote = 100, scorePerGoodNote = 125, scorePerPerfectNote = 150;
    public int currentMultiplier, multiplierTracker;
    public int[] multiplierThresholds;

    public Button tryAgainButton;
    public Button quitButton;
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
        tryAgainButton.gameObject.SetActive(false);  // Hide at start
        quitButton.gameObject.SetActive(false);
        instance = this;
        scoreText.text = "Score: 0";
        currentMultiplier = 1;
        totalNotes = FindObjectsOfType<NoteObject>().Length;
        resultsScreen.SetActive(false);

        // Set the initial background and song
        SetBackground(0);  // Use the first background
        PlayCountdownMusic();  // Start the countdown music

        // Start the countdown
        StartCoroutine(StartCountdown());
    }

    // Countdown coroutine
    IEnumerator StartCountdown()
    {
        int count = countdownTime;
        while (count > 0)
        {
            countdownText.text = count.ToString();  // Display the countdown
            yield return new WaitForSeconds(1f);  // Wait for 1 second
            count--;
        }

        countdownText.text = "Go!";  // Show "Go!" for 1 second
        yield return new WaitForSeconds(1f);

        countdownText.gameObject.SetActive(false);  // Hide the countdown text

        // Switch to the game music
        PlayGameMusic();

        // Start the game
        startPlaying = true;
        theBS.hasStarted = true;
    }

    void PlayCountdownMusic()
    {
        theMusic.clip = countdownMusic;
        theMusic.Play();
    }

    void PlayGameMusic()
    {
        theMusic.Stop();  // Stop the countdown music
        theMusic.clip = gameMusic;
        theMusic.Play();  // Start the game music
    }

    // Update is called once per frame
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
        // Show buttons (if they are hidden at the start)
        tryAgainButton.gameObject.SetActive(true);
        quitButton.gameObject.SetActive(true);
        SaveScoreToFile(totalHit, percentHit);
    }
    void SaveScoreToFile(float totalHit, float percentHit)
    {
        // Use persistentDataPath instead of dataPath
        string filePath = Application.persistentDataPath + "/scores.txt";
        string songTitle = GetSongTitle();  // Get the title of the current song
        Debug.Log("Saving score for song: " + songTitle);

        // Construct the new score entry for this song
        string newScoreEntry = songTitle + ": Total Hits: " + totalHit.ToString("F0") + ", Percent: " + percentHit.ToString("F1") + "%, Score: " + currentScore;

        // Read the entire file or create it if it doesn't exist
        List<string> lines = new List<string>();
        if (File.Exists(filePath))
        {
            lines = File.ReadAllLines(filePath).ToList();
        }
        else
        {
            // If the file doesn't exist, create initial headers for the three songs
            CreateInitialFile(filePath);
            lines = File.ReadAllLines(filePath).ToList();
        }

        // Process each song's section separately, so no data is lost
        lines = UpdateSongScoresInFile(lines, songTitle, newScoreEntry);

        // Write the updated list back to the file
        File.WriteAllLines(filePath, lines);

        Debug.Log("Top 3 scores saved for " + songTitle + " to: " + filePath);
    }
    List<string> UpdateSongScoresInFile(List<string> lines, string songTitle, string newScoreEntry)
    {
        // Find the section header for the current song
        int songHeaderIndex = lines.FindIndex(line => line.Contains(songTitle + " Records:"));

        if (songHeaderIndex != -1)
        {
            // Find where the scores for this song end (next section or end of file)
            int nextSectionIndex = lines.FindIndex(songHeaderIndex + 1, line => line.Contains("Records:") || string.IsNullOrWhiteSpace(line));
            if (nextSectionIndex == -1) nextSectionIndex = lines.Count;  // If no next section, assume end of file

            // Extract the existing scores for this song
            List<string> songScores = lines.Skip(songHeaderIndex + 1).Take(nextSectionIndex - songHeaderIndex - 1).ToList();

            // Add the new score and sort the list
            songScores.Add(newScoreEntry);
            songScores = songScores.OrderByDescending(GetScoreFromEntry).Take(3).ToList();

            // Remove the old scores for this song
            lines.RemoveRange(songHeaderIndex + 1, nextSectionIndex - songHeaderIndex - 1);

            // Insert the updated top 3 scores back into the file content
            lines.InsertRange(songHeaderIndex + 1, songScores);
            lines.Insert(songHeaderIndex + 1 + songScores.Count, "");  // Add a blank line after the scores
        }

        return lines;  // Return the updated list of lines
    }

    void CreateInitialFile(string filePath)
    {
        // Create the initial file with the three song sections
        string initialContent = "Score Records\n\nFISHER Records:\n\nBaby Shark Records:\n\nNANANA Records:\n\n";
        File.WriteAllText(filePath, initialContent);
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

    // This method appends the scores for each song section to the file
    void AppendScoresForSong(string filePath, string songTitle, List<string> scoresList)
    {
        // Write the song title header
        File.AppendAllText(filePath, songTitle + " Records:\n\n");

        // Append the scores for that song
        foreach (string score in scoresList.Where(s => s.StartsWith(songTitle)))
        {
            File.AppendAllText(filePath, score + "\n");
        }

        // Add a blank line after the song's records
        File.AppendAllText(filePath, "\n");
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
    public void NoteHit()
    {
        Debug.Log("Hit In Time");

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
    // Button click events
    public void TryAgain()
    {
        // Reload the current scene to restart the game
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitToMainMenu()
    {
        // Load the main menu scene
        SceneManager.LoadScene("MainMenu");  // Replace "MainMenu" with the actual name of your main menu scene
    }
}
