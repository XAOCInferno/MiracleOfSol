using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LevelUpCanvasManager : MonoBehaviour
{
    public TextMeshProUGUI WeaponCostText;

    public AudioClip[] WeaponUpgradeQuotesLeader;
    public AudioClip[] WeaponUpgradeQuotesPriest;
    public AudioClip[] WeaponUpgradeQuotesAziah;
    public AudioClip[] WeaponUpgradeQuotesSindyr;
    private AudioClip[][] AllWeaponUpgradeQuotes;

    public Transform HPPerkBoneParent;
    public Transform DmgPerkBoneParent;
    public Transform SupportPerkBoneParent;
    public Sprite PerkBasicImg;
    public Sprite PerkAltImg;

    public GameObject[] TerrainArt;
    private int[] MissionStatusTutorial = new int[4] { 0, 0, 0, 0 };
    private int[] MissionStatusLvl1 = new int[4] { 2, 2, 3, 2 };
    private int[] MissionStatusLvl2 = new int[4] { 2, 2, 3, 2 };
    private int[] MissionStatusLvl3 = new int[4] { 2, 2, 3, 2 };
    private int[] MissionStatusLvl4 = new int[4] { 2, 2, 3, 2 };
    private int[][] AllMissionStatus;

    public Button[] HeroInteractionButtons;
    public Sprite[] HIB_IconArt;
    public bool[] HIB_IconIsBlockedLTutorial;
    public bool[] HIB_IconIsBlockedLvl1;
    public bool[] HIB_IconIsBlockedLvl2;
    public bool[] HIB_IconIsBlockedLvl3;
    public bool[] HIB_IconIsBlockedLvl4;
    private bool[][] AllHIB_IconIsBlocked;

    public string[] HeroNames = new string[4] { "Champion Ezekiel of Sol", "Father Oba Tal", "Specialist Aziah", "Dark Wizard Sindyr" };
    public string[] HeroDescriptions_l1 = new string[4] { "Champion Desc Here...", "Priest Desc Here...", "Specialist Desc Here...", "Wizard Desc Here..." };
    public string[] HeroDescriptions_l2 = new string[4] { "Champion Desc Here...", "Priest Desc Here...", "Specialist Desc Here...", "Wizard Desc Here..." };
    public string[] HeroMotives = new string[4] { "Champion Motive Here...", "Priest Motive Here...", "Specialist Motive Here...", "Wizard Motive Here..." };
    public GameObject[] InScene_HeroModels;
    public List<UpdateWeaponArt> InScene_HeroModels_WeaponArt;
    public EBP_Info[] EBPs;

    public bool[] WeaponIsASpecialGroup;
    public WeaponInformation[] WeaponsPrimary;
    public WeaponInformation[] WeaponsSecondary;
    public WeaponInformation[] WeaponsFinal;

    public WeaponInformation[] SpecialWeaponsGroupPrimary;
    public WeaponInformation[] SpecialWeaponsGroupSecondary;
    public WeaponInformation[] SpecialWeaponsGroupFinal;


    public Button[] LevelUpButtons;

    public TextMeshProUGUI NameBlock;
    public TextMeshProUGUI DescBlock_l1;
    public TextMeshProUGUI DescBlock_l2;

    public TextMeshProUGUI MotiveBlock;

    public TextMeshProUGUI HeroLevelBlock;
    public TextMeshProUGUI HeroUnusedPerksBlock;
    public TextMeshProUGUI HeroHPPerksBlock;
    public TextMeshProUGUI HeroDmgPerksBlock;
    public TextMeshProUGUI HeroSupportPerksBlock;
    public TextMeshProUGUI CurrentWeaponPartsBlock;

    public TextMeshProUGUI HPStatBlock;
    public TextMeshProUGUI FAStatBlock;
    public TextMeshProUGUI HPRegenStatBlock;
    public TextMeshProUGUI FARegenStatBlock;
    public TextMeshProUGUI WeaponNameStatBlock;
    public TextMeshProUGUI DPSStatBlock;
    public TextMeshProUGUI SpecialSecondary_WeaponNameStatBlock;
    public TextMeshProUGUI SpecialSecondary_DPSStatBlock;


    private List<ProductionButtonInfo> LvlUpPBIs = new List<ProductionButtonInfo>();

    private int[] Hero_Level;
    private int[] Hero_UnusedPerks;
    private int[] Hero_HPPerkCount;
    private int[] Hero_DmgPerkCount;
    private int[] Hero_SupportPerkCount;
    private int[] CurrentEquippedWeapon;
    private int[] WeaponUpgradeCosts = new int[2] { 250, 500 };

    private int CurrentSelectedEntity = 0;
    private int CurrentWeaponParts;

    private Image[] HPPerkPointImg;
    private Image[] DMGPerkPointImg;
    private Image[] SupportPerkPointImg;
    private AudioSourceController ASC;

    // Start is called before the first frame update
    void Start()
    {
        AllWeaponUpgradeQuotes = new AudioClip[4][] { WeaponUpgradeQuotesLeader, WeaponUpgradeQuotesPriest, WeaponUpgradeQuotesAziah, WeaponUpgradeQuotesSindyr };

        Hero_Level = PlayerPrefsX.GetIntArray("HeroLevelStatus", 0, 4);
        Hero_Level = new int[4] { Mathf.Clamp(Hero_Level[0] + 1, 1, 10), Mathf.Clamp(Hero_Level[1] + 1, 1, 10), Mathf.Clamp(Hero_Level[2] + 1, 1, 10), Mathf.Clamp(Hero_Level[3] + 1, 1, 10) };
        Hero_UnusedPerks = PlayerPrefsX.GetIntArray("HeroUnusedPerks", 0, 4);
        Hero_UnusedPerks = new int[4] { Mathf.Clamp(Hero_UnusedPerks[0] + 1, 1, 40), Mathf.Clamp(Hero_UnusedPerks[1] + 1, 1, 40), Mathf.Clamp(Hero_UnusedPerks[2] + 1, 1, 40), Mathf.Clamp(Hero_UnusedPerks[3] + 1, 1, 40) };
        Hero_HPPerkCount = PlayerPrefsX.GetIntArray("HeroHPPerkCount", 0, 4);
        Hero_DmgPerkCount = PlayerPrefsX.GetIntArray("HeroDmgPerkCount", 0, 4);
        Hero_SupportPerkCount = PlayerPrefsX.GetIntArray("HeroSupportPerkCount", 0, 4);
        CurrentEquippedWeapon = PlayerPrefsX.GetIntArray("HeroEquippedWeapons", 0, 4);
        CurrentWeaponParts = PlayerPrefs.GetInt("WeaponPartsCount", 0);
        CurrentWeaponParts += Random.Range(50, 125);

        AllMissionStatus = new int[5][] { MissionStatusTutorial, MissionStatusLvl1, MissionStatusLvl2, MissionStatusLvl3, MissionStatusLvl4 };
        AllHIB_IconIsBlocked = new bool[5][] { HIB_IconIsBlockedLTutorial, HIB_IconIsBlockedLvl1, HIB_IconIsBlockedLvl2, HIB_IconIsBlockedLvl3, HIB_IconIsBlockedLvl4 };

        HPPerkPointImg = HPPerkBoneParent.GetComponentsInChildren<Image>();
        DMGPerkPointImg = DmgPerkBoneParent.GetComponentsInChildren<Image>();
        SupportPerkPointImg = SupportPerkBoneParent.GetComponentsInChildren<Image>();
        gameObject.TryGetComponent(out ASC);

        foreach(GameObject tmpBlock in InScene_HeroModels)
        {
            InScene_HeroModels_WeaponArt.Add(tmpBlock.GetComponent<UpdateWeaponArt>());
        }

        SetNewHeroSelection(0);
        for (int i = 0; i < HeroInteractionButtons.Length; i++)
        {
            if (AllHIB_IconIsBlocked[GetCurrentLevel()][i])
            {
                HeroInteractionButtons[i].gameObject.SetActive(false);
            }
            else
            {
                HeroInteractionButtons[i].image.sprite = HIB_IconArt[i];
                ProductionButtonInfo PBI = HeroInteractionButtons[i].gameObject.AddComponent<ProductionButtonInfo>();
                PBI.EntityToProduce = i;

                HeroInteractionButtons[i].onClick.AddListener(delegate { SetNewHeroSelection(PBI.EntityToProduce); });
            }
        }

        for (int i = 0; i < LevelUpButtons.Length; i++)
        {
            ProductionButtonInfo tmp_PBI = LevelUpButtons[i].gameObject.AddComponent<ProductionButtonInfo>();
            LvlUpPBIs.Add(tmp_PBI);
        }
    }

    public void SetNewHeroSelection(int Pos)
    {
        CurrentSelectedEntity = Pos;
        NameBlock.text = HeroNames[Pos];
        DescBlock_l1.text = HeroDescriptions_l1[Pos];
        DescBlock_l2.text = HeroDescriptions_l2[Pos];
        MotiveBlock.text = HeroMotives[Pos];

        InScene_HeroModels_WeaponArt[CurrentSelectedEntity].UpdateWeaponArtLvlScreen(CurrentEquippedWeapon[CurrentSelectedEntity]);
        UpdatePerkArt(true,true,true);

        for (int i = 0; i < InScene_HeroModels.Length; i++)
        {
            if (i != Pos) { InScene_HeroModels[i].SetActive(false); } else { InScene_HeroModels[i].SetActive(true); }
        }

        for (int i = 0; i < LvlUpPBIs.Count; i++)
        {
            LvlUpPBIs[i].EntityToProduce = Pos;
        }

        for(int i = 0; i < TerrainArt.Length; i++)
        {
            TerrainArt[i].SetActive(false);
        }

        TerrainArt[AllMissionStatus[GetCurrentLevel()][Pos]].SetActive(true);
    }

    private int GetCurrentLevel()
    {
        string tmpLevel = PlayerPrefs.GetString("NextLevelToRun", "Mission01");

        if (tmpLevel == "Mission00")
        {
            return 0;
        }
        else if (tmpLevel == "Mission01")
        {
            return 1;
        }
        else if (tmpLevel == "Mission02")
        {
            return 2;
        }
        else if (tmpLevel == "Mission03")
        {
            return 3;
        }
        else if (tmpLevel == "MainMenu")
        {
            return 2;
        }
        else
        {
            return 4;
        }
    }

    private void SetPerkArt(bool AsHP = false, bool AsDmg = false, bool AsSupport = false)
    {
        if (AsHP)
        {
            CommitPerkArt(HPPerkPointImg, Hero_HPPerkCount);
        }

        if (AsDmg)
        {
            CommitPerkArt(DMGPerkPointImg, Hero_DmgPerkCount);
        }

        if (AsSupport)
        {
            CommitPerkArt(SupportPerkPointImg, Hero_SupportPerkCount);
        }
    }

    private void CommitPerkArt(Image[] PerkPointList, int[] PerkList)
    {
        for (int i = 0; i < PerkPointList.Length; i++)
        {
            if (i < PerkList[CurrentSelectedEntity])
            {
                PerkPointList[i].sprite = PerkAltImg;
            }
            else
            {
                PerkPointList[i].sprite = PerkBasicImg;
            }
        }
    }

    public void ResetPerks()
    {
        Hero_UnusedPerks[CurrentSelectedEntity] = Hero_Level[CurrentSelectedEntity];
        Hero_HPPerkCount[CurrentSelectedEntity] = 0;
        Hero_DmgPerkCount[CurrentSelectedEntity] = 0;
        Hero_SupportPerkCount[CurrentSelectedEntity] = 0;
        UpdatePerkArt(true,true,true);
    }

    public void ChangeHPPerk()
    {
        if (Hero_UnusedPerks[CurrentSelectedEntity] > 0)
        {
            Hero_UnusedPerks[CurrentSelectedEntity]--;
            Hero_HPPerkCount[CurrentSelectedEntity]++;
            UpdatePerkArt(true,false,false);
        }
    }

    public void ChangeDmgPerk()
    {
        if (Hero_UnusedPerks[CurrentSelectedEntity] > 0)
        {
            Hero_UnusedPerks[CurrentSelectedEntity]--;
            Hero_DmgPerkCount[CurrentSelectedEntity]++;
            UpdatePerkArt(false,true,false);
        }
    }

    public void ChangeSupportPerk()
    {
        if (Hero_UnusedPerks[CurrentSelectedEntity] > 0)
        {
            Hero_UnusedPerks[CurrentSelectedEntity]--;
            Hero_SupportPerkCount[CurrentSelectedEntity]++;
            UpdatePerkArt(false,false,true);
        }
    }

    public void GoToNextLevel()
    {
        PlayerPrefsX.SetIntArray("HeroLevelStatus", Hero_Level);
        PlayerPrefsX.SetIntArray("HeroUnusedPerks", Hero_UnusedPerks);
        PlayerPrefsX.SetIntArray("HeroHPPerkCount", Hero_HPPerkCount);
        PlayerPrefsX.SetIntArray("HeroDmgPerkCount", Hero_DmgPerkCount);
        PlayerPrefsX.SetIntArray("HeroSupportPerkCount", Hero_SupportPerkCount);
        PlayerPrefsX.SetIntArray("HeroEquippedWeapons", CurrentEquippedWeapon);
        PlayerPrefs.SetInt("WeaponPartsCount", CurrentWeaponParts);

        if(PlayerPrefs.GetString("NextLevelToRun", "Mission01") == "MainMenu")
        {
            PlayerPrefsX.SetBool("GameIsWon", true);
            UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
        }

        try
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene(PlayerPrefs.GetString("NextLevelToRun", "Mission01"));
        }
        catch
        {
            PlayerPrefs.SetString("NextLevelToRun", "Mission02");
            UnityEngine.SceneManagement.SceneManager.LoadScene("Mission01");
        }
    }

    public void DEV_AddAPerk()
    {
        Hero_UnusedPerks[CurrentSelectedEntity]++;
        UpdatePerkArt(false,false,false);
    }

    private void UpdatePerkArt(bool AsHP, bool AsDMG, bool AsSupport)
    {
        HeroUnusedPerksBlock.text = "Perks:" + Hero_UnusedPerks[CurrentSelectedEntity].ToString();
        HeroLevelBlock.text = "Lvl:" + Hero_Level[CurrentSelectedEntity].ToString();
        HeroHPPerksBlock.text = Hero_HPPerkCount[CurrentSelectedEntity].ToString();
        HeroDmgPerksBlock.text = Hero_DmgPerkCount[CurrentSelectedEntity].ToString();
        HeroSupportPerksBlock.text = Hero_SupportPerkCount[CurrentSelectedEntity].ToString();
        CurrentWeaponPartsBlock.text = CurrentWeaponParts.ToString();
        UpdateStats();
        SetPerkArt(AsHP, AsDMG, AsSupport);
    }

    public void OrderForgeNewWeapon()
    {
        if (CurrentEquippedWeapon[CurrentSelectedEntity] < WeaponUpgradeCosts.Length)
        {
            int Cost = WeaponUpgradeCosts[CurrentEquippedWeapon[CurrentSelectedEntity]];
            if (CurrentWeaponParts >= Cost)
            {
                CommitNewWeapon(Cost);
            }
        }
    }

    private void CommitNewWeapon(int Cost)
    {
        CurrentWeaponParts -= Cost;
        CurrentEquippedWeapon[CurrentSelectedEntity]++;
        UpdateWeaponPartsArt();
    }

    private void UpdateWeaponPartsArt()
    {
        SetNewHeroSelection(CurrentSelectedEntity);
        ASC.OrderNewSound(AllWeaponUpgradeQuotes[CurrentSelectedEntity][CurrentEquippedWeapon[CurrentSelectedEntity] - 1], new float[2] { 0, 1000000 }, true, false, false);
    }

    private void UpdateStats()
    {
        HPStatBlock.text = "HP: " + (EBPs[CurrentSelectedEntity].MaxHP + 100 * (Hero_Level[CurrentSelectedEntity] + Hero_HPPerkCount[CurrentSelectedEntity])).ToString();
        FAStatBlock.text = "FA: " + (EBPs[CurrentSelectedEntity].MaxArmour + 25 * (Hero_Level[CurrentSelectedEntity] + Hero_HPPerkCount[CurrentSelectedEntity])).ToString();
        HPRegenStatBlock.text = "HP Regen: " + (EBPs[CurrentSelectedEntity].HPRegen + (Hero_Level[CurrentSelectedEntity] + Hero_HPPerkCount[CurrentSelectedEntity])).ToString();
        FARegenStatBlock.text = "FA Regen: " + (EBPs[CurrentSelectedEntity].ArmourRegen + 0.25f * (Hero_Level[CurrentSelectedEntity] + Hero_HPPerkCount[CurrentSelectedEntity])).ToString();

        WeaponInformation CurrentWeapon;
        WeaponInformation SecondaryCurrentWeapon = SpecialWeaponsGroupPrimary[1];

        if (CurrentEquippedWeapon[CurrentSelectedEntity] == 0)
        {
            if (WeaponIsASpecialGroup[CurrentSelectedEntity])
            {
                CurrentWeapon = SpecialWeaponsGroupPrimary[0];
            }
            else
            {
                CurrentWeapon = WeaponsPrimary[CurrentSelectedEntity];
            }
        }
        else if (CurrentEquippedWeapon[CurrentSelectedEntity] == 1)
        {
            if (WeaponIsASpecialGroup[CurrentSelectedEntity])
            {
                CurrentWeapon = SpecialWeaponsGroupSecondary[0];
                SecondaryCurrentWeapon = SpecialWeaponsGroupSecondary[1];
            }
            else
            {
                CurrentWeapon = WeaponsSecondary[CurrentSelectedEntity];
            }
        }
        else
        {
            if (WeaponIsASpecialGroup[CurrentSelectedEntity])
            {
                CurrentWeapon = SpecialWeaponsGroupFinal[0];
                SecondaryCurrentWeapon = SpecialWeaponsGroupFinal[1];
            }
            else
            {
                CurrentWeapon = WeaponsFinal[CurrentSelectedEntity];
            }
        }

        DPSStatBlock.text = "DPS: " + GetAvgWeaponDamage(CurrentWeapon).ToString();
        WeaponNameStatBlock.text = CurrentWeapon.WeaponName;

        if (CurrentEquippedWeapon[CurrentSelectedEntity] != 2)
        {
            WeaponCostText.text = "--" + WeaponUpgradeCosts[CurrentEquippedWeapon[CurrentSelectedEntity]].ToString();
        }
        else
        {
            WeaponCostText.text = "AT MAX!";
        }

        if (WeaponIsASpecialGroup[CurrentSelectedEntity])
        {
            SpecialSecondary_DPSStatBlock.text = "DPS: " + GetAvgWeaponDamage(SecondaryCurrentWeapon).ToString();
            SpecialSecondary_WeaponNameStatBlock.text = SecondaryCurrentWeapon.WeaponName;
        }
        else
        {
            SpecialSecondary_DPSStatBlock.text = "";
            SpecialSecondary_WeaponNameStatBlock.text = "";
        }
    }

    private float GetAvgWeaponDamage(WeaponInformation Weapon)
    {
        float MinDmg = Weapon.MinDamage * (1 + 0.025f * Hero_Level[CurrentSelectedEntity] + 0.05f * Hero_DmgPerkCount[CurrentSelectedEntity]); 
        float MaxDmg = Weapon.MaxDamage * (1 + 0.025f * Hero_Level[CurrentSelectedEntity] + 0.05f * Hero_DmgPerkCount[CurrentSelectedEntity]); 
        float BaseAccuracy = Weapon.Accuracy;
        float AvgRawDmg = (MinDmg + MaxDmg) / 2 * BaseAccuracy;
        float ShotsPerSecond = Weapon.ShotsPerBurst / Weapon.DurationPerBurst;
        AvgRawDmg *= ShotsPerSecond;
        int FinalDamage = (int)AvgRawDmg;

        return FinalDamage;
    }
}
