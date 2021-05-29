using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class Player : MonoBehaviour
{
    //Config
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float runSpeed = 5f;
    [SerializeField] float climbSpeed = 3f;
    [SerializeField] float crouchSpeed = 3f;
    [SerializeField] bool isCrouching;
    [SerializeField] bool isWalking;
    [SerializeField] bool freezeCharacter;
    [SerializeField] public bool birdWhispering;
    [SerializeField] GameObject stepRayUpper;
    [SerializeField] GameObject stepRayLower;
    [SerializeField] float stepHeight;
    [SerializeField] float stepSmooth;
    public bool playerIsWalking;
    

    //Cached Component References
    Rigidbody myRigidbody;
    Animator myAnimator;
    BoxCollider myBoxCollider;
    AudioSource myAudioSource;
    public AudioClip footstep;

    //Other Object References
    GameObject crossHair;
    void Start()
    {
        myRigidbody = GetComponent<Rigidbody>();
        myAnimator = GetComponent<Animator>();
        myBoxCollider = GetComponent<BoxCollider>();
        myAudioSource = GetComponent<AudioSource>();
        isCrouching = myAnimator.GetBool("Crouching");
        crossHair = GameObject.Find("crosshair");
        crossHair.GetComponent<SpriteRenderer>().enabled = false;
    }

    void Update()
    {
        Walk();
        Crouch();
        FlipSprite();
        CharacterCantMove();
        Whispering();
        StepClimb();
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
            birdWhispering = true;
            crossHair.GetComponent<SpriteRenderer>().enabled = true;
            // freeze character position(animator), do animation
            myAnimator.SetBool("TalkToBirds", true);
        }
        else if (Input.GetKeyUp(KeyCode.Q))
        {
            birdWhispering = false;
            crossHair.GetComponent<SpriteRenderer>().enabled = false;
            // unfreeze character (in animator), do animation to release
            myAnimator.SetBool("TalkToBirds", false);
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
                if (Input.GetKey(KeyCode.LeftShift))
                {
                    walkSpeed = 4f;
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
            myAnimator.SetBool("New Walking", playerIsWalking);
        }
        else if (isCrouching)
        {
            CrouchWalking();
        }
    }
    public void Footstep()
    {
        print("step");
        myAudioSource.PlayOneShot(footstep, 1.0f);
    }
    private void Crouch()
    {
        // Animation tells isCrouching = true & freezes character until after animation completes
        if (Input.GetKeyDown(KeyCode.C) && (!isCrouching))
        {
            print("I've pressed C");
            myAnimator.SetBool("Crouching", true);
        }
        // Else if I press C and I am crouching
        else if (Input.GetKeyDown(KeyCode.C) && (isCrouching))
        {
            print("I've pressed C again");
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
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(Vector3.forward), out hitLower, 0.1f))
        {
            print("I've hit on toes");
            RaycastHit hitUpper;
            if (!Physics.Raycast(stepRayUpper.transform.position, transform.TransformDirection(Vector3.forward), out hitUpper, 0.2f))
            {
                print("I'm hitting something");
                myRigidbody.position -= new Vector3(0f, -stepSmooth, 0f);
            }
        }
        RaycastHit hitLower45;
        if (Physics.Raycast(stepRayLower.transform.position, transform.TransformDirection(1.5f,0,1), out hitLower45, 0.1f))
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
}

