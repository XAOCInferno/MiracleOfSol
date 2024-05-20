using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_SwitchBetweenGO : MonoBehaviour
{
    public GameObject[] SwitchObjects;

    public void SwitchToGO(int NewGO)
    {
        for(int i = 0; i < SwitchObjects.Length; i++)
        {
            if(i == NewGO)
            {
                SwitchObjects[i].SetActive(true);
            }
            else
            {
                SwitchObjects[i].SetActive(false);
            }
        }
    }
}
