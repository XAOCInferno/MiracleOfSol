using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Input_TaskbarController : MonoBehaviour
{
    public Menu_EnableDisableManager M_EDM__NewFilePrompt;

    public void Input_ActivateNewMap()
    {
        M_EDM__NewFilePrompt.SetStatus(true);
    }
}
