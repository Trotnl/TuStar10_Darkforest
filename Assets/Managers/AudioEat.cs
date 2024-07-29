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
        instance = this;
    }

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = eat;
        audioSource.loop = false;
        audioSource.volume = 0.5f;
    }

    public static void PlayAudio()
    {
        //instance.audioSource.clip = instance.eat;
        //instance.audioSource.loop = false;
        //instance.audioSource.volume = 0.5f;
        instance.audioSource.Play();
    }

    public static void StopAudio()
    {
        instance.audioSource.Stop();
    }
}
