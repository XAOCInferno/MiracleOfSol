using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChangeImage : MonoBehaviour
{
    private Image ImageRenderer;

    public void SetNewImage(Sprite NewImage)
    {
        if(ImageRenderer == null)
        {
            ImageRenderer = gameObject.GetComponent<Image>();
        }

        ImageRenderer.sprite = NewImage;
    }

    public Sprite GetCurrentImage()
    {
        return ImageRenderer.sprite;
    }
}
