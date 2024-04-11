using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadMainMenuBtn : MonoBehaviour
{
    public void GoToMainMenu()
    {
        Time.timeScale = 1;
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
