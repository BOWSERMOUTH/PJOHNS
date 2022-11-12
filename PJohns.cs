using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PJohns : MonoBehaviour
{
    private CharacterController controller;
    public float speed = 6f;
    // Start is called before the first frame update
    void Start()
    {
        controller = this.gameObject.GetComponent<CharacterController>();
    }
    private void Walk()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical);
    }

    // Update is called once per frame
    void Update()
    {
        Walk();
    }
}
