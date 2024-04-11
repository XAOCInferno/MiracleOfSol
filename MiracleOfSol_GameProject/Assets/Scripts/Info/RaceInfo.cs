using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceInfo : MonoBehaviour
{
    public string RaceName = "__DefaultRace";
    public string RaceDescription = "__DefaultDescription";
    public GameObject[] StartingObjects;
    public Vector3[] StartingObjectPositions;
    public Quaternion[] StartingObjectRotations;

    public void InitRace()
    {
        for (int i = 0; i < StartingObjects.Length; i++)
        {
            Instantiate(StartingObjects[i], StartingObjectPositions[i], StartingObjectRotations[i]);
        }
    }

    public void SetAllInfoFromBlueprint(RaceInfo RI)
    {
        RaceName = RI.RaceName; RaceDescription = RI.RaceDescription; 
        StartingObjects = RI.StartingObjects; StartingObjectPositions = RI.StartingObjectPositions; StartingObjectRotations = RI.StartingObjectRotations;
    }
}
