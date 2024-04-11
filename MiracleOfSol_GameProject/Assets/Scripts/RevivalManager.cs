using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevivalManager : MonoBehaviour
{
    public CapturePoint CP;

    private int HeroType;
    private Health TargetHealth;

    public void SetupRevivalManger(Health tmpHealth, int NewHeroType)//inherit cap faction if need ai to revive
    {
        TargetHealth = tmpHealth;
        HeroType = NewHeroType;
    }

    private void Update()
    {
        try
        {
            if (CP.GetIfCaptured(0))//replace 0 with the cap faction if you require ai to revive
            {
                TargetHealth.Revive(CP.CurrentCappingEntity.gameObject.GetComponent<BasicInfo>().EBPs.PositionInLvlHierarchy);
                Destroy(gameObject);
            }
            else if (!TargetHealth.GetIfIncapacitated())
            {
                Destroy(gameObject);
            }
        }
        catch
        {
            Debug.LogWarning("ERROR! In RevivalManager/Update. CP probably is null.");
        }
    }

    public int GetHeroType() { return HeroType; }
}

