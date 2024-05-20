using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum eAIAggressionTypes 
{
    Passive = 0,
    FullOnAggression = 1,
}

public class CombatManager : MonoBehaviour
{
    private float CombatCheckRate = 1.8f;
    private float TrackEntityRate = 0.8f;
    private float AbilityCheckRate = 0.028f;

    private List<SquadManager> AllPlayers = new List<SquadManager>();
    private List<List<GameObject>> AllSquads = new List<List<GameObject>>();
    private List<List<BasicInfo>> AllSquads_BI = new List<List<BasicInfo>>();
    private List<List<Combat>> AllSquads_Combat = new List<List<Combat>>();
    private List<List<Health>> AllSquads_Health = new List<List<Health>>();
    private List<List<AggressionManager>> AllSquads_AM = new List<List<AggressionManager>>();
    private List<List<Vector3>> AllSquads_Positions = new List<List<Vector3>>();
    private List<ModifierApplier> AllAppliedModifiers = new List<ModifierApplier>();
    private bool TrackingTimeout = false;

    private float PreviousCombatInvokeTime = -1000;
    private float PreviousAbilityInvokeDepth = -1000;
    private float TimeCurrent;
    private float MinInvokeDepthTime = 0.04f;
    private static eAIAggressionTypes AI_Aggression = eAIAggressionTypes.Passive;

    private void OnEnable()
    {

        Actions.OnChangeAIAggression += SetAIAggroState;
        Actions.OnAddNewModifier += AddNewMod;

    }

    private void OnDisable()
    {

        Actions.OnChangeAIAggression -= SetAIAggroState;
        Actions.OnAddNewModifier -= AddNewMod;

    }

    private void Start()
    {
        InvokeRepeating(nameof(SetupBasicVariables),0.5f,0.5f);
    }

    public void SetAIAggroState(eAIAggressionTypes aggression) 
    { 

        AI_Aggression = aggression; 

    }

    public static eAIAggressionTypes GetAIAggroState() { return AI_Aggression; }

    private void Update()
    {
        if (TimeCurrent >= 1000)
        {
            TimeCurrent = 0;
        }
        else
        {
            TimeCurrent += Time.deltaTime;
        }
    }

    public void AddNewMod(ModifierApplier NewMod)
    {
        AllAppliedModifiers.Add(NewMod);
    }

    private void SetupBasicVariables()
    {
        try
        {
            GameObject.FindGameObjectWithTag("GameController").TryGetComponent(out GameInfo tmpGI);
            AllPlayers = tmpGI.AllPlayers_SM;
        }
        catch
        {
            Debug.LogError("ERROR! In 'CombatManager' Basic Setup: Can't find GameController!");
        }

        if (AllPlayers.Count >= 2)
        {
            CancelInvoke();
            InvokeRepeating(nameof(TrackAllEntities), TrackEntityRate, TrackEntityRate);
            InvokeRepeating(nameof(CheckForCombat), CombatCheckRate, CombatCheckRate);
            InvokeRepeating(nameof(CheckForAbility), AbilityCheckRate, AbilityCheckRate);
        }
    }

    private void TrackAllEntities()
    {
        TrackingTimeout = true;
        AllSquads.Clear();
        AllSquads_Combat.Clear();
        AllSquads_AM.Clear();
        AllSquads_Positions.Clear();
        AllSquads_BI.Clear();
        AllSquads_Health.Clear();

        foreach (SquadManager SM in AllPlayers)
        {
            SM.CheckForNullReferences();
        }

        //try
        //{
        for (int i = 0; i < AllPlayers.Count; i++)
        {
            AllSquads.AddRange(AllPlayers[i].Get_AllSquadLists());
            AllSquads_Combat.AddRange(AllPlayers[i].Get_AllCombatLists());
            AllSquads_BI.AddRange(AllPlayers[i].Get_AllSquadListsBI());
            AllSquads_Health.AddRange(AllPlayers[i].Get_AllHealthLists());

            if (i == AllPlayers.Count - 1)
            {
                for (int j = 0; j < AllSquads.Count; j++)
                {
                    AllSquads_AM.Add(new List<AggressionManager>());
                    AllSquads_Positions.Add(new List<Vector3>());

                    for (int z = 0; z < AllSquads_Combat[j].Count; z++)
                    {
                        try
                        {
                            AllSquads_AM[j].Add(AllSquads_Combat[j][z].GetAggressionManager());
                            AllSquads_Positions[j].Add(AllSquads[j][z].transform.position);
                        }
                        catch
                        {
                            try
                            {
                                AllSquads[j].RemoveAt(z);
                                AllSquads_Combat[j].RemoveAt(z);
                                AllSquads_BI[j].RemoveAt(z);
                                AllSquads_AM[j].RemoveAt(z);

                                if (AllSquads_Positions.Count > j)
                                {
                                    if (AllSquads_Positions[j].Count > z)
                                    {
                                        AllSquads_Positions[j].RemoveAt(z);
                                    }
                                }
                            }
                            catch
                            {
                                //We're probably in a cutscene
                            }
                        }
                    }
                }
            }
        }
        //}
        //catch
        //{
        //    Debug.LogError("Error! in 'Combat Manager': 'TrackAllEntities'. Cannot add new Entity Lists!");
        //}

        TrackingTimeout = false;
    }

    private List<List<Vector2>> GetAllEntityRelationLists()
    {
        List<List<Vector2>> AllPlayerRelationLists = new List<List<Vector2>>();
        for(int i = 0; i < AllPlayers.Count; i++)
        {
            AllPlayerRelationLists.Add(new List<Vector2>());
        }

        for (int j = 0; j < AllSquads_BI.Count; j++)
        {
            for (int z = 0; z < AllSquads_BI[j].Count; z++)
            {
                AllPlayerRelationLists[AllSquads_BI[j][z].OwnedByPlayer].Add(new Vector2(j, z));
            }
        }

        return AllPlayerRelationLists;
    }

    private void CheckForCombat()
    {
        if (!TrackingTimeout)
        {
            List<List<Vector2>> AllPlayerRelationLists = GetAllEntityRelationLists();
            for (int i = 0; i < AllPlayerRelationLists.Count; i++)
            {
                for (int j = 0; j < AllPlayerRelationLists[i].Count; j++)
                {
                    List<Transform> tmpEnemyInArea = new List<Transform>();
                    Vector2 tmp_ID = AllPlayerRelationLists[i][j];
                    Combat tmp_Combat = AllSquads_Combat[(int)tmp_ID[0]][(int)tmp_ID[1]];

                    if (tmp_Combat.CombatIsEnabled)
                    {
                        for(int e_list = 0; e_list < AllPlayerRelationLists.Count; e_list++)
                        {
                            if(e_list != i)
                            {
                                for (int e_entity = 0; e_entity < AllPlayerRelationLists[e_list].Count; e_entity++)
                                {
                                    Vector2 tmp_eID = AllPlayerRelationLists[e_list][e_entity];
                                    if (AllSquads[(int)tmp_eID[0]] != null)
                                    {
                                        if (AllSquads[(int)tmp_eID[0]][(int)tmp_eID[1]] != null)
                                        {
                                            if ((Vector3.Distance(AllSquads_Positions[(int)tmp_ID[0]][(int)tmp_ID[1]], AllSquads_Positions[(int)tmp_eID[0]][(int)tmp_eID[1]]) <= tmp_Combat.AggroRange) || AI_Aggression == eAIAggressionTypes.FullOnAggression)
                                            {
                                                if (!AllSquads_Health[(int)tmp_eID[0]][(int)tmp_eID[1]].GetIfIncapacitated())
                                                {
                                                    tmpEnemyInArea.Add(AllSquads[(int)tmp_eID[0]][(int)tmp_eID[1]].transform);
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        try
                        {
                            AllSquads_AM[(int)tmp_ID[0]][(int)tmp_ID[1]].SetEnemiesInArea(tmpEnemyInArea);
                        }
                        catch
                        {
                            Debug.LogWarning("CM: Cannot assign enemies in area, retrying..");
                        }
                    }
                }
            }
        }
        else
        {
            if (Mathf.Abs(TimeCurrent - PreviousCombatInvokeTime) >= MinInvokeDepthTime)
            {
                Invoke(nameof(CheckForCombat), MinInvokeDepthTime);
            }
        }

        PreviousCombatInvokeTime = TimeCurrent;
    }

    private void CheckForAbility()
    {
        if (!TrackingTimeout)
        {
            for (int mod_pos = 0; mod_pos < AllAppliedModifiers.Count; mod_pos++)
            {
                if(AllAppliedModifiers[mod_pos] != null) 
                { 
                    List<List<Vector2>> AllPlayerRelationLists = GetAllEntityRelationLists();
                    for (int i = 0; i < AllPlayerRelationLists.Count; i++)
                    {
                        for (int j = 0; j < AllPlayerRelationLists[i].Count; j++)
                        {
                            List<GameObject> tmpTargetInArea = new List<GameObject>();
                            Vector2 tmp_ID = AllPlayerRelationLists[i][j];

                            for (int e_list = 0; e_list < AllPlayerRelationLists.Count; e_list++)
                            {
                                for (int e_entity = 0; e_entity < AllPlayerRelationLists[e_list].Count; e_entity++)
                                {
                                    Vector2 tmp_eID = AllPlayerRelationLists[e_list][e_entity];
                                    try
                                    {
                                        if(AllSquads_Positions[(int)tmp_eID[0]][(int)tmp_eID[1]] != null && AllSquads[(int)tmp_eID[0]][(int)tmp_eID[1]].gameObject != null && AllAppliedModifiers[mod_pos] != null && AllSquads_BI[(int)tmp_eID[0]][(int)tmp_eID[1]] != null)
                                        {
                                            if (Vector3.Distance(AllSquads_Positions[(int)tmp_eID[0]][(int)tmp_eID[1]], AllAppliedModifiers[mod_pos].transform.position) <= AllAppliedModifiers[mod_pos].transform.localScale[0])//Edit me for non circular abilities
                                            {
                                                if (AllAppliedModifiers[mod_pos].CheckEntityOwner(null, AllSquads_BI[(int)tmp_eID[0]][(int)tmp_eID[1]]))
                                                {
                                                    tmpTargetInArea.Add(AllSquads[(int)tmp_eID[0]][(int)tmp_eID[1]].gameObject);
                                                }
                                                else
                                                {
                                                    AllAppliedModifiers[mod_pos].RemoveActiveObject(AllSquads[(int)tmp_eID[0]][(int)tmp_eID[1]], 0);
                                                }
                                            }
                                            else
                                            {
                                                AllAppliedModifiers[mod_pos].RemoveActiveObject(AllSquads[(int)tmp_eID[0]][(int)tmp_eID[1]], 0);
                                            } 
                                        }
                                    }
                                    catch
                                    {
                                        Debug.LogWarning("CM: AbilityChecker, can't compare positions likely the object is destroyed.");
                                    }
                                }

                                for (int newTarget = 0; newTarget < tmpTargetInArea.Count; newTarget++)
                                {
                                    AllAppliedModifiers[mod_pos].ApplyActiveObject(tmpTargetInArea[newTarget]);
                                }
                            }
                        }
                    }
                }
                else
                {
                    AllAppliedModifiers.RemoveAt(mod_pos);
                }
            }
        }
        else
        {
            if (Mathf.Abs(TimeCurrent - PreviousAbilityInvokeDepth) >= MinInvokeDepthTime)
            {
                Invoke(nameof(CheckForCombat), MinInvokeDepthTime);
            }
        }

        PreviousAbilityInvokeDepth = TimeCurrent;
    }
}
