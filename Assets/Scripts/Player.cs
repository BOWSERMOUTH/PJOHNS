using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
using UnityEngine.UI;
using Cinemachine;

public class Player : MonoBehaviour
{
    //Config
    public enum playerState { GeneralMovement, Climb, Hide, Whisper, Freeze, MindControl, Frozen, Transition, SendAway}
    [SerializeField] playerState state;
    [SerializeField] float jumpHeight = 3f;
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float climbSpeed = 3f;
    [SerializeField] float crouchSpeed = 3f;
    [SerializeField] bool freezeCharacter;
    [SerializeField] bool doubleJump;
    [SerializeField] bool pigeonShield;
    [SerializeField] bool dismiss;
    [SerializeField] bool mindControl;
    [SerializeField] bool fly;
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayLower;
    public List<GameObject> pigeonbox = new List<GameObject>();
    private float stepHeight = .5f;
    private float stepSmooth = 0.1f;
    private int doubletapcount = 0;
    public bool isCrouching;
    public bool playerIsWalking;
    public bool isTouchingGround;
    public bool touchingDumpster;
    public bool touchingLadder;
    public bool holdingLadder;
    public bool imWhispering;
    public bool imHiding;
    public bool pigeonshiding = false;


    //Cached Component References
    Rigidbody myRigidbody;
    Animator myAnimator;
    BoxCollider myBoxCollider;
    public BoxCollider myFootCollider;
    AudioSource myAudioSource;
    public AudioClip footstep;
    public AudioClip[] audioClips;
    Text actionText;
    private GameObject followcam = null;

    //Other Object References
    private GameManager gameManager;
    GameObject crossHair;
    GameObject hotdog;
    public GameObject currentDumpster;
    public GameObject currentLadder;
    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        myRigidbody = GetComponent<Rigidbody>();
        myAnimator = GetComponent<Animator>();
        myBoxCollider = GetComponent<BoxCollider>();
        myAudioSource = GetComponent<AudioSource>();
        crossHair = GameObject.Find("crosshair");
        crossHair.GetComponent<SpriteRenderer>().enabled = false;
        actionText = GameObject.Find("ActionText").GetComponent<Text>();
        hotdog = GameObject.Find("Hotdog");
    }

    void Update()
    {
        //CharacterCantMove();
        PlayerState();
        FlipSprite();
    }
    private void PlayerState()
    {
        if (state == playerState.GeneralMovement)
        {
            Walk();
            Crouch();
            //StepClimb();
            TellTime();
        }
        if (state == playerState.Hide)
        {
            DumpsterDive();
        }
        if (state == playerState.Climb)
        {
            ClimbLadder();
        }
        if (state == playerState.Whisper)
        {
            Whispering();
        }
        if (state == playerState.Frozen)
        {
            myRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
        if (state == playerState.Transition)
        {
            TransitionExit();
            //StepClimb();
        }
        if (state == playerState.SendAway)
        {

        }
    }
    private void OnTriggerEnter(Collider collider)
    {
        if (collider.tag == "Ground")
        {
            isTouchingGround = true;
            myAnimator.SetBool("Jump", false);
        }
        if (collider.gameObject.tag == "Dumpster")
        {
            currentDumpster = collider.gameObject;
            touchingDumpster = true;
        }
        if (collider.gameObject.tag == "Ladder")
        {
            currentLadder = collider.gameObject;
            touchingLadder = true;
        }
        if (collider.gameObject.tag == "TopLadder")
        {
            myAnimator.SetBool("Climb", false);
            myAnimator.SetBool("EndClimb", true);
            StartCoroutine(FinishClimb());
            IEnumerator FinishClimb()
            {
                yield return new WaitForSeconds(1f);
                transform.position = new Vector3(transform.position.x, (transform.position.y + .2f), (transform.position.z + .5f));
                yield return new WaitForSeconds(.5f);
                myAnimator.SetBool("EndClimb", false);
                state = playerState.GeneralMovement;
            }
        }
        if (collider.gameObject.tag == "Lock")
        {
            if (state != playerState.Transition)
            {
                state = playerState.Transition;
            }
            if (state == playerState.Transition)
            {
                state = playerState.GeneralMovement;
                followcam = GameObject.Find("FollowPlayer");
                followcam.GetComponent<CinemachineVirtualCamera>().Follow = gameObject.transform;
            }
        }
        if (collider.gameObject.tag == "NotCaptured")
        {
            pigeonbox.Add(collider.gameObject);
            gameManager.pigeonbox.Add(collider.gameObject);
            collider.gameObject.tag = "Pigeon";
            collider.gameObject.layer = 9;
            collider.gameObject.GetComponent<Pigeon>().state = Pigeon.pigeonState.followplayer;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.tag == "Ground")
        {
            isTouchingGround = false;
            myAnimator.SetBool("Jump", true);
        }
        if (collider.tag == "Dumpster")
        {
            touchingDumpster = false;
            currentDumpster = null;
        }
        if (collider.gameObject.tag == "Ladder")
        {
            currentLadder = null;
            touchingLadder = false;
        }
    }
    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.tag == "Lock")
        {
            actionText.text = "Leave?";
            if (Input.GetKeyDown(KeyCode.E))
            {
                state = playerState.Transition;
                collision.gameObject.GetComponent<BoxCollider>().enabled = false;
                collision.gameObject.GetComponent<Transition>().StartCoroutine("FadeOutAndTransition");
            }
        }
        else
        {
            actionText.text = null;
        }
    }
    private void DumpsterDive()
    {
        if (imHiding == false)
        {
            myRigidbody.velocity = new Vector3(0, 0, 0);
            currentDumpster.gameObject.GetComponent<Dumpster>().IntoDumpster();
            myAnimator.SetBool("IntoDumpster", true);
            imHiding = true;
        }
        else if (Input.GetKeyDown(KeyCode.Space) && imHiding == true)
        {
            currentDumpster.gameObject.GetComponent<Dumpster>().OutDumpster();
            myAnimator.SetBool("OutDumpster", true);
            myAnimator.SetBool("IntoDumpster", false);
            imHiding = false;
            state = playerState.GeneralMovement;
        }
    }
    private void TransitionExit()
    {
        myAnimator.SetBool("Walking", true);
        myAnimator.SetBool("Running", true);
        myRigidbody.velocity = new Vector2((transform.localScale.x * runSpeed/2f), myRigidbody.velocity.y);
    }
    private void ClimbLadder()
    {
        // If I Press space while touching the ladder but not holding it yet, Start Climb animation
        if (Input.GetKeyDown(KeyCode.Space) && touchingLadder == true && holdingLadder == false)
        {
            myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
            myAnimator.SetBool("Climb", true);
        }
        // If I press Space and I'm already holding the ladder, let go of the ladder
        else if (Input.GetKeyDown(KeyCode.Space) && holdingLadder == true)
        {
            myRigidbody.useGravity = true;
            holdingLadder = false;
            myAnimator.SetBool("Climb", false);
        }
        // If I'm holding the ladder, only allow up and down movement
        else if (holdingLadder == true)
        {
            myRigidbody.constraints = RigidbodyConstraints.FreezePositionX;
            myRigidbody.constraints = RigidbodyConstraints.FreezePositionZ;
            float verticalinputspeed = CrossPlatformInputManager.GetAxis("Vertical");
            Vector3 playerZVelocity = new Vector3(myRigidbody.velocity.x, verticalinputspeed * walkSpeed, myRigidbody.velocity.z);
            myRigidbody.velocity = playerZVelocity;
            myAnimator.SetFloat("ClimbSpeed", (.7f * myRigidbody.velocity.y));
        }
    }
    private void MovingIntoDumpster()
    {
        transform.position = new Vector3(transform.position.x, (transform.position.y + .2f), (transform.position.z + .5f));
    }
    private void MovingOutDumpster()
    {
        transform.position = new Vector3(transform.position.x, (transform.position.y - .2f), (transform.position.z - .5f));
    }
    private void GravityOn()
    {
        myRigidbody.useGravity = true;
    }
    private void GravityOff()
    {
        myRigidbody.useGravity = false;
    }
    public void CharacterCantMove()
    {
        if (!freezeCharacter)
        {
            myRigidbody.constraints = RigidbodyConstraints.None;
            myRigidbody.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else if (freezeCharacter)
        {
            myRigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }
    }
    private void Whispering()
    {
        // if you press and HOLD Q..
        if (Input.GetKey(KeyCode.Q))
        {
            // Whisper to birds while holding Q
            myAnimator.SetBool("TalkToBirds", true);
            myAnimator.SetBool("Walking", false);
            crossHair.GetComponent<Crosshair>().crosshairstate = Crosshair.CrosshairState.isactive;
            hotdog.GetComponent<Hotdog>().hotdogState = Hotdog.pigeonState.imlistening;
            pigeonbox[0].GetComponent<Pigeon>().state = Pigeon.pigeonState.imlistening;
        }
        // If you let go of Q after hitting nothing, -> General Movement
        else if (Input.GetKeyUp(KeyCode.Q) && crossHair.GetComponent<Crosshair>().ivehitsomething == false)
        {
            myAnimator.SetBool("TalkToBirds", false);
            //StartCoroutine(FinishWhispering());
            //IEnumerator FinishWhispering()
            //{
            // yield return new WaitForSeconds(.6f);
            //state = Player.playerState.GeneralMovement;
            //}
            state = Player.playerState.GeneralMovement;
            crossHair.GetComponent<Crosshair>().crosshairstate = Crosshair.CrosshairState.isdisabled;
            hotdog.GetComponent<Hotdog>().hotdogState = Hotdog.pigeonState.resetpigeon;
            pigeonbox[0].GetComponent<Pigeon>().state = Pigeon.pigeonState.resetpigeon;
        }
        // If you let go of Q after hitting something -> General Movement
        else if (Input.GetKeyUp(KeyCode.Q) && crossHair.GetComponent<Crosshair>().ivehitsomething == true)
        {
            state = Player.playerState.GeneralMovement;
            myAnimator.SetBool("TalkToBirds", false);
            pigeonbox[0].GetComponent<Pigeon>().state = Pigeon.pigeonState.followplayer;
        }
    }
    private void Walk()
    {
        if (!isCrouching) 
        {
            // X Axis Movement
            float horizontalinputspeed = CrossPlatformInputManager.GetAxis("Horizontal"); // value is between -1 to +1
            Vector2 playerVelocity = new Vector2(horizontalinputspeed * walkSpeed, myRigidbody.velocity.y); // controlling speed of character
            myRigidbody.velocity = playerVelocity; // actually making character move based on above line
            // Player Running? Make animation run
            bool playerisRunning = Mathf.Abs(myRigidbody.velocity.x) > 1.5f;
            myAnimator.SetBool("Running", playerisRunning);
            {
                if (Input.GetKey(KeyCode.LeftShift) && holdingLadder == false)
                {
                    walkSpeed = 4f;
                    if (Input.GetKeyDown(KeyCode.Space) && isTouchingGround)
                    {
                        myRigidbody.velocity = new Vector3(myRigidbody.velocity.x, jumpHeight, myRigidbody.velocity.z);
                        myAudioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
                        myAudioSource.Play();
                    }
                }
                else
                {
                    walkSpeed = 1.5f;
                }
            }
            // Z Axis Movement
            float verticalinputspeed = CrossPlatformInputManager.GetAxis("Vertical");
            Vector3 playerZVelocity = new Vector3(myRigidbody.velocity.x, myRigidbody.velocity.y, verticalinputspeed * walkSpeed);
            myRigidbody.velocity = playerZVelocity;
            if ((myRigidbody.velocity.x == 0f) && (myRigidbody.velocity.z == 0f))
            {
                playerIsWalking = false;
            }
            else
            {
                playerIsWalking= true;
            }
            myAnimator.SetBool("Walking", playerIsWalking);
        }
        else if (isCrouching)
        {
            CrouchWalking();
        }
        // Transition to Climbing
        if (touchingLadder == true && Input.GetKeyDown(KeyCode.Space))
        {
            state = Player.playerState.Climb;
            myAnimator.SetBool("Climb", true);
        }
        // Transition to Hiding
        if (touchingDumpster == true && Input.GetKeyDown(KeyCode.Space))
        {
            state = Player.playerState.Hide;
        }
        // Transition to Whisper or SendAway
        if (Input.GetKeyDown(KeyCode.Q))
        {
            doubletapcount += 1;
            StartCoroutine("SendAwayTimer");
            state = playerState.Whisper;
        }
    }
    private IEnumerator SendAwayTimer()
    {
        yield return new WaitForSeconds(0.5f);
        if (doubletapcount >= 2 && pigeonshiding == false)
        {
            doubletapcount = 0;
            pigeonshiding = true;
            myAnimator.SetBool("SendAway", true);
            yield return new WaitForSeconds(2f);
            myAnimator.SetBool("SendAway", false);
            state = playerState.GeneralMovement;
        }
        else if (doubletapcount >= 2 && pigeonshiding == true)
        {
            doubletapcount = 0;
            myAnimator.SetBool("Recall", true);
            yield return new WaitForSeconds(.3f);
            pigeonshiding = false;
            myAnimator.SetBool("Recall", false);
            state = playerState.GeneralMovement;
        }
        else
        {
            doubletapcount = 0;
        }
    }
    private void TellTime()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            GameObject.Find("Watch").GetComponent<Watch>().WatchSound();
        }
    }
    public void FootstepAudio()
    {
        myAudioSource.PlayOneShot(footstep, 1.0f);
    }
    private void Crouch()
    {
        // Animation tells isCrouching = true & freezes character until after animation completes
        if (Input.GetKeyDown(KeyCode.C) && (!isCrouching))
        {
            myAnimator.SetBool("Crouching", true);
        }
        // Else if I press C and I am crouching
        else if (Input.GetKeyDown(KeyCode.C) && (isCrouching))
        {
            myAnimator.SetBool("Crouching", false);
        }
    }
    private void CrouchWalking()
    {
        if (isCrouching)
        {
            // X Axis Crouching
            float horizontalinputspeed = CrossPlatformInputManager.GetAxis("Horizontal"); // value is between -1 to +1
            Vector2 playerVelocity = new Vector2(horizontalinputspeed * crouchSpeed, myRigidbody.velocity.y); // controlling speed of character
            myRigidbody.velocity = playerVelocity; // actually making character move based on above line

            bool playerHasHorizontalSpeed = Mathf.Abs(myRigidbody.velocity.x) > Mathf.Epsilon; // bool deciding if velocity is higher than 0
            myAnimator.SetBool("CrouchWalking", playerHasHorizontalSpeed);
            // Z Axis Crouching
            float verticalinputspeed = CrossPlatformInputManager.GetAxis("Vertical");
            Vector3 playerZVelocity = new Vector3(myRigidbody.velocity.x, myRigidbody.velocity.y, verticalinputspeed * crouchSpeed);
            myRigidbody.velocity = playerZVelocity;
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
        LayerMask layerMask = 1 << 0;
        Debug.DrawRay(stepRayLower.transform.position, transform.TransformDirection(0, 0, transform.localScale.x), Color.green, .1f);
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f, layerMask))
        {
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f, layerMask))
            {
                myRigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
        RaycastHit hitLower45;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f,0,1), out hitLower45, 0.1f, layerMask))
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

