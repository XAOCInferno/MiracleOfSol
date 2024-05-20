using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenMainMenuButton : MonoBehaviour
{
    private GameObject MainMenuCanvas;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("GameController").TryGetComponent(out GameInfo GI);
        MainMenuCanvas = GI.MenuCanvas;
    }

    public void OpenMenu()
    {
        Time.timeScale = 0;
        MainMenuCanvas.SetActive(true);
    }

    public void CloseMenu()
    {
        Time.timeScale = 1;
        MainMenuCanvas.SetActive(false);
    }
}
