using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficGuy : MonoBehaviour
{
    Animator myAnimator;
    GameObject pJohns;
    // Start is called before the first frame update
    void Start()
    {
        pJohns = GameObject.Find("PJohns");
        myAnimator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            myAnimator.SetBool("Stop", true);
        }
    }
    private void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Player")
        {
            myAnimator.SetBool("Stop", false);
        }
    }
}
