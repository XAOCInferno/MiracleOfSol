using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*using MLAPI;
using MLAPI.Messaging;
using MLAPI.NetworkVariable;*/

public class PickRace : MonoBehaviour
{
    public int MissionNumber = 0;
    public bool SkipPick = true;
    public GameObject World;
    public Transform AllSpawnLocations;
    public Transform ParentForBtnSpawn;
    public Button ButtonTemplate;
    public Vector2 ButtonOffset = new Vector2(100, 100);
    public RaceInfo[] AllRaceInfo;

    private List<Button> RaceButtons = new List<Button>();
    private List<int> StoredRacePicks = new List<int>();
    private GameInfo GI;
    private ResourceManager RM;

    private void InitNewLevel()
    {
        if (ParentForBtnSpawn == null) { ParentForBtnSpawn = transform; }
        if (World == null) { World = GameObject.Find("GameWorld"); }
        if (AllSpawnLocations == null) { AllSpawnLocations = GameObject.Find("AllPlayerPositions").transform; }

        GI = GameObject.FindWithTag("GameController").GetComponent<GameInfo>();
        RM = GI.ResourceManager.GetComponent<ResourceManager>();

        for (int j = 0; j < AllSpawnLocations.childCount; j++)
        {
            for (int i = 0; i < AllRaceInfo.Length; i++)
            {
                Button Temp_NewButton = Instantiate(ButtonTemplate, ParentForBtnSpawn);
                RectTransform RT_NewButton = Temp_NewButton.GetComponent<RectTransform>();
                RT_NewButton.position = new Vector2(RT_NewButton.sizeDelta.x * i, 1 + (RT_NewButton.sizeDelta.y * j)) + ButtonOffset;

                int TempRacePos = i;
                int TempBtnRow = j;
                Temp_NewButton.onClick.AddListener(delegate { SetPlayerRace(TempRacePos, TempBtnRow); });


                Temp_NewButton.transform.GetChild(0).GetComponent<Text>().text = AllRaceInfo[i].name;

                RaceButtons.Add(Temp_NewButton);
            }
        }

        if (SkipPick)
        {
            SetPlayerRace(MissionNumber, 0);
            SetPlayerRace(AllRaceInfo.Length - 1, 1);
        }
    }

    private void SetPlayerRace(int Race, int BtnRow)
    {
        StoredRacePicks.Add(Race);
        for (int race_count = BtnRow * AllRaceInfo.Length; race_count < (BtnRow + 1) * AllRaceInfo.Length; race_count++)
        {
            Destroy(RaceButtons[race_count].gameObject);
        }

        if (StoredRacePicks.Count == AllSpawnLocations.childCount)
        {
            for (int i = 0; i < AllSpawnLocations.childCount; i++)
            {
                GameObject PlayerParent = Instantiate(new GameObject(), World.transform);
                RaceInfo RI = PlayerParent.AddComponent<RaceInfo>();
                PlayerParent.AddComponent<SquadManager>();
                BasicInfo BI = PlayerParent.AddComponent<BasicInfo>();
                BI.OwnedByPlayer = i;
                RI.SetAllInfoFromBlueprint(AllRaceInfo[StoredRacePicks[i]]);

                PlayerParent.name = "Player (" + (i + 1) + ") Master";
                GI.AllPlayers.Add(PlayerParent);
                SquadManager PlayerParent_SquadManager = PlayerParent.GetComponent<SquadManager>();
                GI.AllPlayers_SM.Add(PlayerParent_SquadManager);

                    SpawnSquads(AllRaceInfo[StoredRacePicks[i]].StartingObjects, i, AllSpawnLocations.GetChild(i).position);
                    /*BasicInfo PlayerBI = AllRaceInfo[StoredRacePicks[i]].StartingObjects[j].GetComponent<BasicInfo>();
                    bool IsNewSquad = true;
                    for (int z = 0; z < PlayerBI.SBPs.SquadMin; z++)
                    {
                        GameObject Player = Instantiate(AllRaceInfo[StoredRacePicks[i]].StartingObjects[j], AllSpawnLocations.GetChild(i).position + AllRaceInfo[StoredRacePicks[i]].StartingObjectPositions[j], AllRaceInfo[StoredRacePicks[i]].StartingObjectRotations[j], PlayerParent.transform);

                        PlayerBI.OwnedByPlayer = i;
                        int[] NewID = PlayerParent_SquadManager.CalculateNewIDs(IsNewSquad);
                        IsNewSquad = false;
                        PlayerParent_SquadManager.Set_NewEntity(Player, NewID[1]);
                        PlayerBI.SetID(NewID[0], NewID[1]);
                    }*/
                

                if (i != 0)
                {
                    AI_Controller tmp_AI = PlayerParent.AddComponent<AI_Controller>();
                    tmp_AI.SetBaseLocs(AllSpawnLocations.GetChild(i).position, AllSpawnLocations.GetChild(0).position);
                }
            }

            RM.InitPlayerResources(AllSpawnLocations.childCount);
            World.SetActive(true);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Not All Players Have Picked Races, Left to Pick = " + (AllSpawnLocations.childCount - StoredRacePicks.Count));
        }
    }

    private void SpawnSquads(GameObject[] AllEntitiesToSpawn, int OwnedByPlayer, Vector3 SpawnPos)
    {
        for (int i = 0; i < AllEntitiesToSpawn.Length; i++)
        {
            Transform SpawnParent = GI.AllPlayers[OwnedByPlayer].transform;
            BasicInfo tmp_BI = AllEntitiesToSpawn[i].GetComponent<BasicInfo>();
            SBP_Info Squad = tmp_BI.SBPs;
            bool IsNewSquad = true;
            Vector3 SpawnPosWithOffset = new Vector3(SpawnPos[0] + Random.Range(-1, 1), SpawnPos[1], SpawnPos[2] + Random.Range(-1, 1)) + AllRaceInfo[StoredRacePicks[OwnedByPlayer]].StartingObjectPositions[i];

            GameObject NewSquad;
            if (OwnedByPlayer != 0)
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

                    Setup_EBPs_BI_and_Squad(TempObj, IsNewSquad, OwnedByPlayer);
                    SetBaseDeathExplosions(tmp_BI, TempObj);

                    IsNewSquad = false;
                    x_Offset++;
                }
            }
        }

    }

    private void SetBaseDeathExplosions(BasicInfo tmp_BI, GameObject TempObj)
    {
        for (int DE = 0; DE < tmp_BI.EBPs.DeathExplosionsToSpawn.Length; DE++) { GI.DEM.SetNewDeathExplosion(false, TempObj, tmp_BI.EBPs.DeathExplosionsToSpawn[DE], 0, false, tmp_BI.EBPs.DeathExplosionsNames[DE], true); }
    }

    private void Setup_EBPs_BI_and_Squad(GameObject TempObj, bool IsNewSquad, int OwnedByPlayer)
    {
        BasicInfo TempBI = TempObj.GetComponent<BasicInfo>();
        TempBI.OwnedByPlayer = OwnedByPlayer;

        SquadManager CurrentSquadManager = GI.AllPlayers_SM[OwnedByPlayer]; //GI.AllPlayers[OwnedByPlayer[pos]].GetComponent<SquadManager>();
        int[] NewIDs = CurrentSquadManager.CalculateNewIDs(IsNewSquad);
        CurrentSquadManager.Set_NewEntity(TempObj, NewIDs[1]);

        TempBI.SetID(NewIDs[0], NewIDs[1]);
    }
}
