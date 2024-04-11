using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuLoadGamestate : MonoBehaviour
{
    public GameObject NewGameWarningPopup;
    public GameObject QuitGameWarningPopup;

    private int StartingLevel = 1;

    public void RestartGame()
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void QuitGamePopup()
    {
        int tmpState = PlayerPrefs.GetInt("CurrentLevel", -1);

        QuitGameWarningPopup.SetActive(true);
    }

    public void ConfirmQuitGame()
    {
        Application.Quit();
    }

    public void CheckNewGamePopup()
    {
        int tmpState = PlayerPrefs.GetInt("CurrentLevel", -1);

        if(tmpState == -1)
        {
            InitNewGame();
        }
        else
        {
            NewGameWarningPopup.SetActive(true);
        }
    }

    public void InitNewGame()
    {
        PlayerPrefs.SetInt("CurrentLevel", 0);
        PlayerPrefsX.SetIntArray("HeroLevelStatus", new int[4] { 0, 0, 0, 0 });
        PlayerPrefsX.SetIntArray("HeroUnusedPerks", new int[4] { 0, 0, 0, 0 });
        PlayerPrefsX.SetIntArray("HeroHPPerkCount", new int[4] { 0, 0, 0, 0 });
        PlayerPrefsX.SetIntArray("HeroDmgPerkCount", new int[4] { 0, 0, 0, 0 });
        PlayerPrefsX.SetIntArray("HeroSupportPerkCount", new int[4] { 0, 0, 0, 0 });
        PlayerPrefsX.SetIntArray("HeroEquippedWeapons", new int[4] { 0, 0, 0, 0 });
        PlayerPrefs.SetInt("WeaponPartsCount", 0);
        PlayerPrefs.SetString("NextLevelToRun", "Mission00");
        SceneManager.LoadScene(StartingLevel);        
    }

    public void ContinueGame()
    {
        int tmpLevel = PlayerPrefs.GetInt("CurrentLevel", -1);

        if (tmpLevel == -1)
        {
            //Enable popup "no save game found, start a new game?"
        }
        else
        {
            SceneManager.LoadScene(StartingLevel + tmpLevel);
        }
    }
}
