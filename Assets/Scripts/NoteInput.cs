using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class NoteInput : MonoBehaviour
{
    public ScoreManager scoreManager;

    private void Start()
    {
        scoreManager = GameObject.Find("ScoreManagerObject").GetComponent<ScoreManager>();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            CheckForHit();
        }
    }

    private void CheckForHit()
    {
        Vector2 tapPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(tapPosition, Vector2.zero);

        if (hit.collider != null && hit.collider.gameObject.CompareTag("Note"))
        {
            Destroy(hit.collider.gameObject);
            scoreManager.AddScore(10);
        }
        else
        {
            scoreManager.ResetCombo();  // Reset combo if no note is hit
        }
    }
}
