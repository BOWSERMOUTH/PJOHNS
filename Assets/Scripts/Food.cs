using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Food : MonoBehaviour
{
    public int food = 0;
    private Text text;
    // Start is called before the first frame update
    void Start()
    {
        text = gameObject.GetComponent<Text>();
        text.text = ("Food: " + food);
    }
    public void AddFood()
    {
        food = food + 1;
        text.text = ("Food: " + food);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
