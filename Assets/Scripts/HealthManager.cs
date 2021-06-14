using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] float healthTimer;
    [SerializeField] float currentTimer;
    public int drip = 9;
    Animator heart1;
    Animator heart2;
    Animator heart3;
    Animator heart4;
    // Start is called before the first frame update
    void Start()
    {
        heart1 = GameObject.Find("Heart 1").GetComponent<Animator>();
        heart2 = GameObject.Find("Heart 2").GetComponent<Animator>();
        heart3 = GameObject.Find("Heart 3").GetComponent<Animator>();
        heart4 = GameObject.Find("Heart 4").GetComponent<Animator>();
        currentTimer = healthTimer;
    }

    // Update is called once per frame
    void Update()
    {
        currentTimer -= Time.deltaTime;
        Hunger();
    }
    private void Hunger()
    {
        if (currentTimer <= (healthTimer - 112.5f))
        {
            drip = drip - 1;
            currentTimer = healthTimer;
            RemoveHealth();
        }
    }
    private void RemoveHealth()
    {
        switch (drip)
        {
            case 9:
                break;
            case 8:
                heart4.SetBool("HalfHealth", true);
                break;
            case 7:
                heart4.gameObject.SetActive(false);
                break;
            case 6:
                heart3.SetBool("HalfHealth", true);
                break;
            case 5:
                heart3.gameObject.SetActive(false);
                break;
            case 4:
                heart2.SetBool("HalfHealth", true);
                break;
            case 3:
                heart2.gameObject.SetActive(false);
                break;
            case 2:
                heart1.SetBool("HalfHealth", true);
                break;
            case 1:
                heart1.gameObject.SetActive(false);
                break;
        }
    }
}
