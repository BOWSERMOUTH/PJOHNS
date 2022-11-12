using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Watch : MonoBehaviour
{
    private float timeofday;
    private int fifteenminincrements;
    //Watch Component References
    public GameObject colon;
    private Text watchText;
    private AudioSource myAudioSource;
    private Animator myAnimator;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("Blink");
        watchText = GameObject.Find("TimeText").GetComponent<Text>();
        myAudioSource = gameObject.GetComponent<AudioSource>();
        myAnimator = gameObject.GetComponent<Animator>();
    }
    IEnumerator Blink()
    {
        yield return new WaitForSeconds(1);
        colon.SetActive(false);
        yield return new WaitForSeconds(1);
        colon.SetActive(true);
        StartCoroutine("Blink");
    }
    public void WatchSound()
    {
        myAnimator.SetBool("ShowWatch", true);
        myAudioSource.Play();
        StartCoroutine("FinishAnimation");
    }
    IEnumerator FinishAnimation()
    {
        yield return new WaitForSeconds(3.5f);
        myAnimator.SetBool("ShowWatch", false);
    }
    private void SendToWatchFace()
    {
        fifteenminincrements = GameObject.Find("GameManager").GetComponent<GameManager>().fifteenminincrements;
        switch (fifteenminincrements)
        {
            case 0:
                watchText.text = "09 00";
                break;
            case 1:
                watchText.text = "09 15";
                break;
            case 2:
                watchText.text = "09 30";
                break;
            case 3:
                watchText.text = "09 45";
                break;
            case 4:
                watchText.text = "10 00";
                break;
            case 5:
                watchText.text = "10 15";
                break;
            case 6:
                watchText.text = "10 30";
                break;
            case 7:
                watchText.text = "10 45";
                break;
            case 8:
                watchText.text = "11 00";
                break;
            case 9:
                watchText.text = "11 15";
                break;
            case 10:
                watchText.text = "11 30";
                break;
            case 11:
                watchText.text = "11 45";
                break;
            case 12:
                watchText.text = "12 00";
                break;
            case 13:
                watchText.text = "12 15";
                break;
            case 14:
                watchText.text = "12 30";
                break;
            case 15:
                watchText.text = "12 45";
                break;
            case 16:
                watchText.text = "01 00";
                break;
            case 17:
                watchText.text = "01 15";
                break;
            case 18:
                watchText.text = "01 30";
                break;
            case 19:
                watchText.text = "01 45";
                break;
            case 20:
                watchText.text = "02 00";
                break;
            case 21:
                watchText.text = "02 15";
                break;
            case 22:
                watchText.text = "02 30";
                break;
            case 23:
                watchText.text = "02 45";
                break;
            case 24:
                watchText.text = "03 00";
                break;
            case 25:
                watchText.text = "03 15";
                break;
            case 26:
                watchText.text = "03 30";
                break;
            case 27:
                watchText.text = "03 45";
                break;
            case 28:
                watchText.text = "04 00";
                break;
            case 29:
                watchText.text = "04 15";
                break;
            case 30:
                watchText.text = "04 30";
                break;
            case 31:
                watchText.text = "04 45";
                break;
            case 32:
                watchText.text = "05 00";
                break;
            case 33:
                watchText.text = "05 15";
                break;
            case 34:
                watchText.text = "05 30";
                break;
            case 35:
                watchText.text = "05 45";
                break;
            case 36:
                watchText.text = "06 00";
                break;
            case 37:
                watchText.text = "06 15";
                break;
            case 38:
                watchText.text = "06 30";
                break;
            case 39:
                watchText.text = "06 45";
                break;
            case 40:
                watchText.text = "07 00";
                break;
            case 41:
                watchText.text = "07 15";
                break;
            case 42:
                watchText.text = "07 30";
                break;
            case 43:
                watchText.text = "07 45";
                break;
            case 44:
                watchText.text = "08 00";
                break;
            case 45:
                watchText.text = "08 15";
                break;
            case 46:
                watchText.text = "08 30";
                break;
            case 47:
                watchText.text = "08 45";
                break;
            case 48:
                fifteenminincrements = 0;
                break;
        }
    }
    // Update is called once per frame
    void Update()
    {
        SendToWatchFace();
    }
}
