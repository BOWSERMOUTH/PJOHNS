using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PJohns : MonoBehaviour
{
    public enum playerState { GeneralMovement, Falling, Crouch, Climb, Hide, Whisper, Freeze, MindControl, Frozen, Transition, SendAway, Ledge }
    //Config
    [SerializeField] playerState state;
    [SerializeField] float jumpHeight;
    private float walkSpeed = 2f;
    private float zwalkSpeed = 1.9f;
    private float runSpeed = 4f;
    private float zrunSpeed = 3.9f;
    [SerializeField] float climbSpeed = 3f;
    [SerializeField] float crouchSpeed = 1.8f;
    [SerializeField] bool freezeCharacter;
    [SerializeField] bool doubleJump;
    [SerializeField] bool pigeonShield;
    [SerializeField] bool dismiss;
    [SerializeField] bool mindControl;
    [SerializeField] bool fly;
    public Vector3 fallspeed;
    public Vector3 playerVelocity;
    public Vector3 direction;
    public Vector3 ledgeGrabPoint;
    private float gravity = -9.81f;
    private int doubletapcount = 0;
    public bool isTouchingGround;
    public bool jumppressed = false;
    public bool isCrouching;
    public bool playerIsWalking;
    public bool touchingDumpster;
    public bool touchingLadder;
    public bool holdingLadder;
    public bool imWhispering;
    public bool imHiding;
    public bool pigeonshiding = false;
    public bool ledgecapable;
    public bool topraybool;
    public bool lowraybool;
    private float ledgerayx;


    //Cached Component References
    private GameObject gamemanager;
    public GameObject policeGenerator;
    private CharacterController controller;
    public BoxCollider myCollider;
    Animator myAnimator;
    AudioSource myAudioSource;
    public AudioClip footstep;
    public AudioClip[] audioClips;
    Text actionText;
    private GameObject followcam = null;

    //Other Object References
    public Vector3 currentLedge;
    public List<GameObject> pigeonbox = new List<GameObject>();
    private GameManager gameManager;
    GameObject crossHair;
    GameObject hotdog;
    public GameObject currentDumpster = null;
    public GameObject currentLadder = null;
    private float ladderYpos;


    // Start is called before the first frame update
    void Start()
    {
        gamemanager = GameObject.Find("GameManager");
        myAudioSource = gameObject.GetComponent<AudioSource>();
        myAnimator = gameObject.GetComponent<Animator>();
        controller = gameObject.GetComponent<CharacterController>();
        myCollider = gameObject.GetComponent<BoxCollider>();
        crossHair = GameObject.Find("crosshair");
        crossHair.GetComponent<SpriteRenderer>().enabled = false;
        hotdog = GameObject.Find("Hotdog");
    }
    private void PlayerState()
    {
        if (state == playerState.GeneralMovement)
        {
            Walk();

            //StepClimb();
            //TellTime();
            //Whispering();
        }
        if (state == playerState.Crouch)
        {
            Crouch();
        }
        if (state == playerState.Falling)
        {
            controller.Move(playerVelocity * Time.deltaTime);
        }
        if (state == playerState.Whisper)
        {
            Whispering();
        }
        if (state == playerState.Ledge)
        {
            LedgeHang();
        }
        if (state == playerState.Climb)
        {
            ClimbLadder();
        }
        if (state == playerState.Hide)
        {
            DumpsterDive();
        }
    }
    private void Walk()
    {
        myAnimator.SetBool("CrouchWalking", false);
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        direction = new Vector3((horizontal * walkSpeed), 0, (vertical * zwalkSpeed));
        if (direction.magnitude >= 0.1f)
            {
                controller.Move(direction * Time.deltaTime);
            }
        // Run by holding LEFTSHIFT
        bool playerisRunning = Mathf.Abs(direction.x) > 2.5f;
        myAnimator.SetBool("Running", playerisRunning);
        if (Input.GetKey(KeyCode.LeftShift))
        {
            walkSpeed = runSpeed;
            zwalkSpeed = zrunSpeed;
        }
        else
        {
            walkSpeed = 2f;
            zwalkSpeed = 1.9f;
        }
        //JUMPING LOGIC
        // Press SPACE to Jump if on the ground
        if (Input.GetKeyDown(KeyCode.Space) && isTouchingGround && !jumppressed)
        {
            myAnimator.SetBool("Jump", true);
            myAudioSource.clip = audioClips[Random.Range(0, audioClips.Length)];
            myAudioSource.Play();
            playerVelocity.y = 0f;
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -1.0f * gravity);
            jumppressed = true;
        }
        // If on the ground stop vertical movement
        if (controller.isGrounded && playerVelocity.y < 0)
        {
            myAnimator.SetBool("Jump", false);
            playerVelocity.y = 0f;
            jumppressed = false;
        }
        // Transition to Crouch
        if (Input.GetKeyDown(KeyCode.C))
        {
            playerIsWalking = false;
            state = playerState.Crouch;
        }
        if (Input.GetKey(KeyCode.Q) && (!isCrouching) && (!jumppressed))
        {
            state = playerState.Whisper;
        }
        playerVelocity.y += gravity * Time.deltaTime;
        controller.Move(playerVelocity * Time.deltaTime);

        // Animate walking based on movement
        bool playerHasHorizontalSpeed = Mathf.Abs(direction.x + direction.z) > Mathf.Epsilon; // bool deciding if velocity is higher than 0
        myAnimator.SetBool("Walking", playerHasHorizontalSpeed);
        // Ledge grab logic
        currentLedge.z = transform.position.z;
        Vector3 raydirection = new Vector3(transform.localScale.x, 0f, 0f);
        Vector3 topraycast = new Vector3(transform.position.x, transform.position.y + 1.7f, transform.position.z);
        Vector3 lowraycast = new Vector3(transform.position.x, transform.position.y + 1.3f, transform.position.z);
        Vector3 ledgeraycast = new Vector3(ledgerayx, transform.position.y + 2f, transform.position.z);
        RaycastHit topray;
        if (Physics.Raycast(topraycast, raydirection, out topray, .5f) && topray.collider.tag == "Ledge")
        {
            Debug.DrawRay(topraycast, raydirection * .5f, Color.green);
            topraybool = true;
        }
        else
        {
            topraybool = false;
        }
        RaycastHit lowray;
        if (Physics.Raycast(lowraycast, raydirection, out lowray, .5f) && lowray.collider.tag == "Ledge")
        {
            ledgerayx = lowray.point.x;
            currentLedge.x = lowray.point.x;
            Debug.DrawRay(lowraycast, raydirection * .5f, Color.blue);
            lowraybool = true;
        }
        else
        {
            lowraybool = false;
        }
        RaycastHit ledgeray;
        if (!topraybool && lowraybool)
        {
            if (Physics.Raycast(ledgeraycast, Vector3.down, out ledgeray, 2f) && ledgeray.collider.tag == "Ledge")
            {
                currentLedge.y = ledgeray.point.y;
                Debug.DrawRay(ledgeraycast, Vector3.down * 1f, Color.yellow);
                ledgecapable = true;
            }
        }
        else
        {
            ledgecapable = false;
        }
        // grab the ledge
        if (ledgecapable)
        {
            ledgeGrabPoint = currentLedge;
            state = playerState.Ledge;
        }
        RaycastHit forwardray;
        int layerMask = 1 << 8;
        Vector3 forwardraycast = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        if (Physics.Raycast(forwardraycast, Vector3.forward, out forwardray, .5f, layerMask))
        {
            Debug.DrawRay(forwardraycast, Vector3.forward, Color.cyan);
            if (forwardray.collider.gameObject.tag == "Dumpster")
            {
                touchingDumpster = true;
                currentDumpster = forwardray.collider.gameObject;
            }
            if (forwardray.collider.gameObject.tag == "Ladder")
            {
                ladderYpos = forwardray.point.y;
                currentLadder = forwardray.collider.gameObject;
                if (Input.GetKeyDown(KeyCode.E))
                {
                    holdingLadder = true;
                    transform.position = new Vector3(currentLadder.transform.position.x, ladderYpos, currentLadder.transform.position.z - 0.5f);
                    state = playerState.Climb;
                }
            }
        }
        else
        {
            currentLadder = null;
            touchingDumpster = false;
            currentDumpster = null;
        }
        // Transition to Hiding
        if (touchingDumpster == true && Input.GetKeyDown(KeyCode.E))
        {
            state = playerState.Hide;
        }
        Falling();
    }
    private void Falling()
    {
        if (controller.velocity.y < -6f)
        {
            myAnimator.SetBool("Crouching", false);
            myAnimator.SetBool("Walking", false);
            myAnimator.SetBool("Running", false);
            myAnimator.SetBool("Falling", true);
        }
        else if (isTouchingGround == true)
        {
            myAnimator.SetBool("Falling", false);
        }
    }
    private void Crouch()
    {
        // Animation tells isCrouching = true & freezes character until after animation completes
        if (Input.GetKeyDown(KeyCode.C) && (!isCrouching))
        {
            myAnimator.SetBool("Crouching", true);
            controller.center = new Vector3(0f, 0.45f, 0f);
            controller.height = .86f;
        }
        // Else if I press C and I am crouching
        else if (Input.GetKeyDown(KeyCode.C) && (isCrouching))
        {
            myAnimator.SetBool("Crouching", false);
            myAnimator.SetBool("NotCrouching", false);
            isCrouching = false;
            controller.center = new Vector3(0f, 0.79f, 0f);
            controller.height = 1.47f;
            state = playerState.GeneralMovement;
        }
        // X Axis Crouching
        if (isCrouching == true)
        {
            float horizontalinputspeed = Input.GetAxis("Horizontal"); // value is between -1 to +1
            float verticalinputspeed = Input.GetAxis("Vertical");
            direction = new Vector3((horizontalinputspeed * crouchSpeed), gravity, (verticalinputspeed * crouchSpeed)); // controlling speed of character
            controller.Move(direction * Time.deltaTime);
            bool playerHasHorizontalSpeed = Mathf.Abs(direction.x + direction.z) > Mathf.Epsilon; // bool deciding if velocity is higher than 0
            myAnimator.SetBool("CrouchWalking", playerHasHorizontalSpeed);
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
            //pigeonbox[0].GetComponent<Pigeon>().state = Pigeon.pigeonState.imlistening;
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
            state = playerState.GeneralMovement;
            crossHair.GetComponent<Crosshair>().crosshairstate = Crosshair.CrosshairState.isdisabled;
            hotdog.GetComponent<Hotdog>().hotdogState = Hotdog.pigeonState.resetpigeon;
            //pigeonbox[0].GetComponent<Pigeon>().state = Pigeon.pigeonState.resetpigeon;
        }
        // If you let go of Q after hitting something -> General Movement
        else if (Input.GetKeyUp(KeyCode.Q) && crossHair.GetComponent<Crosshair>().ivehitsomething == true)
        {
            print("I've hit a thing that's interactable");
            state = playerState.GeneralMovement;
            myAnimator.SetBool("TalkToBirds", false);
            //pigeonbox[0].GetComponent<Pigeon>().state = Pigeon.pigeonState.followplayer;
        }
    }
    private void LedgeHang()
    {
        myAnimator.SetBool("Walking", false);
        myAnimator.SetBool("Jump", false);
        myAnimator.SetBool("Running", false);
        myAnimator.SetBool("LedgeGrab", true);
        transform.position = currentLedge;
        myCollider.isTrigger = true;
        if (Input.GetKeyDown(KeyCode.W))
        {
            playerVelocity.y = 0f;
            myAnimator.SetBool("LedgePull", true);
            StartCoroutine(WaitForPull());
        }
        IEnumerator WaitForPull()
        {
            yield return new WaitForSeconds(0.5714285f);
            transform.position = currentLedge;
            myAnimator.SetBool("LedgeGrab", false);
            myAnimator.SetBool("LedgePull", false);
            state = playerState.GeneralMovement;
        }
    }
    private void ClimbLadder()
    {
        myAnimator.SetBool("Climb", true);
        float vertical = Input.GetAxisRaw("Vertical");
        direction = new Vector3(0f, (vertical * climbSpeed), 0f);
        if (direction.magnitude >= 0.1f)
        {
            controller.Move(direction * Time.deltaTime);
            myAnimator.SetFloat("ClimbSpeed", vertical);
        }
        else
        {
            myAnimator.SetFloat("ClimbSpeed", 0f);
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            myAnimator.SetBool("Climb", false);
            state = playerState.GeneralMovement;
        }
    }
    private void DumpsterDive()
    {
        myAnimator.SetBool("Running", false);
        if (imHiding == false)
        {
            controller.enabled = false;
            currentDumpster.gameObject.GetComponent<Dumpster>().IntoDumpster();
            myAnimator.SetBool("IntoDumpster", true);
            imHiding = true;
        }
        else if (Input.GetKeyDown(KeyCode.E) && imHiding == true)
        {
            currentDumpster.gameObject.GetComponent<Dumpster>().OutDumpster();
            myAnimator.SetBool("OutDumpster", true);
            myAnimator.SetBool("IntoDumpster", false);
            imHiding = false;
            state = playerState.GeneralMovement;
        }
    }
    private void MovingIntoDumpster()
    {
        transform.position = new Vector3(transform.position.x, (transform.position.y + .3f), (transform.position.z + .5f));
    }
    private void MovingOutDumpster()
    {
        transform.position = new Vector3(transform.position.x, (transform.position.y - .3f), (transform.position.z - .5f));
        controller.enabled = true;
    }
    private void CalculateGravity()
    {

        if (!isTouchingGround)
        {
            state = playerState.Falling;
        }
        else
        {
            myAnimator.SetBool("Falling", false);
            state = playerState.GeneralMovement;
        }
    }
    private void FlipSprite()
    {
        bool whichDirectionPlayerFacing = Mathf.Abs(direction.x) > Mathf.Epsilon;
        if (whichDirectionPlayerFacing)
        {
            transform.localScale = new Vector3(Mathf.Sign(direction.x), 1f, 1f);
        }
    }
    public void FootstepAudio()
    {
        myAudioSource.PlayOneShot(footstep, 1.0f);
    }
    private bool IsGrounded()
    {
        float floorDistanceFromFoot = controller.stepOffset - .2f;
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, floorDistanceFromFoot) || controller.isGrounded)
        {
            Debug.DrawRay(transform.position, Vector3.down * floorDistanceFromFoot, Color.yellow);
            return true;
        }
        return false;
    }
    private void OnTriggerEnter(Collider collider)
    {
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
        if (collider.gameObject.tag == "Danger")
        {
            gamemanager.GetComponent<GameManager>().PlayPoliceAmbience();
            Vector3 mypos = new Vector3((transform.position.x + 13f), transform.position.y, transform.position.z);
            Instantiate(policeGenerator, mypos, Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        isTouchingGround = IsGrounded();
        IsGrounded();
        //CalculateGravity();
        FlipSprite();
        PlayerState();
    }
}
