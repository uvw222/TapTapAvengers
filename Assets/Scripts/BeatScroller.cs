using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//class that sets the song tempo
public class BeatScroller : MonoBehaviour
{
    public float beatTempo;
    public bool hasStarted;
    void Start()
    {
        beatTempo = beatTempo / 60f;
    }

    void Update()
    {
        if(hasStarted)
        {
            transform.position -= new Vector3(0f, beatTempo * Time.deltaTime, 0f); 
        }
    }
}
