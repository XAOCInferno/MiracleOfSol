using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MissionNameManager : MonoBehaviour
{
    public GameObject[] AllMissionNames;
    private PickRace PR;

    private void Start()
    {
        GameObject.FindGameObjectWithTag("GameController").TryGetComponent(out GameInfo tmpGI);
        tmpGI.PickRaceManager.TryGetComponent(out PR);
        ActivateMissionName();
    }

    private void ActivateMissionName()
    {
        DeActivateMissionNames();
        AllMissionNames[PR.MissionNumber].SetActive(true);
    }

    private void DeActivateMissionNames()
    {
        foreach (GameObject Name in AllMissionNames) { Name.SetActive(false); }
    }
}
