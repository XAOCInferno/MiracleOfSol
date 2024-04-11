using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModifierApplier_Speed : MonoBehaviour
{
    private EntityMovement EntityMovement;
    //private float ModifierChangedBy = 0;
    private int PositionInModHistory;

    private string g_Type = "";
    private string g_ApplicationType = "";
    private string g_UsageType = "";
    private float g_Value = 0;

    public void DoModifier(int PositionInModifierHistory = 0, string Type = "__Default", string ApplicationType = "ENTITY", string UsageType = "MULTIPLICATION", float Value = 1, float TimeUntilEnd = 0)
    {
        gameObject.TryGetComponent(out EntityMovement);
        if (EntityMovement != null)
        {
            PositionInModHistory = PositionInModifierHistory;
            Type = Type.ToUpper(); g_Type = Type;
            ApplicationType = ApplicationType.ToUpper(); g_ApplicationType = ApplicationType;
            UsageType = UsageType.ToUpper(); g_UsageType = UsageType;
            g_Value = Value;

            
            if (Type == "MAXSPEED")
            {
                EntityMovement.MasterFunction_UpdateAllMovementModifiers(ApplicationType, UsageType, Value, 0);
                Invoke(nameof(ReverseModifier_MaxSpeed), TimeUntilEnd);
            }
            else if (Type == "CURRENTSPEED")
            {
                EntityMovement.MasterFunction_UpdateAllMovementModifiers(ApplicationType, UsageType, 0, Value);
                Invoke(nameof(ReverseModifier_CurrentSpeed), TimeUntilEnd);
            }
            if (g_UsageType == "MULTIPLICATION") { g_Value = -1; }
        }
    }

    private void ReverseModifier_MaxSpeed()
    {
        EntityMovement.MasterFunction_UpdateAllMovementModifiers(g_ApplicationType, g_UsageType, -g_Value, 0);

        RemoveModifierHistory_EndModifier();
    }

    private void ReverseModifier_CurrentSpeed()
    {
        EntityMovement.MasterFunction_UpdateAllMovementModifiers(g_ApplicationType, g_UsageType, 0, -g_Value);

        RemoveModifierHistory_EndModifier();
    }

    private void RemoveModifierHistory_EndModifier()
    {
        gameObject.TryGetComponent(out ModifierHistory tmp_MH);
        if (tmp_MH == null)
        {
            gameObject.AddComponent<ModifierHistory>();
        }
        else
        {
            int infinityBlocker = 100;
            while(infinityBlocker != 0 && tmp_MH.CurrentlyActiveMods.Count <= PositionInModHistory)
            {
                infinityBlocker--;
                PositionInModHistory--;
            }
            tmp_MH.CurrentlyActiveMods.RemoveAt(PositionInModHistory);            
        }
        Destroy(this);
    }
}
