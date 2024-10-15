using UnityEngine;

[CreateAssetMenu(fileName = "SongData", menuName = "ScriptableObjects/SongData", order = 1)]
public class SongData : ScriptableObject
{
    public string songName;
    public AudioClip songClip;       
    public Sprite backgroundImage;
    public GameObject easyArrowsPrefab; 
    public GameObject hardArrowsPrefab; 
}
