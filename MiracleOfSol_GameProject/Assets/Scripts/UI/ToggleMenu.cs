using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleMenu : MonoBehaviour
{
    public GameObject Menu;

    private void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            ChangeMenuActiveState();
        }
    }

    public void ChangeMenuActiveState()
    {
        if (Menu.activeSelf)
        {
            Menu.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            Menu.SetActive(true);
            Time.timeScale = 0;
        }
    }
}
