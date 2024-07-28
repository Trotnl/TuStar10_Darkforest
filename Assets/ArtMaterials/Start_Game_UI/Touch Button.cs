using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class CustomButton : MonoBehaviour, IPointerDownHandler
{
    public string nextSceneName; // 目标场景名称

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("Cube 被点击了！");
        // 这里可以添加你想要执行的操作
        SwitchToNextScene();
    }

    private void SwitchToNextScene()
    {
        SceneManager.LoadScene(nextSceneName);
    }

}
