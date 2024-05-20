using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SindyrSpecialCloneSpawner : MonoBehaviour
{
    public List<SindyrCloneLifetime> StoredEntities;
    public GameObject[] AllEntitiesToSpawn;
    public float[] NumberEntitiesToSpawn;
    public int[] OwnedByPlayer;
    public float[] RandomPosOffset;
    public float MaxClonesAtOnce = 4;

    private GameInfo GI;
    private Transform SpawnParent;
    private bool HasDoneMainLogic = false;
    private bool IsActive = false;

    public void ForceKillAllClones()
    {
        foreach(SindyrCloneLifetime SCL in StoredEntities)
        {
            if(SCL != null) { SCL.AutoDestroySelf(); }
        }
    }

    private void Start()
    {
        GI = GameObject.FindWithTag("GameController").GetComponent<GameInfo>();
        IsActive = true;
    }

    private void Update()
    {
        if (GI.AllPlayers.Count == 2 && gameObject.activeSelf && !HasDoneMainLogic)
        {
            SetupCampaignLevel();
        }
    }

    public void OrderSpawn()
    {
        CheckForNulls();
        if (StoredEntities.Count < MaxClonesAtOnce)
        {
            HasDoneMainLogic = false;
            SetupCampaignLevel();
        }
    }

    private void CheckForNulls()
    {
        for(int i = 0; i < StoredEntities.Count; i++)
        {
            if(StoredEntities[i] == null) { StoredEntities.RemoveAt(i); }
        }
    }

    private void SetupCampaignLevel()
    {
        if (IsActive)
        {
            for (int i = 0; i < AllEntitiesToSpawn.Length; i++)
            {
                SpawnParent = GI.AllPlayers[OwnedByPlayer[i]].transform;
                for (int j = 0; j < NumberEntitiesToSpawn[i]; j++)
                {
                    BasicInfo tmp_BI = AllEntitiesToSpawn[i].GetComponent<BasicInfo>();
                    SBP_Info Squad = tmp_BI.SBPs;
                    bool IsNewSquad = true;
                    Vector3 SpawnPos = transform.position + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
                    Vector3 SpawnPosWithOffset = new Vector3(SpawnPos[0] + Random.Range(-RandomPosOffset[i], RandomPosOffset[i]), SpawnPos[1], SpawnPos[2] + Random.Range(-RandomPosOffset[i], RandomPosOffset[i]));

                    GameObject NewSquad;
                    NewSquad = SpawnParent.gameObject;


                    Vector3 Offset = new Vector3(0, 0, 0);
                    int x_Offset = 0;
                    int z_Offset = 0;
                    int OffsetAtModel = (int)Squad.SquadMin / 3;

                    for (int z = 0; z < Squad.SquadMin; z++)
                    {
                        if (z > Squad.SquadMin)
                        {
                            break;
                        }
                        else
                        {
                            if (z >= OffsetAtModel)
                            {
                                OffsetAtModel += (int)Squad.SquadMin / 3;
                                x_Offset = 0;
                                z_Offset++;
                            }
                            Offset += new Vector3(x_Offset, 0, z_Offset);
                            GameObject TempObj = Instantiate(AllEntitiesToSpawn[i], SpawnPosWithOffset + Offset, new Quaternion(), NewSquad.transform);
                            TempObj.name = tmp_BI.EBPs.EntityName + Time.time + ": " + i;

                            TempObj.GetComponent<BoxCollider>().size = tmp_BI.EBPs.EntityScale;

                            Setup_EBPs_BI_and_Squad(TempObj, IsNewSquad, i);
                            SetBaseDeathExplosions(tmp_BI, TempObj); ;

                            IsNewSquad = false;
                            x_Offset++;
                            StoredEntities.Add(TempObj.GetComponent<SindyrCloneLifetime>());
                        }
                    }
                }
            }
            HasDoneMainLogic = true;
        }
    }

    private void SetBaseDeathExplosions(BasicInfo tmp_BI, GameObject TempObj)
    {
        for (int DE = 0; DE < tmp_BI.EBPs.DeathExplosionsToSpawn.Length; DE++) 
        {
            Actions.OnCreateDeathExplosion.InvokeAction(new(false, tmp_BI.EBPs.DeathExplosionsToSpawn[DE], 0, false, tmp_BI.EBPs.DeathExplosionsNames[DE], true), TempObj);
        }
    }

    private void Setup_EBPs_BI_and_Squad(GameObject TempObj, bool IsNewSquad, int pos)
    {
        BasicInfo TempBI = TempObj.GetComponent<BasicInfo>();
        TempBI.OwnedByPlayer = OwnedByPlayer[pos];

        SquadManager CurrentSquadManager = GI.AllPlayers_SM[OwnedByPlayer[pos]]; //GI.AllPlayers[OwnedByPlayer[pos]].GetComponent<SquadManager>();
        int[] NewIDs = CurrentSquadManager.CalculateNewIDs(IsNewSquad);
        CurrentSquadManager.Set_NewEntity(TempObj, NewIDs[1]);

        TempBI.SetID(NewIDs[0], NewIDs[1]);
    }
}
