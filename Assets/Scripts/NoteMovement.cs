using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteMovement : MonoBehaviour
{
    public float speed = 5f;

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);

        // Optional: Destroy the note if it goes off-screen
        if (transform.position.y < -5f)
        {
            Destroy(gameObject);
        }
    }
}


