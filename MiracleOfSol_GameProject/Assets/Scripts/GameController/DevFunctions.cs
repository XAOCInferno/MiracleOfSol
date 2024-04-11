using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DevFunctions : MonoBehaviour
{
    public RTSCameraController RTS_CC;
    public GameObject PauseUnpauseMessage;

    private bool PauseState = false;
    private GameInfo GI;

    private void Start()
    {
        GI = gameObject.GetComponent<GameInfo>();
        PauseUnpauseMessage.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyUp("space"))
        {
            PauseUnpause();
        }

        if (Input.GetKeyUp("c"))
        {
            RTS_CC.Active = !RTS_CC.Active;
        }
    }

    private void PauseUnpause()
    {
        PauseState = !PauseState;
        PauseUnpauseMessage.SetActive(PauseState);

        if (PauseState)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
