using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayAmbienceSoundLoop : MonoBehaviour
{
    public AudioClip[] AllSounds;

    private AudioSource AS;
    private void Start()
    {
        gameObject.TryGetComponent(out AS);
    }

    // Update is called once per frame
    void Update()
    {
        if (!AS.isPlaying)
        {
            AS.PlayOneShot(AllSounds[Random.Range(0, AllSounds.Length)]);
        }
    }
}
