using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPlayRandomClip : MonoBehaviour
{
    public AudioClip[] ClipsToPlay;
    public bool IsLooping = false;
    public float Loop_SoundRepeatRate;
    public bool DetatchFromParent = true;

    // Start is called before the first frame update
    void Start()
    {
        if (DetatchFromParent) { transform.parent = null; }
        if (ClipsToPlay.Length > 0)
        {
            PlayASound();
        }
        else { DestroySelf(); }
    }

    private void PlayASound()
    {
        int tmpRdm = Random.Range(0, ClipsToPlay.Length);
        gameObject.TryGetComponent(out AudioSource tmpAS);
        tmpAS.PlayOneShot(ClipsToPlay[tmpRdm]);

        if (IsLooping)
        {
            Invoke(nameof(PlayASound), Loop_SoundRepeatRate);
            Invoke(nameof(DestroySelf), ClipsToPlay[tmpRdm].length + 0.1f);
        }
        else
        {
            Invoke(nameof(DestroySelf), ClipsToPlay[tmpRdm].length + 0.1f);
        }
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
