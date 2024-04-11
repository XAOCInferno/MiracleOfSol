using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public bool IsInBoss = false;
    public AudioClip BossMusic;

    private AudioSource AS;
    private bool HasActivatedBoss = false;

    private void Start()
    {
        AS = gameObject.GetComponent<AudioSource>();
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
