using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health : MonoBehaviour
{
    public GameObject[] OnDieVFX;
    public GameObject[] OnIncapacitateVFX;
    public GameObject[] OnHitVFX;
    public Transform[] SkeletonBones;

    public HealthBar HP_Bar;
    public HealthBar Armour_Bar;
    public GameObject DamagePrompt;
    public Transform AnchorDmgPrompt;
    public UIChangeImage UI_ArmourTypeIndicator;

    public bool CanDie = true;
    public bool HealthIsArmour = false;
    public bool CanTakeHealthDamage = true;
    public bool CanPassiveHeal = true;
    public bool InCombat_CanPassiveHealArmour = false;


    private float CurrentHP;
    private float CurrentHPRegen;
    private float CurrentHPDegen;
    private float CurrentArmour;
    private float CurrentArmourRegen;
    private float CurrentArmourDegen;

    private float Mod_AddMaxHP;
    private float Mod_AddHPRegen;
    private float Mod_AddDeathPercentHP;
    private float Mod_AddMaxArmour;
    private float Mod_AddArmourRegen;
    private float Mod_AddHPDegen;
    private float Mod_AddArmourDegen;

    private float Mod_MultiMaxHP = 1;
    private float Mod_MultiHPRegen = 1;
    private float Mod_MultiDeathPercentHP = 1;
    private float Mod_MultiMaxArmour = 1;
    private float Mod_MultiArmourRegen = 1;
    private float Mod_MultiHPDegen = 1;
    private float Mod_MultiArmourDegen = 1;

    private float Mod_MultiReceivedRangedDamage = 1;
    private float Mod_MultiReceivedMeleeDamage = 1;

    private bool AIStatus_IsTryingToHeal = false;
    private bool IsIncapacitated = false;
    private bool HasDoneDieLogic = false;

    private BasicInfo BI;
    private BasicFunctions BF;
    private EBP_Info EBPs;
    private GameInfo GI;
    private Combat C_Manager;
    private EntityMovement EM;
    private CustomPhysics CP;
    private GetIfSelected GIS;
    private VoiceLineManager VLM;
    private ExtendedUnitStatusUI EUS_UI;

    private int LastCombatUnit = -1;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindWithTag("GameController").TryGetComponent(out GI);
        GI.TryGetComponent(out BF);
        GI.UI_Canvas.TryGetComponent(out EUS_UI);

        gameObject.TryGetComponent(out BI);
        gameObject.TryGetComponent(out C_Manager);
        gameObject.TryGetComponent(out GIS);
        gameObject.TryGetComponent(out VLM);

        GI.TryGetComponent(out EBP_BackupData tmpBackup);
        tmpBackup.ACTIVE_AllEBP[BI.EBPs.PositionInGMData].TryGetComponent(out EBPs);

        //Revive stuff
        gameObject.TryGetComponent(out GIS);
        gameObject.TryGetComponent(out CP);
        gameObject.TryGetComponent(out EM);
        gameObject.TryGetComponent(out VLM);

        CurrentHP = EBPs.MaxHP;
        CurrentHPRegen = EBPs.HPRegen;
        CurrentHPDegen = EBPs.HPDegen;

        CurrentArmour = EBPs.MaxArmour;
        CurrentArmourRegen = EBPs.ArmourRegen;
        CurrentArmourDegen = EBPs.ArmourDegen;

        HP_Bar.CurrentHP = CurrentHP;
        EUS_UI.HP_Bar.CurrentHP = CurrentHP;

        HP_Bar.MaxHP = CurrentHP;
        EUS_UI.HP_Bar.MaxHP = CurrentHP;

        Armour_Bar.CurrentHP = CurrentArmour;
        EUS_UI.Armour_Bar.CurrentHP = CurrentArmour;

        Armour_Bar.MaxHP = CurrentArmour;
        EUS_UI.Armour_Bar.MaxHP = CurrentArmour;

        UI_SetIndicator();
        InvokeRepeating("PassiveRegen", 1, 1);
    }

    public void ForceKillSelf()
    {
        OrderDie();
    }

    public bool GetIfIncapacitated()
    {
        return IsIncapacitated;
    }

    public bool Get_AI_IsTryingToHeal()
    {
        return AIStatus_IsTryingToHeal;
    }
    
    public void Set_AI_IsTryingToHeal(bool HealStatus)
    {
        AIStatus_IsTryingToHeal = HealStatus;
    }

    private void PassiveRegen()
    {
        float[] CurrentLuxModifiers = CheckCoverStatus();
        CurrentHP -= (CurrentHPDegen + Mod_AddHPDegen + CurrentLuxModifiers[1]) * Mod_MultiHPDegen;
        CurrentArmour -= (CurrentArmourDegen + Mod_AddArmourDegen) * Mod_MultiArmourDegen;

        if (CanPassiveHeal)
        {
            bool IsOutOfCombatLongEnough = false;
            if (C_Manager != null)
            {
                if (C_Manager.GetTimeSinceCombat() >= EBPs.TimeOutOfCombatForBonuses) { IsOutOfCombatLongEnough = true; }
            }

            if (CurrentHP < GetMaxHP())
            {
                CurrentHP += (CurrentHPRegen + Mod_AddHPRegen + CurrentLuxModifiers[0]) * Mod_MultiHPRegen;

                if (IsOutOfCombatLongEnough) { CurrentHP += EBPs.OutOfCombat_HPRegen_Bonus; }

                HP_Bar.SetHP(CurrentHP);

                CheckHealthOverMax(true, false);
            }
            else
            {
                CurrentHP = GetMaxHP();
            }

            try
            {
                if (GIS.IsPrimarySelection && GIS.GetSelectedStatus() == "Selected")
                {
                    EUS_UI.HP_Bar.MaxHP = GetMaxHP();
                    EUS_UI.HP_Bar.SetHP(CurrentHP);
                }
            }
            catch
            {
                Debug.LogWarning("ERROR! In Health/PassiveRegen. Error assigning EUS_UI, probably selection issue.");
            }

            if (C_Manager == null)
            {
                PassiveHealArmour(IsOutOfCombatLongEnough);
            }
            else
            {
                if (!C_Manager.IsInCombat)
                {
                    PassiveHealArmour(IsOutOfCombatLongEnough);
                }
                else if (InCombat_CanPassiveHealArmour)
                {
                    PassiveHealArmour(IsOutOfCombatLongEnough);
                }
            }
        }
    }

    private void PassiveHealArmour(bool IsOutOfCombatLongEnough)
    {
        if (CurrentArmour < GetMaxArmour())
        {
            if (IsOutOfCombatLongEnough) { CurrentArmour += EBPs.OutOfCombat_ArmourRegen_Bonus; }
            CurrentArmour += (CurrentArmourRegen + Mod_AddArmourRegen) * Mod_MultiArmourRegen;

            CheckHealthOverMax(false);
            Armour_Bar.SetHP(CurrentArmour);
        }

        try
        {
            if (GIS.IsPrimarySelection && GIS.GetSelectedStatus() == "Selected")
            {
                EUS_UI.Armour_Bar.MaxHP = GetMaxArmour();
                EUS_UI.Armour_Bar.SetHP(CurrentArmour);
            }
        }
        catch
        {
            Debug.LogWarning("ERROR! In Health/PassiveHealArmour. Cannot set armour UI. Probably i am an enemy.");
        }
    }

    public void SetArmourType(int NewArmourType)
    {
        EBPs.PiercingArmourType = NewArmourType;
    }

    public int GetArmourType()
    {
        return EBPs.PiercingArmourType;
    }

    public float GetMaxHP()
    {
        return (EBPs.MaxHP + Mod_AddMaxHP) * Mod_MultiMaxHP;
    }

    public float GetMaxArmour()
    {
        return (EBPs.MaxArmour + Mod_AddMaxArmour) * Mod_MultiMaxArmour;
    }

    public float GetMinHP()
    {
        return EBPs.MaxHP * ((EBPs.HealthDeathPercent + Mod_AddDeathPercentHP) * Mod_MultiDeathPercentHP);
    }

    public float GetCurrentHP()
    {
        return CurrentHP;
    }

    public float GetCurrentArmour()
    {
        return CurrentArmour;
    }

    public float GetCurrentHPRegen()
    {
        return CurrentHPRegen;
    }

    public float GetCurrentFARegen()
    {
        return CurrentArmourRegen;
    }

    public float GetCurrentHP_AsPercentOfMax()
    {
        return CurrentHP / EBPs.MaxHP;
    }

    public float GetCurrentArmour_AsPercentOfMax()
    {
        return CurrentArmour / EBPs.MaxArmour;
    }

    public void UpdateCurrentHealth(int TakeDamageFromTarget = -1, bool IsMultiply = false, float HPChange = 0, float ArmourChange = 0, bool ChangeHP = true, bool ChangeArmour = true, bool IgnoreArmour = false, bool IsMeleeAttack = false, bool IsRangedAttack = false, bool IgnoreHealthModifiers = true, bool IgnoreBaseMeleeResist = true, bool IgnoreBaseRangedResist = true, bool IgnoreLuxCover = true, bool IgnoreRawCover = true)
    {
        if (CanTakeHealthDamage)
        {
            LastCombatUnit = TakeDamageFromTarget;
            if (HPChange < 0 && Random.Range(0f,1f) >= 0.6f && OnHitVFX.Length != 0 && SkeletonBones.Length != 0)
            {
                GameObject tmpBloodSplat = Instantiate(OnHitVFX[Random.Range(0, OnHitVFX.Length)]);
                tmpBloodSplat.transform.position = SkeletonBones[Random.Range(0, SkeletonBones.Length)].transform.position;
                tmpBloodSplat.transform.rotation = new Quaternion();
            }

            if (!IgnoreHealthModifiers)
            {
                if (IsRangedAttack) { HPChange *= Mod_MultiReceivedRangedDamage; ArmourChange *= Mod_MultiReceivedRangedDamage; }
                if (IsMeleeAttack) { HPChange *= Mod_MultiReceivedMeleeDamage; ArmourChange *= Mod_MultiReceivedMeleeDamage; }
            }

            if (!IgnoreBaseRangedResist && IsRangedAttack) { HPChange *= EBPs.BaseRangedResist; ArmourChange *= EBPs.BaseRangedResist; }
            if (!IgnoreBaseMeleeResist && IsMeleeAttack) { HPChange *= EBPs.BaseMeleeResist; ArmourChange *= EBPs.BaseMeleeResist; }

            if (HealthIsArmour) { ChangeHP = false; }

            if (!IsMultiply)
            {   //                 |Ignore Health Damage if weapon does not pierce armour and entity has armour
                if (ChangeArmour) { CurrentArmour += ArmourChange; CheckHealthOverMax(false, true); if (HealthIsArmour) { CheckIfDie(); } };
                if (ChangeHP) { if (!IgnoreArmour && CurrentArmour > GetMaxArmour() / 10 && HPChange < 0) { HPChange = 0; } else { CurrentHP += HPChange; CheckHealthOverMax(true, false); if (!HealthIsArmour) { CheckIfDie(); } } };
            }
            else
            {
                if (ChangeArmour) { CurrentArmour *= ArmourChange; CheckHealthOverMax(false, true); if (HealthIsArmour) { CheckIfDie(); } };
                if (ChangeHP) { if (!IgnoreArmour || HPChange < 0) { Debug.Log("Armour Save!"); HPChange = 0; } else { CurrentHP *= HPChange; CheckHealthOverMax(true, false); if (!HealthIsArmour) { CheckIfDie(); } } };
            }

            if(HPChange < 0 || ArmourChange < 0)
            {
                C_Manager.IsInCombat = true;
                C_Manager.ResetTimeSinceLastCombat();
            }

            CheckHealthOverMax(true, true);
            CreateDamagePrompt(HPChange, ArmourChange);
        }
    }

    public void UpdateHealthMods(bool IsMultiply = false, float HPChange = 0, float HPRegenChange = 0, float DeathPercent = 0, float ArmourChange = 0, float ArmourRegenChange = 0, float HPDegenChange = 0, float ArmourDegenChange = 0)
    {
        Debug.Log("Entity: '" + gameObject.name + "' Health Modifiers Changed ");

        if (!IsMultiply)
        {
            Mod_AddMaxHP += HPChange;
            Mod_AddHPRegen += HPRegenChange;
            Mod_AddDeathPercentHP += DeathPercent;
            Mod_AddHPDegen += HPDegenChange;

            Mod_AddMaxArmour += ArmourChange;
            Mod_AddArmourRegen += ArmourRegenChange;
            Mod_AddArmourDegen += ArmourDegenChange;

            HP_Bar.MaxHP = GetMaxHP();
            Armour_Bar.MaxHP = GetMaxArmour();
        }
        else
        {
            //Mod_MultiMaxHP += HPChange;
            Mod_MultiMaxHP = HPChange;
            Mod_MultiHPRegen = ArmourRegenChange;
            Mod_MultiDeathPercentHP = DeathPercent;
            Mod_MultiHPDegen = HPDegenChange;

            Mod_MultiMaxArmour = ArmourChange;
            Mod_MultiArmourRegen = ArmourRegenChange;
            Mod_MultiArmourDegen = ArmourDegenChange;

            HP_Bar.MaxHP = GetMaxHP();
            Armour_Bar.MaxHP = GetMaxArmour();
        }

        CheckHealthOverMax(true, true);
    }
    
    public void UpdateBlueprint(bool IsMultiply = false, float HPChange = 0, float HPRegenChange = 0, float DeathPercent = 0, float ArmourChange = 0, float ArmourRegenChange = 0, float HPDegen = 0, float ArmourDegen = 0)
    {
        //Debug.Log("Blueprint For: '" + EBPs.name + "' Health Modifiers Changed ");

        if (!IsMultiply)
        {
            EBPs.MaxHP += HPChange;
            EBPs.HPRegen += HPRegenChange;
            EBPs.HealthDeathPercent += DeathPercent;
            EBPs.HPDegen += HPDegen;

            EBPs.MaxArmour += ArmourChange;
            EBPs.ArmourRegen += ArmourRegenChange;
            EBPs.ArmourDegen += ArmourDegen;
        }
        else
        {
            EBPs.MaxHP = HPChange;
            EBPs.HPRegen = HPRegenChange;
            EBPs.HealthDeathPercent = DeathPercent;
            EBPs.HPDegen = HPDegen;

            EBPs.MaxArmour = ArmourChange;
            EBPs.ArmourRegen = ArmourRegenChange;
            EBPs.ArmourDegen = ArmourDegen;
        }

        CheckHealthOverMax(true, true);
    }

    public void MasterFunction_UpdateAllHealthModifiers(int TakeDamageFromTarget = -1, bool IsChangeRawValues = true, string ModApplictionType = "ENTITY", string ModMathType = "ALL",
                                float AddMaxHP = 0, float AddCurrentHP = 0, float AddHPRegen = 0, float AddDeathPercent = 0,
                                float AddMaxArmour = 0, float AddCurrentArmour = 0, float AddArmourRegen = 0,
                                float AddHPDegen = 0, float AddArmourDegen = 0)
    {
        ModMathType = ModMathType.ToUpper();
        ModApplictionType = ModApplictionType.ToUpper();

        if (ModMathType == "ADDITION" || ModMathType == "ALL")
        {
            if (IsChangeRawValues)
            {
                UpdateCurrentHealth(TakeDamageFromTarget, false, AddCurrentHP, AddCurrentArmour);
            }

            if (ModApplictionType == "ENTITY")
            {
                UpdateHealthMods(false, AddMaxHP, AddHPRegen, AddDeathPercent, AddMaxArmour, AddArmourRegen, AddHPDegen, AddArmourDegen);
            }
            else if(ModApplictionType == "ENTITYTYPE")
            {
                UpdateBlueprint(false, AddMaxHP, AddHPRegen, AddDeathPercent, AddMaxArmour, AddArmourRegen, AddHPDegen, AddArmourDegen);
            }
        }
        
        if (ModMathType == "MULTIPLICATION" || ModMathType == "ALL") 
        {
            if (AddMaxHP == 0) { AddMaxHP = Mod_MultiMaxHP; }
            if (AddHPRegen == 0) { AddHPRegen = Mod_MultiHPRegen; }
            if (AddDeathPercent == 0) { AddDeathPercent = Mod_MultiDeathPercentHP; }
            if (AddMaxArmour == 0) { AddMaxArmour = Mod_MultiMaxArmour; }
            if (AddArmourRegen == 0) { AddArmourRegen = Mod_MultiArmourRegen; }

            if (IsChangeRawValues)
            {
                UpdateCurrentHealth(TakeDamageFromTarget, true, AddCurrentHP, AddCurrentArmour);
            }

            if (ModApplictionType == "ENTITY")
            {
                UpdateHealthMods(true, AddMaxHP, AddHPRegen, AddDeathPercent, AddMaxArmour, AddArmourRegen, AddHPDegen, AddArmourDegen);
            }
            else if (ModApplictionType == "ENTITYTYPE")
            {
                UpdateBlueprint(true, AddMaxHP, AddHPRegen, AddDeathPercent, AddMaxArmour, AddArmourRegen, AddHPDegen, AddArmourDegen);
            }
        }
    }
    
    public void UI_SetIndicator(bool ForceExtendedUIChange = false)
    {
        try
        {
            UI_ArmourTypeIndicator.SetNewImage(GI.UI_ArmourTypeIdentifiers[GetArmourType()]);

            if ((GIS.IsPrimarySelection && GIS.GetSelectedStatus() == "Selected") || ForceExtendedUIChange)
            {
                EUS_UI.UI_SetIndicator(GI, GetArmourType(), C_Manager.UI_DamageTypeIndicator.GetCurrentImage(), BI.EBPs.EntityQuote);
                EUS_UI.CurrentlySelectedEntityImg.sprite = BI.EBPs.UI_Icon;

                EUS_UI.HP_Bar.MaxHP = GetMaxHP();
                EUS_UI.HP_Bar.SetHP(CurrentHP);

                EUS_UI.Armour_Bar.MaxHP = GetMaxArmour();
                EUS_UI.Armour_Bar.SetHP(CurrentArmour);
            }
        }
        catch
        {
            Debug.LogWarning("ERROR! In Health/UI_SetIndicator. Cannot set. Possibly due to selecting while spawning");
        }
    }

    private void CreateDamagePrompt(float HP_PromptValue, float Armour_PromptValue)
    {
        HP_PromptValue = Mathf.Round(HP_PromptValue * 100) / 100;
        Armour_PromptValue = Mathf.Round(Armour_PromptValue * 100) / 100;

        GameObject Prompt = Instantiate(DamagePrompt, AnchorDmgPrompt.position, new Quaternion(), HP_Bar.transform.parent);
        Prompt.TryGetComponent(out UIOnHitSetDmgValues tmpUIHolder);
        tmpUIHolder.SetValues(HP_PromptValue, Armour_PromptValue);
    }

    //This was previously commented due to a weird bug where unit not taking dmg, make sure it's working
    private void CheckHealthOverMax(bool CheckHealth = true, bool CheckArmour = true)
    {
        if (CheckHealth)
        {
            if (CurrentHP >= GetMaxHP())
            {
                CurrentHP = GetMaxHP();
            }else if (CurrentHP < 0) { CurrentHP = 0; }

            HP_Bar.SetHP(CurrentHP);
        }

        if (CheckArmour)
        {
            if (CurrentArmour >= GetMaxArmour())
            {
                CurrentArmour = GetMaxArmour();
            }else if(CurrentArmour < 0) { CurrentArmour = 0; }

            Armour_Bar.SetHP(CurrentArmour);
        }
    }

    private float[] CheckCoverStatus()
    {
        float tmp_HPRegenBonus = 0;
        float tmp_HPDegenBonus = 0;

        if (BF.FindIfArrayContainsTrueOrFalse(BI.GetCoverStatus(), true))
        {
            int CurrentlyActiveCover = BF.ReturnTrueFalsePositionInArray(BI.GetCoverStatus(), true);
            try
            {
                tmp_HPRegenBonus = GI.GetValueOfLuxCover(GI.HPRegenLuxCoverPos, CurrentlyActiveCover, BI.EBPs.LuxCoverArmourType);
                tmp_HPDegenBonus = GI.GetValueOfLuxCover(GI.HPDegenLuxCoverPos, CurrentlyActiveCover, BI.EBPs.LuxCoverArmourType);
            }
            catch
            {

            }
        }

        return new float[2] { tmp_HPRegenBonus, tmp_HPDegenBonus };
    }

    private void CheckIfDie()
    {
        float TempHealth = GetMinHP();

        if (CurrentHP <= TempHealth && !HealthIsArmour)
        {
            OrderDie();
        }
        else if(CurrentArmour <= TempHealth && HealthIsArmour)
        {
            OrderDie();
        }
    }

    private void OrderDie()
    {
        if (CanDie && !HasDoneDieLogic)
        {
            DoDeathExplosions();
            C_Manager.ForceKillAllWeaponMods();
            HasDoneDieLogic = true;
            if (VLM != null && (!IsIncapacitated || !BI.EBPs.CanBeRevived)) { VLM.PlayVoiceLineOfType("DIE"); }

            if (!BI.EBPs.CanBeRevived)
            {
                foreach(AbilityCaster AC in gameObject.GetComponents<AbilityCaster>())
                {
                    AC.KillAllAbilitiesExternally();
                }

                foreach (GameObject tmpObj in OnDieVFX)
                {
                    GameObject tmpBloodSplat = Instantiate(OnDieVFX[Random.Range(0, OnDieVFX.Length)]);
                    tmpBloodSplat.transform.position = SkeletonBones[Random.Range(0, SkeletonBones.Length)].transform.position;
                    tmpBloodSplat.transform.rotation = new Quaternion();
                }

                int[] tmp_IDs = BI.GetIDs();

                if (BI.OwnedByPlayer != 0 && LastCombatUnit != -1)
                {
                    GI.AllPlayers_SM[0].ProvideXPToHero(LastCombatUnit, EBPs.PrimaryXP, EBPs.SecondaryXP);
                }

                if(BI.EBPs.WeaponPartsRewardMax > 0)
                {
                    Actions.OnAddWeaponPartsAtLocation.InvokeAction(Random.Range(BI.EBPs.WeaponPartsRewardMin, BI.EBPs.WeaponPartsRewardMax), transform.position);
                }

                GI.AllPlayers[BI.OwnedByPlayer].TryGetComponent(out SquadManager tmpSM);
                tmpSM.Destroy_EntityOrSquad(tmp_IDs[0], tmp_IDs[1]);

                Invoke(nameof(FinaliseDie), 0.5f);
            }
            else if(!IsIncapacitated)
            {
                DoDeathExplosions();
                foreach (GameObject tmpObj in OnDieVFX)
                {
                    GameObject tmpBloodSplat = Instantiate(OnIncapacitateVFX[Random.Range(0, OnIncapacitateVFX.Length)]);
                    tmpBloodSplat.transform.position = SkeletonBones[Random.Range(0, SkeletonBones.Length)].transform.position;
                    tmpBloodSplat.transform.rotation = new Quaternion();
                }

                IsIncapacitated = true;
                GameObject tmpRevive = Instantiate(GI.RevivalCapturePoint);
                tmpRevive.transform.parent = transform;
                tmpRevive.TryGetComponent(out RevivalManager tmpRev);
                tmpRev.SetupRevivalManger(this, BI.EBPs.PositionInLvlHierarchy);

                C_Manager.enabled = false;
                GIS.ForceNoSelection = true;
                EM.StopCommands(false, false, false);
                EM.enabled = false;
                CP.ApplyAForce(new Vector3(0, 0, 0), 0, true, transform);
                CP.enabled = false;
            }
        }
    }

    private void DoDeathExplosions()
    {
        DeathExplosionManager.External_CheckForLifetimes();
        List<GameObject> tmpDE = DeathExplosionManager.GetEntityDeathExplosions(gameObject.name);

        for(int i = 0; i < tmpDE.Count; i++)
        {
            GameObject tmpObjSpawn = Instantiate(tmpDE[i]);
            tmpObjSpawn.transform.position = transform.position;
            tmpObjSpawn.name = "DeathExplosion of: " + gameObject.name + " : " + Time.time + " : " + i;
            tmpObjSpawn.TryGetComponent(out DeathExplosion _tmpDE);
            _tmpDE.DoDeathExplosion(BI.EBPs.PositionInLvlHierarchy, BI.OwnedByPlayer, null);
        }
    }

    private void FinaliseDie()
    {
        Destroy(gameObject);
    }

    public void Revive(int RevivedByHero)
    {
        if (IsIncapacitated)
        {
            HasDoneDieLogic = false;
            VLM.PlaySpecificRevivedByLine(RevivedByHero);
            CurrentHP = GetMaxHP() / 2;
            CurrentArmour = GetMaxArmour() / 2;
            IsIncapacitated = false;

            C_Manager.enabled = false;
            GIS.ForceNoSelection = false;
            EM.enabled = true;
            CP.enabled = true;
        }
    }
}
