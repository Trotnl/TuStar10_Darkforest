using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioEat : MonoBehaviour
{

    //AudioClip
    public AudioClip eat;

    private AudioSource audioSource;

    private static AudioEat instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject); // 如果希望 AudioTest 在场景切换时不被销毁
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = eat;
        audioSource.loop = true;
        audioSource.volume = 0.5f;
    }

    public static void PlayAudio()
    {
        instance.audioSource.Play();

    }

    public static void StopAudio()
    {
        instance.audioSource.Stop();
    }
}
