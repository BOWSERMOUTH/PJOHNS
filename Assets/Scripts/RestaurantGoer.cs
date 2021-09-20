using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RestaurantGoer : MonoBehaviour
{
    Animator myanimator;
    private float animationspeed;
    // Start is called before the first frame update
    void Start()
    {
        myanimator = gameObject.GetComponent<Animator>();
        myanimator.speed = Random.Range(.02f, .1f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
