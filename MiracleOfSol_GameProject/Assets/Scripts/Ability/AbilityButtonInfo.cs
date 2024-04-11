using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityButtonInfo : MonoBehaviour
{
    public int AbilityToCast = 0;

    public TMPro.TextMeshProUGUI hotkeyText;
    public UnityEngine.UI.Image ImgHolder;

    public void SetHotkey(string NewHotkey)
    {
        hotkeyText.text = NewHotkey;
    }

    public void SetImage(Sprite NewImg)
    {
        ImgHolder.sprite = NewImg;
    }
}
