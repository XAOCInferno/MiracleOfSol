using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RampageController : MonoBehaviour
{
    private AbilityCaster AC;
    private float SpeedBonusWhileRampage = 1;
    private bool IsActive;
    private EntityMovement EM;
    private float DebugMaxRunTime = 6;
    private float CurrentRunTime = 0;
    private float TickRate = 0.4f;
    
    public void SetRampageValues(AbilityCaster NewAC, float SpeedBonus, EntityMovement NewEM)
    {
        AC = NewAC; 
        SpeedBonusWhileRampage = SpeedBonus;
        EM = NewEM;
        if(EM == null) { EM = gameObject.GetComponent<EntityMovement>(); }
    }

    public void EnableRampage()
    {
        if(EM != null)
        {
            if (SpeedBonusWhileRampage != 1) { EM.UpdateSpeedMods(true, SpeedBonusWhileRampage, 1); }
            EM.SetIfRampage(true);
        }
        CurrentRunTime = 0;
        InvokeRepeating(nameof(DoRampageRepeating), 0, TickRate);
    }

    private void DisableRampage()
    {
        if (EM != null)
        {
            if (SpeedBonusWhileRampage != 1) { EM.UpdateSpeedMods(true, 1, 0); }
            print("EM IS DISABLED");
            EM.SetIfRampage(false);
        }
        CancelInvoke();
    }

    private void DoRampageRepeating()
    {
        CurrentRunTime += TickRate;
        if (CurrentRunTime >= DebugMaxRunTime)
        {
            DisableRampage();
        }
        else
        {
            if (EM.GetIfMoving() || CurrentRunTime <= DebugMaxRunTime / 2)
            {
                AC.ActivateAbilityExternally(null, transform.position);
            }
            else
            {
                print("RAMPAGE IS DONE");
                DisableRampage();
            }
        }
    }

}
