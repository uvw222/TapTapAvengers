using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.IO;
using System.Linq;


public class GameManager : MonoBehaviour
{
    private float totalNotes, normalHits, goodHits, perfectHits, missedHits;
    public GameObject resultsScreen;
    public AudioSource theMusic;
    public bool startPlaying;
    public BeatScroller theBS;
    public static GameManager instance;
    public Text scoreText, multiText, percentHitText, normalsText, goodsText, perfectsText, missesText, rankText, finalScoreText, winLoseText;
    public AudioClip countdownMusic;
    public AudioClip gameMusic;
    public Text countdownText;
    public int countdownTime = 3;
    public int currentScore, scorePerNote = 100, scorePerGoodNote = 125, scorePerPerfectNote = 150;
    public int currentMultiplier, multiplierTracker;
    public int[] multiplierThresholds;

    public Button tryAgainButton;
    public Button quitButton;
    public Sprite[] backgrounds; // Array of background images
    public Image backgroundImage;
    public AudioClip[] songs;
    private int currentBackgroundIndex = 0;
    private int currentSongIndex = 0;

    void Start()
    {
        tryAgainButton.gameObject.SetActive(false);
        quitButton.gameObject.SetActive(false);
        instance = this;
        scoreText.text = "Score: 0";
        currentMultiplier = 1;
        totalNotes = FindObjectsOfType<NoteObject>().Length;
        resultsScreen.SetActive(false);

        SetBackground(0);
        PlayCountdownMusic();
        StartCoroutine(StartCountdown());
    }

    IEnumerator StartCountdown()
    {
        int count = countdownTime;
        while (count > 0)
        {
            countdownText.text = count.ToString();
            yield return new WaitForSeconds(1f);
            count--;
        }

        countdownText.text = "Go!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
        PlayGameMusic();
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
        // Stops the countdown music and start the play music
        theMusic.Stop();
        theMusic.clip = gameMusic;
        theMusic.Play();
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

        if(percentHit>70)
        {
            winLoseText.text = "You Win!";
            winLoseText.color = Color.green;
        }
        else
        {
            winLoseText.text = "You Lose!";
            winLoseText.color = Color.red;
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

    void CreateInitialFile(string filePath)
    {
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

    void AppendScoresForSong(string filePath, string songTitle, List<string> scoresList)
    {
        File.AppendAllText(filePath, songTitle + " Records:\n\n");
        foreach (string score in scoresList.Where(s => s.StartsWith(songTitle)))
        {
            File.AppendAllText(filePath, score + "\n");
        }
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

    public void SetBackground(int backgroundIndex)
    {
        if (backgroundIndex >= 0 && backgroundIndex < backgrounds.Length)
        {
            backgroundImage.sprite = backgrounds[backgroundIndex];
            currentBackgroundIndex = backgroundIndex;
        }
    }

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

    public void ChangeBackgroundAndSong(int backgroundIndex, int songIndex)
    {
        SetBackground(backgroundIndex);
        PlaySong(songIndex);
    }

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
    public void TryAgain()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void QuitToMainMenu()
    {
        SceneManager.LoadScene("MainMenu");
    }
}