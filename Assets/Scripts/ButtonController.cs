using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonController : MonoBehaviour
{
    private SpriteRenderer theSR;
    public Sprite defaultImage;
    public Sprite pressedImage;

    public KeyCode KeyToPress;

    // Start is called before the first frame update
    void Start()
    {
        theSR = GetComponent<SpriteRenderer>();

        if (theSR == null)
        {
            Debug.LogError("No SpriteRenderer found on " + gameObject.name);
        }

        // Manually set the sprite to the pressed image at the start
        theSR.sprite = pressedImage;
    }


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyToPress))
        {
            Debug.Log("Key Pressed: " + KeyToPress);
            theSR.sprite = pressedImage;
        }

        if (Input.GetKeyUp(KeyToPress))
        {
            Debug.Log("Key Released: " + KeyToPress);
            theSR.sprite = defaultImage;
        }
    }

}