using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageVisualFeedback : MonoBehaviour
{
    [SerializeField] float alphaWhenVisisble = 0.5f;
    [SerializeField] float timeBeforeFadeIn = 0.5f;
    [SerializeField] float timeToFade = 1.0f;

    Image image;
    bool isFading = false;
    float fadeInRate;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    void Start()
    {
        isFading = false;
        fadeInRate = alphaWhenVisisble / timeToFade;
    }

    void Update()
    {
        if (!isFading) return;

        Color color = image.color;

        image.color = new Color(
            color.r,
            color.g,
            color.b,
            Mathf.Clamp(color.a - fadeInRate * Time.deltaTime, 0, 1)
        );

        if (image.color.a == 0.0f)
        {
            isFading = false;
        }
    }

    public void ShowImage()
    {
        Color color = image.color;
        image.color = new Color(color.r, color.g, color.b, alphaWhenVisisble);
        StartCoroutine(FadeAfterTime());
    }

    private IEnumerator FadeAfterTime()
    {
        yield return new WaitForSeconds(timeBeforeFadeIn);
        isFading = true;
    }
}
