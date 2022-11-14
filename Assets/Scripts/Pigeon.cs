using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityStandardAssets.CrossPlatformInput;

public class Pigeon : MonoBehaviour
{
    // State Machine
    public enum pigeonState { followplayer, followcommand, imlistening, returntoplayer, resetpigeon, respawnpigeon, menu }
    public pigeonState birdState;

    //Pathfinding
    private NavMeshPath path;
    private float elapsed = 0.0f;

    // Cached Component References
    public GameObject target;
    Rigidbody myRigidbody;
    Animator myAnimator;
    BoxCollider myBoxCollider;
    AudioSource myaudiosource;
    public AudioClip wingflap;
    private CharacterController pigeon;
    public GameObject player;
    // MIGHT NOT NEED
    public GameObject playerArm;
    public BoxCollider myFootCollider;
    public bool atPJohnsArm;

    public GameObject crosshair;
    public AudioClip[] audioClips;

    // Floats & Values
    [SerializeField] Vector3 hopDistance;
    Vector3 stop = new Vector3(0f, 0f, 0f);
    Vector3 floorposition = new Vector3(0f, 0f, 0f);
    public Vector3 birdVelocity;
    [SerializeField] float birdSpeed;
    private float gravity = -9.81f;
    Vector3 targetPosition;

    // Booleans
    public bool isTouchingGround;
    public bool jumppressed;
    public bool resetpigeon;

    void Start()
    {
        myaudiosource = gameObject.GetComponent<AudioSource>();
        path = new NavMeshPath();
        elapsed = 0.0f;
        myRigidbody = GetComponent<Rigidbody>();
        myAnimator = GetComponent<Animator>();
        pigeon = GetComponent<CharacterController>();
        myBoxCollider = GetComponent<BoxCollider>();
        player = GameObject.Find("PJohns");
        crosshair = GameObject.Find("crosshair");
        // MIGHT NOT NEED
        playerArm = GameObject.Find("Arm");

        target = player;
    }

    private void PigeonState()
    {
        if (birdState == pigeonState.imlistening)
        {
            ImListening();
        }
        if (birdState == pigeonState.followplayer)
        {
            FollowPlayer();
            IsFlying();
        }
        if (birdState == pigeonState.followcommand)
        {
            FollowCommand();
        }
        if (birdState == pigeonState.returntoplayer)
        {
            Return2Player();
        }
        if (birdState == pigeonState.resetpigeon)
        {
            ResetPigeon();
        }
        if (birdState == pigeonState.respawnpigeon)
        {
            transform.position = new Vector3(target.transform.position.x, target.transform.position.y + 6f, target.transform.position.z);
            birdState = pigeonState.returntoplayer;
        }
        if (birdState == pigeonState.menu)
        {
            return;
        }
    }
    private void Return2Player()
    {
        LookAtTarget();
        target = player;
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, birdSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, playerArm.transform.position) <= 1f)
        {
            crosshair.GetComponent<Crosshair>().thingivehit = null;
            birdState = pigeonState.resetpigeon;
        }
    }
    private void ImListening()
    {
        LookAtTarget();
        myAnimator.SetBool("Walk", false);
        gravity = 0f;
        target = player;
        pigeon.Move(stop);
        transform.position = Vector3.MoveTowards(transform.position, playerArm.transform.position, birdSpeed * Time.deltaTime);

        // If I'm At PJOHNs Arm, Go Behind Him & Freeze My Position While Standing Still
        if (transform.position == playerArm.transform.position)
        {
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 0;
            myAnimator.SetBool("Flying", false);
        }
        else
        {
            myAnimator.SetBool("Flying", true);
        }
    }
    private void ResetPigeon()
    {
        {
            LookAtTarget();
            gameObject.GetComponent<SpriteRenderer>().sortingOrder = 1;
            myAnimator.SetBool("PigeonListen", false);
            myAnimator.SetBool("Flying", false);
            myAnimator.SetBool("Jump", false);
            myAnimator.SetBool("Walk", false);
            resetpigeon = false;
            birdState = pigeonState.followplayer;
        }
    }
    // If Pigeon is touching ground, turn flying off & remove velocity
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Carryable")
        {
            transform.localScale = new Vector3((transform.localScale.x * -1f), transform.localScale.y, transform.localScale.z);
            birdState = pigeonState.returntoplayer;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Interactable")
        {
            target = collision.gameObject;
        }
    }

    // Pigeon Faces Target
    private void LookAtTarget()
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
        LookAtTarget();
        // Target Player & Draw Pathfinding in Red
        gravity = -9.81f;
        target = player;
        elapsed += Time.deltaTime;
        if (elapsed > .1f)
        {
            elapsed -= .1f;
            NavMeshHit floorhit;
            if (NavMesh.SamplePosition(transform.position, out floorhit, 7.0f, NavMesh.AllAreas))
            {
                floorposition = floorhit.position;
                Debug.DrawRay(targetPosition, Vector3.down, Color.white, 7.0f);
            }
            NavMesh.CalculatePath(floorposition, target.transform.position, NavMesh.AllAreas, path);
        }
        for (int i = 0; i < path.corners.Length - 1; i++)
            Debug.DrawLine(path.corners[i], path.corners[i + 1], Color.red);
        Vector3 firstPointDirection = path.corners[1] - transform.position;
        firstPointDirection = firstPointDirection.normalized;
        // If close to PJOHNS, Move Towards him
        if (Vector3.Distance(transform.position, target.transform.position) > 1f && (Vector3.Distance(transform.position, target.transform.position) < 5f))
        {
            if (isTouchingGround)
            {
                hopDistance.x = 2f;
                hopDistance.z = 1f;
                myAnimator.SetBool("Walk", true);
            }
        }
        // If far from PJOHNS, jump towards him
        if (Vector3.Distance(transform.position, target.transform.position) > 5f)
        {
            if (isTouchingGround && !jumppressed)
            {
                hopDistance.x = Random.Range(5f, 8f);
                hopDistance.z = 2f;
                hopDistance.y += Mathf.Sqrt(2.5f * -1.0f * gravity);
                jumppressed = true;
            }
        }
        // If at PJOHNS, stop
        if (Vector3.Distance(transform.position, target.transform.position) < 1f)
        {
            if (isTouchingGround)
            {
                myAnimator.SetBool("Flying", false);
                myAnimator.SetBool("Walk", false);
                hopDistance.x = 0f;
                hopDistance.z = 0f;
            }
        }
        if (Vector3.Distance(transform.position, target.transform.position) > 10f)
        {
            birdState = pigeonState.respawnpigeon;
        }
        // If on the ground stop vertical movement
        if (isTouchingGround && hopDistance.y < 0)
        {
            hopDistance.y = 0f;
            jumppressed = false;
        }
        hopDistance.y += gravity * Time.deltaTime;
        birdVelocity = new Vector3((firstPointDirection.x * hopDistance.x), hopDistance.y, firstPointDirection.z * hopDistance.z);
        pigeon.Move(birdVelocity * Time.deltaTime);
    }
    private void FlipSprite()
    {
        bool whichDirectionPlayerFacing = Mathf.Abs(pigeon.velocity.x) > Mathf.Epsilon;
        if (whichDirectionPlayerFacing)
        {
            transform.localScale = new Vector3(Mathf.Sign(pigeon.velocity.x), 1f, 1f);
        }
    }

    private void FollowCommand()
    {
        LookAtTarget();
        myAnimator.SetBool("Idle", false);
        myAnimator.SetBool("Walk", false);
        myAnimator.SetBool("Flying", true);
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, birdSpeed * Time.deltaTime);
        if (Vector3.Distance(transform.position, target.transform.position) < 0.5f)
        {
            //hotdog.isStopped = true;
            //target.SendMessage("killHealth", 5);
            //if (target.GetComponent<ObjectHealth>().health <= 0)
            {
                //hotdogState = Hotdog.pigeonState.followplayer;
                //crosshair.GetComponent<Crosshair>().thingivehit = null;
                //crosshair.GetComponent<Crosshair>().ivehitsomething = false;
            }
        }
    }
    private bool IsGrounded()
    {
        float floorDistanceFromFoot = pigeon.stepOffset - .25f;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, floorDistanceFromFoot) || pigeon.isGrounded)
        {
            Debug.DrawRay(transform.position, Vector3.down * floorDistanceFromFoot, Color.yellow);
            return true;
        }
        return false;
    }
    private void IsFlying()
    {
        if (!isTouchingGround)
        {
            myAnimator.SetBool("Flying", true);
        }
        else
        {
            myAnimator.SetBool("Flying", false);
        }
    }
    public void CooSound()
    {
        myaudiosource.clip = audioClips[Random.Range(0, 3)];
        myaudiosource.pitch = Random.Range(.9f, 1.3f);
        myaudiosource.Play();
        //myaudiosource.pitch = 1f;
    }

    // Update is called once per frame
    void Update()
    {
        isTouchingGround = IsGrounded();
        IsGrounded();
        FlipSprite();
        PigeonState();
    }
}

