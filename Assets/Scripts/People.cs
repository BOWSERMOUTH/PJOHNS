using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class People : MonoBehaviour
{
    Rigidbody myRigidbody;
    Animator myAnimator;
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayLower;
    private float stepHeight = .5f;
    private float stepSmooth = 0.15f;
    [SerializeField] float walkSpeed;
    // Start is called before the first frame update
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myAnimator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        StepClimb();
        WalkForward();
    }
    private void WalkForward()
    {
        myAnimator.SetBool("Walk", true);
        myRigidbody.velocity = new Vector3(transform.localScale.x * walkSpeed, myRigidbody.velocity.y, myRigidbody.velocity.z);
    }
    private void StepClimb()
    {
        RaycastHit hitLower;
        LayerMask layerMask = 1 << 0;
        Debug.DrawRay(stepRayLower.transform.position, Vector3.forward, Color.green, .1f);
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f, layerMask))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f, layerMask))
            {
                myRigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
        RaycastHit hitLower45;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitLower45, 0.1f, layerMask))
        {
            RaycastHit hitUpper45;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitUpper45, 0.2f, layerMask))
            {
                myRigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
        RaycastHit hitLowerMinus45;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitLowerMinus45, 0.1f, layerMask))
        {
            RaycastHit hitUpperMinus45;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitUpperMinus45, 0.2f, layerMask))
            {
                myRigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
    }
}
