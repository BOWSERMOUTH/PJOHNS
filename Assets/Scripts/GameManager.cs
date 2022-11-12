using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    //Singleton Script
    private static GameManager instance = null;
    // Floats & Variables
    private int foodcount;
    // Time
    public float sunrotationtime = 348;
    public float timeRate = 48f;
    public float timeofday;
    public bool ampm;
    public int fifteenminincrements;
    // GameObject References
    public GameObject player;
    private Vector3 playerlastposition;
    public GameObject hotdog;
    public GameObject daylight;
    public GameObject nightlight;
    public List<GameObject> pigeonbox;
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);
    }
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PJohns");
        hotdog = GameObject.Find("Hotdog");
        daylight = GameObject.Find("DayLight");
        nightlight = GameObject.Find("NightLight");
    }
    public void WhatTimeItIs()
    {
        timeofday += Time.deltaTime;
        if (timeofday >= 18.75f)
        {
            timeofday = 0f;
            fifteenminincrements += 1;
            // Turn On PM
            if (fifteenminincrements >= 1 && ampm == false)
            {
                ampm = true;
            }
            // Turn On AM
            if (fifteenminincrements >= 48 && ampm == true)
            {
                ampm = false;
            }
        }
    }
    private void DaylightOrbit()
    {
        sunrotationtime += Time.deltaTime;
        daylight.transform.rotation = Quaternion.Euler(new Vector3((daylight.transform.rotation.x + sunrotationtime / 12f), 0f, 0f));
        print(daylight.transform.eulerAngles);
    }
    private void PigeonBox()
    {
        //pigeonbox = player.GetComponent<Player>().pigeonbox.Count;
    }
    // Update is called once per frame
    void Update()
    {
        WhatTimeItIs();
        //DaylightOrbit();
    }
}
