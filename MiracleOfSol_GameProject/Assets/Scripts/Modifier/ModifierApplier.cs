using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierApplier : MonoBehaviour
{
    public bool ReviveOnActivate = false;

    //Target Info
    public string DesiredTarget = "ALL"; //NONE [[dummy]], ENEMY, OWN, ALL, ALLIED [[tbc]]
    public int OwnedByPlayer = -1;
    public int TakeDamageFromTarget = -1;

    public float InitialDelayTime;

    public int[] DamagePiercingType = new int[0]; //0:Basic; 1:Armour Shredding; 2:Armour Piercing; 3:Daemonic; 4:Heavy; 5:Anti-Tank; 6:Explosive; 7:Biological;
    public bool[] DamageIsMultiply = new bool[0]; //Leave blank for False
    public float[] MaxDamage = new float[0];
    public float[] MinDamage = new float[0];
    public bool[] DamageExclusive = new bool[0]; //Leave blank for False
    public float[] DamageReloadTime = new float[0];
    public int[][] DamageIgnoreArmourTypes;

   // public int[] RawDamagePiercingType; //0:Basic; 1:Armour Shredding; 2:Armour Piercing; 3:Daemonic; 4:Heavy; 5:Anti-Tank; 6:Explosive; 7:Biological;
    public bool[] RawDamageIsMultiply = new bool[0]; //Leave blank for False
    public float[] RawHPDamage = new float[0];
    public float[] RawArmourDamage = new float[0];
    public bool[] RawDamageExclusive = new bool[0]; //Leave blank for False

    public float[] ForceMax = new float[0];
    public float[] ForceMin = new float[0];
    public Vector3[] ForceDirection = new Vector3[0];
    public float[] ForceTimeInAir = new float[0];
    public bool[] ForceKnockbackToAll = new bool[0];
    public float[] ForceReloadTime = new float[0];

    public ModifierHandler[] ModifierApply = new ModifierHandler[0];
    public List<string> Overload_ModApply_ApplicationType = new List<string>();
    public List<bool> Overload_ModApply_Exclusive = new List<bool>();
    public List<float> Overload_ModApply_Probability = new List<float>();
    public List<string> Overload_ModApply_TargetTypeName = new List<string>();
    public List<string> Overload_ModApply_UsageType = new List<string>();
    public List<float> Overload_ModApply_Value = new List<float>();

    public List<float> ModifierDuration = new List<float>();
    public float[] ModifierReloadTime = new float[0];
    public bool[] Mod_Exclusive = new bool[0]; //Leave blank for False, ie: Stacking Mods

    private GameInfo GI;
    private List<GameObject>[] Damage_HitEntities = new List<GameObject>[0];
    private List<GameObject>[] RawDamage_HitEntities = new List<GameObject>[0];
    private List<GameObject>[] Mod_HitEntities = new List<GameObject>[0];
    public List<GameObject> AllTargets = new List<GameObject>();

    private float[] TimerDamage;
    private bool IsSetup = false;

    private int ModPosUniqueID = 0;

    private void Start()
    {
        GameObject.FindWithTag("GameController").TryGetComponent(out GI);
        Rigidbody tmp_rb = gameObject.AddComponent<Rigidbody>();
        tmp_rb.isKinematic = true;
        DesiredTarget = DesiredTarget.ToUpper();
        SetupStats();
        //Invoke(nameof(SetupStats), 0.1f);
        Invoke("InitialDamage", InitialDelayTime + 0.1f);
    }

    private void SetupStats()
    {
        RawDamage_HitEntities = new List<GameObject>[RawHPDamage.Length];
        Damage_HitEntities = new List<GameObject>[MaxDamage.Length];
        Mod_HitEntities = new List<GameObject>[ModifierApply.Length];
        TimerDamage = new float[DamageReloadTime.Length];
        for (int i = 0; i < TimerDamage.Length; i++) { TimerDamage[i] = InitialDelayTime; }


        for (int i = Overload_ModApply_ApplicationType.Count; i < ModifierApply.Length; i++)
        {
            Overload_ModApply_ApplicationType.Add(ModifierApply[i].ApplicationType);
        }
        for (int i = Overload_ModApply_Exclusive.Count; i < ModifierApply.Length; i++)
        {
            Overload_ModApply_Exclusive.Add(ModifierApply[i].Exclusive);
        }
        for (int i = Overload_ModApply_Probability.Count; i < ModifierApply.Length; i++)
        {
            Overload_ModApply_Probability.Add(ModifierApply[i].ProbabilityOfApplying);
        }
        for (int i = Overload_ModApply_TargetTypeName.Count; i < ModifierApply.Length; i++)
        {
            Overload_ModApply_TargetTypeName.Add(ModifierApply[i].TargetTypeName);
        }
        for (int i = Overload_ModApply_UsageType.Count; i < ModifierApply.Length; i++)
        {
            Overload_ModApply_UsageType.Add(ModifierApply[i].UsageType);
        }
        for (int i = Overload_ModApply_Value.Count; i < ModifierApply.Length; i++)
        {
            Overload_ModApply_Value.Add(ModifierApply[i].Value);
        }
        for (int i = ModifierDuration.Count; i < ModifierApply.Length; i++)
        {
            ModifierDuration.Add(1);
        }

        for (int i = 0; i < ModifierApply.Length; i++)
        {
            ModifierApply[i].InitiateModValues(ModifierApply[i].ModUniqueName, Overload_ModApply_ApplicationType[i], Overload_ModApply_Exclusive[i], Overload_ModApply_Probability[i], Overload_ModApply_TargetTypeName[i], Overload_ModApply_UsageType[i], Overload_ModApply_Value[i]);
        }

        IsSetup = true;
    }

    private void Update()
    {
        if (IsSetup)
        {
            for (int i = 0; i < TimerDamage.Length; i++)
            {
                TimerDamage[i] -= Time.deltaTime;
                if (TimerDamage[i] <= 0)
                {
                    TimerDamage[i] = DamageReloadTime[i];
                    ApplyModToTarget(TakeDamageFromTarget, true, false, false);
                    ApplyModToTarget(TakeDamageFromTarget, false, false, true);//Come back here and look into mod timer.
                }
            }
        }
    }

    public void ApplyActiveObject(GameObject Target)
    {
        bool IsInList = false;

        for(int i = 0; i < AllTargets.Count; i++)
        {
            if(Target == AllTargets[i])
            {
                IsInList = true;
                break;
            }
        }

        if (!IsInList) { AllTargets.Add(Target); }
    }

    public void RemoveActiveObject(GameObject Target, int IfNull_ItemPos = 0)
    {
        bool IsInList = true;
        int ListPos = IfNull_ItemPos;

        if (Target != null)
        {
            IsInList = false;   

            for (int i = 0; i < AllTargets.Count; i++)
            {
                if (Target == AllTargets[i])
                {
                    ListPos = i;
                    IsInList = true;
                    break;
                }
            }            
        }

        if (IsInList) { AllTargets.RemoveAt(ListPos); }
    }

    private void InitialDamage()
    {
        ApplyModToTarget(TakeDamageFromTarget, false, true, false);
        ApplyModToTarget(TakeDamageFromTarget, false, false, true);//Come back here and look into mod timer.
    }

    private void ApplyModToTarget(int TakeDamageFromTarget = -1, bool ApplyDmg = true, bool ApplyRawDmg = true, bool ApplyMod = true)
    {
        for (int i = 0; i < AllTargets.Count; i++)
        {
            GameObject Target = AllTargets[i];
            if (Target == null) { RemoveActiveObject(null, i); }
            else
            {
                Target.TryGetComponent(out Health TargetHealth);
                Target.TryGetComponent(out CustomPhysics TargetPhysics);

                if (TargetHealth != null)
                {
                    if (ReviveOnActivate)
                    {
                        TargetHealth.Revive(2);
                    }

                    if (ApplyDmg)
                    {
                        for (int dmg = 0; dmg < MaxDamage.Length; dmg++)
                        {
                            if (!DamageExclusive[dmg] || !CheckExclusivity(Target, Damage_HitEntities[dmg]))
                            {
                                float[] DamageValue = GI.GetDamageFromPiercingTable(TargetHealth.GetArmourType(), DamagePiercingType[dmg], Random.Range(MinDamage[dmg], MaxDamage[dmg]), 1);
                                TargetHealth.UpdateCurrentHealth(TakeDamageFromTarget, DamageIsMultiply[dmg], -DamageValue[0], -DamageValue[1], !TargetHealth.HealthIsArmour, true, GI.ArmourPenetratesFA[DamagePiercingType[dmg]]);
                            }
                        }
                    }

                    if (ApplyRawDmg)
                    {
                        for (int raw_dmg = 0; raw_dmg < RawHPDamage.Length; raw_dmg++)
                        {
                            if (!RawDamageExclusive[raw_dmg] || !CheckExclusivity(Target, RawDamage_HitEntities[raw_dmg]))
                            {
                                TargetHealth.UpdateCurrentHealth(TakeDamageFromTarget, DamageIsMultiply[raw_dmg], -RawArmourDamage[raw_dmg], -RawHPDamage[raw_dmg], !TargetHealth.HealthIsArmour, true, true);
                            }
                        }
                    }
                }

                if (TargetPhysics != null)
                {
                    for (int phys = 0; phys < ForceMax.Length; phys++)
                    {
                        /*if (TargetPhysics.IsBeingThrown)
                        {
                            TargetPhysics.SetIsBeingThrown(false, transform);
                        }*/

                        float Force = Random.Range(ForceMin[phys], ForceMax[phys]);

                        TargetPhysics.ApplyAForce(ForceDirection[phys] * Force, ForceTimeInAir[phys], ForceKnockbackToAll[phys], transform); //COME BACK TO ME LATER!
                    }
                }

                if (ApplyMod)
                {
                    //I WANT TO APPLY MOD
                    for (int mod = 0; mod < ModifierApply.Length; mod++)
                    {
                        CheckModifierType(mod);
                        //print(CheckExclusivity(Target, Mod_HitEntities[mod]));
                        //if (!Mod_Exclusive[mod] || CheckModExclusivity(Target, ModifierApply[mod]))
                        //{
                        //I AM APPLY MOD
                        //CheckModifierType(mod);
                        //}
                    }
                }
            }
        }
    }

    private bool CheckModExclusivity(GameObject entity, ModifierHandler tmp_MH)
    {
        //   GameObject entity = entity_iter;
        entity.TryGetComponent(out ModifierHistory EModHistory);

        if (EModHistory == null)
        {
            EModHistory = entity.AddComponent<ModifierHistory>();
            EModHistory.CurrentlyActiveMods.Add(gameObject.name + tmp_MH.UniqueID);
            ModPosUniqueID = EModHistory.CurrentlyActiveMods.Count;
            return true;
        }
        else
        {
            if (tmp_MH.Exclusive)
            {
                bool ModIsAlreadyInUse = false;

                foreach (string EMods in EModHistory.CurrentlyActiveMods)
                {
                    if (EMods == gameObject.name + tmp_MH.UniqueID)
                    {
                        ModIsAlreadyInUse = true;
                        break;
                    }
                }

                if (!ModIsAlreadyInUse)
                {
                    ///EMod_ApplicationScript = entity.AddComponent<ModifierApplier_HP>();
                    EModHistory.CurrentlyActiveMods.Add(gameObject.name + tmp_MH.UniqueID);
                    ModPosUniqueID = EModHistory.CurrentlyActiveMods.Count;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                EModHistory.CurrentlyActiveMods.Add(gameObject.name + tmp_MH.UniqueID);
                ModPosUniqueID = EModHistory.CurrentlyActiveMods.Count;
                return true;
            }
        }
    }

    private void CheckModifierType(int ModPos)
    {
        ModifierHandler tmp_MH = ModifierApply[ModPos];
        string modifier = tmp_MH.ModUniqueName.ToUpper();
        //tmp_MH.InitiateModValues(modifier,Overload_ModApply_ApplicationType[ModPos],Overload_ModApply_Exclusive[ModPos],Overload_ModApply_Probability[ModPos],Overload_ModApply_TargetTypeName[ModPos],Overload_ModApply_UsageType[ModPos],Overload_ModApply_Value[ModPos]);

        if (modifier == "MAXHP" || modifier == "CURRENTHP" || modifier == "HPREGEN" || modifier == "HPDEGEN" || modifier == "ARMOURDEGEN")
        {
            foreach (GameObject entity_iter in AllTargets)
            {            
                if(CheckModExclusivity(entity_iter,tmp_MH))
                {//Come back to me to look at applying for squads, types etc etc.
                    ModifierApplier_HP EMod_ApplicationScript = entity_iter.AddComponent<ModifierApplier_HP>();
                    EMod_ApplicationScript.DoModifier(ModPosUniqueID, tmp_MH.name, Overload_ModApply_ApplicationType[ModPos], Overload_ModApply_UsageType[ModPos], Overload_ModApply_Value[ModPos],  ModifierDuration[ModPos]);
                }
            }
        }
        else if(modifier == "MAXSPEED" || modifier == "CURRENTSPEED")
        {
            foreach (GameObject entity_iter in AllTargets)
            {
                if (CheckModExclusivity(entity_iter, tmp_MH))
                {//Come back to me to look at applying for squads, types etc etc.
                    ModifierApplier_Speed EMod_ApplicationScript = entity_iter.AddComponent<ModifierApplier_Speed>();
                    EMod_ApplicationScript.DoModifier(ModPosUniqueID, tmp_MH.name, Overload_ModApply_ApplicationType[ModPos], Overload_ModApply_UsageType[ModPos], Overload_ModApply_Value[ModPos], ModifierDuration[ModPos]);
                }
            }
        }
        else if (modifier == "BURSTCOUNT" || modifier == "BURSTDURATION" || modifier == "RELOADDURATION" || modifier == "BASEWEAPONDAMAGEMULTIPLIER")
        {
            foreach (GameObject entity_iter in AllTargets)
            {
                if (CheckModExclusivity(entity_iter, tmp_MH))
                {//Come back to me to look at applying for squads, types etc etc.
                    ModifierApplier_Combat EMod_ApplicationScript = entity_iter.AddComponent<ModifierApplier_Combat>();
                    EMod_ApplicationScript.DoModifier(ModPosUniqueID, tmp_MH.name, Overload_ModApply_ApplicationType[ModPos], Overload_ModApply_UsageType[ModPos], Overload_ModApply_Value[ModPos], ModifierDuration[ModPos]);
                }
            }
        }
        else
        {
            print("ERROR: In ModifierApplier/CheckModifierType; '" + modifier + "' Is not a known modifier!");
        }
    }

    private bool CheckExclusivity(GameObject CompareItem, List<GameObject> CompareList)
    {
        if (CompareList == null)
        {
            CompareList = new List<GameObject>();
        }
        else
        {
            for (int i = 0; i < CompareList.Count; i++)
            {
                if (CompareItem == CompareList[i])
                {
                    return true;
                }
            }
        }

        CompareList.Add(CompareItem);
        return false;
    }

    /*private void OnTriggerEnter(Collider other)
    {
        bool CanAddEntityToList = false;
        if (other.gameObject.layer == EntityLayer)
        {
            if(DesiredTarget != "ALL")
            {
                if (CheckEntityOwner(other.gameObject))
                {
                    CanAddEntityToList = true;
                }
            }
            else
            {
                CanAddEntityToList = true;
            }

            if (CanAddEntityToList) { ApplyActiveObject(other.gameObject); }
        }
        else
        {
            print("INVALID OBJECT: " + other.gameObject.layer + " ||  NAME: " + other.gameObject.name);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == EntityLayer)
        {
            RemoveActiveObject(other.gameObject);
        }
    }*/

    public bool CheckEntityOwner(GameObject Target, BasicInfo hitBI)
    {
        if (hitBI == null) { Target.TryGetComponent(out hitBI); }

        if (hitBI != null)
        {
            if (DesiredTarget == "ALL")
            {
                return true;
            }
            if ((DesiredTarget == "ALLIED" || DesiredTarget == "OWN") && hitBI.OwnedByPlayer == OwnedByPlayer)
            {
                return true;
            }
            else if (DesiredTarget == "ENEMY" && hitBI.OwnedByPlayer != OwnedByPlayer && hitBI.OwnedByPlayer != -1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

}
