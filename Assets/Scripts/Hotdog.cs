using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Hotdog : MonoBehaviour
{
    [SerializeField] float howCloseToPlayer;
    [SerializeField] Vector3 hopDistance;
    [SerializeField] bool freezePigeon;
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayLower;
    [SerializeField] float stepHeight;
    [SerializeField] float stepSmooth;
    [SerializeField] float pigeonSpeed = 4f;
    public GameObject wayPoint;
    Animator myAnimator;
    Rigidbody myRigidbody;
    BoxCollider myBoxCollider;
    Vector3 targetPosition;
    public GameObject player;
    public GameObject crosshair;
    public BoxCollider myFootCollider;
    public bool isTouchingGround;
    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody>();
        myBoxCollider = GetComponent<BoxCollider>();
        player = GameObject.Find("PJohns");
        crosshair = GameObject.Find("crosshair");
    }

    // Update is called once per frame
    void Update()
    {
        GoTo();
        FlipSprite();
        ImListening();
        FreezePigeon();
        StepClimb();
        LookAtPlayer();
        FollowPlayer();
    }
    private void ImListening()
    {
        if (player.GetComponent<Player>().birdWhispering == true)
        {
            myAnimator.SetBool("PigeonListen", true);
            freezePigeon = true;
        }
        else if (player.GetComponent<Player>().birdWhispering == false)
        {
            freezePigeon = false;
            myAnimator.SetBool("PigeonListen", false);
        }
    }
    private void FreezePigeon()
    {
        if (freezePigeon)
        {
            myRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
        else if (!freezePigeon)
        {
            myRigidbody.constraints = RigidbodyConstraints.None;
            myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground")
        {
            isTouchingGround = true;
            myRigidbody.velocity = new Vector3(0f, 0f, 0f);
            myAnimator.SetBool("Flying", false);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            isTouchingGround = false;
        }
    }
    private void Hop()
    {
        if (isTouchingGround && crosshair.GetComponent<Crosshair>().goherenow == false)
        {
            myAnimator.SetBool("Walk", true);
            myRigidbody.velocity = new Vector3((transform.localScale.x * hopDistance.x), hopDistance.y, hopDistance.z);
        }
    }
    private void FlyToPlayer()
    {
        if (isTouchingGround)
        {
            myRigidbody.velocity = new Vector3(transform.localScale.x * (Random.Range(5f, 8f)), 4f, 0f);
        }
    }
    private void LookAtPlayer()
    {
        if (crosshair.GetComponent<Crosshair>().goherenow == false)
        {
            if (transform.position.x < player.transform.position.x)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else if (transform.position.x > player.transform.position.x)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
        else if (crosshair.GetComponent<Crosshair>().goherenow == true)
        {
            if (transform.position.x < wayPoint.transform.position.x)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            else if (transform.position.x > wayPoint.transform.position.x)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
        }
    }
    private void FollowPlayer()
    {
        if (crosshair.GetComponent<Crosshair>().goherenow == false)
        {
            targetPosition = player.transform.position;
            if (Vector3.Distance(transform.position, targetPosition) > howCloseToPlayer && (Vector3.Distance(transform.position, targetPosition) < 5f))
            {
                Hop();
            }
            else if (Vector3.Distance(transform.position, targetPosition) > 5f)
            {
                FlyToPlayer();
                myAnimator.SetBool("Walk", false);
                myAnimator.SetBool("Flying", true);
            }
            else
            {
                myAnimator.SetBool("Walk", false);
            }
            // Move On Z axis
            if (!freezePigeon)
            {
                transform.position = Vector3.MoveTowards(transform.position, new Vector3(transform.position.x, transform.position.y, player.transform.position.z), .5f * Time.deltaTime);
            }
        }

    }
    private void FlipSprite()
    {
        bool whichDirectionPlayerFacing = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon;
        if (whichDirectionPlayerFacing)
        {
            transform.localScale = new Vector3(Mathf.Sign(myRigidbody.velocity.x), 1f, 1f);
        }
    }
    private void StepClimb()
    {
        RaycastHit hitLower;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f))
            {
                myRigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
        RaycastHit hitLower45;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitLower45, 0.1f))
        {
            RaycastHit hitUpper45;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(1.5f, 0, 1), out hitUpper45, 0.2f))
            {
                myRigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
        RaycastHit hitLowerMinus45;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitLowerMinus45, 0.1f))
        {
            RaycastHit hitUpperMinus45;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(-1.5f, 0, 1), out hitUpperMinus45, 0.2f))
            {
                myRigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
    }
    private void GoTo()
    {
        if (crosshair.GetComponent<Crosshair>().goherenow == true)
        {
            myAnimator.SetBool("Flying", true);
            myAnimator.SetBool("Walk", false);
            myRigidbody.useGravity = false;
            wayPoint = GameObject.Find("SendBirds(Clone)");
            transform.position = Vector3.MoveTowards(transform.position, wayPoint.transform.position, pigeonSpeed * Time.deltaTime);
            if (Vector3.Distance(transform.position, wayPoint.transform.position) < 1f)
            {
                Destroy(wayPoint);
                crosshair.GetComponent<Crosshair>().goherenow = false;
                myAnimator.SetBool("Flying", false);
                myRigidbody.useGravity = true;
            }
        }

        
    }
}
