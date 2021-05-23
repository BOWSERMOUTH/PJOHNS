using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;
public class Crosshair : MonoBehaviour
{
    [SerializeField] float crosshairSpeed;
    Rigidbody2D myRigidbody;
    GameObject player;
    SpriteRenderer spriteRenderer;
    Vector3 originalPos;
    public Camera cam;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        myRigidbody = gameObject.GetComponent<Rigidbody2D>();
        player = GameObject.Find("PJohns");
        spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        originalPos = gameObject.transform.position;
    }
    private void MoveCrosshair()
    {
        if (player.GetComponent<Player>().birdWhispering == true)
        {
            float horizontalThrow = CrossPlatformInputManager.GetAxis("Horizontal"); // value is between -1 to +1
            float verticalThrow = CrossPlatformInputManager.GetAxis("Vertical");
            Vector2 playerVelocity = new Vector2((horizontalThrow * crosshairSpeed), verticalThrow * crosshairSpeed); // controlling speed of character
            myRigidbody.velocity = playerVelocity;
        }
        Vector3 pos = Camera.main.WorldToViewportPoint(transform.position);
        pos.x = Mathf.Clamp01(pos.x);
        pos.y = Mathf.Clamp01(pos.y);
        transform.position = Camera.main.ViewportToWorldPoint(pos);
    }
    // Update is called once per frame
    void Update()
    {
        MoveCrosshair();
        Targetting();
    }
    private void Targetting()
    {
        RaycastHit hit;
        LayerMask layerMask = 1 << 10;
        Vector3 direction = (transform.position - cam.transform.position).normalized;
        Ray ray = cam.ScreenPointToRay(direction);
        Debug.DrawRay(cam.transform.position, direction * 20f, Color.green);
        if (Physics.Raycast(cam.transform.position, direction, out hit, 20f, layerMask))
        {
            if (hit.collider.tag == "Interactable")
            {
                Debug.Log("I've hit a dumpster");
            }
        }
    }
}
