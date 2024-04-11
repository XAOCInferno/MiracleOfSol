using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuDisableParentPopup : MonoBehaviour
{
    public void DisableParentOnClick()
    {
        transform.parent.gameObject.SetActive(false);
    }
}
