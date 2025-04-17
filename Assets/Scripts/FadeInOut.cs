using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FadeInOut : MonoBehaviour
{
    [SerializeField] float fadeInTime = 0.5f;
    [SerializeField] float fadeOutTime = 1.0f;

    Image fadeImage;

    public bool isFadingIn = false;
    public bool isFadingOut = false;

    void Awake()
    {
        fadeImage = GetComponent<Image>();
    }

    void Start()
    {
        fadeImage.color = new Color(
            fadeImage.color.r,
            fadeImage.color.g,
            fadeImage.color.b,
            1.0f
        );
        isFadingIn = true;
    }

    public void SetFadeOutTime(float time)
    {
        fadeOutTime = time;
    }

    public float GetFadeOutTime()
    {
        return fadeOutTime;
    }

    public void StartFadeOut()
    {
        isFadingOut = true;
    }

    private void FixedUpdate()
    {
        if (!isFadingIn && !isFadingOut) return;

        int updateDir = isFadingOut ? 1 : -1;
        float updateTime = isFadingOut ? fadeOutTime : fadeInTime;

        float newAlpha = fadeImage.color.a;
        newAlpha += updateDir * Time.fixedDeltaTime / updateTime;
        newAlpha = Mathf.Clamp(newAlpha, 0.0f, 1.0f);

        fadeImage.color = new Color(
            fadeImage.color.r,
            fadeImage.color.g,
            fadeImage.color.b,
            newAlpha
        );

        if (newAlpha >= 1 && isFadingOut)
        {
            isFadingOut = false;
        }
        else if (newAlpha <= 0 && isFadingIn)
        {
            isFadingIn = false;
        }
    }
}
