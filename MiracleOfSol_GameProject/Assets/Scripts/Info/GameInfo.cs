using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameInfo : MonoBehaviour
{
    public bool SpecialSindyrSpeechIsDone = false;


    public GameObject MainCamera;
    public GameObject ResourceManager;
    public GameObject UI_Canvas;
    public GameObject ProductionUIMaster;
    public GameObject ProductionUI;
    public GameObject AbilityUIMaster;
    public GameObject AbilityUI;
    public GameObject JumpUIMaster;
    public GameObject JumpUI;
    public GameObject VFX_Storage;
    public GameObject GenericAbilityCursorVFX;
    public GameObject ProjectileDefault;
    public GameObject ResourcePointMaster;
    public GameObject DefaultObjectModel;
    public GameObject CombatManager;
    public GameObject GlobalVoicePlayer;
    public GameObject PickRaceManager;
    public GameObject RevivalCapturePoint;
    public GameObject EUS_UI_CurrentlySelectedParent;
    public GameObject SpeechCutsceneBubbleCanvas;
    public GameObject SpeechCutsceneBubble;
    public DeathExplosionManager DEM;
    public GameObject MenuCanvas;

    public Collider[] AllJumpBlockers;
    public List<GameObject> AllPlayers = new List<GameObject>();
    public List<SquadManager> AllPlayers_SM = new List<SquadManager>();
    public Sprite DefaultMMDot;
    public CameraFade CamFade;
    public Transform[] AI_CriticalDefencePoints;
    public List<int> MaxSquadsToDefendCrits;
    public LootManager LootManagerGlobal;


    public int HPRegenLuxCoverPos = 0;
    public int HPDegenLuxCoverPos = 1;
    public int AccuracyBonusLuxCoverPos = 2;
    public int DamageBonusLuxCoverPos = 3;
    public int DamageResistLuxCoverPos = 4;
    public int SpeedBonusLuxCoverPos = 5;

    public bool UITimeout = false;
    public bool IsInCutscene = false;
    public int CurrentMission;

    private List<Vector3> AI_CriticalDefenceLocations = new List<Vector3>();
    private bool IsCheckingForCamFade = false;


    private void Start()
    {
        gameObject.TryGetComponent(out LootManagerGlobal);
        CurrentMission = GetCurrentLevel();

        foreach (Transform crit in AI_CriticalDefencePoints)
        {
            AI_CriticalDefenceLocations.Add(crit.position);
            Destroy(crit.gameObject);
        }

        if (MainCamera == null) { MainCamera = Camera.main.gameObject; }
        if(ResourceManager == null) { ResourceManager = GameObject.Find("RESOURCE_MANAGER"); }
        AllLuxCoverModifierHolders = new float[6][][] { HPRegenBonusFromLuxCover, HPDegenBonusFromLuxCover, AccuracyBonusFromLuxCover, DamageBonusFromLuxCover, DamageResistFromLuxCover, SpeedBonusFromLuxCover }; 
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
        else
        {
            return 4;
        }
    }

    private float[][][] ArmourPiercingTypes = new float[9][][] { 
           new float[2][] { new float[1] { 0.5f }, new float[15] {  1f,     1f,    1f,    0.5f,  0.5f,  0.2f,  1f,    0.5f,  0.2f,  0.2f,  0.05f,  0.01f,  0.5f,  0.05f, 0.5f } }, //Basic
           new float[2][] { new float[1] { 0.7f }, new float[15] {  1f,     1f,    1f,    0.5f,  0.5f,  0.2f,  1f,    0.5f,  0.2f,  0.5f,  0.2f,   0.1f,   1f,    0.1f,  0.4f } }, //Armour Shredding
           new float[2][] { new float[1] { 0.2f }, new float[15] {  0.5f,   0.5f,  0.5f,  0.5f,  0.5f,  0.2f,  0.5f,  0.5f,  0.2f,  0.01f, 0.01f,  0.01f,  0.5f,  0.05f, 0.4f } }, //Armour Piercing
           new float[2][] { new float[1] { 0.4f }, new float[15] {  2f,     2f,    2f,    2f,    1f,    4f,  1f,    0.5f,  0.2f,  0.4f,  0.1f,   0.05f,  1f,    0.10f, 0.5f } }, //Daemonic
           new float[2][] { new float[1] { 0.6f }, new float[15] {  2f,     2f,    2f,    1f,    2f,    1f,    1f,    1f,    1f,    0.6f,  0.4f,   0.2f,   2f,    0.15f, 1f } }, //Heavy
           new float[2][] { new float[1] { 1f },   new float[15] {  0.1f,   0.1f,  0.1f,  0.1f,  0.2f,  0.5f,  0.1f,  0.1f,  1f,    1f,    2f,     1f,     2f,    0.3f,  0.5f } }, //Anti-Tank
           new float[2][] { new float[1] { 1f },   new float[15] {  0.5f,   0.5f,  0.5f,  0.2f,  0.5f,  0.5f,  0.5f,  0.5f,  0.2f,  0.5f,  0.2f,   0.2f,   0.5f,  0.05f, 0.1f } }, //Explosive
           new float[2][] { new float[1] { 0.1f }, new float[15] {  2f,     2f,    2f,    0.5f,  1f,    0.2f,  0.5f,  0.2f,  0.1f,  0.05f, 0.02f,  0.01f,  0.2f,  0.02f, 1f } }, //Biological
           new float[2][] { new float[1] { 1.5f }, new float[15] {  10f,    10f,   10f,   5f,    5f,    0.8f,  8f,    8f,    0.1f,  0.05f, 0.02f,  0.01f,  0.2f,  0.02f, 5f } } //SpecialCampaignTurret
                                       };//F.A                      I L     I M    I H    C      I H.M  I H.H  D L    D M    D H    V L    V M     V H     B L    B H    Boss   
    
    public bool[] ArmourPenetratesFA = new bool[9] { false, false, true, false, false, false, false, false, false };


    private readonly float[][] HPRegenBonusFromLuxCover = new float[5][]
    {//This modifier is addition+
        //Gen.Good Sun, Moon, Sun/Moon, Gen.Evil, Demon, Vehicle
        new float[7]{0, 0, 0, 0, 0f, 0, 0 }, //None
        new float[7]{5, 10, 0, 5, 0, 0, 0 }, //SunBeam
        new float[7]{0, 0, 0, 0, 0, 5, 0 }, //DayShadow
        new float[7]{0, 0, 0, 0, 5, 10, 0 }, //NightShadow
        new float[7]{0, 0, 0, 0, 5, 10, 0 }  //BloodShadow
    };

    private readonly float[][] HPDegenBonusFromLuxCover = new float[5][]
    {//This modifier is added+
        //Gen.Good, Sun, Moon, Sun/Moon, Gen.Evil, Demon, Vehicle
        new float[7]{0, 0, 0, 0, 0f, 0, 0 }, //None
        new float[7]{0, 0, 0, 0, 0, 5, 0 }, //SunBeam
        new float[7]{0, 0, 0, 0, 0, 0, 0 }, //DayShadow
        new float[7]{0, 0, 0, 0, 0, 0, 0 }, //NightShadow
        new float[7]{0, 0, 0, 0, 0, 0, 0 }  //BloodShadow
    };

    private readonly float[][] AccuracyBonusFromLuxCover = new float[5][]
    {//This modifier is added+
        //Gen.Good, Sun, Moon, Sun/Moon, Gen.Evil, Demon, Vehicle
        new float[7]{0, 0, 0, 0, 0f, 0, 0 }, //None
        new float[7]{0, 0, 0, 0, -0.2f, 0, 0 }, //SunBeam
        new float[7]{-0.2f, 0, 0, 0, 0, 0, 0 }, //DayShadow
        new float[7]{-0.2f, -0.2f, 0.2f, 0.1f, 0, 0, 0 }, //NightShadow
        new float[7]{-0.2f, -0.4f, 0, 0, 0, 0, 0 }  //BloodShadow
    };

    private readonly float[][] DamageBonusFromLuxCover = new float[5][]
    {//This modifier is multiplied*
        //Gen.Good, Sun, Moon, Sun/Moon, Gen.Evil, Demon, Vehicle
        new float[7]{1, 1, 1, 1, 1, 1, 1 }, //None
        new float[7]{1, 1.2f, 1, 1.1f, 1, 1, 1 }, //SunBeam
        new float[7]{1, 1, 1, 1, 1, 1.2f, 1 }, //DayShadow
        new float[7]{1, 1, 1, 1, 1, 1.4f, 1 }, //NightShadow
        new float[7]{1, 1, 1, 1, 1, 1.4f, 1 }  //BloodShadow
    };

    private readonly float[][] DamageResistFromLuxCover = new float[5][]
    {//This modifier is multiplied*
        //Gen.Good, Sun, Moon, Sun/Moon, Gen.Evil, Demon, Vehicle
        new float[7]{1, 1, 1, 1, 1, 1, 1 }, //None
        new float[7]{1, 1, 1, 1, 1, 1, 1 }, //SunBeam
        new float[7]{1, 1, 1, 1, 1.2f, 1, 1 }, //DayShadow
        new float[7]{1, 1, 1, 1, 1.2f, 1, 1 }, //NightShadow
        new float[7]{1, 1, 1, 1, 1.2f, 1, 1 }  //BloodShadow
    };

    private readonly float[][] SpeedBonusFromLuxCover = new float[5][]
    {//This modifier is multiplied*
        //Gen.Good, Sun, Moon, Sun/Moon, Gen.Evil, Demon, Vehicle
        new float[7]{1, 1, 1, 1, 1, 1, 1 }, //None
        new float[7]{1, 1, 1, 1, 1, 0.5f, 1 }, //SunBeam
        new float[7]{1, 1, 1, 1, 1, 1, 1 }, //DayShadow
        new float[7]{1, 1, 1, 1, 1, 1, 1 }, //NightShadow
        new float[7]{1, 1, 1, 1, 1, 1.4f, 1 }  //BloodShadow
    };

    private float[][][] AllLuxCoverModifierHolders;

    //                                               HP Bar                 F.A Bar                 HP Gain/Lose Bar      Armour Gain/Lose Bar    Capture Bar     
    public Color[] Bar_MaxColourInfo = new Color[5] { new Color(0, 255, 0), new Color(0, 255, 255), new Color(0, 255, 0), new Color(0, 255, 255), new Color(130, 130, 130) };
    public Color[] Bar_MinColourInfo = new Color[5] { new Color(255, 0, 0), new Color(0, 125, 125), new Color(255, 0, 0), new Color(0, 125, 125), new Color(130, 130, 130) };

    public Sprite[] UI_ArmourTypeIdentifiers;
    public Sprite[] UI_DamageTypeIdentifiers;

    public List<Vector3> GetAllCritLocations() { return AI_CriticalDefenceLocations; }

    public bool CheckJumpLocationBlockers(Vector3 DesiredJump)
    {
        bool IsValidJump = true;

        for(int i = 0; i < AllJumpBlockers.Length; i++)
        {
            if(AllJumpBlockers[i] != null)
            {
                if (AllJumpBlockers[i].bounds.Contains(DesiredJump))
                {
                    IsValidJump = false; break;
                }
            }
        }

        return IsValidJump;
    }

    public float[] GetDamageFromPiercingTable(int Armour = 0, int PiercingType = 0, float Damage = 0, float Accuracy = 0)
    {
        return new float[2] { ArmourPiercingTypes[PiercingType][1][Armour] * Damage * Accuracy, ArmourPiercingTypes[PiercingType][0][0] * Damage * Accuracy };
    }

    public float GetValueOfLuxCover(int LuxCoverType, int LuxType, int LuxArmourType)
    {
        return AllLuxCoverModifierHolders[LuxCoverType][LuxType][LuxArmourType];
    }

    public void EnableCutsceneLogic(bool IsIntroCutscene = false)
    {
        CamFade.ChangeState(true, true);
        if (IsIntroCutscene) { ChangeGameplayState(false); }
    }

    private void Update()
    {
        if (IsCheckingForCamFade)
        {
            CheckForFadeToBlackComplete();
        }
    }

    private void CheckForFadeToBlackComplete()
    {
        if (!CamFade.IsMovingBetweenStates)
        {
            CamFade.ForceChangeColours(new Color(255,255,255),true);
            ChangeGameplayState(false);
        }
    }

    public void EndCutsceneLogic(bool CleanGoIntoCamera = false)
    {
        if (!CleanGoIntoCamera)
        {
            CamFade.ChangeState(true, false);
        }
        else
        {
            CamFade.ForceChangeColours(new Color(0, 0, 0, 0), false);
        }

        ChangeGameplayState(true);
    }

    private void ChangeGameplayState(bool State)
    {
        foreach (SquadManager SM in AllPlayers_SM)
        {
            SM.gameObject.SetActive(State);
        }
    }
}


