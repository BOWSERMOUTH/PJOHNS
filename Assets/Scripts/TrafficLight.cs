using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficLight : MonoBehaviour
{
    [SerializeField] bool greenLight;
    [SerializeField] bool yellowLight;
    [SerializeField] bool redLight;
    [SerializeField] float interactionduration;
    private Rigidbody myRigidbody;
    public GameObject green;
    public GameObject yellow;
    public GameObject red;
    // Start is called before the first frame update
    void Start()
    {
        greenLight = true;
        myRigidbody = gameObject.GetComponent<Rigidbody>();
    }
    private void StreetLightFunction()
    {
        if (greenLight)
        {
            yellowLight = false;
            redLight = false;
            red.SetActive(false);
            yellow.SetActive(false);
            green.SetActive(true);
        }
        else if (yellowLight)
        {
            greenLight = false;
            redLight = false;
            red.SetActive(false);
            yellow.SetActive(true);
            green.SetActive(false);
        }
        else if (redLight)
        {
            greenLight = false;
            yellowLight = false;
            red.SetActive(true);
            yellow.SetActive(false);
            green.SetActive(false);
        }
    }
    // Update is called once per frame
    void Update()
    {
        StreetLightFunction();
    }
    public void TamperWithTraffic()
    {
        StartCoroutine(Wait());
    }
    public IEnumerator Wait()
    {
        yield return new WaitForSeconds(interactionduration);
        myRigidbody.useGravity = true;
    }

}
