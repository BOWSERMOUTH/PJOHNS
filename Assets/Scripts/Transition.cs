using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Transition : MonoBehaviour
{
    private GameObject player;
    private Collider lockcollider;
    private Scene sceneName;
    private string sceneString;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PJohns");
        lockcollider = gameObject.GetComponent<BoxCollider>();
        sceneName = SceneManager.GetActiveScene();
        sceneString = sceneName.name;
        GameObject.Find("FadeToBlack").GetComponent<FadeBlack>().FadeFromBlack();
    }
    public IEnumerator FadeOutAndTransition()
    {
        GameObject.Find("FadeToBlack").GetComponent<FadeBlack>().Fade2Black();
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(levels.ToString());
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
        if (player.transform.position.x < transform.position.x)
        {
            lockcollider.isTrigger = true;
        }
        if (player.transform.position.x > transform.position.x)
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
