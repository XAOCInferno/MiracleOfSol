using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class SpeechBubbleManager : MonoBehaviour
{
    public Image SpeechImgPart;
    public TextMeshProUGUI UnitNamePart;
    public TextMeshProUGUI UnitSpeechPart;
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

    public void UpdateSpeechBubbleArt(Sprite NewImg = null, string NewName = null, string NewSpeech = null)
    {
        SpeechBubbleBG.SetActive(true);
        if (NewImg != null)
        {
            SpeechImgPart.enabled = true;
            SpeechImgPart.sprite = NewImg;
        }
        else
        {
            SpeechImgPart.enabled = false;
        }

        if (NewName != null)
        {
            UnitNamePart.enabled = true;
            UnitNamePart.text = NewName;
        }
        else
        {
            UnitNamePart.enabled = false;
        }

        if (NewSpeech != null)
        {
            UnitSpeechPart.enabled = true;
            UnitSpeechPart.text = NewSpeech;
        }
        else
        {
            UnitSpeechPart.enabled = false;
        }
    }
}
