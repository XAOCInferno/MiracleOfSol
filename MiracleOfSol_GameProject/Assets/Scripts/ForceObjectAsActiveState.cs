using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForceObjectAsActiveState : MonoBehaviour
{
    public GameObject[] Target;
    public bool state = true;

    private void Start()
    {
        InvokeRepeating(nameof(DoState), 0, 1);
    }
    
    private void DoState()
    {
        foreach (GameObject tmpObj in Target)
        {
            if (tmpObj != null)
            {
                if (tmpObj.activeSelf != state) { tmpObj.SetActive(state); }
            }
        }
    }
}
