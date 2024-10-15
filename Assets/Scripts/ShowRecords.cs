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
    public Text fisherEasyName1, fisherEasyName2, fisherEasyName3;
    public Text fisherHardName1, fisherHardName2, fisherHardName3;
    public Text babySharkEasyName1, babySharkEasyName2, babySharkEasyName3;
    public Text babySharkHardName1, babySharkHardName2, babySharkHardName3;
    public Text nananaEasyName1, nananaEasyName2, nananaEasyName3;
    public Text nananaHardName1, nananaHardName2, nananaHardName3;
    private string filePath;
    void Start()
    {
        RecordsScreen.SetActive(false);
        filePath = Application.persistentDataPath + "/gameScores.txt";
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

        DisplayTopRecordsForSong(lines, "FISHER (easy)", fisherEasyName1, fisherEasyName2, fisherEasyName3);
        DisplayTopRecordsForSong(lines, "Baby Shark (easy)", babySharkEasyName1, babySharkEasyName2, babySharkEasyName3);
        DisplayTopRecordsForSong(lines, "NANANA (easy)", nananaEasyName1, nananaEasyName2, nananaEasyName3);

        DisplayTopRecordsForSong(lines, "FISHER (hard)", fisherHardName1, fisherHardName2, fisherHardName3);
        DisplayTopRecordsForSong(lines, "Baby Shark (hard)", babySharkHardName1, babySharkHardName2, babySharkHardName3);
        DisplayTopRecordsForSong(lines, "NANANA (hard)", nananaHardName1, nananaHardName2, nananaHardName3);
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
