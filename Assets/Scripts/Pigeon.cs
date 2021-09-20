using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Pigeon : MonoBehaviour
{
    [SerializeField] Vector3 hopDistance;
    [SerializeField] bool freezePigeon;
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayLower;
    public enum pigeonState { followplayer, notcaptured, imlistening }
    public pigeonState pigeonstate;
    private float stepHeight = .5f;
    private float stepSmooth = 0.2f;
    Animator myAnimator;
    Rigidbody myRigidbody;
    BoxCollider myBoxCollider;
    public Vector3 targetPosition;
    public GameObject player;
    public BoxCollider myFootCollider;
    public bool isTouchingGround;
    public bool resetpigeon;
    public bool captured = false;
    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody>();
        myBoxCollider = GetComponent<BoxCollider>();
        player = GameObject.Find("PJohns");
        pigeonstate = pigeonState.notcaptured;
    }

    // Update is called once per frame
    void Update()
    {
        StepClimb();
        FlipSprite();
        LookAtPlayer();
        PigeonState();
        ResetPigeon();
    }
    private void PigeonState()
    {
        if (pigeonstate == pigeonState.imlistening)
        {
            ImListening();
        }
        if (pigeonstate == pigeonState.followplayer)
        {
            FollowPlayer();
        }
        if (pigeonstate == pigeonState.notcaptured)
        {
            myAnimator.SetBool("Idle", true);
        }
    }
    private void ImListening()
    {
        myAnimator.SetBool("PigeonListen", true);
        freezePigeon = true;
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
    private void ResetPigeon()
    {
        if (resetpigeon == true)
        {
            myAnimator.SetBool("PigeonListen", false);
            myAnimator.SetBool("Flying", false);
            myAnimator.SetBool("Jump", false);
            myAnimator.SetBool("Walk", false);
            freezePigeon = false;
            myRigidbody.useGravity = true;
            resetpigeon = false;
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
        else if (other.tag == "Player" && captured == false)
        {
            print("i've collided with PJOhns");
            player.GetComponent<Player>().pigeonbox.Add(this.gameObject);
            captured = true;
            pigeonstate = pigeonState.followplayer;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Ground")
        {
            isTouchingGround = false;
        }
    }
    private void LookAtPlayer()
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
    private void FollowPlayer()
    {
        myAnimator.SetBool("PigeonListen", false);
        targetPosition = player.transform.position;
        if (Vector3.Distance(transform.position, targetPosition) > 1f && (Vector3.Distance(transform.position, targetPosition) < 5f))
        {
            if (isTouchingGround == true)
            {
                myAnimator.SetBool("Walk", true);
                myRigidbody.velocity = new Vector3((transform.localScale.x * hopDistance.x), hopDistance.y, hopDistance.z);
            }
        }
        else if (Vector3.Distance(transform.position, targetPosition) > 5f)
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
    private void CommandExecute()
    {

    }
    private void MoveOnZ()
    {

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
        Debug.DrawRay(stepRayLower.transform.position, transform.TransformDirection(0, 0, transform.localScale.x), Color.green, .1f);
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(0,0,transform.localScale.x), out hitLower, 0.1f, layerMask))
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
    }
}
