using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTextManager : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public BtnDescBoxInfo BDBI;

    public void OnPointerEnter(PointerEventData eventData)
    {
        BDBI.gameObject.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        BDBI.gameObject.SetActive(false);
    }
}
