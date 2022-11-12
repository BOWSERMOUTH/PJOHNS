using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NightScript : MonoBehaviour
{
    private AudioSource audioSource;
    // Start is called before the first frame update
    void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
        StartCoroutine("PlayTheSong");
    }
    IEnumerator PlayTheSong()
    {
        yield return new WaitForSeconds(6f);
        audioSource.Play();
        print(" I have started");
    }
}
