using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossFightInfo : MonoBehaviour
{
    public GameObject[] ActivatePreEndBoss;
    public GameObject[] ActivateFinalBoss;

    public Transform[] DefensiveTPMkrs;
    public List<Vector3> DefensiveTPMkrsPos;

    private void Start()
    {
        for(int i = 0; i < DefensiveTPMkrs.Length; i++)
        {
            DefensiveTPMkrsPos.Add(DefensiveTPMkrs[i].position);
        }
    }
}
