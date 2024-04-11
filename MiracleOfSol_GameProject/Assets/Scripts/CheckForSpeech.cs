using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForSpeech : MonoBehaviour
{
    public GameObject[] Speeches;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefsX.GetBool("GameIsWon", false))
        {
            Speeches[1].SetActive(true);
            Destroy(Speeches[0]);
        }
        else
        {
            Speeches[0].SetActive(true);
            Destroy(Speeches[1]);
        }
        PlayerPrefsX.SetBool("GameIsWon", false);
        Destroy(gameObject);
    }
}
