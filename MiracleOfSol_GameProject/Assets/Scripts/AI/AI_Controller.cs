using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Controller : MonoBehaviour
{
    private SquadManager SM;
    private ResourcePointManager RPM;
    private GameInfo GI;
    private MusicManager GI_MM;

    private BasicInfo BI;
    private List<Vector3> DefendLocations = new List<Vector3>();
    private List<int> MaxSquadsToDefendALocation = new List<int>();
    private Vector3 HomeBasePos;
    private Vector3 EnemyBasePos;
    private readonly float CriticalHPPoint = 0.25f;
    private readonly float HealUntil = 0.75f;
    private readonly float MaxMoveDistance = 3.75f;
    
    // Start is called before the first frame update
    void Start()
    {
        SM = gameObject.GetComponent<SquadManager>();
        GI = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameInfo>();
        GI.TryGetComponent(out GI_MM);
        //RPM = GI.ResourcePointMaster.GetComponent<ResourcePointManager>();
        BI = gameObject.GetComponent<BasicInfo>();

        InvokeRepeating(nameof(MainAILoop), 1, 1); //I manage AI planning like cap, defend, atk
        InvokeRepeating(nameof(CombatAILoop), 2, 1); //I need to be run more often bc I will manage retreating
    }

    private void MainAILoop()
    {
        List<List<GameObject>> AllEntities = SM.Get_AllSquadLists();
        List<List<BasicInfo>> AllBI = SM.Get_AllSquadListsBI();
        List<List<Combat>> AllCombat = SM.Get_AllCombatLists();

        //CheckCapStatus(AllEntities, AllBI);
        DefendChokepoint(AllEntities, AllBI);
        IdleAILogic(AllEntities, AllBI, AllCombat);

        if (GI_MM.IsInBoss) 
        {
            Actions.OnChangeAIAggression(eAIAggressionTypes.FullOnAggression);
        }
    }

    private void CombatAILoop()
    {
        List<List<GameObject>> AllEntities = SM.Get_AllSquadLists();
        List<List<GameObject>> AllEnemyEntities = GI.AllPlayers_SM[0].Get_AllSquadLists();
        List<List<BasicInfo>> AllBI = SM.Get_AllSquadListsBI();
        List<List<Combat>> AllCombat = SM.Get_AllCombatLists();
        List<List<Health>> AllHealth = GI.AllPlayers_SM[0].Get_AllHealthLists();

        CheckHealStatus(AllEntities, AllBI);
        OrderAttackLoop(AllCombat, AllEntities, AllEnemyEntities, AllHealth, AllBI);
    }

    public void SetBaseLocs(Vector3 NewHomePos, Vector3 EnemyHomePos)
    {
        HomeBasePos = NewHomePos;
        EnemyBasePos = EnemyHomePos;
        
        if(GI == null) { GI = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameInfo>(); }
        DefendLocations = GI.GetAllCritLocations();
        MaxSquadsToDefendALocation = GI.MaxSquadsToDefendCrits;
    }

    public Vector3 GetHomeBasePos()
    {
        return HomeBasePos;
    }

    private void IdleAILogic(List<List<GameObject>> AllEntities, List<List<BasicInfo>> AllBI, List<List<Combat>> AllCombat)
    {
        for (int i = 0; i < AllEntities.Count; i++)
        {
            try
            {
                if (AllEntities.Count > i)
                {
                    for (int j = 0; j < AllEntities[i].Count; j++)
                    {
                        if (AllEntities[i][j] != null)
                        {
                            if (!AllBI[i][j].EBPs.IsBoss_IgnoreAIManager)
                            {
                                int[] tmp_IDs = AllBI[i][j].GetIDs();

                                if (!SM.Get_SingleHealth(tmp_IDs).Get_AI_IsTryingToHeal() && !AllCombat[i][j].HasBeenAggro)
                                {
                                    if (Random.Range(0f, 1f) > 0.33f)
                                    {
                                        //Do a random idle logic

                                        if (Random.Range(0f, 1f) > 0.38f)
                                        {
                                            //Do move to a position within 1.25 units

                                            Vector3 NewMoveLoc = new Vector3(Random.Range(-1.25f, 1.25f), 0, Random.Range(-1.25f, 1.25f));
                                            NewMoveLoc = new Vector3(transform.position.x + NewMoveLoc[0], transform.position.y, transform.position.z + NewMoveLoc[2]);

                                            SM.SquadOrder_SetMoveDestination(tmp_IDs, NewMoveLoc, true);
                                        }
                                        else if (Random.Range(0f, 1f) > 0.7f)
                                        {
                                            //Do move to a random friendly unit

                                            SM.SquadOrder_SetMoveDestination(tmp_IDs, AllEntities[Random.Range(0, AllEntities.Count)][j].transform.position, true);
                                        }
                                        else
                                        {
                                            //Return to start location

                                            SM.SquadOrder_SetMoveDestination(tmp_IDs, AllBI[i][j].GetStartLoc(), true);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            catch
            {
                //I probably have no EM;
            }
        }
    }

    private void CheckCapStatus(List<List<GameObject>> AllEntities, List<List<BasicInfo>> AllBI)
    {
        CapturePoint[] AllCPs = RPM.GetAllCP();
        BasicInfo[] AllCPs_BI = RPM.GetAllCP_BI();

        for (int cp = 0; cp < AllCPs.Length; cp++)
        {
            if(AllCPs_BI[cp].OwnedByPlayer != BI.OwnedByPlayer && AllCPs[cp].CurrentCappingPlayer != BI.OwnedByPlayer)
            {
                OrderACapture(AllCPs[cp], AllEntities, AllBI);
            }
        }
    }

    private void OrderAttackLoop(List<List<Combat>> AllCombat, List<List<GameObject>> AllEntities, List<List<GameObject>> AllEnemyEntities, List<List<Health>> AllEnemyHealth, List<List<BasicInfo>> AllBI)
    {
        try
        {
            List<Vector3> AlliedSquadsPos = new List<Vector3>();

            for (int i = 0; i < AllEnemyEntities.Count; i++)
            {
                AlliedSquadsPos.Add(AllEnemyEntities[i][0].transform.position);
            }

            for (int i = 0; i < AllEntities.Count; i++)
            {
                if (AllBI.Count > i)
                {
                    for (int j = 0; j < AllBI[i].Count; j++)
                    {
                        if (AllBI[i][j] != null)
                        {
                            if (!AllBI[i][j].EBPs.IsBoss_IgnoreAIManager)
                            {
                                int[] tmp_IDs = AllBI[i][j].GetIDs();
                                Health tmp_Health = SM.Get_SingleHealth(tmp_IDs);
                                try
                                {
                                    if (tmp_Health.GetCurrentHP_AsPercentOfMax() <= 0.9f || tmp_Health.GetCurrentArmour_AsPercentOfMax() <= 0.9f || CombatManager.GetAIAggroState() == eAIAggressionTypes.FullOnAggression)
                                    {
                                        AllCombat[i][j].HasBeenAggro = true;
                                    }
                                }
                                catch
                                {
                                    Debug.LogWarning("ERROR! In AI_Controller/OrderAttackLoop. Cannot do attack logic");
                                }

                                if (AllCombat[i][j].HasBeenAggro && !tmp_Health.Get_AI_IsTryingToHeal())
                                {
                                    float tmpDistance = 99999999999;
                                    int tmpEntityPos = 0;
                                    for (int loc = 0; loc < AllEnemyEntities.Count; loc++)
                                    {
                                        if (!AllEnemyHealth[loc][0].GetIfIncapacitated() && AllEntities[i] != null)
                                        {
                                            for (int z = 0; z < AllEntities[i].Count; z++)
                                            {
                                                if (AllEntities[i][z] != null)
                                                {
                                                    float NewDistance = Vector3.Distance(AllEnemyEntities[loc][j].transform.position, AllEntities[i][j].transform.position);

                                                    if (NewDistance < tmpDistance)
                                                    {
                                                        tmpDistance = NewDistance;
                                                        tmpEntityPos = loc;
                                                    }
                                                }
                                            }
                                        }
                                    }

                                    SM.SquadOrder_StopCommands(tmp_IDs, true);
                                    SM.SquadOrder_SetAttackTarget(tmp_IDs, AllEnemyEntities[tmpEntityPos][j].transform, true);
                                }
                                else
                                {
                                    for (int loc = 0; loc < AlliedSquadsPos.Count; loc++)
                                    {
                                        if (Vector3.Distance(AlliedSquadsPos[loc], AllEntities[i][j].transform.position) <= AllBI[i][j].EBPs.AIAggroRange)
                                        {
                                            AllCombat[i][j].HasBeenAggro = true;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
        catch
        {
            Debug.LogWarning("ERROR! In AI_Controller/OrderAttackLoop. Cannot order attack!");
        }
    }

    private void OrderACapture(CapturePoint CP, List<List<GameObject>> AllEntities, List<List<BasicInfo>> AllBI)
    {
        float CurrentDesiredCapDistance = 9999999999999;
        float CurrentCapDistance = 0;
        int[] CurrentSquadIDs = new int[0];

        for(int i = 0; i < AllEntities.Count; i++)
        {
            CurrentCapDistance = Vector3.Distance(SM.GetSquadAvgPosition(i), CP.transform.position);
            if (CurrentCapDistance <= CurrentDesiredCapDistance)//DesiredCapDistance)
            {
                List<GameObject> NewSquad = SM.Get_SquadList(i);

                if (NewSquad.Count > 0)
                {
                    GameObject entity = NewSquad[0];
                    if (entity != null)
                    {
                        int[] tmp_IDs = AllBI[i][0].GetIDs();
                        Health tmp_Health = SM.Get_SingleHealth(tmp_IDs);
                        if (tmp_Health != null)
                        {
                            if (!SM.SquadOrder_GetCaptureTarget(tmp_IDs) && !tmp_Health.Get_AI_IsTryingToHeal())
                            {
                                CurrentSquadIDs = tmp_IDs;
                                CurrentDesiredCapDistance = CurrentCapDistance;
                            }
                        }
                    }
                }
            }
        }

        if(CurrentSquadIDs.Length > 0)
        {
            SM.SquadOrder_SetCaptureTarget(CurrentSquadIDs, CP, true, true);
        }
    }

    private void CheckHealStatus(List<List<GameObject>> AllEntities, List<List<BasicInfo>> AllBI)
    {
        for (int i = 0; i < AllEntities.Count; i++)
        {
            if (AllEntities.Count > i)
            {
                for (int j = 0; j < AllEntities[i].Count; j++)
                {
                    if (AllEntities[i][j] != null)
                    {
                        if (AllBI[i][j] != null)
                        {
                            if (!AllBI[i][j].EBPs.IsBoss_IgnoreAIManager)
                            {
                                if (AllEntities[i] != null)
                                {
                                    try
                                    {
                                        int[] tmp_IDs = AllBI[i][j].GetIDs();
                                        Health tmp_Health = SM.Get_SingleHealth(tmp_IDs);
                                        if (tmp_Health != null)
                                        {
                                            if (!tmp_Health.Get_AI_IsTryingToHeal())
                                            {
                                                if (tmp_Health.GetCurrentHP_AsPercentOfMax() <= CriticalHPPoint)
                                                {
                                                    SM.SquadOrder_SetMoveDestination(tmp_IDs, HomeBasePos, true);
                                                    SM.SquadAIStatus_SetIsHeal(tmp_IDs, true);
                                                }
                                            }
                                            else
                                            {
                                                if (tmp_Health.GetCurrentHP_AsPercentOfMax() >= HealUntil)
                                                {
                                                    SM.SquadOrder_StopCommands(tmp_IDs, true);
                                                    SM.SquadAIStatus_SetIsHeal(tmp_IDs, false);
                                                }
                                            }
                                        }
                                    }
                                    catch
                                    {
                                        Debug.LogWarning("AIController: Unable to retreat a unit, it is probably destroyed.");
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void DefendChokepoint(List<List<GameObject>> AllEntities, List<List<BasicInfo>> AllBI)
    {
        for (int j = 0; j < DefendLocations.Count; j++)
        {
            float CurrentDesiredDefendDistance = 9999999999999;
            float CurrentDefendDistance;
            int[] CurrentSquadIDs;
            int NumberOfDefenders = 0;

            if (NumberOfDefenders < MaxSquadsToDefendALocation[j])
            {
                for (int i = 0; i < AllEntities.Count; i++)
                {
                    try
                    {
                        if (!AllBI[i][0].EBPs.IsBoss_IgnoreAIManager)
                        {
                            if (Random.Range(0f, 1f) > 0.66f)
                            {
                                CurrentDefendDistance = Vector3.Distance(SM.GetSquadAvgPosition(i), DefendLocations[j]);
                                if (CurrentDefendDistance <= CurrentDesiredDefendDistance && CurrentDefendDistance <= MaxMoveDistance)
                                {
                                    List<GameObject> NewSquad = SM.Get_SquadList(i);

                                    if (NewSquad.Count > 0)
                                    {
                                        GameObject entity = NewSquad[0];
                                        if (entity != null)
                                        {
                                            int[] tmp_IDs = AllBI[i][0].GetIDs();

                                            Health tmp_Health = SM.Get_SingleHealth(tmp_IDs);
                                            if (tmp_Health != null)
                                            {
                                                //if (!SM.SquadOrder_GetCaptureTarget(tmp_IDs) && !tmp_Health.Get_AI_IsTryingToHeal())
                                                if (!tmp_Health.Get_AI_IsTryingToHeal())
                                                {
                                                    CurrentSquadIDs = tmp_IDs;
                                                    CurrentDesiredDefendDistance = CurrentDefendDistance;
                                                    NumberOfDefenders++;
                                                    SM.SquadOrder_SetMoveDestination(CurrentSquadIDs, DefendLocations[j] + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f)), true);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    catch
                    {

                    }
                }
            }
        }
    }
}
