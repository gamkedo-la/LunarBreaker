using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LevelManager : MonoBehaviour
{
    [SerializeField] FadeInOut fadeImage;

    public void LoadSceneByNameAfterFadeOut(string sceneName)
    {
        fadeImage.StartFadeOut();
        StartCoroutine(LoadSceneByNameAfterTimeCoroutine(sceneName, fadeImage.GetFadeOutTime()));
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoadSceneByName(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void ReloadCurrentSceneAfterTime(float time)
    {
        fadeImage.SetFadeOutTime(0.99f * time);
        fadeImage.StartFadeOut();
        StartCoroutine(ReloadCurrentSceneAfterTimeCoroutine(time));
    }

    private IEnumerator ReloadCurrentSceneAfterTimeCoroutine(float time)
    {
        yield return new WaitForSeconds(time);
        ReloadCurrentScene();
    }
    private IEnumerator LoadSceneByNameAfterTimeCoroutine(string sceneName, float time)
    {
        yield return new WaitForSeconds(time);
        LoadSceneByName(sceneName);
    }
}
