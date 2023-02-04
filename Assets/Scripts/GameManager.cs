using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    //Singleton Script
    private static GameManager instance = null;


    // Floats & Variables
    [Header("Player Stats")]
    public int foodCount;
    public int pigeonCount;
    public Vector3 playerSpawnPoint;
    public bool lowervolume = false;
    
    [Header("Time Info")]
    public float sunRotationTime = 348;
    public float timeRate = 48f;
    public float timeofday;
    public bool ampm;
    public int fifteenminincrements;

    // GameObject References
    public GameObject player;
    public GameObject hotdog;
    public TMP_Text pigeoncount;
    public List<GameObject> gameObjects;
    private AudioSource myaudio;
    public List<AudioClip> clips;

    // Script References
    private CinemachineVirtualCamera cinemachine;
    private Vector3 playerlastposition;
    public GameObject daylight;
    public GameObject nightlight;

    // Singleton Data
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


    void Start()
    {
        if (player == null)
        {
            GameObject player = Instantiate(gameObjects[0], new Vector3(0, 0, 4), Quaternion.identity);
            player.name = "PJohns";
        }
        else
        {
            return;
        }
        if (hotdog == null)
        {
            GameObject hotdog = Instantiate(gameObjects[2], new Vector3(2f, 0, 4), Quaternion.identity);
            hotdog.name = "Hotdog";
        }
        CinemachineVirtualCamera cinemachine = GameObject.Find("FollowPlayer").GetComponent<CinemachineVirtualCamera>();
        cinemachine.Follow = player.transform;
        myaudio = gameObject.GetComponent<AudioSource>();
        GameObject.Find("crosshair").GetComponent<Crosshair>().InitializeCrosshair();
        pigeoncount = GameObject.Find("PigeonCountText").GetComponent<TMP_Text>();
        pigeoncount.text = "I am shite";
    }
    public void PoliceTime()
    {
        myaudio.clip = clips[0];
        myaudio.loop = true;
        myaudio.Play();
    }
    public void Spotted()
    {
        myaudio.Stop();
        myaudio.PlayOneShot(clips[1], 1f);
    }
    public void UnSpotted()
    {
        myaudio.Play();
    }
    public void LowerTheVolume()
    {
        if (lowervolume)
        {
            myaudio.volume -= myaudio.volume * Time.deltaTime;
            if (myaudio.volume <= .005f)
            {
                myaudio.Pause();
                myaudio.volume = 1f;
                lowervolume = false;
            }
        }
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
    private void TellTime()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameObject.Find("Watch").GetComponent<Watch>().WatchSound();
        }
    }
    private void DaylightOrbit()
    {
        sunRotationTime += Time.deltaTime;
        //daylight.transform.rotation = Quaternion.Euler(new Vector3((daylight.transform.rotation.x + sunrotationtime / 12f), 0f, 0f));
        //print(daylight.transform.eulerAngles);
    }
    // Update is called once per frame
    void Update()
    {
        LowerTheVolume();
        WhatTimeItIs();
        TellTime();
        //DaylightOrbit();
    }
}
