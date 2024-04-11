using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundClipChecker : MonoBehaviour
{
    public AudioClip ErrorSound;
    public float[] DefaultSoundTravelDistance = new float[2] { 0, 25 };
    public UnityEngine.Audio.AudioMixerGroup DefaultAudioMixerGroup;
}
