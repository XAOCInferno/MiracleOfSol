using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierApplier_Combat : MonoBehaviour
{
    private Combat EntityCombat;
    //private float ModifierChangedBy = 0;
    private int PositionInModHistory;

    private string g_Type = "";
    private string g_ApplicationType = "";
    private string g_UsageType = "";
    private float g_Value = 0;

    public void DoModifier(int PositionInModifierHistory = 0, string Type = "__Default", string ApplicationType = "ENTITY", string UsageType = "MULTIPLICATION", float Value = 1, float TimeUntilEnd = 0)
    {
        gameObject.TryGetComponent(out EntityCombat);
        if (EntityCombat != null)
        {
            PositionInModHistory = PositionInModifierHistory;
            Type = Type.ToUpper(); g_Type = Type;
            ApplicationType = ApplicationType.ToUpper(); g_ApplicationType = ApplicationType;
            UsageType = UsageType.ToUpper(); g_UsageType = UsageType;
            g_Value = Value;

       
            if (Type == "BURSTCOUNT")
            {
                EntityCombat.MasterFunction_UpdateAllCombatModifiers(ApplicationType, UsageType, Value, 1, 1, 1);
                Invoke(nameof(ReverseModifier_BurstCount), TimeUntilEnd);
            }
            else if (Type == "BURSTDURATION")
            {
                EntityCombat.MasterFunction_UpdateAllCombatModifiers(ApplicationType, UsageType, 1, Value, 1, 1);
                Invoke(nameof(ReverseModifier_BurstDuration), TimeUntilEnd);
            }
            else if (Type == "RELOADDURATION")
            {
                EntityCombat.MasterFunction_UpdateAllCombatModifiers(ApplicationType, UsageType, 1, 1, Value, 1);
                Invoke(nameof(ReverseModifier_ReloadDuration), TimeUntilEnd);
            }
            else if (Type == "BASEWEAPONDAMAGEMULTIPLIER")
            {
                EntityCombat.MasterFunction_UpdateAllCombatModifiers(ApplicationType, UsageType, 1, 1, 1, Value);
                Invoke(nameof(ReverseModifier_BaseWeaponDamage), TimeUntilEnd);
            }
            if (g_UsageType == "MULTIPLICATION") { g_Value = -1; }
        }
    }

    private void ReverseModifier_BurstCount()
    {
        EntityCombat.MasterFunction_UpdateAllCombatModifiers(g_ApplicationType, "SET", 1, -1, -1, -1);

        RemoveModifierHistory_EndModifier();
    }

    private void ReverseModifier_BurstDuration()
    {
        EntityCombat.MasterFunction_UpdateAllCombatModifiers(g_ApplicationType, "SET", -1, 1, -1, -1);

        RemoveModifierHistory_EndModifier();
    }

    private void ReverseModifier_ReloadDuration()
    {
        EntityCombat.MasterFunction_UpdateAllCombatModifiers(g_ApplicationType, "SET", -1, -1, 1, -1);

        RemoveModifierHistory_EndModifier();
    }

    private void ReverseModifier_BaseWeaponDamage()
    {
        EntityCombat.MasterFunction_UpdateAllCombatModifiers(g_ApplicationType, "SET", -1, -1, -1, 1);

        RemoveModifierHistory_EndModifier();
    }

    private void RemoveModifierHistory_EndModifier()
    {
        gameObject.TryGetComponent(out ModifierHistory tmp_MH);
        if (tmp_MH == null)
        {
            tmp_MH = gameObject.AddComponent<ModifierHistory>();
        }
        else
        {
            int infinityBlocker = 100;
            while (infinityBlocker != 0 && tmp_MH.CurrentlyActiveMods.Count <= PositionInModHistory)
            {
                infinityBlocker--;
                PositionInModHistory--;
            }
            tmp_MH.CurrentlyActiveMods.RemoveAt(PositionInModHistory);
        }
        Destroy(this);
    }
}
