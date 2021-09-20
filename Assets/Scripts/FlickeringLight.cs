using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlickeringLight : MonoBehaviour
{
    public bool lightswitch = false;
    private float timeDelay;

    // Update is called once per frame
    void Update()
    {
        if (lightswitch == false)
        {
            StartCoroutine(LightFlicker());
        }
    }
    IEnumerator LightFlicker()
    {
        lightswitch = true;
        this.gameObject.GetComponent<Light>().enabled = false;
        timeDelay = Random.Range(0.01f, 0.2f);
        yield return new WaitForSeconds(timeDelay);
        this.gameObject.GetComponent<Light>().enabled = true;
        timeDelay = Random.Range(0.01f, 1f);
        yield return new WaitForSeconds(timeDelay);
        lightswitch = false;
    }
}
