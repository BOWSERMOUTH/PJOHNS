using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class orbitscript : MonoBehaviour
{
    public GameObject centerBall;
    public Transform leftcube;
    public Transform rightcube;
    public float journeyTime = 1.0f;
    private float startTime;
    public float speed;
    public bool repeatable;

    [SerializeField] Vector3 centerPoint;
    Vector3 startRelCenter;
    Vector3 endRelCenter;

    // Update is called once per frame
    private void Start()
    {
        GetCenter(Vector3.up);
    }
    public void GetCenter(Vector3 direction)
    {
        centerPoint = (leftcube.position + rightcube.position) * .5f;
        centerPoint -= direction;
        startRelCenter = leftcube.position - centerPoint;
        endRelCenter = rightcube.position - centerPoint;
    }
    void Update()
    {
        centerBall.transform.position = centerPoint;
        //GetCenter(Vector3.up);
        if (!repeatable)
        {
            float fracComplete = (Time.time - startTime) / journeyTime * speed;
            transform.position = Vector3.Slerp(startRelCenter, endRelCenter, fracComplete * speed);
            transform.position += centerPoint;
        } else
        {
            float fracComplete = Mathf.PingPong(Time.time - startTime, journeyTime / speed);
            transform.position = Vector3.Slerp(startRelCenter, endRelCenter, fracComplete * speed);
            transform.position += centerPoint;
        }
    }
}
