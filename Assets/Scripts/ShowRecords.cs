using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class ShowRecords : MonoBehaviour
{
    public GameObject RecordsScreen;
    public Text fisherName1, fisherName2, fisherName3;
    public Text babySharkName1, babySharkName2, babySharkName3;
    public Text nananaName1, nananaName2, nananaName3;
    private string filePath;
    void Start()
    {
        RecordsScreen.SetActive(false);
        filePath = Application.persistentDataPath + "/scores.txt";
    }

    void Update()
    {
        
    }

    public void DisplayRecords()
    {
        if (!RecordsScreen.activeInHierarchy)
        {
            RecordsScreen.SetActive(true);
        }
            if (!File.Exists(filePath))
        {
            return;
        }

        string[] lines = File.ReadAllLines(filePath);

        DisplayTopRecordsForSong(lines, "FISHER", fisherName1, fisherName2, fisherName3);
        DisplayTopRecordsForSong(lines, "Baby Shark", babySharkName1, babySharkName2, babySharkName3);
        DisplayTopRecordsForSong(lines, "NANANA", nananaName1, nananaName2, nananaName3);
    }

    void DisplayTopRecordsForSong(string[] lines, string songTitle, Text name1, Text name2, Text name3)
    {
        int songHeaderIndex = System.Array.FindIndex(lines, line => line.Contains(songTitle + " Records:"));
        if (songHeaderIndex == -1)
        {
            return;
        }

        List<string> topScores = new List<string>();

        for (int i = songHeaderIndex + 1; i < lines.Length && topScores.Count < 3; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                break;  

            int scoreIndex = lines[i].IndexOf("Score:");
            if (scoreIndex != -1)
            {
                string score = lines[i].Substring(scoreIndex + 6).Trim(); 

                int parsedScore;
                if (int.TryParse(score, out parsedScore))
                {
                    topScores.Add(score); 
                }
            }
        }

        name1.text = topScores.Count > 0 ? topScores[0] : "XXX";
        name2.text = topScores.Count > 1 ? topScores[1] : "XXX";
        name3.text = topScores.Count > 2 ? topScores[2] : "XXX";
    }
    public void HideRecordsScreen()
    {
        RecordsScreen.SetActive(false);
    }
}
