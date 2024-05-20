using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetryMissionBtn : MonoBehaviour
{
    public void RetryCurrentMission()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }
}
