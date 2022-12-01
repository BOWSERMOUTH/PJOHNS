using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Police : MonoBehaviour
{
    Animator myAnimator;
    public NavMeshAgent myNma;
    public SpriteRenderer myspriteren;
    [SerializeField] GameObject targetPosition;
    public enum policeState { Walking, Stop, Whistle, Aim }
    [SerializeField] policeState state;
    // Start is called before the first frame update
    void Start()
    {

        targetPosition = GameObject.Find("TeleporterEnd");
        myspriteren = GetComponentInChildren<SpriteRenderer>();
        myAnimator = GetComponentInChildren<Animator>();
        myNma = GetComponent<NavMeshAgent>();
    }
    private void PoliceState()
    {
        if (state == policeState.Walking)
        {
            LaserSight();
            myAnimator.SetBool("Walking", true);
            myNma.SetDestination(targetPosition.transform.position);
            if (transform.position == targetPosition.transform.position)
            {
                myAnimator.SetBool("Walking", false);
            }
            float distance = Vector3.Distance(transform.position, targetPosition.transform.position);
            if (distance <= .3f)
            {
                Destroy(gameObject);
            }
        }
    }
    private void LaserSight()
    {
        RaycastHit hitRight;
        RaycastHit hitMiddle;
        RaycastHit hitLeft;
        LayerMask defaultLayerMask = 1 << 0;
        LayerMask playerLayerMask = 1 << 6;
        LayerMask interLayerMask = 1 << 8;
        LayerMask mask = defaultLayerMask | playerLayerMask | interLayerMask;
        Vector3 rayOrigin = new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z);
        var right45 = (transform.right - transform.forward).normalized * myNma.velocity.x;
        var middle = (transform.right).normalized * myNma.velocity.x;
        var left45 = (transform.right + transform.forward).normalized * myNma.velocity.x;
        Vector3 raydirection = new Vector3(transform.localScale.x, 0f, 0f);
        if (Physics.Raycast(rayOrigin, right45, out hitRight, 3f, mask))
        {
            Debug.DrawRay(rayOrigin, right45 * 3f, Color.red);
            print("I've hit " + hitRight.transform.gameObject);
        }
        if (Physics.Raycast(rayOrigin, middle, out hitMiddle, 3f, mask))
        {
            Debug.DrawRay(rayOrigin, middle * 3f, Color.red);
            print("I've hit " + hitMiddle.transform.gameObject);
        }
        if (Physics.Raycast(rayOrigin, left45 * 3f, out hitLeft, 3f, mask))
        {
            Debug.DrawRay(rayOrigin, left45 * 3f, Color.red);
            print("I've hit " + hitLeft.transform.gameObject);
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
    // Update is called once per frame
    void Update()
    {
        PoliceState();
        FlipSprite();
    }
}
