using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteObject : MonoBehaviour
{
    public bool canBePressed;
    public KeyCode keyToPress;
    private bool noteHit = false;

    public GameObject hitEffect, missEffect;

    void Update()
    {
        if (Input.GetKeyDown(keyToPress) && canBePressed)
        {
            if (!noteHit)
            {
                noteHit = true; 
                gameObject.SetActive(false);

                if(Mathf.Abs(transform.position.y) > 0.25f)
                {
                    GameManager.instance.NormalHit();
                    Instantiate(hitEffect, transform.position, hitEffect.transform.rotation);
                }
                
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Activator")
        {
            canBePressed = true;
        }
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.tag == "Activator")
        {
            if (!noteHit)
            {
                canBePressed = false;
                GameManager.instance.NoteMissed();
                Instantiate(missEffect, transform.position, missEffect.transform.rotation);
            }
        }
    }
}
