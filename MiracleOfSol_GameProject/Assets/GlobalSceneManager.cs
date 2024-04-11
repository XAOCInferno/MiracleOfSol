using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GlobalSceneManager : MonoBehaviour
{
    [SerializeField] private GlobalTimeManager GTM;
    private int CurrentLevelID;

    private void Start()
    {
        if(SceneManager.GetActiveScene().buildIndex == 0)
        {
            LoadNextLevel();
        }
    }

    public void LoadNextLevel()
    {
        CurrentLevelID++;
        var scene = SceneManager.LoadSceneAsync(CurrentLevelID);
        while (scene.progress != 1) { }
        GTM.SetupGTM();
    }
}
