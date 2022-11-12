using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Street1 : MonoBehaviour
{
    [SerializeField] GameObject[] nightlight;
    private int currentactiveindex = 0;
    // Start is called before the first frame update
    void Start()
    {
        nightlight = GameObject.FindGameObjectsWithTag("NightLight");
        foreach (GameObject light in nightlight)
        {
            light.SetActive(false);
        }
    }
    private void TurnOnLights()
    {
        if (GameObject.Find("GameManager").GetComponent<GameManager>().ampm == true)
        {
            foreach(GameObject light in nightlight)
            {
                light.SetActive(true);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        TurnOnLights();
    }
}
