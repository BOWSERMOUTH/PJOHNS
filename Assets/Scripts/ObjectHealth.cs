using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectHealth : MonoBehaviour
{
    public float health = 5f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void killHealth()
    {
        health = health - Time.deltaTime;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
