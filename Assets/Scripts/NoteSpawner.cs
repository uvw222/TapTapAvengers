using UnityEngine;

public class NoteSpawner : MonoBehaviour
{
    public GameObject notePrefab;  // This should create a field in the Inspector
    public Transform spawnPoint;
    public float spawnInterval = 1.0f;

    private void Start()
    {
        InvokeRepeating("SpawnNote", 1.0f, spawnInterval);
    }

    void SpawnNote()
    {
        // Ensure that each note starts at the fixed spawn position
        Vector3 spawnPosition = new Vector3(spawnPoint.position.x, spawnPoint.position.y, 0);
        Instantiate(notePrefab, spawnPosition, Quaternion.identity);
    }



}
