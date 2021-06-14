using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Awareness : MonoBehaviour
{
    private SphereCollider sphereCollider;
    private Animator myAnimator;
    private SpriteRenderer spriteRenderer;
    [SerializeField] private GameObject exclaim;

    // Start is called before the first frame update
    void Start()
    {
        sphereCollider = GetComponent<SphereCollider>();
        myAnimator = exclaim.GetComponent<Animator>();
        spriteRenderer = exclaim.GetComponent<SpriteRenderer>();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Interactable")
        {
            print("I see something interactable");
            spriteRenderer.enabled = true;
            myAnimator.SetBool("!", true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Interactable")
        {
            print("I've left something");
            myAnimator.SetBool("!", false);
            spriteRenderer.enabled = false;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
