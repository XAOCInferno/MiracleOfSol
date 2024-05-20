using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundsOfType : MonoBehaviour
{
    public AudioSource[] AS;
    private int CurrentAS = 0;

    public void PlayFromAS()
    {
        if(CurrentAS == AS.Length) { CurrentAS = 0; }
        AS[CurrentAS].Play();
        CurrentAS++;
    }
}
