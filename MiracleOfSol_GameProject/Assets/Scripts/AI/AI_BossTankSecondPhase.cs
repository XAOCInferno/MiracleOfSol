using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_BossTankSecondPhase : MonoBehaviour
{
    public AudioClip[] AllAbilityCastLines;
    public AudioClip[] AllRampageCastLines;

    private EntityMovement EM;
    private Combat selfCombat;
    private Health selfHealth;
    private BasicInfo BI;
    private RampageController RC;
    private VoiceLineManager VLM;
    private bool FirstStageDone = false;
    private Vector3 DesiredPos;
    private Transform[] AllMoveLocsInAggroPhase;

    private string[] AllStates = new string[4] { "Idle", "Attack", "Ability", "Rampage" };
    private float[] ChanceToActivateStates = new float[4] { 0.1f, 0.5f, 0.4f, 0.4f };
    private float[] TimePerState = new float[4] { 5, 8, 6, 8 };
    private float[] RdmTimePerState = new float[4] { 2, 2, 0, 0 };
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
    private JumpAndTeleport JaT;
    private List<GameObject> AllEnemyHeroes = new List<GameObject>();
    private AbilityCaster AC;
    private UnityEngine.AI.NavMeshAgent NavAgent;
    private int CurrentMaxAbilityCount;
    private Vector3 CurrentRampageTargetPos;
    private AI_TankBoss_RampageInfo AI_TB_RI;

    // Start is called before the first frame update
    void Start()
    {
        GameObject.FindGameObjectWithTag("GameController").TryGetComponent(out GI);
        GI.TryGetComponent(out MM);
        gameObject.TryGetComponent(out EM);
        gameObject.TryGetComponent(out selfHealth);
        gameObject.TryGetComponent(out BI);
        gameObject.TryGetComponent(out selfCombat);
        gameObject.TryGetComponent(out RC);
        gameObject.TryGetComponent(out VLM);
        gameObject.TryGetComponent(out AC);
        gameObject.TryGetComponent(out AI_TB_RI);

        AI_TB_RI.RampageRelatedAbility = Instantiate(AI_TB_RI.RampageRelatedAbility);
        BasicInfo AI_TB_RI_BI = AI_TB_RI.RampageRelatedAbility.gameObject.AddComponent<BasicInfo>();
        AI_TB_RI_BI.OwnedByPlayer = BI.OwnedByPlayer;

        AC.SetIsAI();
        RC.SetRampageValues(AI_TB_RI.RampageRelatedAbility, AI_TB_RI.RampageSpeedBoost, EM);

        selfHealth.CanPassiveHeal = false;
        selfHealth.CanDie = false;
    }

    private void Update()
    {
        if (Active)
        {
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
    }

    private void TrySetupEnemy()
    {
        try
        {
            CancelInvoke();
            EnemySM = GI.AllPlayers_SM[0];
            ActivateTankBossDelayed();
        }
        catch
        {
            Debug.LogWarning("AI_BossTankSecondPhase in TrySetupEnemy: No Units Found! Trying Again...");
        }
    }

    public void ActivateTankBoss(Transform[] AllMoveLocs)
    {
        AllMoveLocsInAggroPhase = AllMoveLocs;
        InvokeRepeating(nameof(TrySetupEnemy), 0.5f, 0.5f);
    }

    private void ActivateTankBossDelayed()
    {
        print("TANK BOSS ACTIVE");
        Transform tmpParent = transform.parent;
        transform.parent = null;
        if (tmpParent != null) { Destroy(tmpParent.gameObject); }
        SetNewState(0);
        Active = true;

        //Invoke(nameof(ForceMoveToDesiredPos), 4.5f + Random.Range(-1f, 2f));
    }

    private void ForceMoveToDesiredPos()
    {
        EM.SetMoveDestination(DesiredPos);

        if (!FirstStageDone)
        {
            DesiredPos = AllMoveLocsInAggroPhase[Random.Range(0, AllMoveLocsInAggroPhase.Length)].position;
            //Invoke(nameof(ForceMoveToDesiredPos), 4.5f + Random.Range(-1f, 2f));
        }
    }


    //GENERIC BOSS STUFF TAKEN FROM AI BOSS SINDYR

    private List<GameObject> CheckIfEnemyInRange()
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

    private void Invoke_SetNewState()
    {
        SetNewState();
    }

    private void SetNewState(int ForceNewState = -1)
    {
        Debug.Log("SETTING NEW STATE");
        if (AllEnemyHeroes.Count == 0)
        {
            IsReseting = true;
        }
        else if (ForceNewState < 0)
        {
            IsReseting = false;
            int tmpPreviousState = CurrentStateNumb;

            int tmpCurrentStateIteration = 0; //Prevent Perma Loop
            while (CurrentStateNumb == tmpPreviousState || tmpCurrentStateIteration > 100)
            {
                float tmpMarginOfDifference = 0;
                for (int i = 0; i < ChanceToActivateStates.Length; i++)
                {
                    float tmpRdm = Random.Range(0f, 1f);
                    if (tmpRdm < ChanceToActivateStates[i])
                    {
                        if (ChanceToActivateStates[i] - tmpRdm > tmpMarginOfDifference)
                        {
                            CurrentStateNumb = i;
                            tmpMarginOfDifference = ChanceToActivateStates[i] - tmpRdm;
                        }
                    }
                }
                tmpCurrentStateIteration++;
                CurrentStateNumb = Random.Range(0, AllStates.Length);
            }

            Invoke(nameof(Invoke_SetNewState), TimePerState[CurrentStateNumb] + Random.Range(-RdmTimePerState[CurrentStateNumb], RdmTimePerState[CurrentStateNumb]));
        }
        else
        {
            IsReseting = false;
            CurrentStateNumb = ForceNewState;
            Invoke(nameof(Invoke_SetNewState), TimePerState[CurrentStateNumb] + Random.Range(-RdmTimePerState[CurrentStateNumb], RdmTimePerState[CurrentStateNumb]));
        }

        DoNewState();
    }

    private void DoNewState()
    {
        if (!IsReseting)
        {
            if (AllStates[CurrentStateNumb] == "Idle")
            {
                print("BOSS TANK IS IDLE");
                DoIdle();
            }
            else if (AllStates[CurrentStateNumb] == "Attack")
            {
                print("BOSS TANK IS ATTACK");
                ForceMoveToDesiredPos();
                //DoAnAttack();
            }
            else if (AllStates[CurrentStateNumb] == "Ability")
            {
                print("BOSS TANK IS ABILITY");
                DoAbility();
            }
            else if (AllStates[CurrentStateNumb] == "Rampage")
            {
                print("BOSS TANK IS RAMPAGE");
                DoRampage();
            }
            else
            {
                Debug.LogWarning("AI_BossSindyr in DoNewState: Invalid AI state! ForcingToIdle...");
                SetNewState(0);
            }
        }
        else
        {
            print("BOSS TANK IS RESETTING");
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

    private void DoAbility()
    {
        if (AllAbilityCastLines.Length > 0)
        {
            VLM.PlaySpecificVoiceLine(AllAbilityCastLines[Random.Range(0, AllAbilityCastLines.Length)], true);
        }

        CurrentRampageTargetPos = AllEnemyHeroes[Random.Range(0, AllEnemyHeroes.Count)].transform.position;
        EM.StopCommands();
        Invoke(nameof(DoAbilityAsDelayed), 4);
    }

    private void DoAbilityAsDelayed()
    {
        print(AC);
        AC.ActivateAbilityExternally(null, CurrentRampageTargetPos); //error here?
    }

    private void DoRampage(bool TeleportToLocation = false)
    {
        if (AllRampageCastLines.Length > 0)
        {
            VLM.PlaySpecificVoiceLine(AllRampageCastLines[Random.Range(0, AllRampageCastLines.Length)], true);
        }

        CurrentRampageTargetPos = AllEnemyHeroes[Random.Range(0, AllEnemyHeroes.Count)].transform.position;
        EM.StopCommands();
        Invoke(nameof(ActivateRampageAsDelayed), 3);
    }

    private void ActivateRampageAsDelayed()
    {
        EM.SetMoveDestination(CurrentRampageTargetPos);

        RC.EnableRampage();
    }
}
