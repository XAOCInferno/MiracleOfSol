using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    private static MusicManager _instance;
    public static MusicManager Instance { get { return _instance; } }

    public bool IsInBoss = false;
    public AudioClip BossMusic;

    private AudioSource AS;
    private bool HasActivatedBoss = false;

    private void Awake()
    {
        _instance = this;
    }

    private void Start()
    {
        TryGetComponent(out AS);
        if(AS == null) { AS = gameObject.AddComponent<AudioSource>(); }
        InvokeRepeating(nameof(CheckForBossFight), 1, 1);
    }

    private void CheckForBossFight()
    {
        if (IsInBoss && !HasActivatedBoss)
        {
            HasActivatedBoss = true;
            AS.Stop();
            AS.clip = BossMusic;
            AS.Play();
            AS.loop = true;
            CancelInvoke();
        }
        else if(!IsInBoss && HasActivatedBoss)
        {
            HasActivatedBoss = false;
            AS.Stop();
        }
    }

    public void ForcePlayTrack(AudioClip NewTrack)
    {
        AS.Stop();
        AS.clip = NewTrack;
        AS.Play();
        AS.loop = true;
        CancelInvoke();
    }
}
