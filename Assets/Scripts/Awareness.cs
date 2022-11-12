using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Awareness : MonoBehaviour
{
    private SphereCollider sphereCollider;
    private Animator myAnimator;
    private SpriteRenderer spriteRenderer;
    public GameObject exclaim;
    [SerializeField] private AudioSource exclaimsound;

    // Start is called before the first frame update
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        myAnimator = exclaim.GetComponent<Animator>();
        spriteRenderer = exclaim.GetComponent<SpriteRenderer>();
        exclaimsound = exclaim.GetComponent<AudioSource>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Interactable")
        {
            spriteRenderer.enabled = true;
            myAnimator.SetBool("!", true);
            exclaimsound.Play();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Interactable")
        {
            myAnimator.SetBool("!", false);
            spriteRenderer.enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
