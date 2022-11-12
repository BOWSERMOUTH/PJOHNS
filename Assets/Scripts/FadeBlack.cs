using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class FadeBlack : MonoBehaviour
{
    public Image image;
    void OnEnable()
    {
        FadeFromBlack();
    }
    public void Fade2Black()
    {
        Color alpha = image.color;
        alpha.a = 1f;
        image.color = alpha;
        image.CrossFadeAlpha(0f, 0f, true);
        image.CrossFadeAlpha(1, 1.5f, false);
    }
    public void FadeFromBlack()
    {
        Color alpha = image.color;
        alpha.a = 1f;
        image.color = alpha;
        image.CrossFadeAlpha(1f, 0f, true);
        image.CrossFadeAlpha(0, 1.5f, false);
    }
    private void Update()
    {
        
    }
}
