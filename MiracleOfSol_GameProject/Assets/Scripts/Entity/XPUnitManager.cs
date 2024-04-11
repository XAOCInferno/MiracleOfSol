using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XPUnitManager : MonoBehaviour
{
    public GameObject VFXPlayOnLvlUp;
    public Transform VFXLvlUpBone;

    public int CurrentUnusedPerks = 0;
    public int CurrentXPLevel = 0;
    public float CurrentXP = 0;
    public int EnableMeleeChargeAtLevel = 0;
    public int EnableMeleeLeapAtLevel = 0;

    private int MaxLevel = 10;
    private int XPForNextLevel = 500;
    private XP_MasterStorage XP_MS;
    private EBP_Info self_EBP;
    private VoiceLineManager VLM;

    private int[] AllHeroLevels;
    private int[] AllHeroUnusuedPerks;
    private int[] AllHeroCurrentXP;

    private void Start()
    {
        self_EBP = gameObject.GetComponent<BasicInfo>().EBPs;
        VLM = gameObject.GetComponent<VoiceLineManager>();
        XP_MS = GameObject.FindWithTag("GameController").GetComponent<GameInfo>().GetComponent<XP_MasterStorage>();

        AllHeroLevels = PlayerPrefsX.GetIntArray("HeroLevelStatus", 0, 4);
        AllHeroUnusuedPerks = PlayerPrefsX.GetIntArray("HeroUnusedPerks", 0, 4);
        CurrentXPLevel = AllHeroLevels[self_EBP.PositionInLvlHierarchy];
        CurrentUnusedPerks = AllHeroUnusuedPerks[self_EBP.PositionInLvlHierarchy];

        AllHeroCurrentXP = PlayerPrefsX.GetIntArray("Hero_CurrentXP", 0, 4);
        CurrentXP = AllHeroCurrentXP[self_EBP.PositionInLvlHierarchy];
        XPForNextLevel = XP_MS.GetXPRequirement(CurrentXPLevel + 1);
    }

    public void AddXP(float ChangeBy)
    {
        if (CurrentXPLevel < MaxLevel)
        {
            CurrentXP += ChangeBy;
            if (CurrentXP >= XPForNextLevel)
            {
                LevelUpLogic();
            }
        }
        else
        {
            CurrentXP = 0;
        }
    }

    private void LevelUpLogic()
    {
        GameObject tmpVFX = Instantiate(VFXPlayOnLvlUp);
        tmpVFX.transform.position = VFXLvlUpBone.position;
        tmpVFX.transform.rotation = new Quaternion();
        VLM.PlayVoiceLineOfType("LEVELUP");
        CurrentXP = 0;
        CurrentXPLevel++;
        CurrentUnusedPerks++;

        AllHeroLevels = PlayerPrefsX.GetIntArray("HeroLevelStatus", 0, 4);
        AllHeroLevels[self_EBP.PositionInLvlHierarchy] = CurrentXPLevel;

        AllHeroUnusuedPerks = PlayerPrefsX.GetIntArray("Hero_UnusedPerks", 0, 4);
        AllHeroUnusuedPerks[self_EBP.PositionInLvlHierarchy] = CurrentUnusedPerks;

        XP_MS.GetXPRequirement(CurrentXPLevel + 1);
    }
}
