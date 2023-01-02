using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Police : MonoBehaviour
{
    // States
    public enum policeState { Walking, Stop, Spotted, CheckingArea, Whistle, Aim }
    [SerializeField] List<AudioClip> clips;
    [SerializeField] policeState state;

    // Police Components
    private Light policeLight;
    private AudioSource myAudio;
    public AudioSource spottedAudio;
    Animator myAnimator;
    public GameObject currentTarget = null;
    public bool spotted = false;
    public NavMeshAgent myNma;
    public SpriteRenderer myspriteren;
    public float spotlightTime;


    // Other GameObject References
    public GameManager gameman;
    private GameObject player;
    private Vector3 lastSpottedPoint;
    [SerializeField] GameObject targetPosition;

    void Start()
    {
        policeLight = gameObject.GetComponentInChildren<Light>();
        policeLight.enabled = false;
        myAudio = gameObject.GetComponent<AudioSource>();
        gameman = GameObject.Find("GameManager").GetComponent<GameManager>();
        player = GameObject.Find("PJohns");
        targetPosition = GameObject.Find("TeleporterEnd");
        myspriteren = GetComponentInChildren<SpriteRenderer>();
        myAnimator = GetComponentInChildren<Animator>();
        myNma = GetComponent<NavMeshAgent>();
    }
    private void PoliceState()
    {
        print(state);
        if (state == policeState.Walking)
        {
            print(myAudio.isPlaying);
            policeLight.enabled = false;
            // Police is on the move to destination
            myNma.isStopped = false;
            myAnimator.SetBool("Walking", true);
            myAnimator.speed = 1f;
            myNma.speed = 1.65f;
            // Police Audio Matches PoliceAmbience Audio
            myAudio.time = gameman.GetComponent<AudioSource>().time;
            if (!myAudio.isPlaying)
            {
                myAudio.pitch = 1f;
                myAudio.Play();
            }
            // 3 fanned out Rays looking for PJohns
            GeneralSight();
            // Set Destination To TeleporterEnd
            myNma.SetDestination(targetPosition.transform.position);
            // If Police is at TeleporterEnd, stop walking
            if (transform.position == targetPosition.transform.position)
            {
                myAnimator.SetBool("Walking", false);
            }
            // If Police is at TeleporterEnd, destroy self
            float distance = Vector3.Distance(transform.position, targetPosition.transform.position);
            if (distance <= .3f)
            {
                gameman.lowervolume = true;
                Destroy(gameObject);
            }
        }
        // SPOTTED
        if (state == policeState.Spotted)
        {
            // If spotted, stop PoliceForce audio and stop walking
            myAudio.Pause();
            myNma.isStopped = true;
            myAnimator.SetBool("Walking", false);
            // Shoots a raycast that always looks at PJohns, but will hit Default and Interactable objects inbetween. 
            HonedSight();
            if (spotted)
            {
                if (!spottedAudio.isPlaying)
                {
                    spottedAudio.UnPause();
                }
                StartCoroutine(SpottingTimer());
                IEnumerator SpottingTimer()
                {
                    spotted = false;
                    yield return new WaitForSeconds(3);
                    myAnimator.SetBool("Walking", true);
                    state = policeState.CheckingArea;
                }
            }
            else if (!spotted)
            {
                spottedAudio.Pause();
            }
        }
        if (state == policeState.CheckingArea)
        {
            if (!myAudio.isPlaying)
            {
                myAudio.pitch = .3f;
                myAudio.Play();
            }
            myAnimator.speed = .5f;
            myNma.SetDestination(lastSpottedPoint);
            myNma.speed = 1f;
            myNma.isStopped = false;

            if (spotlightTime >= 3f)
            {
                myAudio.pitch = 1f;
                myAudio.PlayOneShot(clips[0], 1f);
                state = policeState.Whistle;
            }
            float distance = Vector3.Distance(transform.position, lastSpottedPoint);
            if (distance <= .1f && spotted == false)
            {
                myNma.isStopped = true;
                myAnimator.SetBool("Walking", false);
                StartCoroutine(SecondTimer());
                IEnumerator SecondTimer()
                {
                    spotted = true;
                    yield return new WaitForSeconds(3);
                    myAudio.pitch = 1f;
                    gameman.UnSpotted();
                    state = policeState.Walking;
                }
            }
        }
        if (state == policeState.Whistle)
        {
            myAnimator.SetBool("Whistle", true);
            myNma.SetDestination(player.transform.position);
            spotlightTime = 0f;
        }
    }
    private void GeneralSight()
    {
        // Creates Raycasts fanning out in front of him
        RaycastHit righthit;
        RaycastHit middlehit;
        RaycastHit lefthit;
        LayerMask defaultLayerMask = 1 << 0;
        LayerMask playerLayerMask = 1 << 6;
        LayerMask interLayerMask = 1 << 8;
        LayerMask mask = defaultLayerMask | playerLayerMask | interLayerMask;
        Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        var right45 = (transform.right - transform.forward).normalized * myNma.velocity.x;
        var middle = (transform.right).normalized * myNma.velocity.x;
        var left45 = (transform.right + transform.forward).normalized * myNma.velocity.x;

        Vector3 raydirection = new Vector3(transform.localScale.x, 0f, 0f);
        Debug.DrawRay(rayOrigin, right45 * 3f, Color.red);
        Debug.DrawRay(rayOrigin, middle * 3f, Color.red);
        Debug.DrawRay(rayOrigin, left45 * 3f, Color.red);

        // If the raycast hits Player, go to SPOTTED mode. 
        if (Physics.Raycast(rayOrigin, right45, out righthit, 3f, mask))
        {
            if (righthit.transform.tag == "Player")
            {
                spotted = true;
                currentTarget = righthit.transform.gameObject;
                lastSpottedPoint = righthit.point;
                gameman.Spotted();
                spottedAudio.Play();
                state = policeState.Spotted;
            }
        }
        else
        {
            spotted = false;
        }
        if (Physics.Raycast(rayOrigin, middle, out middlehit, 3f, mask))
        {
            if (middlehit.transform.tag == "Player")
            {
                spotted = true;
                currentTarget = middlehit.transform.gameObject;
                lastSpottedPoint = middlehit.point;
                gameman.Spotted();
                state = policeState.Spotted;
            }
        }
        else
        {
            spotted = false;
        }
        if (Physics.Raycast(rayOrigin, left45 * 3f, out lefthit, 3f, mask))
        {
            if (lefthit.transform.tag == "Player")
            {
                spotted = true;
                currentTarget = lefthit.transform.gameObject;
                lastSpottedPoint = lefthit.point;
                gameman.Spotted();
                state = policeState.Spotted;
            }
        }
        else
        {
            spotted = false;
        }
    }
    private void HonedSight()
    {
        RaycastHit hit;
        LayerMask defaultLayerMask = 1 << 0;
        LayerMask playerLayerMask = 1 << 6;
        LayerMask interLayerMask = 1 << 8;
        LayerMask mask = defaultLayerMask | playerLayerMask | interLayerMask;
        Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        Vector3 raydirection = new Vector3(transform.localScale.x, 0f, 0f);
        Debug.DrawRay(rayOrigin, (currentTarget.transform.position - rayOrigin) * 3f, Color.blue);
        if (Physics.Raycast(rayOrigin, (currentTarget.transform.position - rayOrigin), out hit, 10f, mask))
        {
            // If spotted PJohns, start 3 second countdown to chase. Have flashlight look at PJohns last location.
            if (hit.collider.tag == "Player")
            {
                spotted = true;
                spotlightTime += Time.deltaTime;
                lastSpottedPoint = hit.point;
                policeLight.enabled = true;
                policeLight.gameObject.transform.LookAt(lastSpottedPoint);
            }
            else
            {
                spotted = false;
            }
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


    void Update()
    {
        PoliceState();
        FlipSprite();
    }
}
