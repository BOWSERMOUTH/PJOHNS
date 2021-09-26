using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubwayTrain : MonoBehaviour
{
    public GameObject ldoor1;
    public GameObject ldoor2;
    public GameObject ldoor3;
    public GameObject rdoor1;
    public GameObject rdoor2;
    public GameObject rdoor3;
    private float movementspeed = 2f;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void OpenDoors()
    {
        ldoor1.transform.position = new Vector3((ldoor1.transform.position.x - 2), transform.position.y, transform.position.z);
        ldoor2.transform.position = new Vector3((ldoor2.transform.position.x - 2), transform.position.y, transform.position.z);
        ldoor3.transform.position = new Vector3((ldoor3.transform.position.x - 2), transform.position.y, transform.position.z);
        rdoor1.transform.position = new Vector3((rdoor1.transform.position.x + 2), transform.position.y, transform.position.z);
        rdoor2.transform.position = new Vector3((rdoor2.transform.position.x + 2), transform.position.y, transform.position.z);
        rdoor3.transform.position = new Vector3((rdoor3.transform.position.x + 2), transform.position.y, transform.position.z);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
