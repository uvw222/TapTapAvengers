using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI comboText;  // Add a TextMeshProUGUI for displaying the combo
    private int score = 0;
    private int combo = 0;

    void Start()
    {
        UpdateScoreText();
        UpdateComboText();
    }

    public void AddScore(int points)
    {
        combo++;
        score += points * combo;  // Multiply the points by the current combo
        UpdateScoreText();
        UpdateComboText();
    }

    public void ResetCombo()
    {
        combo = 0;
        UpdateComboText();
    }

    private void UpdateScoreText()
    {
        scoreText.text = "Score: " + score.ToString();
    }

    private void UpdateComboText()
    {
        comboText.text = "Combo: " + combo.ToString();
    }
}
