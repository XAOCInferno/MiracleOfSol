using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionNameManager : MonoBehaviour
{
    public GameObject[] AllMissionNames;

    private void Start()
    {
        ActivateMissionName();
    }

    private void ActivateMissionName()
    {
        DeActivateMissionNames();
        AllMissionNames[PickRace.MissionNumber].SetActive(true);
    }

    private void DeActivateMissionNames()
    {
        foreach (GameObject Name in AllMissionNames) { Name.SetActive(false); }
    }
}
