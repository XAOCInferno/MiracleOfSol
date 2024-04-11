using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FullyHealFaction : MonoBehaviour
{
    public int Faction = 0;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("GameController").TryGetComponent(out GameInfo GI);
        List<List<Health>> FactionHealth = GI.AllPlayers_SM[Faction].Get_AllHealthLists();

        for (int i = 0; i < FactionHealth.Count; i++)
        {
            FactionHealth[i][0].UpdateCurrentHealth(-1, false, 10000, 10000);
        }

        Destroy(gameObject);
    }
}
