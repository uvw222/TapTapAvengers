using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{
    private float totalNotes, normalHits,missedHits;
    public GameObject resultsScreen;
    public AudioSource theMusic;  
    public AudioSource countdownMusicSource;
    public bool startPlaying;
    public BeatScroller theBS;
    public static GameManager instance;
    public Text winLoseText, scoreText, multiText, percentHitText, normalsText, missesText, rankText, finalScoreText, countdownText;
    public int currentScore, scorePerNote = 100;
    public int currentMultiplier, multiplierTracker;
    public int[] multiplierThresholds;
    public Button tryAgainButton;
    public Button quitButton;
    private SongData currentSongData; 
    public SpriteRenderer bg;
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

        currentSongData = GameManager2.Instance.GetSongData();

        if (currentSongData != null)
        {
            SetBackground(currentSongData.backgroundImage);  
            SpawnArrows(currentSongData.arrowsPrefab);      
            StartCoroutine(PlayCountdownAndStartSong());    
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
            bg.sprite = backgroundSprite; 
        }
    }

    void SpawnArrows(GameObject arrowsPrefab)
    {
        if (arrowsPrefab == null)
        {
            Debug.LogError("No arrows prefab assigned in SongData.");
            return;
        }
        GameObject noteHolder = GameObject.Find("NoteHolder");
        if (noteHolder == null)
        {
            noteHolder = new GameObject("NoteHolder");
        }

        GameObject arrowsInstance = Instantiate(arrowsPrefab, noteHolder.transform);
    }

    IEnumerator PlayCountdownAndStartSong()
    {
        countdownText.gameObject.SetActive(true);        
        for (int i = 3; i > 0; i--)
        {
            countdownText.text = i.ToString();  
            yield return new WaitForSeconds(1f);
        }

        countdownText.text = "Go!";
        yield return new WaitForSeconds(1f);
        countdownText.gameObject.SetActive(false);
        PlayGameMusic();
        startPlaying = true;
        StartBeatScroller();
    }

    void PlayGameMusic()
    {
        
        if (currentSongData != null && currentSongData.songClip != null)
        {
            theMusic.clip = currentSongData.songClip; 
            theMusic.Play();
        }
        else
        {
            Debug.LogError("No songClip assigned in the current SongData.");
        }
    }

    void StartBeatScroller()
    {
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
        missesText.text = missedHits.ToString();
        float totalHit = normalHits;
        float arrowAmount = currentSongData.arrowsPrefab.transform.childCount;
        float percentHit = (totalHit / arrowAmount) * 100f;
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
        if (percentHit > 70)
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


