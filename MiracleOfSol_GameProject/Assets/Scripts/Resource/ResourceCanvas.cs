using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceCanvas : MonoBehaviour
{
    public TextMeshProUGUI[] ResCanvas;

    public void ChangeResources(float[] NewResources)
    {
        for(int i = 0; i < Mathf.Min(ResCanvas.Length,NewResources.Length); i++)
        {
            ResCanvas[i].text = NewResources[i].ToString();       
        }
    }
}
