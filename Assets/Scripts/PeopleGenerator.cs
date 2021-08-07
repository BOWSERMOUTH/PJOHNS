using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PeopleGenerator : MonoBehaviour
{
    public GameObject carlos;
    public GameObject todd;
    public GameObject alex;
    public GameObject trevor;
    public GameObject mark;
    private float pedestrianlimit = 10f;
    public List<GameObject> peoplebox = new List<GameObject>();
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("generatehuman");
    }
    private IEnumerator generatehuman()
    {
        for (int pedestriancount = 0; pedestriancount < pedestrianlimit; pedestriancount++)
        {
            int whichPerson = Random.Range(0, peoplebox.Count);
            yield return new WaitForSeconds(Random.Range(5f, 10f));
            Instantiate(peoplebox[whichPerson], transform.position, Quaternion.identity);
            generatehuman();
        }
    }
    void Update()
    {
        
    }
}
