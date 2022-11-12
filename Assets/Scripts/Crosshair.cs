using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
public class Crosshair : MonoBehaviour
{

    public Vector3 screenPosition;
    public Vector3 worldPosition;

    [SerializeField] float crosshairSpeed;
    Rigidbody2D myRigidbody;
    public GameObject player;
    public GameObject hotdog;
    SpriteRenderer spriteRenderer;
    public Camera cam;
    public GameObject sendBirds;
    public bool ivehitsomething;
    public Transform thingivehit;

    public enum CrosshairState { isactive, isdisabled }
    public CrosshairState crosshairstate;
    // Start is called before the first frame update
    void Start()
    {
        crosshairstate = CrosshairState.isdisabled;
        myRigidbody = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("PJohns");
        hotdog = GameObject.Find("Hotdog");
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
    }
    // Update is called once per frame
    void Update()
    {
        CrossHairState();
    }
    public void CrossHairState()
    {
        if (crosshairstate == CrosshairState.isactive)
        {
            spriteRenderer.enabled = true;
            MoveCrosshair();
            Targetting();
        }
        if (crosshairstate == CrosshairState.isdisabled)
        {
            spriteRenderer.enabled = false;
        }
    }
    private void MoveCrosshair()
    {
        // Controller Controls
        //float horizontalThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // value is between -1 to +1
        //float verticalThrow = CrossPlatformInputManager.GetAxis("Vertical");
        //Vector2 playerVelocity = new Vector2((horizontalThrow * crosshairSpeed), verticalThrow * crosshairSpeed); // controlling speed of character
        //myRigidbody.velocity = playerVelocity;
        //Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        //pos.x = Mathf.Clamp01(pos.x);
        //pos.y = Mathf.Clamp01(pos.y);

        // Mouse Controls
        screenPosition = Input.mousePosition;
        screenPosition.z = Camera.main.nearClipPlane + 6f;
        worldPosition = Camera.main.ScreenToWorldPoint(screenPosition);
        transform.position = worldPosition;
    }
    private void Targetting()
    {
        RaycastHit hit;
        LayerMask layerMask = 1 << 8;
        Vector3 direction = (transform.position - cam.transform.position).normalized;
        Ray ray = cam.ScreenPointToRay(direction);
        Debug.DrawRay(cam.transform.position, direction * 15f, Color.green);
        Physics.Raycast(cam.transform.position, direction, out hit, 15f, layerMask);
        thingivehit = hit.transform;
        if (hit.collider != null)
        {
            ivehitsomething = true;
            if (Input.GetKeyUp(KeyCode.Q))
            {
                hotdog.GetComponent<Hotdog>().target = thingivehit.gameObject;
                hotdog.GetComponent<Hotdog>().hotdogState = Hotdog.pigeonState.followcommand;
                crosshairstate = CrosshairState.isdisabled;
                thingivehit = null;
            }
        }
        else if (hit.collider == null)
        {
            thingivehit = null;
            ivehitsomething = false;
        }
    }
}
