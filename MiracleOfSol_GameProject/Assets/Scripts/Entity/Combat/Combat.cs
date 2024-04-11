using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat : MonoBehaviour
{
    public GameObject[] AllWeaponArt;
    public GameObject[] AllWeaponArtSecondary;

    public GameObject[] VFX_Muzzle;
    public Transform[] VFX_MuzzleBone;
    public GameObject AggroShape;
    public List<WeaponInformation> AllWeapons = new List<WeaponInformation>();
    public List<WeaponInformation> AllWeapons01 = new List<WeaponInformation>();
    public List<WeaponInformation> AllWeapons02 = new List<WeaponInformation>();
    public List<WeaponInformation> AllWeapons03 = new List<WeaponInformation>();
    public List<WeaponInformation> AllWeapons04 = new List<WeaponInformation>();
    public List<WeaponInformation> AllWeapons05 = new List<WeaponInformation>();
    public List<WeaponInformation> AllWeapons06 = new List<WeaponInformation>();
    public List<WeaponInformation> AllWeapons07 = new List<WeaponInformation>();
    public List<WeaponInformation> AllWeapons08 = new List<WeaponInformation>();
    public List<WeaponInformation> AllWeapons09 = new List<WeaponInformation>();
    public List<List<WeaponInformation>> AllWeaponGroups = new List<List<WeaponInformation>>();
    public List<bool> AllWeaponGroups_ActiveStatus = new List<bool>();
    public float AggroRange = 40;
    public bool HasBeenAggro = false;
    public GameObject Target;
    public GameObject DesiredTarget;
    public UIChangeImage UI_DamageTypeIndicator;
    public float MaxAccuracyBonusFromVerticality = 0.25f;
    public float MinAccuracyBonusFromVerticality = 0.25f;
    public float VerticalityMax = 10;

    public bool CombatIsEnabled = true;
    public bool IsInCombat = false;
    public bool IsInMelee = false;
    public bool IsMeleeCharging = false;
    public bool IsMeleeLeap = false;

    public bool HasMeleeCharge = true;
    public float MeleeChargeRange = 20;
    public float MeleeCharge_SpeedBonus = 1.2f;

    public bool HasMeleeLeap = false;
    public bool MeleeLeapIsTeleport = false;
    public float MeleeLeapTimeInAir = 1.75f;
    public AnimationCurve MeleeLeapYHeight;
    public float MeleeLeapRange = 15;
    public float MeleeLeapCooldown = 30;
    public float MeleeLeapSetupTime = 1;
    public float MeleeLeapBreakdownTime = 1;
    public GameObject[] ModApplyOnLeapEnd;
    public float MeleeLeapModDuration = 1;

    public float BaseReloadTimeMultiplier = 1;
    public float BaseBurstDurationMultiplier = 1;
    public float BaseBurstCountMultiplier = 1;
    public float BaseWeaponDamageMultiplier = 1;

    private float CurrentLeapCooldown = 0;

    private bool IsMoving = false;
    private float TimeSinceLastCombat;
    private float ModStatus_Multi_AllWeaponBurst = 1;
    private float ModStatus_Add_AllWeaponBurst = 0;


    private GameInfo GI;
    //private LineRenderer LR; //Remove me later on!
    private EntityMovement EM;
    private AudioSourceController ASC;
    private AggressionManager AM;
    private CombatManager CM;

    private List<GameObject> AppliedModsHolder_Target = new List<GameObject>();
    private List<GameObject> AppliedModsHolder_Caster = new List<GameObject>();

    private List<float> AppliedModsLifetime_Target = new List<float>();
    private List<float> AppliedModsMaxLifetime_Target = new List<float>();
    private List<float> AppliedModsLifetime_Caster = new List<float>();
    private List<float> AppliedModsMaxLifetime_Caster = new List<float>();

    private List<GameObject> ProjectileList = new List<GameObject>();
    private List<ProjectileInfo> ProjectileHasCollided = new List<ProjectileInfo>();
    private List<WeaponInformation> ProjectileFromWeapon = new List<WeaponInformation>();
    private List<Vector3> ProjectileDesiredLocation = new List<Vector3>();
    private GameObject ProjectilePrefab;
    private float TimeSinceLastMove = 0;

    private Transform VFX_Storage;
    private List<GameObject> ActiveVFX = new List<GameObject>();
    private List<float> MaxLifetimeActiveVFX = new List<float>();
    private List<float> CurrentLifetimeActiveVFX = new List<float>();
    private GameObject MeleeDuelEntity;
    public float MeleeDuelRange = 3;

    private BasicInfo BI;
    private AnimationPlayer AP;
    private LayerMask EntityLayer;
    private float TickRate = 0.015f;
    private EBP_Info self_EBP;

    private List<ProjectileMovement> AllPM = new List<ProjectileMovement>();
    private List<ProjectileInfo> AllPI = new List<ProjectileInfo>();
    private List<bool> ProjectileHasReachedTarget = new List<bool>();

    private void Start()
    {
        WeaponActivation();
    }
    private void WeaponActivation()
    {
        if (BI == null)
        {
            gameObject.TryGetComponent(out BI);
        }

        self_EBP = BI.EBPs;
        Invoke(nameof(DelayedStart), 0.5f + Random.Range(0.1f, 0.4f));
    }

    private void DelayedStart()
    {
        if (ASC == null) { gameObject.TryGetComponent(out ASC); }
        if (AP == null) { gameObject.TryGetComponent(out AP); }

        EntityLayer = LayerMask.GetMask("Entity");

        if (AllWeapons.Count > 0)
        {
            GameObject.FindWithTag("GameController").gameObject.TryGetComponent(out GI);
            GI.CombatManager.TryGetComponent(out CM);
            gameObject.TryGetComponent(out EM);

            VFX_Storage = GI.VFX_Storage.transform;
            ProjectilePrefab = GI.ProjectileDefault;

            AllWeaponGroups.Add(AllWeapons); AllWeaponGroups.Add(AllWeapons01); AllWeaponGroups.Add(AllWeapons02); AllWeaponGroups.Add(AllWeapons03); AllWeaponGroups.Add(AllWeapons04);
            AllWeaponGroups.Add(AllWeapons05); AllWeaponGroups.Add(AllWeapons06); AllWeaponGroups.Add(AllWeapons07); AllWeaponGroups.Add(AllWeapons08); AllWeaponGroups.Add(AllWeapons09);

            GI.TryGetComponent(out Weapon_BackupData GI_WeaponData);
            for (int i = 0; i < AllWeaponGroups.Count; i++)
            {
                for (int j = 0; j < AllWeaponGroups[i].Count; j++)
                {
                    GI_WeaponData.ACTIVE_AllWeapon[AllWeaponGroups[i][j].PositionInGMData].TryGetComponent(out WeaponInformation tmpWeapon);
                    AllWeaponGroups[i][j] = tmpWeapon;
                    AllWeaponGroups[i][j].SetupParentCombat(this);
                }
            }

            for (int i = AllWeaponGroups_ActiveStatus.Count; i < AllWeaponGroups.Count; i++)
            {
                AllWeaponGroups_ActiveStatus.Add(false);
            }

            if (BI.EBPs.PositionInLvlHierarchy != -1) { SetWeaponStatusNonAI(); } else { AllWeaponGroups_ActiveStatus[0] = true; }

            //CHECK IF THIS AM STUFF IS NEEDED !!!
            AggroShape.TryGetComponent(out AM);
            gameObject.TryGetComponent(out Combat tmpCombat);
            AM.SetCombatMaster(tmpCombat);
            AggroShape.transform.localScale = new Vector3(AllWeaponGroups[0][0].MaxRange, 1, AllWeaponGroups[0][0].MaxRange);

            UI_SetIndicator();
            InvokeRepeating(nameof(MainCombatLoop), TickRate, TickRate);
        }
    }

    private void SetWeaponStatusNonAI()
    {
        int[] CurrentEquippedWeapon = PlayerPrefsX.GetIntArray("HeroEquippedWeapons", 0, 4);

        if (CurrentEquippedWeapon[BI.EBPs.PositionInLvlHierarchy] == 0)
        {
            AllWeaponGroups_ActiveStatus[0] = true;
            AllWeaponGroups_ActiveStatus[1] = false;
            AllWeaponGroups_ActiveStatus[2] = false;

            if (BI.EBPs.PositionInLvlHierarchy == 0)
            {
                try
                {
                    AllWeaponArt[3].SetActive(false);
                }
                catch
                {
                    Debug.LogWarning("ERROR! In Combat/SetWeaponsStatusNonAI. Cannot set weapon art!");
                }
            }
            else if (BI.EBPs.PositionInLvlHierarchy == 2)
            {
                try
                {
                    AllWeaponArtSecondary[0].SetActive(true);
                    AllWeaponArtSecondary[1].SetActive(false);
                    AllWeaponArtSecondary[2].SetActive(false);
                }
                catch
                {
                    Debug.LogWarning("ERROR! In Combat/SetWeaponsStatusNonAI. Cannot set weapon art!");
                }
            }

            try
            {
                AllWeaponArt[0].SetActive(true);
                AllWeaponArt[1].SetActive(false);
                AllWeaponArt[2].SetActive(false);
            }
            catch
            {
                Debug.LogWarning("ERROR! In Combat/SetWeaponsStatusNonAI. Cannot set weapon art!");
            }
        }
        else
        {
            if (CurrentEquippedWeapon[BI.EBPs.PositionInLvlHierarchy] == 2)
            {
                AllWeaponGroups_ActiveStatus[0] = false;
                AllWeaponGroups_ActiveStatus[1] = false;
                AllWeaponGroups_ActiveStatus[2] = true;

                if (BI.EBPs.PositionInLvlHierarchy == 0)
                {
                    try
                    {
                        AllWeaponArt[0].SetActive(false);
                        AllWeaponArt[1].SetActive(true);
                        AllWeaponArt[2].SetActive(true);
                        AllWeaponArt[3].SetActive(true);
                    }
                    catch
                    {
                        Debug.LogWarning("ERROR! In Combat/SetWeaponsStatusNonAI. Cannot set weapon art!");
                    }

                    AllWeaponGroups_ActiveStatus[0] = false;
                    AllWeaponGroups_ActiveStatus[1] = true;
                    AllWeaponGroups_ActiveStatus[2] = true;
                }
                else if (BI.EBPs.PositionInLvlHierarchy == 2)
                {
                    try
                    {
                        AllWeaponArt[0].SetActive(false);
                        AllWeaponArt[1].SetActive(false);
                        AllWeaponArt[2].SetActive(true);

                        AllWeaponArtSecondary[0].SetActive(false);
                        AllWeaponArtSecondary[1].SetActive(false);
                        AllWeaponArtSecondary[2].SetActive(true);
                    }
                    catch
                    {
                        Debug.LogWarning("ERROR! In Combat/SetWeaponsStatusNonAI. Cannot set weapon art!");
                    }
                }
                else
                {
                    try
                    {
                        AllWeaponArt[0].SetActive(false);
                        AllWeaponArt[1].SetActive(false);
                        AllWeaponArt[2].SetActive(true);
                    }
                    catch
                    {
                        Debug.LogWarning("ERROR! In Combat/SetWeaponsStatusNonAI. Cannot set weapon art!");
                    }
                }
            }
            else
            {
                try
                {
                    AllWeaponArt[0].SetActive(false);
                    AllWeaponArt[1].SetActive(true);
                    AllWeaponArt[2].SetActive(false);

                    AllWeaponGroups_ActiveStatus[0] = false;
                    AllWeaponGroups_ActiveStatus[1] = true;
                    AllWeaponGroups_ActiveStatus[2] = false;
                }
                catch
                {
                    Debug.LogWarning("ERROR! In Combat/SetWeaponsStatusNonAI. Cannot set weapon art!");
                }

                if (BI.EBPs.PositionInLvlHierarchy == 0)
                {
                    try
                    {
                        AllWeaponArt[3].SetActive(false);
                    }
                    catch
                    {
                        Debug.LogWarning("ERROR! In Combat/SetWeaponsStatusNonAI. Cannot set weapon art!");
                    }
                }
                else if (BI.EBPs.PositionInLvlHierarchy == 2)
                {
                    try
                    {
                        AllWeaponArt[0].SetActive(false);
                        AllWeaponArt[1].SetActive(true);
                        AllWeaponArt[2].SetActive(false);

                        AllWeaponArtSecondary[0].SetActive(false);
                        AllWeaponArtSecondary[1].SetActive(true);
                        AllWeaponArtSecondary[2].SetActive(false);
                    }
                    catch
                    {
                        Debug.LogWarning("ERROR! In Combat/SetWeaponsStatusNonAI. Cannot set weapon art!");
                    }
                }
            }
        }
    }

    public AggressionManager GetAggressionManager() { return AM; }

    public float GetTimeSinceCombat() { return TimeSinceLastCombat; }
    public void ResetTimeSinceLastCombat() { TimeSinceLastCombat = 0; }

    public void DisableCombat()
    {
        CombatIsEnabled = false;
        Target = null;
        DesiredTarget = null;
        SetCombatStatus(false);
    }

    private void MainCombatLoop()
    {
        CheckModifierLifetimes();
        CheckFXLifetimes();

        if (CombatIsEnabled)
        {
            CheckMeleeCharges();
            CheckDuels();
            FightLogic(); 
        }
    }

    private void FightLogic()
    {
        try
        {
            if (CombatIsEnabled)
            {
                for (int i = 0; i < AllWeaponGroups_ActiveStatus.Count; i++)
                {
                    if (AllWeaponGroups_ActiveStatus[i])
                    {
                        foreach (WeaponInformation Weapon in AllWeaponGroups[i])
                        {
                            if (!IsInCombat)
                            {
                                if (TimeSinceLastCombat >= BI.EBPs.TimeOutOfCombatForBonuses)
                                {
                                    Weapon.ResetWeaponBurst(BaseBurstCountMultiplier);
                                }
                                else if (TimeSinceLastCombat < BI.EBPs.TimeOutOfCombatForBonuses)
                                {
                                    TimeSinceLastCombat += TickRate;
                                }
                            }
                            else
                            {
                                HasBeenAggro = true;
                                TimeSinceLastCombat = 0;
                                if (TimeSinceLastMove > Weapon.SetupTime)
                                {
                                    Target = AM.FindClosestEnemyTarget();

                                    if (DesiredTarget != null)
                                    {
                                        float tmp_distance = Vector3.Distance(transform.position, DesiredTarget.transform.position);
                                        if ((tmp_distance <= Weapon.MaxRange && tmp_distance >= Weapon.MinRange) || Weapon.IsMelee)
                                        {
                                            Target = DesiredTarget;
                                        }
                                    }
                                    else if (Target != null)
                                    {
                                        float tmp_distance = Vector3.Distance(transform.position, Target.transform.position);
                                        if (!Weapon.IsMelee && (tmp_distance >= Weapon.MaxRange || tmp_distance <= Weapon.MinRange))
                                        {
                                            Target = null;
                                        }
                                    }

                                    if (Target != null)
                                    {
                                        IsInCombat = true;

                                        if (Weapon.IsMelee)
                                        {
                                            Target.TryGetComponent(out Combat tmp_combat);
                                            if (tmp_combat != null)
                                            {
                                                tmp_combat.AddNewMeleeDuel(gameObject);
                                            }
                                            AddNewMeleeDuel(Target);
                                        }

                                        bool temp_CheckFireOnMove = true;
                                        if ((!Weapon.FireOnMove || Weapon.IsMelee) && EM.GetIfMoving())
                                        {
                                            temp_CheckFireOnMove = false;
                                        }

                                        if ((IsInMelee && Weapon.IsMelee) || temp_CheckFireOnMove)//!IsInMelee)
                                        {
                                            if (Weapon.UpdateWeaponBurst(ModStatus_Multi_AllWeaponBurst, ModStatus_Add_AllWeaponBurst))
                                            {
                                                for (int z = 0; z < VFX_Muzzle.Length; z++)
                                                {
                                                    if (VFX_Muzzle[z] != null && VFX_MuzzleBone[z].gameObject.activeSelf)
                                                    {
                                                        GameObject tmpVFX = GameObject.Instantiate(VFX_Muzzle[z]);
                                                        tmpVFX.transform.position = VFX_MuzzleBone[z].position;
                                                        tmpVFX.transform.parent = VFX_MuzzleBone[z];
                                                    }
                                                }

                                                if (!IsInMelee) { AP.SetAnimationState(null, null, null, new bool[1] { true }); }

                                                if (!Weapon.UsesProjectile)
                                                {
                                                    float tmpDistance = Vector3.Distance(transform.position, Target.transform.position);
                                                    if (tmpDistance <= Weapon.MaxRange && tmpDistance >= Weapon.MinRange)
                                                    {
                                                        ApplyWeaponEffectsToTarget(Weapon);
                                                        if (Weapon.TracerEffect != null)
                                                        {
                                                            GameObject tmpTracer = Instantiate(Weapon.TracerEffect);
                                                            tmpTracer.transform.position = transform.position;
                                                            try
                                                            {
                                                                tmpTracer.transform.position = VFX_MuzzleBone[i].position;
                                                            }
                                                            catch
                                                            {
                                                                //No bones assigned!
                                                            }
                                                            tmpTracer.name = "Tracer: " + Weapon.WeaponName + "|" + gameObject.name + "|" + Time.time;
                                                            tmpTracer.TryGetComponent(out TracerMoveToTarget tmp_TracerMover);
                                                            tmp_TracerMover.EnableTracer(Target.transform);
                                                        }
                                                    }
                                                    else if (BI.EBPs.AIAggroRange <= tmpDistance)
                                                    {
                                                        EM.SetAttackTarget(Target.transform, true);
                                                    }
                                                    else
                                                    {
                                                        EM.SetMoveDestination(Target.transform.position);
                                                    }
                                                }
                                                else
                                                {
                                                    float tmpDistance = Vector3.Distance(transform.position, Target.transform.position);
                                                    if (tmpDistance <= Weapon.MaxRange && tmpDistance >= Weapon.MinRange)
                                                    {
                                                        ProjectileFromWeapon.Add(Weapon);

                                                        GameObject NewProjectile = Instantiate(ProjectilePrefab, transform.position, new Quaternion(), null);

                                                        try
                                                        {
                                                            NewProjectile.transform.position = VFX_MuzzleBone[i].position;
                                                        }
                                                        catch
                                                        {

                                                        }
                                                        ProjectileMovement tmpPM = NewProjectile.AddComponent<ProjectileMovement>();
                                                        ProjectileInfo tmpPI = NewProjectile.AddComponent<ProjectileInfo>();
                                                        ASC.OrderNewSound(Weapon.RepeatingTracerSFX, Weapon.RepeatingTracerSoundTravelDistance, true, false, false);

                                                        Transform tmpTarget = null; if (Weapon.ProjectileIsHoming) { tmpTarget = Target.transform; }

                                                        try
                                                        {
                                                            tmpPM.SetupProjectile(tmpTarget, Target.transform.position, Weapon.ProjectileSpeed, 1.75f, Weapon.ProjectileUseCurve, Weapon.ProjectileAC, Weapon.ProjectileMaxHeight, VFX_MuzzleBone[i].position);
                                                        }
                                                        catch
                                                        {
                                                            tmpPM.SetupProjectile(tmpTarget, Target.transform.position, Weapon.ProjectileSpeed, 1.75f, Weapon.ProjectileUseCurve, Weapon.ProjectileAC, Weapon.ProjectileMaxHeight, transform.position);
                                                        }
                                                        
                                                        AllPI.Add(tmpPI);
                                                        AllPM.Add(tmpPM);

                                                        for (int fx_spawn = 0; fx_spawn < Weapon.ProjectileVFX.Length; fx_spawn++)
                                                        {
                                                            GameObject NewFX = Instantiate(Weapon.ProjectileVFX[fx_spawn], transform.position, new Quaternion(), null);
                                                            NewFX.transform.parent = NewProjectile.transform;
                                                        }
                                                    }
                                                    else if (BI.EBPs.AIAggroRange <= tmpDistance)
                                                    {
                                                        EM.SetAttackTarget(Target.transform, true);
                                                    }
                                                    else
                                                    {
                                                        EM.SetMoveDestination(Target.transform.position);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        //LR.SetPosition(1, transform.position);
                                        AP.SetAnimationState(null, null, null, new bool[1] { false });
                                        //LR.enabled = false;
                                        IsInCombat = false;
                                        //Debug.Log("Target Entity No Longer Exists!");
                                    }
                                }

                                if (!EM.GetIfMoving())
                                {
                                    TimeSinceLastMove += TickRate;
                                }
                                else
                                {
                                    TimeSinceLastMove = 0;
                                }
                            }
                        }

                        if (AllPM.Count > 0)
                        {
                            CheckProjectileStatus();
                        }
                    }
                }
            }
        }
        catch
        {
            //Cannot do fight logic, probably in a cutscene!
        }
    }

    private float[] CheckCoverStatus()
    {
        float tmp_AccuracyBonus = 0;
        float tmp_DamageBonus = 1;
        
        if (BI.GetCoverStatus().GetArrayItem(out _, out int CurrentlyActiveCover, true))
        {
            tmp_AccuracyBonus = GI.GetValueOfLuxCover(GI.AccuracyBonusLuxCoverPos, CurrentlyActiveCover, BI.EBPs.LuxCoverArmourType);
            tmp_DamageBonus = GI.GetValueOfLuxCover(GI.DamageBonusLuxCoverPos, CurrentlyActiveCover, BI.EBPs.LuxCoverArmourType);
        }

        return new float[2] { tmp_AccuracyBonus, tmp_DamageBonus };
    }

    public void AddNewMeleeDuel(GameObject NewDuel)
    {
        if (MeleeDuelEntity == null || IsInMelee == false) { MeleeDuelEntity = NewDuel; }
    }

    private void CheckMeleeCharges()
    {
        if (DesiredTarget != null)
        {
            float tmp_distance = Vector3.Distance(transform.position, DesiredTarget.transform.position);

            if (tmp_distance <= MeleeChargeRange)
            {
                IsMeleeCharging = true;
            }
            else
            {
                IsMeleeCharging = false;
            }

            if (HasMeleeLeap && tmp_distance <= MeleeLeapRange && CurrentLeapCooldown <= 0 && !IsMeleeLeap)
            {
                IsMeleeLeap = true;
                CurrentLeapCooldown = MeleeLeapCooldown;
                EM.SetupMeleeLeap(DesiredTarget.transform);
            }
        }
        else
        {
            IsMeleeCharging = false;
        }

        if (IsMeleeLeap)
        {
            CurrentLeapCooldown -= TickRate;
        }
    }

    private void CheckDuels()
    {
        if (MeleeDuelEntity != null)
        {
            bool DuelIsValid = false;
            RaycastHit hit;

            if (Physics.Raycast(transform.position, (MeleeDuelEntity.transform.position - transform.position), out hit, Mathf.Infinity))
            {
                if (hit.transform == MeleeDuelEntity.transform)
                {
                    DuelIsValid = true;
                }
            }

            if (DuelIsValid)
            {
                float tmp_distance = Vector3.Distance(transform.position, hit.point);

                if (Vector3.Distance(transform.position, MeleeDuelEntity.transform.position) < MeleeDuelRange)
                {
                    tmp_distance = MeleeDuelRange;
                }

                if (tmp_distance > MeleeDuelRange)
                {
                    IsInMelee = false;
                }
                else
                {
                    IsMeleeCharging = false;
                }

                if (tmp_distance <= MeleeDuelRange)
                {
                    IsMeleeCharging = false;
                    IsInMelee = true;
                }
            }
        }
        else
        {
            IsInMelee = false;
        }

        if (!IsMeleeLeap)
        {
            if (!IsInMelee) { AP.SetAnimationState(null, null, null, null, new bool[1] { false }, null); }
            else { AP.SetAnimationState(null, null, null, new bool[1] { false }, new bool[1] { true }, null); }
        }
    }

    public void UI_SetIndicator()
    {
        for (int j = 0; j < AllWeaponGroups_ActiveStatus.Count; j++)
        {
            if (AllWeaponGroups_ActiveStatus[j])
            {
                if (AllWeaponGroups[j].Count > 0)
                {
                    int ImagePlace = 0;

                    for (int i = 0; i < AllWeaponGroups[j].Count; i++)
                    {
                        if (AllWeaponGroups[j][i].DisplayIsMainWeapon)
                        {
                            ImagePlace = i;
                            break;
                        }
                    }

                    UI_DamageTypeIndicator.SetNewImage(GI.UI_DamageTypeIdentifiers[AllWeaponGroups[j][ImagePlace].DamagePiercingType]);
                }
            }
        }
    }

    public void SetCombatStatus(bool Status = false)
    {
        IsInCombat = Status;
    }

    private void ApplyProjectileEffects(int projectile)
    {
        for (int i = 0; i < ProjectileFromWeapon[projectile].ModifiersToApply_Target.Count; i++)
        {
            if ((Random.Range(0, 100) / 100) <= ProjectileFromWeapon[projectile].ModifierChance_Target[i])
            {
                if (ProjectileFromWeapon[projectile].VFX_Target.Count > i)
                {
                    for (int fx = 0; fx < ProjectileFromWeapon[projectile].VFX_Target.Count; fx++)
                    {
                        GameObject tmp_obj = ProjectileFromWeapon[projectile].VFX_Target[fx];
                        if (tmp_obj == null) { tmp_obj = new GameObject(); }//CHECK WHY THIS IS HAPPENING
                        GameObject tmp_VFX = Instantiate(tmp_obj, VFX_Storage);
                        tmp_VFX.transform.position = ProjectileDesiredLocation[projectile];
                        tmp_VFX.name = "VFX_Weapon; Caster FX for: " + ProjectileFromWeapon[projectile].WeaponName;

                        ActiveVFX.Add(tmp_VFX);
                        MaxLifetimeActiveVFX.Add(ProjectileFromWeapon[projectile].VFX_Lifetime_Target[fx]);
                        CurrentLifetimeActiveVFX.Add(0);
                    }
                }
                ApplyAModifier(ProjectileFromWeapon[projectile].ModifierNames_Target[i], ProjectileFromWeapon[projectile].WeaponName, AppliedModsHolder_Target, AppliedModsLifetime_Target, ProjectileFromWeapon[projectile].ModifiersToApply_Target[i], ProjectileFromWeapon[projectile].ModifierRadius_Target[i], null, ProjectileList[i].transform.position); //ProjectileDesiredLocation[projectile]);
                AppliedModsMaxLifetime_Target.Add(ProjectileFromWeapon[projectile].ModifierAoETime_Target[i]);
            }
        }
        for (int i = 0; i < ProjectileFromWeapon[projectile].ModifiersToApply_Caster.Count; i++)
        {
            if ((Random.Range(0, 100) / 100) <= ProjectileFromWeapon[projectile].ModifierChance_Caster[i])
            {
                ApplyAModifier(ProjectileFromWeapon[projectile].ModifierNames_Caster[i], ProjectileFromWeapon[projectile].WeaponName, AppliedModsHolder_Caster, AppliedModsLifetime_Caster, ProjectileFromWeapon[projectile].ModifiersToApply_Caster[i], ProjectileFromWeapon[projectile].ModifierRadius_Caster[i], null, transform.position);
                AppliedModsMaxLifetime_Caster.Add(ProjectileFromWeapon[projectile].ModifierAoETime_Target[i]);
            }
        }

        Destroy(ProjectileList[projectile]);
        ProjectileList.RemoveAt(projectile);
        ProjectileHasCollided.RemoveAt(projectile);
        ProjectileFromWeapon.RemoveAt(projectile);
        ProjectileDesiredLocation.RemoveAt(projectile);
    }

    private void CheckModifierLifetimes()
    {
        for (int i = 0; i < AppliedModsHolder_Target.Count; i++)
        {
            if (AppliedModsLifetime_Target[i] >= AppliedModsMaxLifetime_Target[i])
            {
                Destroy(AppliedModsHolder_Target[i]);
                AppliedModsLifetime_Target.RemoveAt(i); AppliedModsMaxLifetime_Target.RemoveAt(i); AppliedModsHolder_Target.RemoveAt(i);
            }
            else { AppliedModsLifetime_Target[i] += TickRate; }
        }
        for (int i = 0; i < AppliedModsHolder_Caster.Count; i++)
        {
            if (AppliedModsLifetime_Caster[i] >= AppliedModsMaxLifetime_Caster[i])
            {
                Destroy(AppliedModsHolder_Caster[i]);
                AppliedModsLifetime_Caster.RemoveAt(i); AppliedModsMaxLifetime_Caster.RemoveAt(i); AppliedModsHolder_Caster.RemoveAt(i);
            }
            else { AppliedModsLifetime_Caster[i] += TickRate; }
        }
    }

    public void ForceKillAllWeaponMods()
    {
        for (int i = 0; i < AppliedModsHolder_Target.Count; i++)
        {
            Destroy(AppliedModsHolder_Target[i]);
            AppliedModsLifetime_Target.RemoveAt(i); AppliedModsMaxLifetime_Target.RemoveAt(i); AppliedModsHolder_Target.RemoveAt(i);
        }
        for (int i = 0; i < AppliedModsHolder_Caster.Count; i++)
        {
            Destroy(AppliedModsHolder_Caster[i]);
            AppliedModsLifetime_Caster.RemoveAt(i); AppliedModsMaxLifetime_Caster.RemoveAt(i); AppliedModsHolder_Caster.RemoveAt(i);
        }
    }

    private void CheckFXLifetimes()
    {
        for (int i = 0; i < ActiveVFX.Count; i++)
        {
            if (CurrentLifetimeActiveVFX[i] >= MaxLifetimeActiveVFX[i])
            {
                Kill_VFX(i);
            }
            else
            {
                CurrentLifetimeActiveVFX[i] += TickRate;
            }
        }
    }

    private void ApplyAModifier(string AbilityName, string WeaponName, List<GameObject> AddToList, List<float> AddToListLifetime, GameObject ApplyMod, float Size, Transform Target, Vector3 ModPos)
    {
        GameObject NewMod = Instantiate(ApplyMod, ModPos, new Quaternion(), null);
        NewMod.TryGetComponent(out ModifierApplier NewMod_MA);
        NewMod_MA.TakeDamageFromTarget = self_EBP.PositionInLvlHierarchy;
        if (Target != null) { NewMod_MA.ApplyActiveObject(Target.gameObject); }
        NewMod_MA.OwnedByPlayer = BI.OwnedByPlayer;
        NewMod.transform.localScale = new Vector3(Size, Size, Size);
        NewMod.transform.position = ModPos;
        NewMod.name = "Modifier: " + AbilityName + " |From Weapon: " + WeaponName;
        CM.AddNewMod(NewMod_MA);
        AddToList.Add(NewMod);
        AddToListLifetime.Add(0);
    }

    private void ApplyWeaponEffectsToTarget(WeaponInformation Weapon)
    {
        if (Target == null)
        {
            Target = AM.FindClosestEnemyTarget();
        }

        if (Target != null)
        {
            float TmpDmg = Random.Range(Weapon.MinDamage, Weapon.MaxDamage);

            if (TmpDmg != 0)
            {
                Health TargetHealth = Target.GetComponent<Health>();
                if (TargetHealth != null)
                {
                    ASC.OrderNewSound(Weapon.RepeatingTracerSFX, Weapon.RepeatingTracerSoundTravelDistance, true, false, false);
                    float TmpAccuracy = Weapon.Accuracy;
                    if (IsMoving) { TmpAccuracy -= Weapon.AccuracyReductionOnMove; }

                    float[] CurrentCoverModifiers = CheckCoverStatus();
                    TmpAccuracy += CurrentCoverModifiers[0];

                    if (Weapon.BenefitsFromVerticality)//Verticality
                    {
                        float YDifference = transform.position[1] - Target.transform.position[1];

                        YDifference /= Target.transform.position[1];
                        if (transform.position[1] > Target.transform.position[1])
                        {
                            YDifference *= MaxAccuracyBonusFromVerticality;
                        }
                        else
                        {
                            YDifference *= MinAccuracyBonusFromVerticality;
                        }
                        TmpAccuracy += YDifference;
                    }

                    TmpDmg *= Weapon.DamageScale_Distance.Evaluate(Vector3.Distance(transform.position, Target.transform.position) / (Weapon.MaxRange - Weapon.MinRange));
                    TmpDmg *= BaseWeaponDamageMultiplier;
                    float[] DmgAfterPiercing = GI.GetDamageFromPiercingTable(TargetHealth.GetArmourType(), Weapon.DamagePiercingType, TmpDmg, TmpAccuracy);
                    DmgAfterPiercing = new float[2] { DmgAfterPiercing[0] * CurrentCoverModifiers[1], DmgAfterPiercing[1] * CurrentCoverModifiers[1] };
                    TargetHealth.UpdateCurrentHealth(self_EBP.PositionInLvlHierarchy, false, -DmgAfterPiercing[0], -DmgAfterPiercing[1], true, true, GI.ArmourPenetratesFA[Weapon.DamagePiercingType], Weapon.IsMelee, !Weapon.IsMelee, Weapon.IgnoreHealthModifiers, Weapon.IgnoreMeleeResist, Weapon.IgnoreRangedResist, Weapon.IgnoreLuxCover, Weapon.IgnoreRawCoverModifiers);

                    if (Weapon.ModifiersToApply_Target.Count > 0)
                    {
                        for (int i = 0; i < Weapon.ModifiersToApply_Target.Count; i++)
                        {
                            if ((Random.Range(0, 100) / 100) <= Weapon.ModifierChance_Target[i])
                            {
                                Transform tmp_target;
                                if (Weapon.ModifierTargetGround_Target[i]) { tmp_target = null; } else { tmp_target = Target.transform; }
                                ApplyAModifier(Weapon.ModifierNames_Target[i], Weapon.WeaponName, AppliedModsHolder_Target, AppliedModsLifetime_Target, Weapon.ModifiersToApply_Target[i], Weapon.ModifierRadius_Target[i], tmp_target, Target.transform.position);
                                AppliedModsMaxLifetime_Target.Add(Weapon.ModifierAoETime_Target[i]);
                            }
                        }
                    }
                    if (Weapon.ModifiersToApply_Caster.Count > 0)
                    {
                        for (int i = 0; i < Weapon.ModifiersToApply_Caster.Count; i++)
                        {
                            if ((Random.Range(0, 100) / 100) <= Weapon.ModifierChance_Caster[i])
                            {
                                Transform tmp_target;
                                if (Weapon.ModifierTargetGround_Target[i]) { tmp_target = null; } else { tmp_target = transform; }
                                ApplyAModifier(Weapon.ModifierNames_Caster[i], Weapon.WeaponName, AppliedModsHolder_Caster, AppliedModsLifetime_Caster, Weapon.ModifiersToApply_Caster[i], Weapon.ModifierRadius_Caster[i], tmp_target, transform.position);
                                AppliedModsMaxLifetime_Target.Add(Weapon.ModifierAoETime_Caster[i]);
                            }
                        }
                    }
                }
            }
        }
    }

    private void CheckProjectileStatus()
    {
        int WeaponPos = 0;

        for (int z = 0; z < AllWeaponGroups.Count; z++)
        {
            for (int j = 0; j < AllWeaponGroups[z].Count; j++)
            {
                WeaponInformation tmpWeapon = AllWeaponGroups[z][j];

                if (tmpWeapon.UsesProjectile && AllWeaponGroups_ActiveStatus[z])
                {
                    for (int i = 0; i < AllPM.Count; i++)
                    {
                        if (tmpWeapon.CanCollide && AllPI[i].IHaveCollided)
                        {
                            for (int mod = 0; mod < tmpWeapon.ModifiersToApply_Target.Count; mod++)
                            {
                                ApplyAModifier(ProjectileFromWeapon[WeaponPos].name, tmpWeapon.WeaponName, AppliedModsHolder_Target, AppliedModsLifetime_Target, tmpWeapon.ModifiersToApply_Target[mod], tmpWeapon.ModifierRadius_Target[mod], AllPI[i].CollidedObj.transform, AllPM[i].transform.position);
                                AppliedModsMaxLifetime_Target.Add(ProjectileFromWeapon[WeaponPos].ModifierAoETime_Target[mod]);
                                if (tmpWeapon.VFX_Target.Count != 0)
                                {
                                    CreateStandardVFXFromArray(tmpWeapon.VFX_Target, "VFX_WeaponAbility; Target FX for: ", AllPI[i].CollidedObj.transform.position, AllPI[i].CollidedObj.transform);
                                }
                            }

                            for (int mod = 0; mod < tmpWeapon.ModifiersToApply_Caster.Count; mod++)
                            {
                                ApplyAModifier(ProjectileFromWeapon[WeaponPos].name, tmpWeapon.WeaponName, AppliedModsHolder_Caster, AppliedModsLifetime_Caster, tmpWeapon.ModifiersToApply_Caster[mod], tmpWeapon.ModifierRadius_Caster[mod], transform, AllPM[i].transform.position);
                                AppliedModsMaxLifetime_Target.Add(ProjectileFromWeapon[WeaponPos].ModifierAoETime_Caster[mod]);
                                if (tmpWeapon.VFX_Caster.Count != 0)
                                {
                                    CreateStandardVFXFromArray(tmpWeapon.VFX_Caster, "VFX_WeaponAbility; Caster FX for: ", transform.position, transform);
                                }
                            }

                            Destroy(AllPM[i].gameObject);
                            AllPM.RemoveAt(i);
                            AllPI.RemoveAt(i);
                        }
                        else if (AllPM[i].GetProgress())
                        {                            
                            for (int mod = 0; mod < tmpWeapon.ModifiersToApply_Target.Count; mod++)
                            {
                                ApplyAModifier(ProjectileFromWeapon[WeaponPos].name, tmpWeapon.WeaponName, AppliedModsHolder_Target, AppliedModsLifetime_Target, tmpWeapon.ModifiersToApply_Target[mod], tmpWeapon.ModifierRadius_Target[mod], Target.transform, AllPM[i].transform.position);
                                AppliedModsMaxLifetime_Target.Add(ProjectileFromWeapon[WeaponPos].ModifierAoETime_Target[mod]);
                                if (tmpWeapon.VFX_Target.Count != 0)
                                {
                                    CreateStandardVFXFromArray(tmpWeapon.VFX_Target, "VFX_Ability; Target FX for: ", AllPM[i].transform.position, Target.transform);
                                }
                            }

                            for (int mod = 0; mod < tmpWeapon.ModifiersToApply_Caster.Count; mod++)
                            {
                                ApplyAModifier(ProjectileFromWeapon[WeaponPos].name, tmpWeapon.WeaponName, AppliedModsHolder_Caster, AppliedModsLifetime_Caster, tmpWeapon.ModifiersToApply_Caster[mod], tmpWeapon.ModifierRadius_Caster[mod], transform, AllPM[i].transform.position);
                                AppliedModsMaxLifetime_Target.Add(ProjectileFromWeapon[WeaponPos].ModifierAoETime_Caster[mod]);
                                if (tmpWeapon.VFX_Caster.Count != 0)
                                {
                                    CreateStandardVFXFromArray(tmpWeapon.VFX_Caster, "VFX_Ability; Caster FX for: ", transform.position, transform);
                                }
                            }

                            Destroy(AllPM[i].gameObject);
                            AllPM.RemoveAt(i);
                            AllPI.RemoveAt(i);
                        }
                    }

                    WeaponPos++;
                }
            }
        }
    }

    private void Kill_VFX(int PositionInList)
    {
        //GameObject.Destroy(ActiveVFX[PositionInList].gameObject);
        ActiveVFX.RemoveAt(PositionInList);
        CurrentLifetimeActiveVFX.RemoveAt(PositionInList);
        MaxLifetimeActiveVFX.RemoveAt(PositionInList);
    }

    private void CreateStandardVFXFromArray(List<GameObject> FX_List, string NameHelpText, Vector3 FX_Pos, Transform ParentObj)
    {
        foreach (GameObject tmp_obj in FX_List)
        {
            GameObject tmp_VFX = Instantiate(tmp_obj, ParentObj);
            tmp_VFX.transform.position = FX_Pos;
            tmp_VFX.name = NameHelpText + gameObject.name;
            //ActiveVFXObj.Add(tmp_VFX);
        }
    }

    public void UpdateCombatMods(bool IsMultiply = false, float BurstCount = 1, float BurstDuration = 1, float BaseReload = 1, float BaseWeaponDamage = 1)
    {
        if (!IsMultiply)
        {
        }
        else
        {
            BaseBurstCountMultiplier = BurstCount;
            BaseBurstDurationMultiplier = BurstDuration;
            BaseReloadTimeMultiplier = BaseReload;
            BaseWeaponDamageMultiplier = BaseWeaponDamage;
        }
    }

    public void MasterFunction_UpdateAllCombatModifiers(string ModApplicationType = "ENTITY", string ModMathType = "ALL", float BurstCount = 1, float BurstDuration = 1, float BaseReload = 1, float BaseWeaponDamage = 1)
    {
        if (ModMathType == "SET")
        {
            if (BurstCount >= 0) { BaseBurstCountMultiplier = BurstCount; }
            if (BurstDuration >= 0) { BaseBurstDurationMultiplier = BurstDuration; }
            if (BaseReload >= 0) { BaseReloadTimeMultiplier = BaseReload; }
            if (BaseWeaponDamage >= 0) { BaseWeaponDamageMultiplier = BaseWeaponDamage; }
        }
        else if (ModMathType == "MULTIPLICATION" || ModMathType == "ALL")
        {
            UpdateCombatMods(true, BurstCount, BurstDuration, BaseReload, BaseWeaponDamage);
        }

        for (int i = 0; i < AllWeaponGroups_ActiveStatus.Count; i++)
        {
            if (AllWeaponGroups_ActiveStatus[i])
            {
                foreach (WeaponInformation Weapon in AllWeaponGroups[i])
                {
                    Weapon.FullyResetBurst();
                }
            }
        }
    }
}
