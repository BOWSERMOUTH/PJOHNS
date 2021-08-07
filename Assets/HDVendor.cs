using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HDVendor : MonoBehaviour
{
    GameObject player;
    Animator myAnimator;
    public BoxCollider awareness;
    public BoxCollider shoobox;
    // Start is called before the first frame update
    void Start()
    {
        myAnimator = gameObject.GetComponent<Animator>();
        player = GameObject.Find("PJohns");
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            myAnimator.SetBool("Defense", true);
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            Awareness();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Player")
        {
            myAnimator.SetBool("Defense", false);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            myAnimator.SetBool("Shoo", true);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            myAnimator.SetBool("Shoo", false);
        }
    }
    private void Awareness()
    {
        if (transform.position.x < player.transform.position.x)
        {
            transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else if (transform.position.x > player.transform.position.x)
        {
            transform.localScale = new Vector3(-1f, 1f, 1f);
        }
    }
}
