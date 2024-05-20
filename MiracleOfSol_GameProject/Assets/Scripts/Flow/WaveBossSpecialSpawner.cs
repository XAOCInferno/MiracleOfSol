using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveBossSpecialSpawner : MonoBehaviour
{
    public GameObject[] ActivateOnComplete;
    public GameObject[] AllEntitiesToSpawn;
    public Transform[] MarkerToSpawnAt;
    public float[] NumberEntitiesToSpawn;
    public int[] SpawnAtWaveNumber;
    public int[] OwnedByPlayer;
    public float DelayBetweenWaves = 4;

    public bool UpdateHeroIcons = false;
    public float[] RandomPosOffset;
    public bool TrackEntities = false;
    public GameObject[] PostTrackingActivate;
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
    private int CurrentWave = 0;
    private bool WaveIsComplete = true;

    private void Start()
    {
        GameObject.FindWithTag("GameController").TryGetComponent(out GI);
        Actions.OnChangeAIAggression(eAIAggressionTypes.FullOnAggression);
    }

    private void Update()
    {
        if (GI.AllPlayers.Count == 2 && gameObject.activeSelf && !HasDoneMainLogic && WaveIsComplete)
        {
            Invoke(nameof(DoNextWave), DelayBetweenWaves);
            WaveIsComplete = false;
            //HasDoneMainLogic = true;
        }
        else if (HasDoneMainLogic && !WaveIsComplete)
        {
            CheckForWaveComplete();
        }
    }

    private void EndWaveBoss()
    {
        foreach (GameObject tmpActive in ActivateOnComplete)
        {
            tmpActive.SetActive(true);
        }
        Destroy(this);
    }

    private void SetBaseDeathExplosions(BasicInfo tmp_BI, GameObject TempObj)
    {
        for (int DE = 0; DE < tmp_BI.EBPs.DeathExplosionsToSpawn.Length; DE++) { GI.DEM.SetNewDeathExplosion(false, TempObj, tmp_BI.EBPs.DeathExplosionsToSpawn[DE], 0, false, tmp_BI.EBPs.DeathExplosionsNames[DE], true); }
    }

    private void DoNextWave()
    {
        bool HasSpawnedAUnit = false;
        for (int i = 0; i < AllEntitiesToSpawn.Length; i++)
        {
            if (SpawnAtWaveNumber[i] == CurrentWave && AllEntitiesToSpawn[i] != null)
            {
                HasSpawnedAUnit = true;
                SpawnParent = GI.AllPlayers[OwnedByPlayer[i]].transform;
                for (int j = 0; j < NumberEntitiesToSpawn[i]; j++)
                {
                    AllEntitiesToSpawn[i].TryGetComponent(out BasicInfo tmp_BI);
                    SBP_Info Squad = tmp_BI.SBPs;
                    bool IsNewSquad = true;
                    Vector3 SpawnPos = MarkerToSpawnAt[i].position;
                    Vector3 SpawnPosWithOffset = new Vector3(SpawnPos[0] + Random.Range(-RandomPosOffset[i], RandomPosOffset[i]), SpawnPos[1], SpawnPos[2] + Random.Range(-RandomPosOffset[i], RandomPosOffset[i]));
                    GameObject NewSquad = Instantiate(new GameObject(), SpawnPosWithOffset, new Quaternion(), SpawnParent);
                    NewSquad.gameObject.name = "Squad: " + Time.time;

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
                            TempObj.GetComponent<BoxCollider>().size = tmp_BI.EBPs.EntityScale;
                            //EBP_Info TempEBP = TempObj.GetComponent<EBP_Info>();

                            //Setup_EBPs_OSA(TempObj, TempEBP);
                            if (TrackEntities) { TrackedObjects.Add(TempObj); }
                            Setup_EBPs_BI_and_Squad(TempObj, IsNewSquad, i);
                            SetBaseDeathExplosions(tmp_BI, TempObj);
                            IsNewSquad = false;
                            x_Offset++;

                            if (IsSpecialTankBossSpawn)
                            {
                                AI_BossTank TmpTankInfo = TempObj.GetComponent<AI_BossTank>();
                                if (TmpTankInfo != null) 
                                {
                                    TmpTankInfo.ActivateTankBoss(TankBossMoveToInit, TankBossMoveToDelayed, TankBossMoveToLocations, TankBossActivateOnRetreat, TankBossForceSecondPhase);
                                }
                            }
                            else if (TankBossMoveToInit != null) //NotWorking?
                            {
                                TempObj.GetComponent<EntityMovement>().SetMoveDestination(TankBossMoveToInit.position);
                            }
                        }
                    }
                }
            }
        }

        if (!HasSpawnedAUnit) { EndWaveBoss(); }
        ClearSpawnedEntitiesFromList();
        HasDoneMainLogic = true;
    }

    private void CheckForWaveComplete()
    {
        bool NoEntities = true;
        for (int i = 0; i < TrackedObjects.Count; i++)
        {
            NoEntities = false;

            if (TrackedObjects[i] == null) { TrackedObjects.RemoveAt(i); }

            if (TrackedObjects.Count == 0)
            {
                CurrentWave++;
                WaveIsComplete = true;
                HasDoneMainLogic = false;
            }
        }

        if (NoEntities)
        {
            if (TrackedObjects.Count == 0)
            {
                CurrentWave++;
                WaveIsComplete = true;
                HasDoneMainLogic = false;
            }
        }
    }

    private void ClearSpawnedEntitiesFromList()
    {
        for(int i = 0; i< AllEntitiesToSpawn.Length; i++)
        {
            if(SpawnAtWaveNumber[i] <= CurrentWave)
            {
                AllEntitiesToSpawn[i] = null;
            }
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
