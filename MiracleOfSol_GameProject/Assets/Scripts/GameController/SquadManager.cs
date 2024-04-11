using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SquadManager : MonoBehaviour
{
    //This is a simple storage device for all player owned squads
    private List<List<GameObject>> AllOwnedObjects = new List<List<GameObject>>();
    private List<List<BasicInfo>> AllObjBI = new List<List<BasicInfo>>();
    private List<List<EntityMovement>> AllObjEM = new List<List<EntityMovement>>();
    private List<List<GetIfSelected>> AllObjGIS = new List<List<GetIfSelected>>();
    private List<List<Combat>> AllObjCombat = new List<List<Combat>>();
    private List<List<Health>> AllObjHealth = new List<List<Health>>();
    private List<List<XPUnitManager>> AllObjXP = new List<List<XPUnitManager>>();

    private int CurrentEntityID = -1;
    private int CurrentSquadID = -1;

    private void Start()
    {
        InvokeRepeating(nameof(CheckForNullReferences), 5, 0.5f);
    }

    public void CheckForNullReferences()
    {
        for(int i = 0; i < AllOwnedObjects.Count; i++)
        {
            if (AllOwnedObjects[i] == null)
            {
                Destroy_EntityOrSquad(i, -1);
            }
            else if(AllOwnedObjects[i].Count == 0)
            {
                Destroy_EntityOrSquad(i, -1);
            }
            else
            {
                for (int j = 0; j < AllOwnedObjects[i].Count; j++)
                {
                    if (AllOwnedObjects[i][j] == null)
                    {
                        Destroy_EntityOrSquad(i, j);
                    }
                }
            }
        }
    }

    public List<GameObject> Get_SquadList(int AtPosition = 0)
    {
        return AllOwnedObjects[AtPosition];
    }

    public List<List<GameObject>> Get_AllSquadLists()
    {
        return AllOwnedObjects;
    }

    public List<List<BasicInfo>> Get_AllSquadListsBI()
    {
        return AllObjBI;
    }

    public List<List<Combat>> Get_AllCombatLists()
    {
        return AllObjCombat;
    }
    public List<List<Health>> Get_AllHealthLists()
    {
        return AllObjHealth;
    }

    public Health Get_SingleHealth(int[] IDs)
    {
        int SquadID = IDs[1];
        for (int entity = 0; entity < AllOwnedObjects[SquadID].Count; entity++)
        {
            if (AllObjHealth[SquadID][entity] != null)
            {
                return AllObjHealth[SquadID][entity];
            }
        }
        return null;
    }

    public Vector3 GetSquadAvgPosition(int SquadID)
    {
        Vector3 pos = new Vector3();

        for(int i = 0; i < AllOwnedObjects[SquadID].Count; i++)
        {
            pos += AllOwnedObjects[SquadID][i].transform.position;
        }

        pos.Scale(new Vector3(AllOwnedObjects[SquadID].Count, AllOwnedObjects[SquadID].Count, AllOwnedObjects[SquadID].Count));

        return pos;
    }

    public void Set_NewSquad(List<GameObject> NewSquad)
    {        
        AllOwnedObjects.Add(NewSquad);
        AllObjEM.Add(new List<EntityMovement>());
        AllObjBI.Add(new List<BasicInfo>());
        AllObjGIS.Add(new List<GetIfSelected>());
        AllObjCombat.Add(new List<Combat>());
        AllObjXP.Add(new List<XPUnitManager>());
        AllObjHealth.Add(new List<Health>());
    }

    public void Set_NewEntity(GameObject NewEntity, int Pos)
    {
        if (AllOwnedObjects.Count < Pos) { Set_NewSquad(new List<GameObject>()); }
        AllOwnedObjects[Pos].Add(NewEntity);
        AllObjEM[Pos].Add(NewEntity.GetComponent<EntityMovement>());
        AllObjBI[Pos].Add(NewEntity.GetComponent<BasicInfo>());
        AllObjGIS[Pos].Add(NewEntity.GetComponent<GetIfSelected>());
        AllObjCombat[Pos].Add(NewEntity.GetComponent<Combat>());
        AllObjXP[Pos].Add(NewEntity.GetComponent<XPUnitManager>());
        AllObjHealth[Pos].Add(NewEntity.GetComponent<Health>());
    }

    public void Destroy_EntityOrSquad(int EntityPos = -1, int SquadPos = 0)
    {
        if (EntityPos == -1)
        {
            try
            {
                AllOwnedObjects.RemoveAt(SquadPos);
                AllObjEM[SquadPos].RemoveAt(EntityPos);
                AllObjBI[SquadPos].RemoveAt(EntityPos);
                AllObjGIS[SquadPos].RemoveAt(EntityPos);
                AllObjCombat[SquadPos].RemoveAt(EntityPos);
                AllObjXP[SquadPos].RemoveAt(EntityPos);
                AllObjHealth[SquadPos].RemoveAt(EntityPos);
            }
            catch
            {
                Debug.LogWarning("SquadManager: Error Deleting Whole Squad At EntityPos{" + EntityPos + "} SquadPos{" + SquadPos + "}");
            }
        } //AllOwnedObjects[SquadPos] = null; } //
        else
        {
            try
            {
                if (AllOwnedObjects[SquadPos].Count >= EntityPos)
                {
                    AllOwnedObjects[SquadPos].RemoveAt(EntityPos);
                    AllObjEM[SquadPos].RemoveAt(EntityPos);
                    AllObjBI[SquadPos].RemoveAt(EntityPos);
                    AllObjGIS[SquadPos].RemoveAt(EntityPos);
                    AllObjCombat[SquadPos].RemoveAt(EntityPos);
                    AllObjXP[SquadPos].RemoveAt(EntityPos);
                    AllObjHealth[SquadPos].RemoveAt(EntityPos);
                }
            }
            catch
            {
                if (SquadPos != -1)
                {
                    Debug.LogWarning("SquadManager: Error Deleting Entity At EntityPos{" + EntityPos + "} SquadPos{" + SquadPos + "}");
                }
            }
        }
    }

    public int[] CalculateNewIDs(bool IsNewSquad = false) 
    {
        CurrentEntityID++;
        if (IsNewSquad) { CurrentSquadID++; Set_NewSquad(new List<GameObject>()); } 
        return new int[2] { CurrentEntityID, CurrentSquadID };
    }

    private bool CheckEntityIsNotTarget(int[] IDs, int CurrentEntity)
    {
        int SquadID = IDs[1]; int CasterEntityID = IDs[0];
        if (AllObjBI[SquadID][CurrentEntity].GetIDs()[0] == CasterEntityID)
        {
            return false;
        }
        else { return true; }        
    }
    
    public void SquadAIStatus_SetIsHeal(int[] IDs, bool Status)
    {
        int SquadID = IDs[1];
        for (int entity = 0; entity < AllOwnedObjects[SquadID].Count; entity++)
        {
            if (AllObjEM[SquadID][entity] != null)
            {
                AllObjHealth[SquadID][entity].Set_AI_IsTryingToHeal(Status);
            }
        }
    }

    public void SquadOrder_SetMoveDestination(int[] IDs, Vector3 NewPos, bool DoOrderForCaster = false)
{
        int SquadID = IDs[1];
        for (int entity = 0; entity < AllOwnedObjects[SquadID].Count; entity++)
        {
            if (AllObjEM[SquadID][entity] != null)
            {
                if (DoOrderForCaster)
                {
                    AllObjEM[SquadID][entity].SetMoveDestination(NewPos);
                }
                else if (CheckEntityIsNotTarget(IDs, entity))
                {
                    AllObjEM[SquadID][entity].SetMoveDestination(NewPos);
                }
            }
        }        
    }

    public void SquadOrder_SetAttackTarget(int[] IDs, Transform NewTarget, bool DoOrderForCaster = false)
    {
        int SquadID = IDs[1];
        for (int entity = 0; entity < AllOwnedObjects[SquadID].Count; entity++)
        {
            if (AllObjEM[SquadID][entity] != null)
            {
                if (DoOrderForCaster)
                {
                    AllObjEM[SquadID][entity].SetAttackTarget(NewTarget);
                }
                else if (CheckEntityIsNotTarget(IDs, entity))
                {
                    AllObjEM[SquadID][entity].SetAttackTarget(NewTarget);
                }
            }
        }
    }

    public void SquadOrder_SetCaptureTarget(int[] IDs, CapturePoint NewCap, bool DoOrderForCaster = false, bool ForceAMove = false)
    {
        int SquadID = IDs[1];
        for (int entity = 0; entity < AllOwnedObjects[SquadID].Count; entity++)
        {
            if (AllObjEM[SquadID][entity] != null)
            {
                if (DoOrderForCaster)
                {
                    AllObjEM[SquadID][entity].SetCaptureTarget(NewCap, ForceAMove);
                }
                else if (CheckEntityIsNotTarget(IDs, entity))
                {
                    AllObjEM[SquadID][entity].SetCaptureTarget(NewCap, ForceAMove);
                }
            }
        }
    }
    public bool SquadOrder_GetCaptureTarget(int[] IDs)
    {
        int SquadID = IDs[1];
        for (int entity = 0; entity < AllOwnedObjects[SquadID].Count; entity++)
        {
            if (AllObjEM[SquadID][entity] != null)
            {
                return AllObjEM[SquadID][entity].GetCaptureStatus();
            }
        }

        return false;
    }

    public void SquadOrder_StopCommands(int[] IDs, bool DoOrderForCaster = false)
    {
        try
        {
            int SquadID = IDs[1];
            for (int entity = 0; entity < AllOwnedObjects[SquadID].Count; entity++)
            {
                if (AllObjEM[SquadID][entity] != null)
                {
                    if (DoOrderForCaster)
                    {
                        AllObjEM[SquadID][entity].StopCommands();
                    }
                    else if (CheckEntityIsNotTarget(IDs, entity))
                    {
                        AllObjEM[SquadID][entity].StopCommands();
                    }
                }
            }
        }        
        catch
        {
            Debug.LogWarning("ERROR! In SquadManager/SquadOrder_StopCommands. Cannot do logic");
        }
    }

    public void SquadSelection_SetNeutralStatus(int[] IDs)
    {
        try
        {
            int SquadID = IDs[1];
            for (int entity = 0; entity < AllOwnedObjects[SquadID].Count; entity++)
            {
                if (AllObjGIS[SquadID][entity] != null)
                {
                    if (CheckEntityIsNotTarget(IDs, entity))
                    {
                        AllObjGIS[SquadID][entity].ResetSelectionStatus();
                    }
                }
            }
        }
        catch
        {
            Debug.LogWarning("ERROR! In SquadManager/SquadSelection_SetNeutralStatus. Cannot do logic");
        }
    }

    public void SquadSelection_SetSelectedStatus(int[] IDs)
    {
        try
        {
            int SquadID = IDs[1];
            for (int entity = 0; entity < AllOwnedObjects[SquadID].Count; entity++)
            {
                if (AllObjGIS[SquadID][entity] != null)
                {
                    if (CheckEntityIsNotTarget(IDs, entity))
                    {
                        AllObjGIS[SquadID][entity].SetSelectedStatus();
                    }
                }
            }
        }
        catch
        {
            Debug.LogWarning("ERROR! In SquadManager/SquadSelection_SetSelectedStatus. Cannot do logic");
        }
    }

    public void SquadSelection_SetHoverStatus(int[] IDs)
    {
        try
        {
            int SquadID = IDs[1];
            for (int entity = 0; entity < AllOwnedObjects[SquadID].Count; entity++)
            {
                if (AllObjGIS[SquadID][entity] != null)
                {
                    if (CheckEntityIsNotTarget(IDs, entity))
                    {
                        AllObjGIS[SquadID][entity].SetHoverSelectionStatus();
                    }
                }
            }
        }
        catch
        {
            Debug.LogWarning("ERROR! In SquadManager/SquadSelection_SetHoverStatus. Cannot do logic");
        }
    }

    public void SquadMove_TeleportToLocation(int[] IDs, Vector3 TPLocation)
    {
        Vector3 Offset = new Vector3(0, 0, 0);
        SquadOrder_StopCommands(IDs, true);

        int SquadID = IDs[1];
        for (int entity = 0; entity < AllOwnedObjects[SquadID].Count; entity++)
        {
            AllOwnedObjects[SquadID][entity].transform.position = TPLocation + Offset;

            if(entity % 4 == 0)
            {
                Offset = new Vector3(0, 0, Offset[2] + 1);
            }
            else
            {
                Offset += new Vector3(1, 0, 0);
            }
        }
    }

    public void ProvideXPToHero(int HeroPos, int MainXP, int SecondaryXP)
    {
        for(int i = 0; i < AllObjXP.Count; i++)
        {
            if(AllObjBI[i][0].EBPs.PositionInLvlHierarchy != HeroPos)
            {
                //print("SECONDARY: " + AllObjBI[i][0]);
                AllObjXP[i][0].AddXP(SecondaryXP);
            }
            else
            {
                //print("PRIMARY: " + AllObjBI[i][0]);
                AllObjXP[i][0].AddXP(MainXP);
            }
        }
    }

    public void SetXPToPlayerPrefs()
    {
        int[] tmpHeroLevel = PlayerPrefsX.GetIntArray("HeroLevelStatus", 0, 4);
        int[] tmpHeroUnusedPerks = PlayerPrefsX.GetIntArray("Hero_UnusedPerks", 0, 4);

        for (int i = 0; i < AllObjXP.Count; i++)
        {
            tmpHeroLevel[i] = AllObjXP[i][0].CurrentXPLevel;
            tmpHeroUnusedPerks[i] = AllObjXP[i][0].CurrentUnusedPerks;
        }

        PlayerPrefsX.SetIntArray("HeroLevelStatus", tmpHeroLevel);
        PlayerPrefsX.SetIntArray("HeroUnusedPerks", tmpHeroUnusedPerks);
    }
}
