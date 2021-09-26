using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Hotdog : MonoBehaviour
{
    [SerializeField] Vector3 hopDistance;
    [SerializeField] bool freezePigeon;
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayLower;
    private float stepHeight = .5f;
    private float stepSmooth = 0.1f;
    [SerializeField] float pigeonSpeed = 10f;
    public enum pigeonState { followplayer, followcommand, imlistening, returntoplayer, resetpigeon}
    public pigeonState hotdogState;
    public GameObject target;
    Animator myAnimator;
    Rigidbody myRigidbody;
    BoxCollider myBoxCollider;
    Vector3 targetPosition;
    public GameObject player;
    public GameObject playerArm;
    public GameObject crosshair;
    public BoxCollider myFootCollider;
    public bool isTouchingGround;
    public bool resetpigeon;
    public bool atPJohnsArm;
    public GameObject targetObject = null;
    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody>();
        myBoxCollider = GetComponent<BoxCollider>();
        player = GameObject.Find("PJohns");
        crosshair = GameObject.Find("crosshair");
        playerArm = GameObject.Find("Arm");
        target = player;
    }

    // Update is called once per frame
    void Update()
    {
        StepClimb();
        LookAtPlayerOrWayPoint();
        PigeonState();
    }
    private void PigeonState()
    {
        if (hotdogState == pigeonState.imlistening)
        {
            ImListening();
        }
        if (hotdogState == pigeonState.followplayer)
        {
            FollowPlayer();
        }
        if(hotdogState == pigeonState.followcommand)
        {
            FollowCommand();
        }
        if (hotdogState == pigeonState.returntoplayer)
        {
            Return2Player();
        }
        if (hotdogState == pigeonState.resetpigeon)
        {
            ResetPigeon();
        }
    }
    private void Return2Player()
    {
        target = player;
        myAnimator.SetBool("Flying", true);
        myRigidbody.velocity = new Vector3(0f, 0f, 0f);
        transform.position = Vector3.MoveTowards(transform.position, playerArm.transform.position, pigeonSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, playerArm.transform.position) <= .1f)
        {
            crosshair.GetComponent<Crosshair>().thingivehit = null;
            hotdogState = pigeonState.resetpigeon;
        }
    }
    private void ImListening()
    {
        target = player;
        myRigidbody.velocity = new Vector3(0f, 0f, 0f);
        transform.position = Vector3.MoveTowards(transform.position, playerArm.transform.position, pigeonSpeed * Time.deltaTime);
        myRigidbody.useGravity = false;
        myAnimator.SetBool("Flying", true);


        // If I'm At PJOHNs Arm, Go Behind Him & Freeze My Position While Standing Still
        if (transform.position == playerArm.transform.position)
        {
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
            myAnimator.SetBool("Flying", false);
            freezePigeon = true;
        }
    }
    private void ResetPigeon()
    {
        {
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
            myAnimator.SetBool("PigeonListen", false);
            myAnimator.SetBool("Flying", false);
            myAnimator.SetBool("Jump", false);
            myAnimator.SetBool("Walk", false);
            freezePigeon = false;
            myRigidbody.useGravity = true;
            resetpigeon = false;
            hotdogState = pigeonState.followplayer;
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
    // If Pigeon is touching ground, turn flying off & remove velocity
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Ground")
        {
            isTouchingGround = true;
            myRigidbody.velocity = new Vector3(0f, 0f, 0f);
            myAnimator.SetBool("Flying", false);
            myAnimator.SetBool("Walk", true);
        }
        if (other.tag == "Carryable")
        {
            transform.localScale = new Vector3((transform.localScale.x * -1f), transform.localScale.y, transform.localScale.z);
            hotdogState = pigeonState.returntoplayer;
        }
    }
    // If Pigeon is not touching ground, istouchingground = false
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            isTouchingGround = false;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.tag == "Interactable")
        {
            targetObject = collision.gameObject;
        }
    }

    // Makes Pigeon look at either player or waypoint
    private void LookAtPlayerOrWayPoint()
    {
            if (transform.position.x < target.transform.position.x)
            {
                transform.localScale = new Vector3(1f, 1f, 1f);
            }
            // Look left
            else if (transform.position.x > target.transform.position.x)
            {
                transform.localScale = new Vector3(-1f, 1f, 1f);
            }
    }
    // Hops in direction of PJohn
    private void FollowPlayer()
    {
        target = player.gameObject;
        if (Vector3.Distance(transform.position, target.transform.position) > 1f && (Vector3.Distance(transform.position, target.transform.position) < 5f))
        {
            if (isTouchingGround == true)
            {
                myAnimator.SetBool("Walk", true);
                myRigidbody.velocity = new Vector3((transform.localScale.x * hopDistance.x), hopDistance.y, hopDistance.z);
            }
        }
        else if (Vector3.Distance(transform.position, target.transform.position) > 5f)
        {
            if (isTouchingGround == true)
            {
                myRigidbody.velocity = new Vector3(transform.localScale.x * (Random.Range(5f, 8f)), 4f, 0f);
                myAnimator.SetBool("Flying", true);
                myAnimator.SetBool("Walk", false);
            }
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
        LayerMask layerMask = 1 << 0;
        RaycastHit hitLower;
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
    private void FollowCommand()
    {
        myAnimator.SetBool("Idle", false);
        myAnimator.SetBool("Walk", false);
        myAnimator.SetBool("Flying", true);
        myRigidbody.useGravity = false;
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, pigeonSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target.transform.position) < 0.5f)
        {
            freezePigeon = true;
            targetObject.SendMessage("killHealth", 5);
            if (targetObject.GetComponent<ObjectHealth>().health <= 0)
            {
                hotdogState = Hotdog.pigeonState.followplayer;
                crosshair.GetComponent<Crosshair>().thingivehit = null;
                crosshair.GetComponent<Crosshair>().ivehitsomething = false;
            }
        }
    }
}
