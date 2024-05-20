using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnClickSliderPlaySound : MonoBehaviour
{
    public AudioClip[] SoundsToPlay;
    private AudioSource AS;

    private bool SearchingForMouseUp = false;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.TryGetComponent(out AS);
    }

    private void Update()
    {
        if (SearchingForMouseUp)
        {
            if (Input.GetMouseButtonUp(0))
            {
                if (AS != null)
                {
                    AS.Stop();
                    SearchingForMouseUp = false;
                }
            }
        }
    }

    public void OnClickPlaySound()
    {
        if (AS != null)
        {
            SearchingForMouseUp = true;
            if (!AS.isPlaying)
            {
                AS.PlayOneShot(SoundsToPlay[Random.Range(0, SoundsToPlay.Length)]);
            }
        }
    }
}
