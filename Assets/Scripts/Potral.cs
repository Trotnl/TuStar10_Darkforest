using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Potral : MonoBehaviour
{
    public int index; // 2 一层跃迁点，3 二层跃迁点，4 三层直接获胜

    public AudioSource Portal_Music;
    public AudioClip portalSound;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.UsePotral(index);
            if (player.score == 3) 
            {
                Portal_Music.PlayOneShot(portalSound);
            }
        }
    }

    private void OnEnable()
    {
        if (index == 2)
        {
            EventManager.Listen(EEventType.open_first_door.ToString(), Active);
        }
        else if (index == 3)
        {
            EventManager.Listen(EEventType.open_second_door.ToString(), Active);
        }
        else if (index == 4)
        {
            EventManager.Listen(EEventType.open_third_door.ToString(), Active);
        }
    }

    private void OnDestroy()
    {
        if (index == 2)
        {
            EventManager.Ignore(EEventType.open_first_door.ToString(), Active);
        }
        else if (index == 3)
        {
            EventManager.Ignore(EEventType.open_second_door.ToString(), Active);
        }
        else if (index == 4)
        {
            EventManager.Ignore(EEventType.open_third_door.ToString(), Active);
        }
    }

    public void Active(object[] arr)
    {
        transform.GetComponent<BoxCollider2D>().enabled = true;
        transform.Find("Potral/enable").gameObject.SetActive(true);
    }

    // 设置初始状态
    private void Start()
    {
        transform.GetComponent<BoxCollider2D>().enabled = false;
        transform.Find("Potral/enable").gameObject.SetActive(false);
    }
}
