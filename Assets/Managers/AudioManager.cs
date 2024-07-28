using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;


public class AudioManager : MonoBehaviour
{
    [System.Serializable]
    public class Sound
    {
        [Header("��Ƶ����")]
        public AudioClip clip;

        [Header("��Ƶ����")]
        public AudioMixerGroup outputGroup;

        [Header("��Ƶ����")]
        [Range(0, 1)]
        public float volume;

        [Header("��Ƶ�Ƿ񿪾ֲ���")]
        public bool playOnAwake;

        [Header("��Ƶ�Ƿ�ѭ������")]
        public bool loop;

    }


    public List<Sound> sounds;

    private Dictionary<string, AudioSource> audioDict;

    private static AudioManager instance;

    private void Awake()
    {
        audioDict = new Dictionary<string, AudioSource>();
    }

    private void Start()
    {
        foreach (var sound in sounds)
        {
            GameObject obj = new GameObject(sound.clip.name);
            obj.transform.SetParent(transform);

            AudioSource source = obj.AddComponent<AudioSource>();
            source.clip = sound.clip;
            source.playOnAwake = sound.playOnAwake;
            source.loop = sound.loop;
            source.volume = sound.volume;
            source.outputAudioMixerGroup = sound.outputGroup;

            if (sound.playOnAwake)
                source.Play();

            audioDict.Add(sound.clip.name, source);

        }
    }

    public static void PlayAudio(string name, bool isWait = false)
    {
        instance.audioDict[name].Play();
    }

    public static void StopAudio(string name)
    {
        instance.audioDict[name].Stop();
    }







}
