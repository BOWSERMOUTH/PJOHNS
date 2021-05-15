using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Pigeon : MonoBehaviour
{
    [SerializeField] float howCloseToPlayer;
    [SerializeField] Vector3 hopDistance;
    [SerializeField] bool freezePigeon;
    Animator myAnimator;
    Rigidbody myRigidbody;
    BoxCollider myBoxCollider;
    public Vector3 targetPosition;
    public GameObject player;
    public BoxCollider myFootCollider;
    public bool isTouchingGround;
    // Start is called before the first frame update
    void Start()
    {
        myAnimator = GetComponent<Animator>();
        myRigidbody = GetComponent<Rigidbody>();
        myBoxCollider = GetComponent<BoxCollider>();
        player = GameObject.Find("PJohns");
    }

    // Update is called once per frame
    void Update()
    {
        FlipSprite();
        FollowPlayer();
        LookAtPlayer();
        MoveOnZ();
        ImListening();
        FreezePigeon();
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
        if (isTouchingGround)
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
}
