using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioSourceController : MonoBehaviour
{
    public bool IsLocalEmitter = true;
    public AudioMixerGroup MixerGroup;

    private AudioSource AS;
    private AudioSource ASLooping;
    private List<AudioClip> SoundsToPlay = new List<AudioClip>();
    private List<float[]> g_SoundTravelDistance = new List<float[]>();
    private List<AudioRolloffMode> g_ARM = new List<AudioRolloffMode>();
    private SoundClipChecker SCC;

    private AudioSource SetupAudioSource(AudioSource BaseSource, bool IsLooping)
    {
        if(BaseSource == null)
        {
            GameObject tmp_Obj = Instantiate(new GameObject());
            tmp_Obj.transform.position = transform.position;
            tmp_Obj.transform.parent = transform;

            BaseSource = tmp_Obj.AddComponent<AudioSource>();
            if(MixerGroup == null) 
            {
                CheckSCCNotNull();
                MixerGroup = SCC.DefaultAudioMixerGroup;
            }
            BaseSource.outputAudioMixerGroup = MixerGroup;
            BaseSource.playOnAwake = false;
            BaseSource.loop = IsLooping;
            BaseSource.Stop();
            BaseSource.spatialBlend = 1;
            BaseSource.rolloffMode = AudioRolloffMode.Linear;
        }

        return BaseSource;
    }

    private void Update()
    {
        if (AS != null)
        {
            if (!AS.isPlaying && SoundsToPlay.Count > 0)
            {
                //print("PLAYING FROM DO NEXT SOUND");
                DoNextSound();
            }
        }
    }

    private void DoNextSound()
    {
        AS.Stop();
        if (SoundsToPlay.Count > 0)
        {
            AS.minDistance = g_SoundTravelDistance[0][0]; AS.maxDistance = g_SoundTravelDistance[0][1];
            AS.rolloffMode = g_ARM[0];
            AS.PlayOneShot(SoundsToPlay[0]);
        }

        SoundsToPlay.RemoveAt(0);
        g_SoundTravelDistance.RemoveAt(0);
        g_ARM.RemoveAt(0);
    }

    public void StopLoopingSound()
    {
        if (ASLooping != null) { ASLooping.Stop(); }
    }

    public void OrderNewSound(AudioClip sound, float[] SoundTravelDistance, bool PlayNow = false, bool IsLoop = false, bool PlayNext = false, AudioRolloffMode ARM = AudioRolloffMode.Linear)
    {
        if (sound != null) //Disable this for searching for missing sounds
        {
            sound = CheckNullSound(sound);
            SoundTravelDistance = CheckSoundTravelDistance(SoundTravelDistance);

            if (AS == null) { AS = SetupAudioSource(AS, false); }
            if (ASLooping == null) { ASLooping = SetupAudioSource(ASLooping, true); }


            if (PlayNow)
            {
                if (!IsLoop)
                {
                    if (SoundsToPlay.Count > 0)
                    {
                        AS.Stop();
                        SoundsToPlay[0] = sound;
                        g_SoundTravelDistance[0] = SoundTravelDistance;
                        g_ARM[0] = ARM;
                    }
                    else
                    {
                        SoundsToPlay.Add(sound);
                        g_SoundTravelDistance.Add(SoundTravelDistance);
                        g_ARM.Add(ARM);
                    }
                }
                else
                {
                    if (!ASLooping.isPlaying)
                    {
                        ASLooping.minDistance = SoundTravelDistance[0]; ASLooping.maxDistance = SoundTravelDistance[1];
                        ASLooping.rolloffMode = ARM;
                        ASLooping.clip = sound;
                        ASLooping.Play();
                    }
                }
            }
            else if (PlayNext)
            {
                List<AudioClip> tmp_NewSoundList = new List<AudioClip>();
                if (SoundsToPlay.Count > 0) { tmp_NewSoundList.Add(SoundsToPlay[0]); }
                tmp_NewSoundList.Add(sound);

                for (int i = 2; i < SoundsToPlay.Count; i++)
                {
                    tmp_NewSoundList.Add(SoundsToPlay[i]);
                }


                List<float[]> tmp_SoundTravelDistance = new List<float[]>();
                if (g_SoundTravelDistance.Count > 0) { tmp_SoundTravelDistance.Add(g_SoundTravelDistance[0]); }
                tmp_SoundTravelDistance.Add(SoundTravelDistance);

                for (int i = 2; i < g_SoundTravelDistance.Count; i++)
                {
                    tmp_SoundTravelDistance.Add(g_SoundTravelDistance[i]);
                }


                List<AudioRolloffMode> tmp_ARM = new List<AudioRolloffMode>();
                if (g_ARM.Count > 0) { tmp_ARM.Add(g_ARM[0]); }
                tmp_ARM.Add(ARM);

                for (int i = 2; i < g_ARM.Count; i++)
                {
                    tmp_ARM.Add(g_ARM[i]);
                }
            }
            else if (!IsLoop)
            {
                SoundsToPlay.Add(sound);
                g_SoundTravelDistance.Add(SoundTravelDistance);
                g_ARM.Add(ARM);
            }
            else
            {
                if (!ASLooping.isPlaying)
                {
                    ASLooping.minDistance = SoundTravelDistance[0]; ASLooping.maxDistance = SoundTravelDistance[1];
                    ASLooping.rolloffMode = ARM;
                    ASLooping.clip = sound;
                    ASLooping.Play();
                }
            }
        }
    }

    private AudioClip CheckNullSound(AudioClip sound)
    {
        if (sound == null)
        {
            CheckSCCNotNull();
            sound = SCC.ErrorSound;
        }

        return sound;
    }

    private float[] CheckSoundTravelDistance(float[] SoundTravelDistance)
    {
        if(SoundTravelDistance == null)
        {
            CheckSCCNotNull();
            SoundTravelDistance = SCC.DefaultSoundTravelDistance;
        }
        else if (SoundTravelDistance.Length != 2)
        {
            CheckSCCNotNull();
            SoundTravelDistance = SCC.DefaultSoundTravelDistance;
        }

        return SoundTravelDistance;
    }

    private void CheckSCCNotNull()
    {
        if (SCC == null)
        {
            GameObject.FindGameObjectWithTag("GameController").TryGetComponent(out SCC);
        }
    }
}
