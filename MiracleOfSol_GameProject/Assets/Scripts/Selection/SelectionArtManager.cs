using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionArtManager : MonoBehaviour
{
    private GameObject HoverArt;
    private GameObject SelectArt;

    private void Start()
    {
        HoverArt = transform.GetChild(0).gameObject;
        SelectArt = transform.GetChild(1).gameObject;
    }

    public void EnableArt(bool IsDisable = true, bool IsSelection = false, bool IsHover = false)
    {
        if (IsDisable)
        {
            HoverArt.SetActive(false);
            SelectArt.SetActive(false);
        }
        else if (IsSelection)
        {
            HoverArt.SetActive(false);
            SelectArt.SetActive(true);
        }
        else if(IsHover)
        {
            HoverArt.SetActive(true);
            SelectArt.SetActive(false);
        }
    }
}
