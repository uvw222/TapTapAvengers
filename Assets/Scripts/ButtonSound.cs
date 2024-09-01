using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler
{
    public AudioClip hoverSound;
    public AudioClip clickSound;
    private AudioSource audioSource;

    public float soundVolume = 1.0f;  // Default volume for the button sounds

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound, soundVolume);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound, soundVolume);
        }
    }
}




