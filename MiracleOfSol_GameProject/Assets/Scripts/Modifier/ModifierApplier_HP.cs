using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierApplier_HP : MonoBehaviour
{
    private Health EntityHP;
    //private float ModifierChangedBy = 0;
    private int PositionInModHistory;

    private string g_Type = "";
    private string g_ApplicationType = "";
    private string g_UsageType = "";
    private float g_Value = 0; 

    public void DoModifier(int PositionInModifierHistory = 0, string Type = "__Default", string ApplicationType = "ENTITY", string UsageType = "MULTIPLICATION",  float Value = 1, float TimeUntilEnd = 0)
    {
        EntityHP = gameObject.GetComponent<Health>();
        PositionInModHistory = PositionInModifierHistory;
        Type = Type.ToUpper(); g_Type = Type;
        ApplicationType = ApplicationType.ToUpper(); g_ApplicationType = ApplicationType;
        UsageType = UsageType.ToUpper(); g_UsageType = UsageType;
        g_Value = Value;

        
        if(Type == "MAXHP")
        {
            EntityHP.MasterFunction_UpdateAllHealthModifiers(-1, true, ApplicationType, UsageType, Value, 0, 0, 0, 0, 0, 0, 0, 0);
            Invoke(nameof(ReverseModifier_MaxHP), TimeUntilEnd);
        }
        else if(Type == "CURRENTHP")
        {
            EntityHP.MasterFunction_UpdateAllHealthModifiers(-1, true, ApplicationType, UsageType, 0, Value, 0, 0, 0, 0, 0, 0, 0); 
            Invoke(nameof(ReverseModifier_CurrentHP), TimeUntilEnd);
        }
        else if(Type == "MAXFA")
        {
            EntityHP.MasterFunction_UpdateAllHealthModifiers(-1, true, ApplicationType, UsageType, 0, 0, 0, 0, Value, 0, 0, 0, 0);
            Invoke(nameof(ReverseModifier_MaxFA), TimeUntilEnd);
        }
        else if(Type == "CURRENTFA")
        {
            EntityHP.MasterFunction_UpdateAllHealthModifiers(-1, true, ApplicationType, UsageType, 0, 0, 0, 0, 0, Value, 0, 0, 0);
            Invoke(nameof(ReverseModifier_CurrentFA), TimeUntilEnd);
        }
        else if(Type == "HPREGEN")
        {
            float tmp_old_HealthValue = EntityHP.GetCurrentHPRegen();
            EntityHP.MasterFunction_UpdateAllHealthModifiers(-1, true, ApplicationType, UsageType, 0, 0, Value, 0, 0, 0, 0, 0, 0);
            Invoke(nameof(ReverseModifier_HPRegen), TimeUntilEnd);
        }
        else if (Type == "FAREGEN")
        {
            EntityHP.MasterFunction_UpdateAllHealthModifiers(-1, true, ApplicationType, UsageType, 0, 0, 0, 0, 0, 0, Value, 0, 0);
            Invoke(nameof(ReverseModifier_FARegen), TimeUntilEnd);
        }
        else if(Type == "DEATHPERCENT")
        {
            EntityHP.MasterFunction_UpdateAllHealthModifiers(-1, true, ApplicationType, UsageType, 0, 0, 0, Value, 0, 0, 0, 0, 0);
            Invoke(nameof(ReverseModifier_DeathPercent), TimeUntilEnd);
        }
        else if(Type == "HPDEGEN")
        {
            EntityHP.MasterFunction_UpdateAllHealthModifiers(-1, true, ApplicationType, UsageType, 0, 0, 0, 0, 0, 0, 0, Value, 0);
            Invoke(nameof(ReverseModifier_HPDegen), TimeUntilEnd);
        }
        else if (Type == "ARMOURDEGEN")
        {
            EntityHP.MasterFunction_UpdateAllHealthModifiers(-1, true, ApplicationType, UsageType, 0, 0, 0, 0, 0, 0, 0, 0, Value);
            Invoke(nameof(ReverseModifier_ArmourDegen), TimeUntilEnd);
        }
            
        if (g_UsageType == "MULTIPLICATION") { g_Value = -1; }//Shouldn't this be *=-1?

    }

    private void ReverseModifier_MaxHP()
    {
        EntityHP.MasterFunction_UpdateAllHealthModifiers(-1, true, g_ApplicationType, g_UsageType, -g_Value, 0, 0, 0, 0, 0, 0);

        RemoveModifierHistory_EndModifier();
    }

    private void ReverseModifier_CurrentHP()
    {
        EntityHP.MasterFunction_UpdateAllHealthModifiers(-1, true, g_ApplicationType, g_UsageType, 0, -g_Value, 0, 0, 0, 0, 0);

        RemoveModifierHistory_EndModifier();
    }

    private void ReverseModifier_MaxFA()
    {
        EntityHP.MasterFunction_UpdateAllHealthModifiers(-1, true, g_ApplicationType, g_UsageType, 0, 0, 0, 0, -g_Value, 0, 0);

        RemoveModifierHistory_EndModifier();
    }

    private void ReverseModifier_CurrentFA()
    {
        EntityHP.MasterFunction_UpdateAllHealthModifiers(-1, true, g_ApplicationType, g_UsageType, 0, 0, 0, 0, 0, -g_Value, 0);

        RemoveModifierHistory_EndModifier();
    }

    private void ReverseModifier_HPRegen()
    {
        EntityHP.MasterFunction_UpdateAllHealthModifiers(-1, true, g_ApplicationType, g_UsageType, 0, 0, -g_Value, 0, 0, 0, 0);

        RemoveModifierHistory_EndModifier();
    }

    private void ReverseModifier_FARegen()
    {
        EntityHP.MasterFunction_UpdateAllHealthModifiers(-1, true, g_ApplicationType, g_UsageType, 0, 0, 0, 0, 0, 0, -g_Value);

        RemoveModifierHistory_EndModifier();
    }

    private void ReverseModifier_DeathPercent()
    {
        EntityHP.MasterFunction_UpdateAllHealthModifiers(-1, true, g_ApplicationType, g_UsageType, 0, 0, 0, -g_Value, 0, 0, 0);

        RemoveModifierHistory_EndModifier();
    }

    private void ReverseModifier_HPDegen()
    {
        EntityHP.MasterFunction_UpdateAllHealthModifiers(-1, true, g_ApplicationType, g_UsageType, 0, 0, 0, 0, 0, -g_Value, 0);

        RemoveModifierHistory_EndModifier();
    }

    private void ReverseModifier_ArmourDegen()
    {
        EntityHP.MasterFunction_UpdateAllHealthModifiers(-1, true, g_ApplicationType, g_UsageType, 0, 0, 0, 0, 0, 0, -g_Value);

        RemoveModifierHistory_EndModifier();
    }

    private void RemoveModifierHistory_EndModifier()
    {
        ModifierHistory tmp_MH = gameObject.GetComponent<ModifierHistory>();
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
