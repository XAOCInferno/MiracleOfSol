using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroQuotes_LevelUpScreen : MonoBehaviour
{
    public AudioClip[] Quotes_Tutorial;
    public AudioClip[] Quotes_Lvl1;
    public AudioClip[] Quotes_Lvl2;
    public AudioClip[] Quotes_Lvl3;
    public AudioClip[] Quotes_Lvl4;

    private AudioClip[][] AllQuotes;
    private int CurrentQuote = 0;
    private int CurrentLevel;
    private AudioSource AS;
    private UnityEngine.UI.Button selfButton;

    private void Start()
    {
        AllQuotes = new AudioClip[5][] { Quotes_Tutorial, Quotes_Lvl1, Quotes_Lvl2, Quotes_Lvl3, Quotes_Lvl4 };
        gameObject.TryGetComponent(out selfButton);

        SetupAudioSource();
        GetCurrentLevel();
    }

    private void Update()
    {
        if (AS.isPlaying) { selfButton.interactable = false; } else { selfButton.interactable = true; }
    }

    private void GetCurrentLevel()
    {
        string tmpLevel = PlayerPrefs.GetString("NextLevelToRun", "Mission01");

        if (tmpLevel == "Mission00")
        {
            CurrentLevel = 0;
        }
        else if (tmpLevel == "Mission01")
        {
            CurrentLevel = 1;
        }
        else if (tmpLevel == "Mission02")
        {
            CurrentLevel = 2;
        }
        else if (tmpLevel == "Mission03")
        {
            CurrentLevel = 3;
        }
        else
        {
            CurrentLevel = 4;
        }
    }

    private void SetupAudioSource()
    {
        transform.parent.transform.parent.TryGetComponent(out AS);
        if (AS == null) { AS = transform.parent.transform.parent.gameObject.AddComponent<AudioSource>(); }
    }

    public void PlayQuote()
    {
        if (!AS.isPlaying) 
        { 
            AS.PlayOneShot(AllQuotes[CurrentLevel][CurrentQuote]);
            selfButton.interactable = false;

            CurrentQuote++;

            if(CurrentQuote == AllQuotes[CurrentLevel].Length)
            {
                CurrentQuote = 0;
            }
        }
    }
}
