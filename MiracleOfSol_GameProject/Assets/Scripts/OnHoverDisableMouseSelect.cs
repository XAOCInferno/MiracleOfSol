using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnHoverDisableMouseSelect : MonoBehaviour
{
    private GameInfo GI;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindWithTag("GameController").TryGetComponent(out GI);
    }

    public void DisableClickSelect()
    {
        if (GI != null)
        {
            GI.UITimeout = true;
        }
    }

    public void EnableClickSelect()
    {
        if (GI != null)
        {
            GI.UITimeout = false;
        }
    }
}
