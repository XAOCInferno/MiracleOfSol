using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpdateWeaponArt : MonoBehaviour
{
    public GameObject[] Lvl1Weapons;
    public GameObject[] Lvl2Weapons;
    public GameObject[] Lvl3Weapons;

    public void UpdateWeaponArtLvlScreen(int CurrentWeaponPos)
    {
        if(CurrentWeaponPos == 0)
        {
            foreach (GameObject tmpObj in Lvl3Weapons)
            {
                tmpObj.SetActive(false);
            }

            foreach (GameObject tmpObj in Lvl2Weapons)
            {
                tmpObj.SetActive(false);
            }

            foreach (GameObject tmpObj in Lvl1Weapons)
            {
                tmpObj.SetActive(true);
            }
        }
        else if(CurrentWeaponPos == 1)
        {
            foreach (GameObject tmpObj in Lvl3Weapons)
            {
                tmpObj.SetActive(false);
            }

            foreach (GameObject tmpObj in Lvl1Weapons)
            {
                tmpObj.SetActive(false);
            }

            foreach (GameObject tmpObj in Lvl2Weapons)
            {
                tmpObj.SetActive(true);
            }
        }
        else
        {

            foreach (GameObject tmpObj in Lvl2Weapons)
            {
                tmpObj.SetActive(false);
            }

            foreach (GameObject tmpObj in Lvl1Weapons)
            {
                tmpObj.SetActive(false);
            }
            foreach (GameObject tmpObj in Lvl3Weapons)
            {
                tmpObj.SetActive(true);
            }
        }
    }
}
