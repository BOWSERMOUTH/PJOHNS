using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightScript : MonoBehaviour
{
    private AudioSource audioSource;
    private AudioClip clip;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        clip = gameObject.GetComponent<AudioClip>();
    }
    private void PlaySong()
    {
        audioSource.Play();
        print(" I have started");
    }
}
