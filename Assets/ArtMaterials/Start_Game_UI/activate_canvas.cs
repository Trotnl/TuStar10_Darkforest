using UnityEngine;

public class ActivateCanvasOnLoad : MonoBehaviour
{
    public GameObject targetCanvas; // 目标 Canvas

    private void Start()
    {
        // 确保目标 Canvas 在场景加载时处于活动状态
        if (targetCanvas != null)
        {
            targetCanvas.SetActive(true);
        }
    }
}