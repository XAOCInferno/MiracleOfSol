using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CampaignLevelSetup : MonoBehaviour
{
    public bool SpawnAsAlliedHero = false;
    public GameObject[] AllEntitiesToSpawn;
    public Transform[] MarkerToSpawnAt;
    public float[] NumberEntitiesToSpawn;
    public int[] OwnedByPlayer;
    public bool UpdateHeroIcons = false;
    public float[] RandomPosOffset;
    public bool TrackEntities = false;
    public GameObject[] PostTrackingActivate;

    public bool IsSpecialLeaderBossSpawn = false;
    public Transform LeaderBossJumpToInit;
    public GameObject LeaderBoss_OnWinActivate;
    public GameObject LeaderBoss_OnLoseActivate;

    public bool IsSpecialTankBossSpawn = false;
    public bool TankBossForceSecondPhase = false;
    public Transform TankBossMoveToInit;
    public Transform TankBossMoveToDelayed;
    public Transform[] TankBossMoveToLocations;
    public GameObject TankBossActivateOnRetreat;
    public MoveOnProximity OpenMeOnFinalComplete;

    private GameInfo GI;
    private Transform SpawnParent;
    private bool HasDoneMainLogic = false;
    private List<GameObject> TrackedObjects = new List<GameObject>();

    private void Start()
    {
        GI = GameObject.FindWithTag("GameController").GetComponent<GameInfo>();
    }

    private void Update()
    {
        if(GI.AllPlayers.Count == 2 && gameObject.activeSelf && !HasDoneMainLogic)
        {
            SetupCampaignLevel();
        }

        if(TrackEntities && HasDoneMainLogic)
        {
            for(int i  = 0; i < TrackedObjects.Count; i++)
            {
                if(TrackedObjects[i] == null) { TrackedObjects.RemoveAt(i); }

                if(TrackedObjects.Count == 0)
                {
                    foreach(GameObject tmpObj in PostTrackingActivate) { tmpObj.SetActive(true); }
                    if (OpenMeOnFinalComplete != null) { OpenMeOnFinalComplete.ForceMove(); }
                    Destroy(gameObject);
                }
            }
        }
    }

    private void SetupCampaignLevel()
    {
        for (int i = 0; i < AllEntitiesToSpawn.Length; i++)
        {
            SpawnParent = GI.AllPlayers[OwnedByPlayer[i]].transform;
            for (int j = 0; j < NumberEntitiesToSpawn[i]; j++)
            {
                BasicInfo tmp_BI = AllEntitiesToSpawn[i].GetComponent<BasicInfo>();
                SBP_Info Squad = tmp_BI.SBPs;
                bool IsNewSquad = true;
                Vector3 SpawnPos = MarkerToSpawnAt[i].position;
                Vector3 SpawnPosWithOffset = new Vector3(SpawnPos[0] + Random.Range(-RandomPosOffset[i], RandomPosOffset[i]), SpawnPos[1], SpawnPos[2] + Random.Range(-RandomPosOffset[i], RandomPosOffset[i]));
                
                GameObject NewSquad;
                if (!SpawnAsAlliedHero)
                {
                    NewSquad = Instantiate(new GameObject(), SpawnPosWithOffset, new Quaternion(), SpawnParent);
                    NewSquad.gameObject.name = "Squad: " + Time.time + ": " + i;
                }
                else
                {
                    NewSquad = SpawnParent.gameObject;
                }

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
                        //EBP_Info TempEBP = TempObj.GetComponent<EBP_Info>();

                        //Setup_EBPs_OSA(TempObj, TempEBP);
                        if (TrackEntities) { TrackedObjects.Add(TempObj); }

                        Setup_EBPs_BI_and_Squad(TempObj, IsNewSquad, i);
                        SetBaseDeathExplosions(tmp_BI, TempObj);
                        CheckForBoss(TempObj);

                        IsNewSquad = false;
                        x_Offset++;
                    }
                }
            }
        }

        if (!TrackEntities)
        {
            Destroy(gameObject);
        }
        HasDoneMainLogic = true;
    }

    private void SetBaseDeathExplosions(BasicInfo tmp_BI, GameObject TempObj)
    {
        for (int DE = 0; DE < tmp_BI.EBPs.DeathExplosionsToSpawn.Length; DE++) { GI.DEM.SetNewDeathExplosion(false, TempObj, tmp_BI.EBPs.DeathExplosionsToSpawn[DE], 0, false, tmp_BI.EBPs.DeathExplosionsNames[DE], true); }
    }

    private void CheckForBoss(GameObject TempObj)
    {
        if (IsSpecialTankBossSpawn)
        {
            TempObj.GetComponent<AI_BossTank>().ActivateTankBoss(TankBossMoveToInit, TankBossMoveToDelayed, TankBossMoveToLocations, TankBossActivateOnRetreat, TankBossForceSecondPhase);
        }
        else if (IsSpecialLeaderBossSpawn)
        {
            TempObj.GetComponent<AI_BossLeader>().ActivateLeaderBoss(LeaderBossJumpToInit.position, LeaderBoss_OnWinActivate, LeaderBoss_OnLoseActivate);
        }
        else if (TankBossMoveToInit != null) //NotWorking?
        {
            TempObj.GetComponent<EntityMovement>().SetMoveDestination(TankBossMoveToInit.position);
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
