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

    private void Start()
    {
        if (ParentForBtnSpawn == null) { ParentForBtnSpawn = transform; }
        if (World == null) { World = GameObject.Find("GameWorld"); }
        if (AllSpawnLocations == null) { AllSpawnLocations = GameObject.Find("AllPlayerPositions").transform; }

        GI = GameObject.FindWithTag("GameController").GetComponent<GameInfo>();

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
            SetPlayerRace(AllRaceInfo.Length-1, 1);
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

                for (int j = 0; j < AllRaceInfo[StoredRacePicks[i]].StartingObjects.Length; j++)
                {
                    GameObject Player = Instantiate(AllRaceInfo[StoredRacePicks[i]].StartingObjects[j], AllSpawnLocations.GetChild(i).position + AllRaceInfo[StoredRacePicks[i]].StartingObjectPositions[j], AllRaceInfo[StoredRacePicks[i]].StartingObjectRotations[j], PlayerParent.transform);

                    BasicInfo PlayerBI = Player.GetComponent<BasicInfo>();
                    PlayerBI.OwnedByPlayer = i;

                    int[] NewID = PlayerParent_SquadManager.CalculateNewIDs(true);
                    PlayerParent_SquadManager.Set_NewEntity(Player, NewID[1]);
                    PlayerBI.SetID(NewID[0], NewID[1]);
                }

                if (i != 0)
                {
                    AI_Controller tmp_AI = PlayerParent.AddComponent<AI_Controller>();
                    tmp_AI.SetBaseLocs(AllSpawnLocations.GetChild(i).position, AllSpawnLocations.GetChild(0).position);
                }
            }

            Actions.OnInitiatePlayerResources.InvokeAction(AllSpawnLocations.childCount);
            World.SetActive(true);
            Destroy(gameObject);
        }
        else
        {
            Debug.Log("Not All Players Have Picked Races, Left to Pick = " + (AllSpawnLocations.childCount - StoredRacePicks.Count));
        }
    }
}
