using UnityEngine;
using UnityEngine.UI;

public class CanvasSwitcher : MonoBehaviour
{
    public GameObject mainMenuCanvas;
    public GameObject storyCanvas;
    public Button btnMatch;

    private void Start()
    {
        // 为按钮添加点击事件监听器
        btnMatch.onClick.AddListener(OnMatchButtonClicked);

        // 确保初始状态下只有主菜单 Canvas 启用
        mainMenuCanvas.SetActive(true);
        storyCanvas.SetActive(false);
    }

    private void OnMatchButtonClicked()
    {
        // 启用房间 Canvas，禁用主菜单 Canvas
        mainMenuCanvas.SetActive(false);
        storyCanvas.SetActive(true);

        // 如果需要启动匹配逻辑，可以在这里添加
        StartMatch();
    }

    private void StartMatch()
    {
        // 这里添加匹配逻辑，例如连接服务器
        Debug.Log("Match started!");
    }
}