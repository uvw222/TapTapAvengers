using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class ShowRecords : MonoBehaviour//, IPointerClickHandler
{
    public GameObject RecordsScreen;
    public Text fisherName1, fisherName2, fisherName3;
    public Text babySharkName1, babySharkName2, babySharkName3;
    public Text nananaName1, nananaName2, nananaName3;
    private string filePath;
    // Start is called before the first frame update
    void Start()
    {
        RecordsScreen.SetActive(false);
        filePath = Application.persistentDataPath + "/scores.txt";
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    /*public void OnPointerClick(PointerEventData eventData)
    {
        if (!RecordsScreen.activeInHierarchy)
        {
            RecordsScreen.SetActive(true);
            DisplayRecords();
        }
    }*/

    public void DisplayRecords()
    {
        if (!RecordsScreen.activeInHierarchy)
        {
            RecordsScreen.SetActive(true);
        }
            if (!File.Exists(filePath))
        {
            Debug.LogError("Scores file not found: " + filePath);
            return;
        }

        // Read all lines from the scores file
        string[] lines = File.ReadAllLines(filePath);

        // Find the sections for each song and display the top 3 scores
        DisplayTopRecordsForSong(lines, "FISHER", fisherName1, fisherName2, fisherName3);
        DisplayTopRecordsForSong(lines, "Baby Shark", babySharkName1, babySharkName2, babySharkName3);
        DisplayTopRecordsForSong(lines, "NANANA", nananaName1, nananaName2, nananaName3);
    }

    void DisplayTopRecordsForSong(string[] lines, string songTitle, Text name1, Text name2, Text name3)
    {
        // Find the index of the song's records in the file
        int songHeaderIndex = System.Array.FindIndex(lines, line => line.Contains(songTitle + " Records:"));

        if (songHeaderIndex == -1)
        {
            Debug.LogError("No records found for song: " + songTitle);
            return;
        }

        // Extract the top 3 records for this song
        List<string> topScores = new List<string>();

        for (int i = songHeaderIndex + 1; i < lines.Length && topScores.Count < 3; i++)
        {
            if (string.IsNullOrWhiteSpace(lines[i]))
                break;  // Stop if we reach a blank line (end of section)

            // Find the "Score:" substring and extract the number that follows
            int scoreIndex = lines[i].IndexOf("Score:");
            if (scoreIndex != -1)
            {
                // Extract the score value from the record
                string score = lines[i].Substring(scoreIndex + 6).Trim();  // 6 is the length of "Score: "

                // Ensure the extracted part is a valid number
                int parsedScore;
                if (int.TryParse(score, out parsedScore))
                {
                    topScores.Add(score);  // Add the score part to the list
                }
            }
        }

        // Display the records in the appropriate text fields (only scores)
        name1.text = topScores.Count > 0 ? topScores[0] : "XXX";
        name2.text = topScores.Count > 1 ? topScores[1] : "XXX";
        name3.text = topScores.Count > 2 ? topScores[2] : "XXX";
    }
    public void HideRecordsScreen()
    {
        RecordsScreen.SetActive(false);
    }
}
