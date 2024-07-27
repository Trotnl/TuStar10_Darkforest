using System.Collections;
using UnityEngine;
using TMPro;

public class FadeInAndSwitchCanvas : MonoBehaviour
{
    public TextMeshProUGUI textComponent;
    public TextMeshProUGUI skipHintTextComponent;
    public GameObject nextCanvas;
    public float fadeDuration = 5.0f; // 渐变持续时间
    public float delayBeforeHint = 0f; // 提示出现前的延迟

    private bool isDisplaying = false;
    private bool skipRequested = false;

    private void Start()
    {
        StartCoroutine(FadeInText());
    }

    private IEnumerator FadeInText()
    {
        // 确保文本开始时完全透明
        textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 0);
        skipHintTextComponent.color = new Color(skipHintTextComponent.color.r, skipHintTextComponent.color.g, skipHintTextComponent.color.b, 0);
        isDisplaying = true;

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            if (skipRequested)
            {
                // 跳过逻辑，直接显示全透明
                textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 1);
                break;
            }

            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        // 确保完全可见
        textComponent.color = new Color(textComponent.color.r, textComponent.color.g, textComponent.color.b, 1);

        // 等待一段时间再显示提示
        yield return new WaitForSeconds(delayBeforeHint);

        elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            if (skipRequested)
            {
                // 跳过逻辑，直接显示全透明
                skipHintTextComponent.color = new Color(skipHintTextComponent.color.r, skipHintTextComponent.color.g, skipHintTextComponent.color.b, 1);
                break;
            }

            float alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
            skipHintTextComponent.color = new Color(skipHintTextComponent.color.r, skipHintTextComponent.color.g, skipHintTextComponent.color.b, alpha);
            elapsedTime += Time.deltaTime;
            yield return null; // 等待下一帧
        }

        // 确保完全可见
        skipHintTextComponent.color = new Color(skipHintTextComponent.color.r, skipHintTextComponent.color.g, skipHintTextComponent.color.b, 1);
        isDisplaying = false;
    }

    private void Update()
    {
        // 检查是否点击屏幕且文本已经完全显示
        if (Input.GetMouseButtonDown(0))
        {
            skipRequested = true;
            if (!isDisplaying)
            {
                SwitchToNextCanvas();
            }
        }
    }

    private void SwitchToNextCanvas()
    {
        // 启用下一部分的 Canvas，禁用当前 Canvas
        nextCanvas.SetActive(true);
        gameObject.SetActive(false);
    }

    private void ShowSkipHint()
    {
        skipHintTextComponent.text = "点击屏幕跳过";
        skipHintTextComponent.color = Color.yellow; // 设置金色
        // 将提示文本显示在页面底部
        RectTransform rectTransform = skipHintTextComponent.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0.5f, 0); // X 轴中间，Y 轴底部
        rectTransform.anchorMax = new Vector2(0.5f, 0); // X 轴中间，Y 轴底部
        rectTransform.anchoredPosition = new Vector2(0, 50); // 距离底部 50 像素
    }
}
