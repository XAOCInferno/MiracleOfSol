using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialAbility_ShiftWeaponry : MonoBehaviour
{
    public GameObject[] ChangeStateOnSwap;

    public Vector2 PositionInAbilityUI = new Vector2(0, 0);
    public Sprite UI_Icon;
    public Sprite UI_IconAlt;
    public KeyCode AbilityHK = KeyCode.Q;
    public string AbilityDisplayName = "Equip: ";
    public string AbilityName = "Special_SwapWeaponry";
    public string AbilityDesc = "Effective against: ";
    public float[] FireCost = new float[3] { 0, 0, 0 };
    public float EntityBusyTime = 0;

    public List<WeaponInformation> AllAltWeapons = new List<WeaponInformation>();
    public List<WeaponInformation> AllAltWeapons01 = new List<WeaponInformation>();
    public List<WeaponInformation> AllAltWeapons02 = new List<WeaponInformation>();
    public List<WeaponInformation> AllAltWeapons03 = new List<WeaponInformation>();
    public List<WeaponInformation> AllAltWeapons04 = new List<WeaponInformation>();
    public List<WeaponInformation> AllAltWeapons05 = new List<WeaponInformation>();
    public List<WeaponInformation> AllAltWeapons06 = new List<WeaponInformation>();
    public List<WeaponInformation> AllAltWeapons07 = new List<WeaponInformation>();
    public List<WeaponInformation> AllAltWeapons08 = new List<WeaponInformation>();
    public List<WeaponInformation> AllAltWeapons09 = new List<WeaponInformation>();

    private List<List<WeaponInformation>> AllAltWeaponGroups = new List<List<WeaponInformation>>();

    private Combat self_Combat;
    private string SavedAbilityName;
    private string SavedAbilityDesc;
    public bool IsSwapped = false;

    private void Start()
    {
        self_Combat = gameObject.GetComponent<Combat>();
        AllAltWeaponGroups.Add(AllAltWeapons); AllAltWeaponGroups.Add(AllAltWeapons01); AllAltWeaponGroups.Add(AllAltWeapons02); AllAltWeaponGroups.Add(AllAltWeapons03); AllAltWeaponGroups.Add(AllAltWeapons04);
        AllAltWeaponGroups.Add(AllAltWeapons05); AllAltWeaponGroups.Add(AllAltWeapons06); AllAltWeaponGroups.Add(AllAltWeapons07); AllAltWeaponGroups.Add(AllAltWeapons08); AllAltWeaponGroups.Add(AllAltWeapons09);

        for (int i = 0; i < AllAltWeaponGroups.Count; i++)
        {
            for (int j = 0; j < AllAltWeaponGroups[i].Count; j++)
            {
                AllAltWeaponGroups[i][j] = GameObject.FindWithTag("GameController").GetComponent<Weapon_BackupData>().ACTIVE_AllWeapon[AllAltWeaponGroups[i][j].PositionInGMData].GetComponent<WeaponInformation>();
            }
        }

        SavedAbilityName = AbilityDisplayName; SavedAbilityDesc = AbilityDesc;
        AbilityDisplayName = SavedAbilityName + AllAltWeapons[0].WeaponName;
        AbilityDesc = SavedAbilityDesc + AllAltWeapons[0].WeaponEffectiveAgainst;
    }

    public void ChangeButtonDisplay(bool ChangeIcon = false, bool ChangeDescription = false)
    {
        if (ChangeIcon) { SetNewIcon(); }
        if (ChangeDescription) { SetNewWeaponStrings(); }
    }

    public void ShiftWeapons()
    {
        IsSwapped = !IsSwapped;
        ChangeButtonDisplay(true, true);
        List<List<WeaponInformation>> TempStoreWeapons = self_Combat.AllWeaponGroups;
        self_Combat.AllWeaponGroups = AllAltWeaponGroups;
        foreach(WeaponInformation tmpWeapon in self_Combat.AllWeaponGroups[0]) { tmpWeapon.SetupParentCombat(self_Combat); }
        foreach (WeaponInformation tmpWeapon in self_Combat.AllWeaponGroups[1]) { tmpWeapon.SetupParentCombat(self_Combat); }
        foreach (WeaponInformation tmpWeapon in self_Combat.AllWeaponGroups[2]) { tmpWeapon.SetupParentCombat(self_Combat); }
        AllAltWeaponGroups = TempStoreWeapons;
        self_Combat.UI_SetIndicator();

        foreach(GameObject tmpObj in ChangeStateOnSwap)
        {
            tmpObj.SetActive(!tmpObj.activeSelf);
        }
    }

    private void SetNewWeaponStrings()
    {
        if (self_Combat.AllWeaponGroups_ActiveStatus != null)
        {
            bool HasSetData = false;

            for (int i = 0; i < self_Combat.AllWeaponGroups_ActiveStatus.Count; i++)
            {
                if (self_Combat.AllWeaponGroups_ActiveStatus[i])
                {
                    AbilityDisplayName = SavedAbilityName + self_Combat.AllWeaponGroups[i][0].WeaponName;
                    AbilityDesc = SavedAbilityDesc + self_Combat.AllWeaponGroups[i][0].WeaponEffectiveAgainst;
                    HasSetData = true;
                    break;
                }
            }

            if (!HasSetData)
            {
                AbilityDisplayName = SavedAbilityName + AllAltWeapons[0].WeaponName;
                AbilityDesc = SavedAbilityDesc + AllAltWeapons[0].WeaponEffectiveAgainst;
            }
        }
        else
        {
            AbilityDisplayName = SavedAbilityName + AllAltWeapons[0].WeaponName;
            AbilityDesc = SavedAbilityDesc + AllAltWeapons[0].WeaponEffectiveAgainst;
        }
    }

    private void SetNewIcon()
    {
        Sprite SavedIcon = UI_Icon;
        UI_Icon = UI_IconAlt;
        UI_IconAlt = SavedIcon;
    }
}
