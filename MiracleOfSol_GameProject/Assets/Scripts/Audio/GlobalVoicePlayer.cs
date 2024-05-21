using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class GlobalVoicePlayer : MonoBehaviour
{
    private static GlobalVoicePlayer _instance;

    [SerializeField] private AudioSource source;

    private void Awake()
    {
        _instance = this;   
    }

    public static void PlayVoiceLine(AudioClip clip)
    {
        _instance.source.PlayOneShot(clip);
    }

    public static void StopPlaying()
    {
        _instance.source.Stop();
    }

    public static bool GetIfPlaying()
    {
        return _instance.source.isPlaying;
    }
}
