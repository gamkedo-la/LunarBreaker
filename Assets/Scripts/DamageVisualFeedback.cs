using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageVisualFeedback : MonoBehaviour
{
    [SerializeField] float alpha = 0.5f;
    [SerializeField] float timeBeforeFadeOut = 0.5f;
    Image image;

    void Awake()
    {
        image = GetComponent<Image>();
    }

    public void ShowImage()
    {
        Color color = image.color;
        image.color = new Color(color.r, color.g, color.b, alpha);
        StartCoroutine(FadeAfterTime());
    }

    private IEnumerator FadeAfterTime()
    {
        yield return new WaitForSeconds(timeBeforeFadeOut);
        Color color = image.color;
        image.color = new Color(color.r, color.g, color.b, 0.0f);
    }
}
