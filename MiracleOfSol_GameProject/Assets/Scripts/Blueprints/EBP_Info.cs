using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBP_Info : MonoBehaviour
{
    public int PositionInGMData = 0; //DON'T FORGET TO ASSIGN ME
    public int PositionInLvlHierarchy = -1;
    public bool IsBoss_IgnoreAIManager = false;

    //Generic Info
    public string EntityName = "__Default";
    public string EntityDesc = "__DefaultDesc";
    public string EntityQuote = "I NEED A QUOTE AHHHHHHH!";
    public bool IsASquad = true;
    public bool IsProduction = false;

    public bool CanCapturePoints = true;
    public float CaptureRate = 1;

    //UI
    public Vector2 PositionInBuildUI = new Vector2(0, 0);
    public Sprite UI_Icon;
    public bool HasAbility = false;

    //Build Info
    public float TotalBuildTime;
    public float BuildTimePerEntity = 10;
    public float[] CostPerEntity = new float[3] { 0, 0, 0 };
    public int BuildTier = 0;

    //Resource Info
    public bool GeneratesResources = false;
    public ResourceGroup ResourcesToGenerate = new ResourceGroup( 0, 0, 0 );
    public bool UpdateCanvas = false;

    //Health Info
    public float TimeOutOfCombatForBonuses = 8;

    public bool CanBeAttacked = true;
    public bool CanPassiveHeal = true;
    public float MaxHP = 100;
    public float HPRegen = 0.5f; //Per second
    public float OutOfCombat_HPRegen_Bonus = 50; 
    public float HPDegen = 0;
    public float StartingHPPercent = 1;
    public float HealthDeathPercent = 0;
    public float MaxArmour = 10;
    public float ArmourRegen = 0;
    public float OutOfCombat_ArmourRegen_Bonus = 5;
    public float ArmourDegen = 0;
    public float StartingArmourPercent = 1;
    public int PiercingArmourType = 0;
    public float BaseMeleeResist = 1;
    public float BaseRangedResist = 1;

    public bool CanBeRevived = false;

    //Movement Info
    public float MaxVelocity = 20;
    public float AccelerationRate = 20;
    public float RotationRate = 10;

    //Scale Info
    public Vector3 EntityScale = new Vector3(1,1,1);

    //Jump Info
    public bool HasJump = false;
    public bool JumpIsTP = false;

    public float JumpDistanceMax = 100;
    public float JumpHeightMax = 2;
    public AnimationCurve JumpCurve = AnimationCurve.Linear(0, 1, 1, 1);
    public float JumpVel = 5f;

    public float JumpSetupTime = 1;
    public float JumpBreakdownTime = 1;

    public Sprite JumpCustomImage;
    public ModifierApplier[] ModsApplyOnLand;

    //Death Explosion
    public GameObject[] DeathExplosionsToSpawn;
    public string[] DeathExplosionsNames;

    //Art
    public GameObject ObjectModel;

    //LuxCover
    public int LuxCoverArmourType = 0; //0 = Gen.Good, 1 = Sun, 2 = Moon, 3 = Sun/Moon, 4 = Gen.Evil, 5 = Demon, 6 = Vehicle

    //AI
    public float AIAggroRange = 12;

    //XP
    public int PrimaryXP = 0;
    public int SecondaryXP = 0;
    public int WeaponPartsRewardMax = 0;
    public int WeaponPartsRewardMin = 0;

    //SBP
    private SBP_Info SBP;
}
