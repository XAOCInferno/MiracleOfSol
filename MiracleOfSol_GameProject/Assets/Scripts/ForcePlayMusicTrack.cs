using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForcePlayMusicTrack : MonoBehaviour
{
    public AudioClip NewTrack;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<MusicManager>().ForcePlayTrack(NewTrack);
        Destroy(this);
    }
}
