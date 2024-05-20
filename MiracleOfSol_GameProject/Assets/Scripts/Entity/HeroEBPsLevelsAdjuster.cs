using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeroEBPsLevelsAdjuster : MonoBehaviour
{

    private int Hero_Level;
    private int Hero_HPPerkCount;
    private int Hero_DmgPerkCount;
    private int Hero_SupportPerkCount;

    private int HeroPos;

    private Health selfHealth;
    private Combat selfCombat;
    private AbilityCaster[] selfACs;

    private float BaseHP;
    private float BaseFA;
    private float BaseHPRegen;
    private float BaseFARegen;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.TryGetComponent(out BasicInfo tmpBI);
        gameObject.TryGetComponent(out selfHealth);
        gameObject.TryGetComponent(out selfCombat);

        HeroPos = tmpBI.EBPs.PositionInLvlHierarchy;
        BaseHP = selfHealth.GetMaxHP();
        BaseFA = selfHealth.GetMaxArmour();
        BaseHPRegen = selfHealth.GetCurrentHPRegen();
        BaseFARegen = selfHealth.GetCurrentFARegen();

        if (HeroPos >= 0)
        {
            SetAllMods();
        }
        else
        {
            AISetAllMods();
        }
    }

    public void SetAllMods()
    {
        Hero_Level = PlayerPrefsX.GetIntArray("HeroLevelStatus", 0, 4)[HeroPos];

        int tmp_UnusedPerks = PlayerPrefsX.GetIntArray("HeroUnusedPerks", 0, 4)[HeroPos];
        Hero_HPPerkCount = PlayerPrefsX.GetIntArray("HeroHPPerkCount", 0, 4)[HeroPos];
        Hero_DmgPerkCount = PlayerPrefsX.GetIntArray("HeroDmgPerkCount", 0, 4)[HeroPos];
        Hero_SupportPerkCount = PlayerPrefsX.GetIntArray("HeroSupportPerkCount", 0, 4)[HeroPos];

        selfHealth.UpdateBlueprint(true, 100 * (Hero_Level + Hero_HPPerkCount) + BaseHP, Hero_Level + Hero_HPPerkCount + BaseHPRegen, 0, 25 * (Hero_Level + Hero_HPPerkCount) + BaseFA, 0.25f * (Hero_Level + Hero_HPPerkCount) + BaseFARegen, 0, 0);
        selfCombat.BaseWeaponDamageMultiplier = 1 + 0.025f * Hero_Level + 0.05f * Hero_DmgPerkCount;
    }

    public void AISetAllMods()
    {
        int[] tmpLevels = PlayerPrefsX.GetIntArray("HeroLevelStatus", 0, 4);
        

        int AllPerkCount = 0;
        foreach (int level in tmpLevels)
        {
            AllPerkCount += level;
        }

        int AvgPerkCount = AllPerkCount / tmpLevels.Length;

        selfHealth.UpdateBlueprint(true, 100 * AvgPerkCount + BaseHP, AvgPerkCount + BaseHPRegen, 0, 25 * AvgPerkCount + BaseFA, 0.25f * AvgPerkCount + BaseFARegen, 0, 0);
        selfCombat.BaseWeaponDamageMultiplier = 1 + 0.01f * AvgPerkCount;
    }
}
