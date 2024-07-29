// 28.22.56 wyuanshu

using UnityEngine;
using TMPro;

public class GameTimer : MonoBehaviour
{
    public TextMeshProUGUI timerText; // UI 显示计时器的 TextMeshPro 组件
    public float gameDuration = 300f; // 总游戏时间，单位秒
    private float elapsedTime = 0f;
    private bool timerStarted = false;

    //public GameObject level1WarpPoint; // 地图一层的跃迁点
    //public GameObject level2WarpPoint; // 地图二层的跃迁点

    //private bool level1WarpPointOpened = false;
    //private bool level2WarpPointOpened = false;
    private bool gameEnded = false;

    void Start()
    {
        elapsedTime = 0;
        //    level1WarpPoint.SetActive(false);
        //    level2WarpPoint.SetActive(false);
        UpdateTimerDisplay();
    }

    void Update()
    {
        if (gameEnded)
            return;
        Debug.Log("111");
        if (!transform.Find("/Canvas").GetComponent<CanvasHUD>().isBegin)
        {
            Debug.Log("222");
            return;
        }


        elapsedTime += Time.deltaTime;
        UpdateTimerDisplay();

        if (elapsedTime >= 180f)
        {
            EventManager.Trigger(EEventType.open_third_door.ToString());
        }

        // active warp point on the 2nd fl after 2 min
        else if (elapsedTime >= 120f /* && !level2WarpPointOpened */)
        {
            //OpenLevel2WarpPoint();
            EventManager.Trigger(EEventType.open_second_door.ToString());

        }

        // active warp point on the 1st fl after 1 minute
        else if (elapsedTime >= 60f /* && !level1WarpPointOpened */)
        {
            //OpenLevel1WarpPoint();
            EventManager.Trigger(EEventType.open_first_door.ToString());
        }
    }

    void UpdateTimerDisplay()
    {
        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }


    /*
     * TODO:
     * if ( ISREADY )
        {
            GameTimer gameTimer = FindObjectOfType<GameTimer>();
            if (gameTimer != null)
            {
                gameTimer.StartTimer();
            }
        }
     *   ADD THESE ABOVE CODE TO SUN_YI'S UI
     */

    public void EndGame()
    {
        gameEnded = true;
    }

    //void OpenLevel1WarpPoint()
    //{
    //    level1WarpPointOpened = true;
    //    level1WarpPoint.SetActive(true);
    //}

    //void OpenLevel2WarpPoint()
    //{
    //    level2WarpPointOpened = true;
    //    level2WarpPoint.SetActive(true);
    //}
}