using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private SpriteRenderer theSR;
    public Sprite defaultImage;
    public Sprite pressedImage;

    public KeyCode KeyToPress;

    void Start()
    {
        theSR = GetComponent<SpriteRenderer>();
        theSR.sprite = defaultImage;
    }


    void Update()
    {
        if (Input.GetKeyDown(KeyToPress))
        {
            theSR.sprite = pressedImage;
        }

        if (Input.GetKeyUp(KeyToPress))
        {
            theSR.sprite = defaultImage;
        }
    }

}