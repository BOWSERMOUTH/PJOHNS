using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Police : MonoBehaviour
{
    // States
    public enum policeState { Searching, Stop, Spotted, Investigating, Caught, Aim }
    [SerializeField] List<AudioClip> clips;
    [SerializeField] policeState state;
    private bool startedSpotting = false;
    private bool startedSearching = true;
    private bool startedInvestigating = false;
    private bool atInvestigationPoint = false;

    // Police Components
    public Light lastSpottedLight;
    private Light policeLight;
    private AudioSource myAudio;
    public AudioSource spottedAudio;
    Animator myAnimator;
    public GameObject currentTarget = null;
    public bool spotted = false;
    public NavMeshAgent myNma;
    public SpriteRenderer myspriteren;
    public float spotlightTime;
    public float sightrotation;

    // Other GameObject References
    public GameManager gameman;
    private GameObject player;
    private Vector3 lastSpottedPoint;
    [SerializeField] GameObject targetPosition;

    void Start()
    {
        lastSpottedLight.enabled = false;
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
        // SEARCHING
        if (state == policeState.Searching)
        {
            // ONLY HAPPENS ONCE
            if (startedSearching)
            {
                lastSpottedLight.enabled = false;
                policeLight.enabled = false;
                // Police is on the move to destination
                myNma.isStopped = false;
                myAnimator.SetBool("Walking", true);
                myAnimator.speed = 1f;
                myNma.speed = 1.65f;
                // Set Destination To TeleporterEnd
                myNma.SetDestination(targetPosition.transform.position);
                startedSearching = false;
            }
            // Police Audio Matches PoliceAmbience Audio
            myAudio.time = gameman.GetComponent<AudioSource>().time;
            if (!myAudio.isPlaying)
            {
                myAudio.pitch = 1f;
                myAudio.Play();
            }
            // If Police is at TeleporterEnd, destroy self
            float distance = Vector3.Distance(transform.position, targetPosition.transform.position);
            if (distance <= .3f)
            {
                myAnimator.SetBool("Walking", false);
                gameman.lowervolume = true;
                Destroy(gameObject);
            }
            // 3 fanned out Rays looking for PJohns
            GeneralSight();
        }
        // SPOTTED
        if (state == policeState.Spotted)
        {
            // ONLY HAPPENS ONCE
            if (startedSpotting)
            {
                // If spotted, stop PoliceForce audio and stop walking
                myAudio.Pause();
                myNma.isStopped = true;
                myAnimator.SetBool("Walking", false);
                startedSpotting = false;
            }
            if (spotted)
            {
                StartCoroutine(SpottingTimer());
                IEnumerator SpottingTimer()
                {
                    spotted = false;
                    yield return new WaitForSeconds(3);
                    myAnimator.SetBool("Walking", true);
                    startedInvestigating = true;
                    state = policeState.Investigating;
                }
            }
            // Shoots a raycast that always looks at PJohns, but will hit Default and Interactable objects inbetween. 
            HonedSight();
        }
        // INVESTIGATING
        if (state == policeState.Investigating)
        {
            // ONLY PLAYS ONCE
            if (startedInvestigating)
            {
                myAudio.pitch = .3f;
                myAudio.Play();
                myAnimator.speed = .5f;
                myNma.SetDestination(lastSpottedPoint);
                myNma.speed = 1f;
                myNma.isStopped = false;
                atInvestigationPoint = true;
                startedInvestigating = false;
            }
            if (spotlightTime >= 3f)
            {
                myAudio.pitch = 1f;
                myAudio.PlayOneShot(clips[0], 1f);
                state = policeState.Caught;
            }
            // If you get to investigation place and haven't seen PJohns, wait 3s & return to Searching
            float distance = Vector3.Distance(transform.position, lastSpottedPoint);
            if (distance <= .3f && atInvestigationPoint)
            {
                myNma.isStopped = true;
                myAnimator.SetBool("Walking", false);
                StartCoroutine(SecondTimer());
                IEnumerator SecondTimer()
                {
                    yield return new WaitForSeconds(3);
                    myAudio.pitch = 1f;
                    gameman.UnSpotted();
                    startedSearching = true;
                    state = policeState.Searching;
                }
                atInvestigationPoint = false;
            }
            HonedSight();
        }
        // CAUGHT
        if (state == policeState.Caught)
        {
            myAnimator.SetBool("Whistle", true);
            myNma.SetDestination(player.transform.position);
            spotlightTime = 0f;
        }
    }
    private void GeneralSight()
    {
        // Creates 3 Raycasts fanning out in front of him
        RaycastHit hit;
        LayerMask defaultLayerMask = 1 << 0;
        LayerMask playerLayerMask = 1 << 6;
        LayerMask interLayerMask = 1 << 8;
        LayerMask mask = defaultLayerMask | playerLayerMask | interLayerMask;
        Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        Vector3 currentPos = Vector3.back * 3f;
        sightrotation += 520 * Time.deltaTime;
        if (sightrotation >= 180)
        {
            sightrotation = 0f;
        }
        currentPos = Quaternion.Euler(0, sightrotation, 0) * currentPos;
        Debug.DrawRay(rayOrigin, currentPos, Color.green);
        // If the raycast hits Player, go to SPOTTED mode. 
        if (Physics.Raycast(rayOrigin, currentPos, out hit, 3f, mask))
        {
            if (hit.transform.tag == "Player")
            {
                spotted = true;
                currentTarget = hit.transform.gameObject;
                lastSpottedPoint = hit.point;
                gameman.Spotted();
                spottedAudio.Play();
                startedSpotting = true;
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
        lastSpottedLight.enabled = true;
        lastSpottedLight.transform.position = lastSpottedPoint;
        RaycastHit hit;
        LayerMask defaultLayerMask = 1 << 0;
        LayerMask playerLayerMask = 1 << 6;
        LayerMask interLayerMask = 1 << 8;
        LayerMask mask = defaultLayerMask | playerLayerMask | interLayerMask;
        Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        Debug.DrawRay(rayOrigin, (currentTarget.transform.position - rayOrigin) * 3f, Color.blue);
        if (Physics.Raycast(rayOrigin, (currentTarget.transform.position - rayOrigin), out hit, 10f, mask))
        {
            // If spotted PJohns, start 3 second countdown to chase. Have flashlight look at PJohns last location.
            if (hit.collider.tag == "Player")
            {
                if (!myAudio.isPlaying)
                {
                    spottedAudio.Play();
                }
                spotlightTime += Time.deltaTime;
                lastSpottedPoint = hit.point;
                policeLight.enabled = true;
                policeLight.gameObject.transform.LookAt(lastSpottedPoint);
            }
            else
            {
                spottedAudio.Pause();
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
