using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Carryable : MonoBehaviour
{
    private GameObject foodobject;
    private BoxCollider boxCollider;
    // Start is called before the first frame update
    void Start()
    {
        foodobject = GameObject.Find("Food");
        boxCollider = gameObject.GetComponent<BoxCollider>();   
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Hotdog")
        {
            boxCollider.isTrigger = true;
            transform.parent = other.gameObject.transform;
            gameObject.transform.position = new Vector3(other.gameObject.transform.position.x, (other.gameObject.transform.position.y - .1f), other.gameObject.transform.position.z);
        }
        if (other.gameObject.tag == "Player")
        {
            foodobject.GetComponent<Food>().AddFood();
            Destroy(gameObject);
        }
    }
}
