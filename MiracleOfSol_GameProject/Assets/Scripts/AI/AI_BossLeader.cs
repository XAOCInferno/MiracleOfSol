using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_BossLeader : MonoBehaviour
{
    public GameObject ActivateOnBossWin;
    public GameObject ActivateOnBossLose;

    public AudioClip[] TauntLines;
    private float TauntMinDelay = 12;
    private float TauntRandomDelayBonus = 10;

    public Transform[] DesiredMoveLocsStartingArea;
    public GameObject[] AbilityWarningEffects;
    public float[] AbilityWarningEffectScales;

    private EntityMovement EM;
    private JumpAndTeleport JaT;
    private Combat selfCombat;
    private Health selfHealth;
    private BasicInfo BI;
    private GameInfo GI;
    private Vector3 DesiredPos;

    private float[] StagePercentTriggers = new float[2] { 0.825f, 0.45f };
    private bool[] StageIsTriggered = new bool[2] { false, false };
    private float EndBossHealthPercent = 0.1f;
    private bool BossIsEnded = false;

    private float InitialDelay = 3;
    private float MainLogicDelay = 2;
    private int NumberOfJumps = 1;
    private int CurrentJumps = 0;
    private bool LogicEnabled = false;

    private float DelayBetweenJumps = 2.75f;
    private float DelayBeforeGrenade = 2.75f;
    private float DelayBeforeBurn = 1.5f;
    private float DelayBeforeRocket = 1f;

    public GameObject LocalArmourLockVFX;
    private float ArmourLockDuration = 15;
    private float ArmourLockMinCooldown = 12;
    private float ArmourLockRandomCooldown = 6;
    private bool AllowArmourLock = true;

    private float StateCooldownTime = 6;
    private float JumpCooldown = 4;
    private float GrenadeCooldown = 8;
    private float BurnCooldown = 8;
    private float RocketBarrageCooldown = 12;

    private float JumpCooldownCurrent;
    private float GrenadeCooldownCurrent;
    private float BurnCooldownCurrent;
    private float RocketBarrageCooldownCurrent;

    private bool HasOrderedJump = true;
    private Vector3 AllTargetPos;

    private AbilityCaster[] AllAC;
    private AbilityCaster GrenadeAC;
    private AbilityCaster BurnGroundAC;
    private AbilityCaster RocketBarrageAC;

    private bool IsBusy = false;
    private bool IsJumping = false;
    private bool IsJumpingToRetreat = false;
    private bool IsThrowGrenade = false;
    private bool IsBurnGround = false;
    private bool IsRocketBarrage = false;
    private bool IsIdle = false;
    private bool HasDoneWinLogic = false;
    private bool FullyBlockRetreatTP = false;

    private VoiceLineManager VLM;
    private bool GrenadeHasBeenThrown = false;
    private bool RocketHasBeenLaunched = false;
    private bool GroundHasBeenBurned = false;
    private Vector3 RetreatJumpToLocation;
    private bool IsBlockedFromRetreatState = false;


    private void SetupBasicVar()
    {
        GameObject.FindGameObjectWithTag("GameController").TryGetComponent(out GI);
        gameObject.TryGetComponent(out EM);
        gameObject.TryGetComponent(out selfHealth);
        gameObject.TryGetComponent(out BI);
        gameObject.TryGetComponent(out selfCombat);
        gameObject.TryGetComponent(out JaT);
        gameObject.TryGetComponent(out VLM);

        GetAbilities();
        Invoke(nameof(EnableLogic), MainLogicDelay);
    }
    
    private void EnableLogic()
    {
        LogicEnabled = true;
        GI.AllPlayers_SM[1].Get_AllHealthLists()[0][0].CanDie = false;
        Invoke(nameof(RepeatingTaunt), TauntMinDelay + Random.Range(0, TauntRandomDelayBonus));
    }

    private void RepeatingTaunt()
    {
        VLM.PlaySpecificVoiceLine(TauntLines[Random.Range(0, TauntLines.Length)], true, false);
        Invoke(nameof(RepeatingTaunt), TauntMinDelay + Random.Range(0, TauntRandomDelayBonus));
    }

    private void GetAbilities()
    {
        AllAC = gameObject.GetComponents<AbilityCaster>();

        foreach(AbilityCaster AC in AllAC)
        {
            if (AC.AbilityName == "Grenade" && GrenadeAC == null) { GrenadeAC = AC; }
            if (AC.AbilityName == "BurnGround" && BurnGroundAC == null) { BurnGroundAC = AC; }
            if (AC.AbilityName == "RocketBarrage" && RocketBarrageAC == null) { RocketBarrageAC = AC; }
        }
    }
    
    public void ActivateLeaderBoss(Vector3 NewSpawnMove, GameObject OnBossWin, GameObject OnBossLose)
    {
        AllTargetPos = NewSpawnMove;
        ActivateOnBossWin = OnBossWin;
        ActivateOnBossLose = OnBossLose;
        RetreatJumpToLocation = transform.position;
        Invoke(nameof(DoInitialJump), InitialDelay);
    }

    private void DoInitialJump()
    {
        SetupBasicVar();
        ForceJumpToDesiredPos();
    }

    private void Update()
    {
        if (LogicEnabled && !BossIsEnded)
        {
            CheckIncapacitatedStatus();
            CheckForNewStateTrigger();
            UpdateCooldowns();

            if (!IsIdle)
            {
                CheckRetreatTP();
                CheckAggressiveTP();
                CheckForGrenade();
                CheckForBurnGround();
                CheckRocketBarrage();
                CheckArmourLock();
            }
            else if(EM.GetAttackTarget() == null)
            {
                 EM.SetAttackTarget(GI.AllPlayers_SM[0].Get_SquadList(0)[0].transform);
            }
        }
    }

    private void CheckForGrenade()
    {
        if (!IsBusy || IsThrowGrenade)
        {
            if (Random.Range(0f, 1f) > 0.5f)
            {
                if (!IsThrowGrenade && GrenadeCooldownCurrent >= GrenadeCooldown && !GrenadeAC.GetAbilityStatus())
                {
                    EM.StopCommands(false, false, false);
                    GrenadeCooldownCurrent = 0;
                    IsThrowGrenade = true;
                    IsBusy = true;
                    ActivateGrenadeThrow();
                }
                else if (GrenadeHasBeenThrown)
                {
                    GrenadeCooldownCurrent = 0;
                    IsBusy = false;
                    IsThrowGrenade = false;
                    DoIdle();
                }
            }
        }
    }

    private void CheckRetreatTP()
    {
        if (!IsBlockedFromRetreatState && !FullyBlockRetreatTP)
        {
            if (!IsBusy || IsJumpingToRetreat)
            {
                if (Random.Range(0f, 1f) > 0.75f)
                {
                    if (JaT.GetIfJumpClear() && !HasOrderedJump && JumpCooldownCurrent >= JumpCooldown)
                    {
                        EM.StopCommands(false, false, false);
                        JumpCooldownCurrent = 0;
                        IsJumpingToRetreat = true;
                        IsBusy = true;
                        DoDefensiveTP();
                    }
                    else if (JaT.GetIfJumpClear() && !HasOrderedJump)
                    {
                        JumpCooldownCurrent = 0;
                        IsJumpingToRetreat = false;
                        IsBusy = false;
                        IsBlockedFromRetreatState = true;
                        DoIdle();
                    }
                }
            }
        }
    }

    private void CheckAggressiveTP()
    {
        if (!IsBusy || IsJumping)
        {
            if (Random.Range(0f, 1f) > 0.4f)
            {
                if (JaT.GetIfJumpClear() && CurrentJumps < NumberOfJumps && !HasOrderedJump && JumpCooldownCurrent >= JumpCooldown)
                {
                    IsBlockedFromRetreatState = false;
                    EM.StopCommands(false, false, false);
                    IsJumping = true;
                    IsBusy = true;
                    CurrentJumps++;
                    DoAggressiveJump();
                }
                else if (CurrentJumps >= NumberOfJumps && JaT.GetIfJumpClear() && !HasOrderedJump)
                {
                    JumpCooldownCurrent = 0;
                    IsJumping = false;
                    IsBusy = false;
                    CurrentJumps = 0;
                    DoIdle();
                }
            }
        }
    }

    private void CheckForBurnGround()
    {
        if (!IsBusy || IsBurnGround)
        {
            if (Random.Range(0f, 1f) > 0.6f)
            {
                if (!IsBurnGround && BurnCooldownCurrent >= BurnCooldown)
                {
                    EM.StopCommands(false, false, false);
                    BurnCooldownCurrent = 0;
                    IsBurnGround = true;
                    IsBusy = true;
                    ActivateBurnGround();
                }
                else if (GroundHasBeenBurned)
                {
                    BurnCooldownCurrent = 0;
                    IsBusy = false;
                    IsBurnGround = false;
                    GroundHasBeenBurned = false;
                    DoIdle();
                }
            }
        }
    }

    private void CheckRocketBarrage()
    {
        if ((!IsBusy || IsRocketBarrage) && StageIsTriggered[1])
        {
            if (Random.Range(0f, 1f) > 0.3f)
            {
                if (!IsRocketBarrage && RocketBarrageCooldownCurrent >= RocketBarrageCooldown)
                {
                    EM.StopCommands(false, false, false);
                    RocketBarrageCooldownCurrent = 0;
                    IsRocketBarrage = true;
                    IsBusy = true;
                    ActivateRocketBarrage();
                }
                else if (RocketHasBeenLaunched)
                {
                    BurnCooldownCurrent = 0;
                    IsBusy = false;
                    IsRocketBarrage = false;
                    RocketHasBeenLaunched = false;
                    DoIdle();
                }
            }
        }
    }

    private void CheckArmourLock() 
    {
        if (AllowArmourLock && !IsJumpingToRetreat)
        {
            DoArmourLock();
        }
    }

    private void CheckForNewStateTrigger()
    {
        for (int i = 0; i < StageIsTriggered.Length; i++)
        {
            if (!StageIsTriggered[i])
            {
                if (selfHealth.GetCurrentHP() / selfHealth.GetMaxHP() <= StagePercentTriggers[i])
                {
                    print("STATE: " + i + "IS DONE");
                    StageIsTriggered[i] = true;
                    NumberOfJumps++;
                    StateCooldownTime -= 2;
                    JumpCooldown -= 1;
                    GrenadeCooldown -= 3;
                    BurnCooldown -= 3;
                    RocketBarrageCooldown -= 4;
                }
            }
        }
    }

    private void CheckIncapacitatedStatus()
    {
        if (!HasDoneWinLogic)
        {
            if (GI.AllPlayers_SM[0].Get_AllHealthLists()[0][0].GetIfIncapacitated())
            {
                HasDoneWinLogic = true;
                DoAziahIncapacitated();
            }
            else if (selfHealth.GetCurrentHP() / selfHealth.GetMaxHP() <= EndBossHealthPercent)
            {
                HasDoneWinLogic = true;
                DoLeaderIncapacitated();
            }
        }
    }

    private void EndStateSetup()
    {
        CancelInvoke();
        LogicEnabled = false;
        List<List<Combat>> tmpCombat = GI.AllPlayers_SM[1].Get_AllCombatLists();
        foreach (List<Combat> tmpList in tmpCombat) 
        {
            foreach (Combat tmp in tmpList)
            {
                tmp.CombatIsEnabled = false;
            }
        }

        tmpCombat = GI.AllPlayers_SM[0].Get_AllCombatLists();
        foreach (List<Combat> tmpList in tmpCombat)
        {
            foreach (Combat tmp in tmpList)
            {
                tmp.CombatIsEnabled = false;
            }
        }
        /*GI.AllPlayers_SM[1].Get_AllSquadListsBI()[0][0].OwnedByPlayer = 0;
        List<GameObject> tmpSquad = GI.AllPlayers_SM[1].Get_AllSquadLists()[0];
        tmpSquad[0].transform.parent = GI.AllPlayers_SM[0].transform;
        GI.AllPlayers_SM[0].Set_NewSquad(tmpSquad);*/
    }

    private void DoGenericEndLogic()
    {
        BossIsEnded = true;
        CancelInvoke();
        EndStateSetup();
    }

    private void DoLeaderIncapacitated()
    {
        DoGenericEndLogic();
        ActivateOnBossWin.SetActive(true);
    }

    private void DoAziahIncapacitated()
    {
        DoGenericEndLogic();
        ActivateOnBossLose.SetActive(true);
    }

    private void DoArmourLock()
    {
        LocalArmourLockVFX.SetActive(true);
        AllowArmourLock = false;
        selfHealth.SetArmourType(11);
        Invoke(nameof(EndArmourLock), ArmourLockDuration);
    }

    private void EndArmourLock()
    {
        LocalArmourLockVFX.SetActive(false);
        selfHealth.SetArmourType(3);
        Invoke(nameof(ResetArmourLockCD), ArmourLockMinCooldown + Random.Range(0, ArmourLockRandomCooldown));
    }

    private void ResetArmourLockCD() { AllowArmourLock = true; }

    private void DoIdle()
    {
        if (StageIsTriggered[0])
        {
            FullyBlockRetreatTP = true;
        }
        IsIdle = true;
        Invoke(nameof(BreakIdle), StateCooldownTime);
    }

    private void BreakIdle() { IsIdle = false; }

    private void UpdateCooldowns()
    {
        if (!IsJumping) { JumpCooldownCurrent += Time.deltaTime; } 
        if (!IsThrowGrenade) { GrenadeCooldownCurrent += Time.deltaTime; }
        if (!IsBurnGround) { BurnCooldownCurrent += Time.deltaTime; }
        if (!IsRocketBarrage) { RocketBarrageCooldownCurrent += Time.deltaTime; }
    }

    private void ForceJumpToDesiredPos()
    {
        if (!BossIsEnded)
        {
            EM.StopCommands(false, false, false);
            JaT.IssueAJump(AllTargetPos);

            HasOrderedJump = false;
        }
    }

    private void DoAggressiveJump()
    {
        if (!BossIsEnded)
        {
            HasOrderedJump = true;

            SetTargetAsEnemy();
            DoAbilityWarning(0);

            Invoke(nameof(ForceJumpToDesiredPos), DelayBetweenJumps);
        }
    }

    private void DoDefensiveTP()
    {
        if (!BossIsEnded)
        {
            HasOrderedJump = true;
            AllTargetPos = RetreatJumpToLocation;

            Invoke(nameof(ForceJumpToDesiredPos), DelayBetweenJumps);
        }
    }

    private void ActivateGrenadeThrow()
    {
        if (!BossIsEnded)
        {
            SetTargetAsEnemy();
            DoAbilityWarning(1);
            Invoke(nameof(CommitGrenadeThrow), DelayBeforeGrenade);
        }
    }

    private void CommitGrenadeThrow()
    {
        if (!BossIsEnded)
        {
            GrenadeAC.ActivateAbilityExternally(null, AllTargetPos);
            GrenadeHasBeenThrown = true;
        }
    }

    private void ActivateBurnGround()
    {
        if (!BossIsEnded)
        {
            SetTargetAsEnemy();
            DoAbilityWarning(2);
            Invoke(nameof(CommitBurnGround), DelayBeforeBurn);
        }
    }

    private void CommitBurnGround()
    {
        if (!BossIsEnded)
        {
            BurnGroundAC.ActivateAbilityExternally(null, AllTargetPos);
            GroundHasBeenBurned = true;
        }
    }

    private void ActivateRocketBarrage()
    {
        if (!BossIsEnded)
        {
            SetTargetAsEnemy();
            DoAbilityWarning(3);
            Invoke(nameof(CommitRocketBarrage), DelayBeforeRocket);
        }
    }

    private void CommitRocketBarrage()
    {
        if (!BossIsEnded)
        {
            RocketBarrageAC.ActivateAbilityExternally(null, AllTargetPos);
            RocketHasBeenLaunched = true;
        }
    }

    private void DoAbilityWarning(int i)
    {
        GameObject NewFX = Instantiate(AbilityWarningEffects[i]);
        NewFX.transform.position = AllTargetPos;
        NewFX.transform.localScale = new Vector3(AbilityWarningEffectScales[i], AbilityWarningEffectScales[i], AbilityWarningEffectScales[i]);
    }

    private void SetTargetAsEnemy() 
    {
        List<List<GameObject>> tmpList = GI.AllPlayers_SM[0].Get_AllSquadLists();
        List<List<Health>> tmpHP = GI.AllPlayers_SM[0].Get_AllHealthLists();
        for(int i = 0; i< tmpHP.Count; i++)
        {
            if (tmpHP[i][0].GetIfIncapacitated())
            {
                tmpList.RemoveAt(i);
            }
        }
        AllTargetPos = tmpList[Random.Range(0,tmpList.Count)][0].transform.position; 
    }
}
