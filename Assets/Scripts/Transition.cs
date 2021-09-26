using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    private GameObject player;
    public Collider lockcollider;
    private Scene sceneName;
    private string sceneString;
    [SerializeField] bool flipzone;
    public float flipfloat;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PJohns");
        lockcollider = gameObject.GetComponent<BoxCollider>();
        sceneName = SceneManager.GetActiveScene();
        sceneString = sceneName.name;
        FlipTheZone();
        print(lockcollider.transform.position.x * flipfloat);
    }
    public IEnumerator FadeOutAndTransition()
    {
        GameObject.Find("FadeToBlack").GetComponent<FadeBlack>().Fade2Black();
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(levels.ToString());
    }
    private void FlipTheZone()
    {
        if (flipzone == true)
        {
            flipfloat = -1f;
            print("this happened");
        }
        else if (flipzone == false)
        {
            flipfloat = 1f;
        }
    }
    public enum Levels
    {
        MainStreet,
        UnderBridge,
        EastSubway,
        Restaurant
    }
    public Levels levels;
    private void IsTrigger()
    {
        if (player.transform.position.x < (transform.position.x * flipfloat))
        {
            lockcollider.isTrigger = true;
        }
        if ((player.transform.position.x * flipfloat) > (transform.position.x * flipfloat))
        {
            lockcollider.isTrigger = false;
        }
    }
    void Update()
    {
        IsTrigger();
        if ((sceneString == "UnderBridge") || (sceneString == "Restaurant"))
        {
            if(Input.GetKeyDown(KeyCode.Space))
            {
                StartCoroutine("FadeOutAndTransition");
                GameObject.Find("FadeToBlack").GetComponent<FadeBlack>().Fade2Black();
            }
        }
    }
}
