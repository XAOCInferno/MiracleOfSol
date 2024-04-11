using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponInformation : MonoBehaviour
{
    //Generic
    public string WeaponName = "__Default";
    public string WeaponEffectiveAgainst = "All";
    public int PositionInGMData = 0;
    public bool UsesProjectile = false;
    public bool ProjectileIsHoming = false;
    public bool CanCollide = true;
    public float ProjectileSpeed = 5;
    public float ProjectileMaxHeight = 4;
    public bool ProjectileUseCurve = true;
    public AnimationCurve ProjectileAC = AnimationCurve.Linear(0, 1, 1, 1);
    public GameObject[] ProjectileVFX;
    public AudioClip RepeatingTracerSFX;
    public float[] RepeatingTracerSoundTravelDistance;
    public GameObject TracerEffect;

    //Stats
    public bool DisplayIsMainWeapon = false;
    public int DamagePiercingType = 0;
    public float Accuracy = 0.5f;
    public float AccuracyReductionOnMove = 0.3f;
    public float MaxDamage = 10;
    public float MinDamage = 5;
    public AnimationCurve DamageScale_Distance = AnimationCurve.Linear(0,1.15f,1,0.75f);
    public float SetupTime = 0;
    public float DurationPerBurst = 1;
    public int ShotsPerBurst = 6;
    public int BurstRandomness = 4;
    public float ReloadDuration = 3;
    public float MinReloadTime = 0;
    public float MaxRange = 30;
    public float MinRange = 0;
    public bool FireOnMove = true;
    public float FireArc = 360;

    public bool BenefitsFromVerticality = true;
    public bool IgnoreLuxCover = false;
    public bool IgnoreHealthModifiers = false;
    public bool IgnoreRawCoverModifiers = false;
    public bool IgnoreMeleeResist = false;
    public bool IgnoreRangedResist = false;
    public bool IsMelee = false;


    public List<GameObject> VFX_Target = new List<GameObject>();
    public List<float> VFX_Lifetime_Target = new List<float>();
    public List<GameObject> ModifiersToApply_Target = new List<GameObject>();
    public List<float> ModifierAoETime_Target = new List<float>();
    public List<string> ModifierNames_Target = new List<string>();
    public List<float> ModifierChance_Target = new List<float>();
    public List<float> ModifierRadius_Target = new List<float>();
    public List<bool> ModifierTargetGround_Target = new List<bool>();

    public List<GameObject> VFX_Caster = new List<GameObject>();
    public List<GameObject> ModifiersToApply_Caster = new List<GameObject>();
    public List<float> ModifierAoETime_Caster = new List<float>();
    public List<string> ModifierNames_Caster = new List<string>();
    public List<float> ModifierChance_Caster = new List<float>();
    public List<float> ModifierRadius_Caster = new List<float>();
    public List<bool> ModifierTargetGround_Caster = new List<bool>();

    private float CurrentSavedBurstCount = 0;
    private float CurrentBurstCount = 0;
    private float CurrentBurstTimer = 0;
    private float CurrentReloadTimer = 0;

    private bool IsReloading = false;
    private AudioSourceController ASC;
    private Combat selfCombat;

    private bool IsActive = false;

    private void Start()
    {
        gameObject.TryGetComponent(out ASC);
        for (int i = ModifierNames_Target.Count; i < ModifiersToApply_Target.Count; i++)
        {
            ModifierNames_Target.Add("_Default");
        }
        for (int i = ModifierAoETime_Target.Count; i < ModifiersToApply_Target.Count; i++)
        {
            ModifierAoETime_Target.Add(0);
        }
        for (int i = ModifierChance_Target.Count; i < ModifiersToApply_Target.Count; i++)
        {
            ModifierChance_Target.Add(1);
        }
        for (int i = ModifierRadius_Target.Count; i < ModifiersToApply_Target.Count; i++)
        {
            ModifierRadius_Target.Add(0);
        }
        for (int i = ModifierTargetGround_Target.Count; i < ModifiersToApply_Target.Count; i++)
        {
            ModifierTargetGround_Target.Add(true);
        }
        for (int i = VFX_Target.Count; i < ModifiersToApply_Target.Count; i++)
        {
            VFX_Target.Add(null);
        }
        for (int i = VFX_Lifetime_Target.Count; i < ModifiersToApply_Target.Count; i++)
        {
            VFX_Lifetime_Target.Add(1);
        }

        for (int i = ModifierNames_Caster.Count; i < ModifiersToApply_Caster.Count; i++)
        {
            ModifierNames_Caster.Add("_Default");
        }
        for (int i = ModifierAoETime_Caster.Count; i < ModifiersToApply_Target.Count; i++)
        {
            ModifierAoETime_Caster.Add(0);
        }
        for (int i = ModifierChance_Caster.Count; i < ModifiersToApply_Caster.Count; i++)
        {
            ModifierChance_Caster.Add(1);
        }
        for (int i = ModifierRadius_Caster.Count; i < ModifiersToApply_Caster.Count; i++)
        {
            ModifierRadius_Caster.Add(0);
        }
        for (int i = ModifierTargetGround_Caster.Count; i < ModifiersToApply_Caster.Count; i++)
        {
            ModifierTargetGround_Caster.Add(true);
        }

        FullyResetBurst();
    }

    public void FullyResetBurst()
    {
        CurrentBurstCount = SetBurstCount();
        CurrentSavedBurstCount = CurrentBurstCount;
    }

    public void SetupParentCombat(Combat NewCombat)
    {
        selfCombat = NewCombat;
        CurrentBurstTimer = CalculateTimerUsingMods(DurationPerBurst * selfCombat.BaseBurstDurationMultiplier / CurrentBurstCount);
        IsActive = true;
    }

    private void Update()
    {
        if (IsReloading && IsActive)
        {
            UpdateWeaponReload();
        }
    }

    public void ResetWeaponBurst(float Mod_MultiBurstTime = 1, float Mod_AddBurstTime = 0)
    {
        if (IsActive)
        {
            CurrentBurstCount = CalculateTimerUsingMods(CurrentBurstCount, Mod_MultiBurstTime, Mod_AddBurstTime);
        }
    }

    public bool UpdateWeaponBurst(float Mod_MultiBurstTime = 1, float Mod_AddBurstTime = 0)
    {
        if (IsActive)
        {
            if (CurrentBurstCount > 0)
            {
                if (CurrentBurstTimer <= 0)
                {
                    CurrentBurstCount--;
                    CurrentBurstTimer = CalculateTimerUsingMods(DurationPerBurst * selfCombat.BaseBurstDurationMultiplier / CurrentSavedBurstCount);
                    return true;
                }
                else
                {
                    CurrentBurstTimer -= Time.deltaTime;
                    return false;
                }
            }
            else if (!IsReloading)
            {
                IsReloading = true;
                CurrentReloadTimer = CalculateTimerUsingMods(ReloadDuration, selfCombat.BaseReloadTimeMultiplier, MinReloadTime);
                return false;
            }
            else
            {
                return false;
            }
        }
        return false;
    }

    public void UpdateWeaponReload(float Mod_MultiReloadTime = 1, float Mod_AddReloadTime = 0)
    {
        if (IsActive)
        {
            if (CurrentReloadTimer <= 0)
            {
                CurrentBurstCount = SetBurstCount();
                CurrentSavedBurstCount = CurrentBurstCount;
                CurrentBurstTimer = CalculateTimerUsingMods(DurationPerBurst * selfCombat.BaseBurstDurationMultiplier / CurrentSavedBurstCount);
                IsReloading = false;
            }
            else
            {
                CurrentReloadTimer -= Time.deltaTime;
            }
            /*else
            {//Work on this not working properly 
                CurrentReloadTimer = CalculateTimerUsingMods(ReloadDuration, Mod_MultiReloadTime, Mod_AddReloadTime);
            }*/
        }
    }

    private float CalculateTimerUsingMods(float Timer = 0, float TimerToMulti = 1, float TimerToAdd = 0, float Min = 0)
    {
        float tmpReload = Timer * TimerToMulti + TimerToAdd;
       
        if(tmpReload < Min)
        {
            tmpReload = Min;
        }
        return tmpReload;
    }

    private int SetBurstCount()
    {
        if (IsActive)
        {
            return (int)((ShotsPerBurst + Random.Range(-BurstRandomness, BurstRandomness)) * selfCombat.BaseBurstCountMultiplier);
        }
        return 0;
    }
}
