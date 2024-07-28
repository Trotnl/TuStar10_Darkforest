using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;
using TMPro;
using UnityEngine.EventSystems;

public class StoryTelling : MonoBehaviour, IPointerDownHandler
{
    public GameObject BackgroundStoryImage1;
    public GameObject BackgroundStoryImage2;
    public GameObject BackgroundStoryImage3;
    public GameObject BackgroundStoryImage4;
    public TMP_Text clickToSkipText;  // ??????
    public float fadeDuration = 2.0f;
    public float displayDuration = 2.0f;
    public string nextSceneName;  // ?????????

    private bool isSkipping = false;

    private void Start()
    {
        BackgroundStoryImage1.SetActive(false);
        BackgroundStoryImage2.SetActive(false);
        BackgroundStoryImage3.SetActive(false);
        BackgroundStoryImage4.SetActive(false);

        clickToSkipText.gameObject.SetActive(true);

        StartCoroutine(ShowImagesSequentially());
    }

    private void Update()
    {
        // ??????????
        if (isSkipping == true )
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }

    private IEnumerator ShowImagesSequentially()
    {
        yield return StartCoroutine(FadeInAndOut(BackgroundStoryImage1));
        if (isSkipping) yield break;
        yield return StartCoroutine(FadeInAndOut(BackgroundStoryImage2));
        if (isSkipping) yield break;
        yield return StartCoroutine(FadeInAndOut(BackgroundStoryImage3));
        if (isSkipping) yield break;
        yield return StartCoroutine(FadeInAndOut(BackgroundStoryImage4));
    }

    private IEnumerator FadeInAndOut(GameObject image)
    {
        image.SetActive(true);
        CanvasGroup canvasGroup = image.GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = image.AddComponent<CanvasGroup>();
        }

        // ????
        for (float t = 0.0f; t < fadeDuration; t += Time.deltaTime)
        {
            if (isSkipping) yield break;
            canvasGroup.alpha = t / fadeDuration;
            yield return null;
        }
        canvasGroup.alpha = 1.0f;

        // ????
        yield return new WaitForSeconds(displayDuration);

        // ????
        for (float t = 0.0f; t < fadeDuration; t += Time.deltaTime)
        {
            if (isSkipping) yield break;
            canvasGroup.alpha = 1.0f - (t / fadeDuration);
            yield return null;
        }
        canvasGroup.alpha = 0.0f;

        image.SetActive(false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Cube ?????");
        // ??????????????
        isSkipping = true;
    }
}
