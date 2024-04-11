using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_BossSindyr : MonoBehaviour
{
    public GameObject FallingSword;
    public GameObject[] VFX_ToSpawn;
    private bool HasCommitedToFinalAction = false;


    private List<Vector3> TPLocations_Defensive = new List<Vector3>();
    private string[] AllStates = new string[5] { "Idle", "Attack", "DefensiveTeleport", "AggressiveTeleport", "Spellcasting" };
    private float[] ChanceToActivateStates = new float[5] { 0.1f, 0.6f, 0.2f, 0.3f, 0.35f };
    private float[] TimePerState = new float[5] { 5, 8, 1, 1, 6 };
    private float[] RdmTimePerState = new float[5] { 2, 4, 0, 0, 3 };
    private bool[] MustTransitionToDefensiveTP = new bool[5] { false, false, false, false, false };
    private bool[] MustTransitionToAggressiveTP = new bool[5] { false, true, true, false, false };
    private int CurrentStateNumb = 0;
    private Vector3 StartPos;
    private float BossAggroRange = 40;
    private SquadManager EnemySM;
    private GameInfo GI;
    private bool Active = false;
    private bool IsAggro = false;
    private bool IsReseting = true;
    private MusicManager MM;
    private BossFightInfo BFI;
    private EntityMovement EM;
    private JumpAndTeleport JaT;
    private Health selfHealth;
    private List<GameObject> AllEnemyHeroes = new List<GameObject>();
    private AbilityCaster[] AC;
    private UnityEngine.AI.NavMeshAgent NavAgent;
    private int CurrentAbilityCount = 0;
    private int MaxAbilityCount = 8;
    private int RandomAbilityCountBonus = 4;
    private int CurrentMaxAbilityCount;
    private float EndBossHealthPercent = 0.333f;
    private bool BossIsEnded = false;
    private int CurrentMagicType = 0;

    //Leader boss stuff

    private Vector3 AllTargetPos;
    private bool[] StageIsTriggered = new bool[1] { false };



    public Transform[] DesiredMoveLocsStartingArea;
    public GameObject[] AbilityWarningEffects;
    public float[] AbilityWarningEffectScales;


    private AbilityCaster[] AllAC;
    private AbilityCaster MagicBlastAbility;
    private AbilityCaster FireRockAbility;
    private AbilityCaster ArtilleryFireRockAbility;

    private int MagicBlastRepeatRate = 1;
    private int MagicBlastRepeatRateRdm = 4;
    private int MagicBlastCurrentRepeatRate;
    private int MagicBlastCastCurrent = 0;

    private bool IsDoingDoom = false;
    private bool IsDoingRock = false;
    private bool IsDoingArtillery = false;

    private float DelayBeforeMagicBlast = 1.5f;
    private float DelayBeforeFireRock = 4;
    private float DelayBeforeArtillery = 2;

    private float MaxCooldownMagicBlast = 1;
    private float MaxCooldownRock = 4;
    private float MaxCooldownArtillery = 16;

    private bool IsMagicBlast = false;
    private bool IsFireRock = false;
    private bool IsArtillery = false;
    private float MagicBlastCooldownCurrent;
    private float FireRockCooldownCurrent;
    private float ArtilleryCooldownCurrent;


    private SindyrSpecialCloneSpawner SSCF;

    private void Start()
    {
        gameObject.TryGetComponent(out selfHealth);
        gameObject.TryGetComponent(out SSCF);

        GI = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameInfo>();
        MM = GI.GetComponent<MusicManager>();
        EM = gameObject.GetComponent<EntityMovement>();
        JaT = gameObject.GetComponent<JumpAndTeleport>();
        BFI = GameObject.FindGameObjectWithTag("MarkerManager").GetComponent<BossFightInfo>();
        AC = gameObject.GetComponents<AbilityCaster>();
        NavAgent = gameObject.GetComponent<UnityEngine.AI.NavMeshAgent>();
        StartPos = transform.position;

        TPLocations_Defensive = BFI.DefensiveTPMkrsPos;
        GetAbilities();
        InvokeRepeating(nameof(TrySetupEnemy), 1, 1);
    }

    private void TrySetupEnemy()
    {
        try
        {
            CancelInvoke();
            EnemySM = GI.AllPlayers_SM[0];
            SetupGame();
        }
        catch
        {
            Debug.LogWarning("AI_BossSindyr in TrySetupEnemy: No Units Found! Trying Again...");
        }
    }

    private void SetupGame()
    {
        Transform tmpParent = transform.parent;
        transform.parent = null;
        if(tmpParent != null) { Destroy(tmpParent.gameObject); }
        SetNewState(0);
        Active = true;
        InvokeRepeating(nameof(CheckIfEnemyInRange), 1, 1);
    }

    private void Update()
    {
        if (Active)
        {
            MM.IsInBoss = true;
            CheckIncapcitated();
            UpdateCooldowns();
            if (selfHealth.GetCurrentHP_AsPercentOfMax() <= 0.625f)
            {
                StageIsTriggered[0] = true;
            }

            if (!IsAggro)
            {
                List<GameObject> InRangeEnemies = CheckIfEnemyInRange();
            }
            else
            {
                if (IsReseting)
                {
                    Invoke(nameof(Invoke_SetNewState), 0);
                }
                MM.IsInBoss = true;
            }
        }  
        else if(!HasCommitedToFinalAction && GI.SpecialSindyrSpeechIsDone)
        {
            MM.IsInBoss = false;
            HasCommitedToFinalAction = true;
            CommitToEndBoss();
        }        
    }

    private List<GameObject> CheckIfEnemyInRange()
    {
        try
        {
            List<GameObject> AllInRangeUnits = new List<GameObject>();
            AllEnemyHeroes = EnemySM.Get_SquadList(0);
            List<bool> tmpIsInRange = new List<bool> { };
            bool IsInRange = false;

            for (int i = 0; i < AllEnemyHeroes.Count; i++)
            {
                if (Vector3.Distance(transform.position, AllEnemyHeroes[i].transform.position) <= BossAggroRange + BossAggroRange / 5)
                {
                    IsInRange = true;
                    tmpIsInRange.Add(true);
                    AllInRangeUnits.Add(AllEnemyHeroes[i]);
                }
                else
                {
                    tmpIsInRange.Add(false);
                }
            }

            AllEnemyHeroes = AllInRangeUnits;
            IsAggro = IsInRange;
            return AllInRangeUnits;
        }
        catch
        {
            Debug.LogWarning("ERROR! In AI_BossSindyr. Cannot Check for enemy in range, returning null.");
            return null;
        }
    }

    private void Invoke_SetNewState()
    {
        SetNewState();
    }

    private void SetNewState(int ForceNewState = -1)
    {
        if (AllEnemyHeroes.Count == 0)
        {
            IsReseting = true;
        }
        else if (ForceNewState < 0)
        {
            IsReseting = false;
            int tmpPreviousState = CurrentStateNumb;
            if (MustTransitionToAggressiveTP[CurrentStateNumb])
            {
                CurrentStateNumb = 3;
            }
            else if (MustTransitionToDefensiveTP[CurrentStateNumb])
            {
                CurrentStateNumb = 2;
            }
            else
            {
                int tmpCurrentStateIteration = 0; //Prevent Perma Loop
                while (CurrentStateNumb == tmpPreviousState || tmpCurrentStateIteration > 100)
                {
                    float tmpMarginOfDifference = 0;
                    for(int i = 0; i < ChanceToActivateStates.Length; i++)
                    {
                        float tmpRdm = Random.Range(0f, 1f);
                        if (tmpRdm < ChanceToActivateStates[i])
                        {
                            if(ChanceToActivateStates[i] - tmpRdm > tmpMarginOfDifference)
                            {
                                CurrentStateNumb = i;
                                tmpMarginOfDifference = ChanceToActivateStates[i] - tmpRdm;
                            }
                        }
                    }
                    tmpCurrentStateIteration++;
                    CurrentStateNumb = Random.Range(0, AllStates.Length);
                }
            }

            if (CurrentStateNumb != 4)
            {
                Invoke(nameof(Invoke_SetNewState), TimePerState[CurrentStateNumb] + Random.Range(-RdmTimePerState[CurrentStateNumb], RdmTimePerState[CurrentStateNumb]));
            }
        }
        else
        {
            IsReseting = false;
            CurrentStateNumb = ForceNewState;
            if (CurrentStateNumb != 4)
            {
                Invoke(nameof(Invoke_SetNewState), TimePerState[CurrentStateNumb] + Random.Range(-RdmTimePerState[CurrentStateNumb], RdmTimePerState[CurrentStateNumb]));
            }
        }

        DoNewState();
    }

    private void DoNewState()
    {
        if (!IsReseting)
        {
            if (AllStates[CurrentStateNumb] == "Idle")
            {
                DoIdle();
            }
            else if (AllStates[CurrentStateNumb] == "Attack")
            {
                DoAnAttack();
            }
            else if (AllStates[CurrentStateNumb] == "AggressiveTeleport")
            {
                DoAnAttack(true);
            }
            else if (AllStates[CurrentStateNumb] == "DefensiveTeleport")
            {
                DoDefensiveTeleport();
            }
            else if (AllStates[CurrentStateNumb] == "Spellcasting")
            {
                SetNewMagicStatus();
                DoSpellcasting();
            }
            else
            {
                Debug.LogWarning("AI_BossSindyr in DoNewState: Invalid AI state! ForcingToIdle...");
                SetNewState(0);
            }

            if(Random.Range(0f,1f) >= 0.8f)
            {
                SSCF.OrderSpawn();
            }
        }
        else
        {
            EM.SetMoveDestination(StartPos);
        }
    }

    private void DoAnAttack(bool TeleportToLocation = false)
    {
        GameObject tmpTarget = AllEnemyHeroes[Random.Range(0, AllEnemyHeroes.Count)];
        EM.SetAttackTarget(tmpTarget.transform);

        if (TeleportToLocation) 
        {
            JaT.IssueAJump(tmpTarget.transform.position);
        }
    }

    private void DoIdle()
    {
        EM.StopCommands();
    }

    private void DoDefensiveTeleport()
    {
        EM.StopCommands();
        NavAgent.enabled = false;
        JaT.IssueAJump(TPLocations_Defensive[Random.Range(0, TPLocations_Defensive.Count)] + new Vector3(0,2,0));
        NavAgent.enabled = true;

        if (Random.Range(0f, 1) >= 0.6f)
        {
            if(Random.Range(0f,1f) >= 0.8f)
            {
                Invoke(nameof(DoSpellcasting), 0.5f);
            }

            Invoke(nameof(DoDefensiveTeleport), 1.5f + Random.Range(-0.5f, 0.5f));
        }
    }



    private void SetNewMagicStatus()
    {
        if (Random.Range(0f, 1f) > 0.5f && MagicBlastCooldownCurrent >= MaxCooldownMagicBlast)
        {
            MagicBlastCurrentRepeatRate = MagicBlastRepeatRate + Random.Range(0, MagicBlastRepeatRateRdm);
            MagicBlastCastCurrent = 0;
            CurrentMagicType = 0;
            MagicBlastCooldownCurrent = 0;
        }
        else if(Random.Range(0f, 1f) > 0.5f && FireRockCooldownCurrent >= MaxCooldownRock)
        {
            CurrentMagicType = 1;
            FireRockCooldownCurrent = 0;
        }
        else if (StageIsTriggered[0] && ArtilleryCooldownCurrent >= MaxCooldownArtillery)
        {
            CurrentMagicType = 2;
            ArtilleryCooldownCurrent = 0;
        }
        else if(MagicBlastCooldownCurrent >= MaxCooldownMagicBlast)
        {
            MagicBlastCurrentRepeatRate = MagicBlastRepeatRate + Random.Range(0, MagicBlastRepeatRateRdm);
            MagicBlastCastCurrent = 0;
            CurrentMagicType = 0;
            MagicBlastCooldownCurrent = 0;
        }
        else
        {
            CurrentMagicType = -1;
        }
    }

    private void DoSpellcasting()
    {
        if(CurrentMagicType == 0)
        {
            ActivateDoomMagic();
        }
        else if(CurrentMagicType == 1)
        {
            ActivateFireRock();
        }
        else if(CurrentMagicType == 2)
        {
            ActivateArtillery();
        }
        else
        {
            SetNewState(0);
        }
    }

    private void ActivateDoomMagic()
    {
        if (!BossIsEnded)
        {
            SetTargetAsEnemy();
            DoAbilityWarning(1);
            Invoke(nameof(CommitDoomMagic), DelayBeforeMagicBlast);
        }
    }

    private void CommitDoomMagic()
    {
        if (!BossIsEnded)
        {
            MagicBlastCastCurrent++;
            MagicBlastAbility.ActivateAbilityExternally(null, AllTargetPos);
            IsDoingDoom = true;

            if(MagicBlastCastCurrent < MagicBlastCurrentRepeatRate)
            {
                Invoke(nameof(ActivateDoomMagic), 1 + Random.Range(0.5f, 1));
            }
            else
            {
                IsDoingDoom = false;
                SetNewState();
            }
        }
    }

    private void ActivateFireRock()
    {
        if (!BossIsEnded)
        {
            SetTargetAsEnemy();
            DoAbilityWarning(2);
            Invoke(nameof(CommitFireRock), DelayBeforeFireRock);
        }
    }

    private void CommitFireRock()
    {
        if (!BossIsEnded)
        {
            MagicBlastAbility.ActivateAbilityExternally(null, AllTargetPos);
            SetNewState();
        }
    }

    private void ActivateArtillery()
    {
        if (!BossIsEnded)
        {
            SetTargetAsEnemy();
            DoAbilityWarning(3);
            Invoke(nameof(CommitArtillery), DelayBeforeArtillery);
        }
    }

    private void CommitArtillery()
    {
        if (!BossIsEnded)
        {
            MagicBlastAbility.ActivateAbilityExternally(null, AllTargetPos);
            SetNewState();
        }
    }

    private void UpdateCooldowns()
    {
        if (!IsMagicBlast) { MagicBlastCooldownCurrent += Time.deltaTime; }
        if (!IsFireRock) { FireRockCooldownCurrent += Time.deltaTime; }
        if (!IsArtillery) { ArtilleryCooldownCurrent += Time.deltaTime; }
    }

    private void CheckIncapcitated()
    {
        if(selfHealth.GetCurrentHP() / selfHealth.GetMaxHP() <= EndBossHealthPercent)
        {
            CancelInvoke();
            IsAggro = false;
            Active = false;
            foreach (GameObject tmp in BFI.ActivatePreEndBoss) { tmp.SetActive(true); }
        }
    }

    public void CommitToEndBoss()
    {
        FallingSword.SetActive(true);
        Invoke(nameof(KillClones), 3);
        Invoke(nameof(FinishBoss), 5);
    }

    private void KillClones()
    {
        foreach (GameObject VFX in VFX_ToSpawn)
        {
            GameObject.Instantiate(VFX, transform.position, new Quaternion(), null);
        }

        SSCF.ForceKillAllClones();
        selfHealth.ForceKillSelf();
    }

    private void FinishBoss()
    {
        foreach(GameObject tmp in BFI.ActivateFinalBoss) { tmp.SetActive(true); }
        Invoke(nameof(DisableSelf), 2);
    }

    private void DisableSelf()
    {
        gameObject.SetActive(false);
    }

    private void GetAbilities()
    {
        AllAC = gameObject.GetComponents<AbilityCaster>();

        foreach (AbilityCaster AC in AllAC)
        {
            if (AC.AbilityName == "DarkMagicBlast" && MagicBlastAbility == null) { MagicBlastAbility = AC; }
            if (AC.AbilityName == "DarkMagicRock" && FireRockAbility == null) { FireRockAbility = AC; }
            if (AC.AbilityName == "DarkMagicRockArtillery" && ArtilleryFireRockAbility == null) { ArtilleryFireRockAbility = AC; }
        }
    }

    private void SetTargetAsEnemy()
    {
        List<List<GameObject>> tmpList = GI.AllPlayers_SM[0].Get_AllSquadLists();
        List<List<Health>> tmpHP = GI.AllPlayers_SM[0].Get_AllHealthLists();
        for (int i = 0; i < tmpHP.Count; i++)
        {
            if (tmpHP[i][0].GetIfIncapacitated())
            {
                tmpList.RemoveAt(i);
            }
        }
        AllTargetPos = tmpList[Random.Range(0, tmpList.Count)][0].transform.position;
    }

    private void DoAbilityWarning(int i)
    {
        GameObject NewFX = Instantiate(AbilityWarningEffects[i]);
        NewFX.transform.position = AllTargetPos;
        NewFX.transform.localScale = new Vector3(AbilityWarningEffectScales[i], AbilityWarningEffectScales[i], AbilityWarningEffectScales[i]);
    }
}
