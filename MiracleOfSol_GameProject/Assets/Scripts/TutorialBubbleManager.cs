using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialBubbleManager : MonoBehaviour
{
    public TextMeshProUGUI ObjDescPart;
    public GameObject SpeechBubbleBG;

    private void Start()
    {
        ClearSpeechBubble();
    }

    public void ClearSpeechBubble()
    {
        UpdateSpeechBubbleArt();
        SpeechBubbleBG.SetActive(false);
    }

    public void UpdateSpeechBubbleArt(string NewObj = null)
    {
        SpeechBubbleBG.SetActive(true);

        if (NewObj != null)
        {
            ObjDescPart.enabled = true;
            ObjDescPart.text = NewObj;
        }
        else
        {
            ObjDescPart.enabled = false;
        }
    }
}
