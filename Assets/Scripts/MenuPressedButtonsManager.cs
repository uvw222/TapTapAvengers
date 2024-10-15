using UnityEngine;
using UnityEngine.UI;

public class ButtonSelector : MonoBehaviour
{
    public Button[] songList;
    public Button[] levelList;
    public Button startButton;  

    private Button selectedSongButton;
    private Button selectedLevelButton;

    public Sprite normalSprite;
    public Sprite pressedSprite;

    void Start()
    {
        startButton.gameObject.SetActive(false); 
    }

    public void OnSongButtonClick(Button clickedButton)
    {
        if (selectedSongButton != null && selectedSongButton != clickedButton)
        {
            selectedSongButton.GetComponent<Image>().sprite = normalSprite;
        }

        clickedButton.GetComponent<Image>().sprite = pressedSprite;
        selectedSongButton = clickedButton;

        CheckSelections();
    }

    public void OnLevelButtonClick(Button clickedButton)
    {
        if (selectedLevelButton != null && selectedLevelButton != clickedButton)
        {
            selectedLevelButton.GetComponent<Image>().sprite = normalSprite;
        }

        clickedButton.GetComponent<Image>().sprite = pressedSprite;
        selectedLevelButton = clickedButton;

        CheckSelections();
    }

    private void CheckSelections()
    {
        if (selectedSongButton != null && selectedLevelButton != null)
        {
            startButton.gameObject.SetActive(true);
        }
        else
        {
            startButton.gameObject.SetActive(false);
        }
    }
}
