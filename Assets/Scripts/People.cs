using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class People : MonoBehaviour
{
    Animator myAnimator;
    NavMeshAgent myNma;
    SpriteRenderer myspriteren;
    [SerializeField] GameObject targetPosition;
    // Start is called before the first frame update
    void Start()
    {
        targetPosition = GameObject.Find("TeleporterEnd");
        myspriteren = GetComponentInChildren<SpriteRenderer>();
        myAnimator = GetComponentInChildren<Animator>();
        myNma = GetComponentInChildren<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        WalkForward();
        FlipSprite();
    }
    private void WalkForward()
    {
        myAnimator.SetBool("Walk", true);
        myNma.SetDestination(targetPosition.transform.position);
        float distance = Vector3.Distance(transform.position, targetPosition.transform.position);
        if (distance <= .3f)
        {
            Destroy(gameObject);
        }
    }
    private void FlipSprite()
    {
        bool whichDirectionPlayerFacing = myNma.velocity.x > 0f;
        if (whichDirectionPlayerFacing)
        {
            myspriteren.flipX = false;
        }
        else
        {
            myspriteren.flipX = true;
        }
    }
    //private void StepClimb()
    //{
    //    RaycastHit hitLower;
    //    LayerMask layerMask = 1 << 0;
    //    Debug.DrawRay(stepRayLower.transform.position, Vector3.forward, Color.green, .1f);
    //    if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f, layerMask))
    //    {
    //        RaycastHit hitUpper;
    //        if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f, layerMask))
    //        {
    //            myRigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
    //        }
    //    }
    //    RaycastHit hitLower45;
    //    if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitLower45, 0.1f, layerMask))
    //    {
    //        RaycastHit hitUpper45;
    //        if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitUpper45, 0.2f, layerMask))
    //        {
    //            myRigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
    //        }
    //    }
    //    RaycastHit hitLowerMinus45;
    //    if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitLowerMinus45, 0.1f, layerMask))
    //    {
    //        RaycastHit hitUpperMinus45;
    //        if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitUpperMinus45, 0.2f, layerMask))
    //        {
    //            myRigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
    //        }
    //    }
    //}
}
