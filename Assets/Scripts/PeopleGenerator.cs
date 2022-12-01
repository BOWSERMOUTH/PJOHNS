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
    [SerializeField] float pedestrianlimit = 10f;
    [SerializeField] float minTimer;
    [SerializeField] float maxTimer;
    public List<GameObject> peoplebox = new List<GameObject>();
    public List<GameObject> outInTheField = new List<GameObject>();
    public int pedestriancount;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine("generatehuman");
    }
    private IEnumerator generatehuman()
    {
        for (pedestriancount = outInTheField.Count; pedestriancount < pedestrianlimit; pedestriancount++)
        {
            int whichPerson = Random.Range(0, peoplebox.Count);
            yield return new WaitForSeconds(Random.Range(minTimer, maxTimer));
            GameObject spawnedHuman = (GameObject)Instantiate(peoplebox[whichPerson], transform.position, Quaternion.identity);
            outInTheField.Add(spawnedHuman);
            generatehuman();
            outInTheField.RemoveAll(x => x == null);
            pedestriancount = outInTheField.Count;
        }
    }
    void Update()
    {

    }
}
