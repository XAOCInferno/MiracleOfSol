using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EntityMovement : MonoBehaviour
{
    public bool Active = true;
    public KeyCode StopButton = KeyCode.Z;
    public Vector3 StoppingDistance = new Vector3(1, 1, 1);
    public AudioClip RepeatingMovementSFX;
    public float[] RepeatingMovementSoundTravelDistance;

    private LayerMask FloorLayer;
    private LayerMask CapPointLayer;
    private LayerMask EntityLayer;

    private bool IsTryingToCapture = false;
    private CapturePoint CapPoint;

    private bool IsTryingToAttack = false;
    private Transform AttackTarget;

    private BasicInfo BI;
    private BasicFunctions BF;
    private NavMeshAgent Agent;
    private GetIfSelected GIS;
    private SquadManager SM;
    private GameInfo GI;
    private Combat CombatManager;
    private AudioSourceController ASC;

    private float CurrentSpeed = 0;

    //Modifiers
    private float Mod_AddMaxSpeed = 0;
    private float Mod_AddCurrentSpeed = 0;

    private float Mod_MultiMaxSpeed = 1;
    private float Mod_MultiCurrentSpeed = 1;

    //Status
    private bool IsMoving = false;
    private NavMeshPath MovementPath;
    private AnimationPlayer AP;

    //Melee Leap
    private Transform MeleeLeapTarget;
    private Vector3 MeleeLeapTarget_LastLoc;
    private Vector3 MeleeLeap_StartLoc;
    private float CurrentLeapTime;
    private bool LeapIsComplete = false;
    private bool LeapHasStarted = false;
    private List<GameObject> TemporaryModDurationHolder = new List<GameObject>();

    //Misc
    private Vector3 NewMovePos;
    private Vector3 PreviousMovePos;
    private float PathCalcTimerMax = 1;
    private float PathCalcTimerCurrent = 0;
    private bool IsAI;
    private bool IsBoss;
    private bool IsRampaging = false;
    private VoiceLineManager VLM;
    private bool HasSetup = false;

    // Start is called before the first frame update
    void Start()
    {
        MovementPath = new NavMeshPath();

        GameObject.FindWithTag("GameController").TryGetComponent(out GI);
        GI.TryGetComponent(out BF);

        gameObject.TryGetComponent(out BI);
        gameObject.TryGetComponent(out Agent);
        gameObject.TryGetComponent(out GIS);
        gameObject.TryGetComponent(out CombatManager);
        gameObject.TryGetComponent(out ASC);
        gameObject.TryGetComponent(out AP);
        gameObject.TryGetComponent(out VLM);

        FloorLayer = LayerMask.GetMask("Terrain");
        CapPointLayer = LayerMask.GetMask("CapturePoint");
        EntityLayer = LayerMask.GetMask("Entity");
        NewMovePos = transform.position;

        if (BI.OwnedByPlayer == 0)
        {
            PathCalcTimerMax = 0.1f;
        }
        else
        {
            PathCalcTimerMax += Random.Range(0.5f, 2f);
        }

        Agent.angularSpeed = BI.EBPs.RotationRate * 10;
        PreviousMovePos = transform.position;

        Invoke(nameof(DelayStart), 1);
    }

    private void DelayStart()
    {
        AI_Controller AI_Comp; BossAIManager Boss_Comp;
        gameObject.TryGetComponent(out AI_Comp);
        gameObject.TryGetComponent(out Boss_Comp);
        if (AI_Comp == null) { IsAI = false; } else { IsAI = true; if (Boss_Comp != null) { IsBoss = true; } else { IsBoss = false; } }
        HasSetup = true;
    }

    public Transform GetAttackTarget() { return AttackTarget; }

    private float GetMaxSpeed()
    {
        float LuxCoverSpeedMod = CheckCoverStatus();
        float tmp_TotalMultipliedSpeedMod = LuxCoverSpeedMod * Mod_MultiMaxSpeed;
        float MeleeChargeBonus = 1;
        try
        {
            if (CombatManager.IsMeleeCharging)
            {
                MeleeChargeBonus = CombatManager.MeleeCharge_SpeedBonus;
            }
        }
        catch
        {
            Debug.LogWarning("Error! In EntityMovement: GetMaxSpeed. Cannot get melee charge bonuses for '" + gameObject.name + "'");
        }

        tmp_TotalMultipliedSpeedMod *= MeleeChargeBonus;

        try
        {
            return (BI.EBPs.MaxVelocity + Mod_AddMaxSpeed) * tmp_TotalMultipliedSpeedMod;
        }
        catch
        {
            Debug.LogWarning("Error! In EntityMovement: GetMaxSpeed. Cannot return max speed for '" + gameObject.name + "'" + " Reseting to 1");
            return 1;
        }
    }

    public void SetIfRampage(bool State) { IsRampaging = State; CurrentSpeed = GetMaxSpeed(); }

    public bool GetIfMoving()
    {
        return IsMoving;
    }

    public void SetIfMoving(bool State)
    {
        if (!IsRampaging)
        {
            IsMoving = State;
        }
    }

    private void CheckForRampageEnd() 
    { 
        if (Vector3.Distance(transform.position, Agent.destination) <= 1f)
        {
            SetIfRampage(false);
            SetIfMoving(false);
        }
    }

    private void CheckIfSpeedOverMax()
    {
        if (CurrentSpeed > GetMaxSpeed())
        {
            CurrentSpeed = GetMaxSpeed();
        }
    }

    private void CheckIfSpeedUnderMin()
    {
        if (CurrentSpeed < 0)
        {
            CurrentSpeed = 0;
        }
    }

    private void CheckSpeedMinMax()
    {
        CheckIfSpeedOverMax();
        CheckIfSpeedUnderMin();
    }

    // Update is called once per frame
    void Update()
    {
        if (HasSetup)
        {
            PathCalcTimerCurrent += Time.deltaTime;
            if (PathCalcTimerCurrent >= PathCalcTimerMax)
            {
                PathCalcTimerCurrent = 0;
                CalculatePath();
            }

            if (IsRampaging) { CheckForRampageEnd(); }

            if (Active && Agent.enabled)
            {
                DoGenericMovementLogic();
            }

            if (CombatManager.Target != null)
            {
                RotateTowardsObject();
            }
            else
            {
                AP.SetAnimationState(new bool[1] { false }, null, null);
            }

            if (CombatManager.HasMeleeLeap && CombatManager.IsMeleeLeap)
            {
                if (!IsRampaging)
                {
                    if (LeapHasStarted && !LeapIsComplete)
                    {
                        if (CombatManager.MeleeLeapIsTeleport)
                        {
                            CheckMeleeLeapHoming();
                            transform.position = MeleeLeapTarget_LastLoc;
                            ApplyMeleeLeapModifiers();
                            EndMeleeLeap();
                        }
                        else
                        {
                            DoMeleeLeap();
                        }
                    }
                }
            }
            else
            {
                if (Vector3.Distance(transform.position, Agent.destination) >= CombatManager.MeleeDuelRange + 1 && CurrentSpeed < GetMaxSpeed())
                {
                    float MeleeChargeBonus = 1;
                    if (CombatManager.IsMeleeCharging)
                    {
                        MeleeChargeBonus = CombatManager.MeleeCharge_SpeedBonus;
                    }

                    float BaseRate = BI.EBPs.AccelerationRate * MeleeChargeBonus;

                    CurrentSpeed += BaseRate * Time.deltaTime;
                    SetIfMoving(true);

                    CheckIfSpeedOverMax();
                    ASC.OrderNewSound(RepeatingMovementSFX, RepeatingMovementSoundTravelDistance, false, true);
                }
                else
                {
                    SetIfMoving(false);
                    if (CurrentSpeed > 0)
                    {
                        CurrentSpeed -= 2 * BI.EBPs.AccelerationRate * Time.deltaTime;
                        if (transform.position == Agent.destination)
                        {
                            ASC.StopLoopingSound();
                            AP.SetAnimationState(null, new bool[1] { false }, null);
                        }
                    }
                    else
                    {
                        CurrentSpeed = 0;
                        ASC.StopLoopingSound();
                        AP.SetAnimationState(null, new bool[1] { false }, null);
                    }
                }
            }
            Agent.speed = CurrentSpeed;
        }
    }


    private void RotateTowardsObject()
    {
        /*This bit taken from https://answers.unity.com/questions/540120/how-do-you-update-navmesh-rotation-after-stopping.html*/

        Vector3 Direction = (CombatManager.Target.transform.position - transform.position).normalized;
        if (Direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(Direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * BI.EBPs.RotationRate);
        }
    }

    private void DoGenericMovementLogic()
    {
        //if (!Agent.enabled) { Agent.enabled = true; print("Enabled via EM"); }

        if (!IsRampaging)
        {
            if (IsTryingToAttack)
            {
                MoveToAttack();
            }

            if (Input.GetMouseButtonUp(1) && GIS.GetSelectedStatus() == "Selected")
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;

                if (Physics.Raycast(ray, out hit, 1000, EntityLayer))
                {
                    hit.collider.TryGetComponent(out BasicInfo Temp_BI);
                    if (Temp_BI == null)
                    {
                        Debug.Log("ERROR: EntityBI not defined");
                    }
                    else
                    {
                        if (Temp_BI.OwnedByPlayer != -1 && Temp_BI.OwnedByPlayer != BI.OwnedByPlayer)
                        {
                            SetAttackTarget(hit.transform);
                            GI.AllPlayers_SM[BI.OwnedByPlayer].SquadOrder_SetAttackTarget(BI.GetIDs(), hit.transform);
                        }
                        else
                        {
                            SetAttackTarget();
                            GI.AllPlayers_SM[BI.OwnedByPlayer].SquadOrder_SetAttackTarget(BI.GetIDs(), null);

                            SetMoveDestination(hit.point);
                            GI.AllPlayers_SM[BI.OwnedByPlayer].SquadOrder_SetMoveDestination(BI.GetIDs(), hit.point);
                            VLM.PlayVoiceLineOfType("MOVE");
                        }
                    }
                }
                else if (Physics.Raycast(ray, out hit, 1000, CapPointLayer))
                {
                    hit.collider.TryGetComponent(out CapturePoint NewCap);

                    SetAttackTarget();
                    GI.AllPlayers_SM[BI.OwnedByPlayer].SquadOrder_SetAttackTarget(BI.GetIDs(), null);

                    SetCaptureTarget(NewCap);
                    GI.AllPlayers_SM[BI.OwnedByPlayer].SquadOrder_SetCaptureTarget(BI.GetIDs(), NewCap);

                    SetMoveDestination(hit.collider.transform.position); //If this is buggy change to hit.pos;
                    GI.AllPlayers_SM[BI.OwnedByPlayer].SquadOrder_SetMoveDestination(BI.GetIDs(), hit.collider.transform.position);
                }
                else if (Physics.Raycast(ray, out hit, 1000, FloorLayer))
                {
                    SetAttackTarget();
                    GI.AllPlayers_SM[BI.OwnedByPlayer].SquadOrder_SetAttackTarget(BI.GetIDs(), null);

                    SetMoveDestination(hit.point);
                    GI.AllPlayers_SM[BI.OwnedByPlayer].SquadOrder_SetMoveDestination(BI.GetIDs(), hit.point);
                    VLM.PlayVoiceLineOfType("MOVE");
                }
            }

            if (Input.GetKey(StopButton) && GIS.GetSelectedStatus() == "Selected")
            {
                StopCommands();
                GI.AllPlayers_SM[BI.OwnedByPlayer].SquadOrder_StopCommands(BI.GetIDs());
            }
        }
    }

    public void StopCommands(bool IsStoppingInput = true, bool OrderedFromAtk = false, bool OrderedFromCapture = false)
    {
        if (!OrderedFromAtk) { SetAttackTarget(); }
        if (!OrderedFromCapture) { SetCaptureTarget(); }
        if (IsStoppingInput) { /*SetMoveDestination(transform.position + Vector3.Scale(StoppingDistance, transform.TransformDirection(Vector3.forward)));*/ if (!IsAI) { VLM.PlayVoiceLineOfType("STOP"); } }
        SetMoveDestination(transform.position + Vector3.Scale(StoppingDistance, transform.TransformDirection(Vector3.forward)));  //(!OrderedFromAtk && !OrderedFromCapture && !IsStoppingInput)
    }

    public void SetCaptureTarget(CapturePoint NewCap = null, bool ForceAMove = false)
    {
        if (NewCap != null)
        {
            StopCommands(false, false, true);
            if (NewCap.gameObject.tag == "RevivalCap")
            {
                NewCap.transform.parent.TryGetComponent(out RevivalManager tmpRevManager);
                VLM.PlaySpecificReviveTargetLine(tmpRevManager.GetHeroType());
            }
            else
            {
                VLM.PlayVoiceLineOfType("CAPTURE");
            }

            CapPoint = NewCap;
            IsTryingToCapture = true;
            InvokeRepeating(nameof(SetupCapture), 1, 1);

            if (ForceAMove)
            {
                SetMoveDestination(NewCap.transform.position);
            }
        }
        else { IsTryingToCapture = false; }

    }

    public bool GetCaptureStatus()
    {
        return IsTryingToCapture;
    }

    public void SetAttackTarget(Transform Target = null, bool DoNotPlayLine = false)
    {
        if (Target != null)
        {
            try
            {
                if (AttackTarget != Target)
                {
                    IsTryingToAttack = true;
                    AttackTarget = Target.transform;
                    CombatManager.DesiredTarget = Target.gameObject;
                }
                AP.SetAnimationState(new bool[1] { true }, null, null);
            }
            catch
            {
                Debug.LogWarning("ERROR! In EM/SetAttackTarget. Cannot set new attack target.");
            }

            try
            {
                if (!DoNotPlayLine) { VLM.PlayVoiceLineOfType("ATTACK"); }
            }
            catch
            {
                Debug.LogWarning("Error playing ATTACK voiceline in EM for " + gameObject.name);
            }

            StopCommands(false, true, false);
            GI.AllPlayers_SM[BI.OwnedByPlayer].SquadOrder_StopCommands(BI.GetIDs());
        }
        else
        {
            IsTryingToAttack = false;
            AttackTarget = null;
            CombatManager.DesiredTarget = null;
        }
    }

    public void SetMoveDestination(Vector3 NewPos)
    {
        IsMoving = true;
        NewMovePos = NewPos;
        CalculatePath();
    }

    private void CalculatePath()
    {
        try
        {
            if (Agent.isOnNavMesh)
            {
                float tmpDistance = Vector3.Distance(transform.position, NewMovePos);

                if (tmpDistance > 0.4f && (!IsAI || IsBoss) && NewMovePos != PreviousMovePos)
                {
                    PreviousMovePos = NewMovePos;
                    try
                    {
                        Agent.isStopped = false;
                        Agent.CalculatePath(NewMovePos, MovementPath);
                        Agent.path = MovementPath;
                        AP.SetAnimationState(null, new bool[1] { true }, null);
                    }
                    catch
                    {
                        Debug.LogWarning(gameObject.name + ": Move Path Invalid");
                    }
                }
                else if (tmpDistance <= 0.4f && Agent.destination != transform.position)
                {
                    Agent.SetDestination(transform.position);
                }
            }
        }
        catch
        {
            Debug.LogWarning("ERROR! In EM/CalcPath. Cannot calculate navmesh path.");
        }
    }

    public void UpdateSpeedMods(bool IsMultiply = false, float MaxSpeed = 0, float CurrentSpeed = 0)
    {
        if (!IsMultiply)
        {
            Mod_AddMaxSpeed += MaxSpeed;
            Mod_AddCurrentSpeed += CurrentSpeed;
        }
        else
        {
            Mod_MultiMaxSpeed = MaxSpeed;
            Mod_MultiCurrentSpeed = CurrentSpeed;
        }
    }
    public void UpdateBlueprint(bool IsMultiply = false, float MaxSpeed = 0, float CurrentSpeed = 0)
    {
        if (!IsMultiply)
        {
            BI.EBPs.MaxVelocity += MaxSpeed;
            Mod_AddCurrentSpeed += CurrentSpeed;
        }
        else
        {
            BI.EBPs.MaxVelocity = MaxSpeed;
            Mod_MultiCurrentSpeed = CurrentSpeed;
        }
    }

    public void MasterFunction_UpdateAllMovementModifiers(string ModApplicationType = "ENTITY", string ModMathType = "ALL", float AddMaxSpeed = 0, float AddCurrentSpeed = 0)
    {
        if(ModMathType == "ADDITION" || ModMathType == "ALL")
        {
            if(ModApplicationType == "ENTITY")
            {
                UpdateSpeedMods(false, AddMaxSpeed, AddCurrentSpeed);
            }
            else if(ModApplicationType == "ENTITYTYPE")
            {
                UpdateBlueprint(false, AddMaxSpeed, AddCurrentSpeed);
            }
        }

        if(ModMathType == "MULTIPLICATION" || ModMathType == "ALL")
        {
            if (AddMaxSpeed == 0) { AddMaxSpeed = Mod_MultiMaxSpeed; };
            if (AddCurrentSpeed == 0) { AddCurrentSpeed = Mod_MultiCurrentSpeed; };

            if (ModApplicationType == "ENTITY")
            {
                UpdateSpeedMods(true, AddMaxSpeed, AddCurrentSpeed);
            }
            else if (ModApplicationType == "ENTITYTYPE")
            {
                UpdateBlueprint(true, AddMaxSpeed, AddCurrentSpeed);
            }
        }

        CheckSpeedMinMax();
    }

    public void EnableEM(bool Status = true)
    {
        try
        {
            Active = Status;
            Agent.enabled = Status;
        }
        catch
        {
            Debug.LogError("Error! Cannot run 'EnableEM' for '" + gameObject.name + "'");
        }
    }

    private void SetupCapture()
    {
        if (IsTryingToCapture)
        {
            //IsTryingToCapture = true;
            if (CapPoint == null)
            {
                IsTryingToCapture = false;
                CancelInvoke();
            }
            else
            {
                CapPoint.SetupNewCapper(transform, BI);

                if (CapPoint.GetIfCaptured(BI.OwnedByPlayer))
                {
                    IsTryingToCapture = false;
                    CancelInvoke();
                }
            }
        }else { CancelInvoke(); }
    }

    private void MoveToAttack()
    {
        if (AttackTarget != null)
        {
            if (Vector3.Distance(transform.position, AttackTarget.position) > CombatManager.AggroRange)
            {
                SetMoveDestination(AttackTarget.position);
            }
            else
            {
                SetMoveDestination(transform.position);
            }
        }
        else { IsTryingToAttack = false; }
    }

    private float CheckCoverStatus()
    {
        float tmp_SpeedBonus = 0;

        try
        {
            if (BF.FindIfArrayContainsTrueOrFalse(BI.GetCoverStatus(), true))
            {
                int CurrentlyActiveCover = BF.ReturnTrueFalsePositionInArray(BI.GetCoverStatus(), true);
                tmp_SpeedBonus = GI.GetValueOfLuxCover(GI.SpeedBonusLuxCoverPos, CurrentlyActiveCover, BI.EBPs.LuxCoverArmourType);
            }
        }
        catch
        {
            Debug.LogError("Error! in EntityMovement: CheckCoverStatus for '" + gameObject.name + "'");
        }

        return tmp_SpeedBonus;
    }

    private void ResetMeleeLeap()
    {
        CurrentLeapTime = 0;
        LeapIsComplete = false;
        LeapHasStarted = false;
    }

    public void SetupMeleeLeap(Transform tmp_target)
    {
        MeleeLeapTarget = tmp_target;
        MeleeLeapTarget_LastLoc = tmp_target.position;
        MeleeLeap_StartLoc = transform.position;
        ResetMeleeLeap();
        Agent.enabled = false;
        Invoke(nameof(StartMeleeLeap), CombatManager.MeleeLeapSetupTime);
    }

    private void StartMeleeLeap()
    {
        LeapHasStarted = true;

        if (!CombatManager.MeleeLeapIsTeleport)
        {
            Invoke(nameof(EndMeleeLeap), CombatManager.MeleeLeapTimeInAir + CombatManager.MeleeLeapBreakdownTime);
        }
    }

    private void CheckMeleeLeapHoming()
    {
        //Check if target is too far away to home in on. Can home in on targets that are up to 30% higher leap range
        Vector3 TargetNewPos = MeleeLeapTarget.transform.position;
        if (Vector3.Distance(transform.position, TargetNewPos) < CombatManager.MeleeLeapRange * 1.3f)
        {
            MeleeLeapTarget_LastLoc = TargetNewPos;
        }
        //
    }

    private void DoMeleeLeap()
    {
        CurrentLeapTime += Time.deltaTime;
        float TimeAsPercent = CurrentLeapTime / CombatManager.MeleeLeapTimeInAir;
        float NewY = CombatManager.MeleeLeapYHeight.Evaluate(TimeAsPercent);

        CheckMeleeLeapHoming();

        transform.position = Vector3.Lerp(MeleeLeap_StartLoc + new Vector3(0,0,-1), MeleeLeapTarget_LastLoc, TimeAsPercent) + new Vector3(0, NewY, 0);

        if (CurrentLeapTime / CombatManager.MeleeLeapTimeInAir >= 0.999f)
        {
            LeapIsComplete = true;
            ApplyMeleeLeapModifiers();
        }
    }

    private void ApplyMeleeLeapModifiers()
    {
        if (CombatManager.ModApplyOnLeapEnd.Length > 0)
        {
            for (int i = 0; i < CombatManager.ModApplyOnLeapEnd.Length; i++)
            {
                GameObject tmp_newmod = Instantiate(CombatManager.ModApplyOnLeapEnd[i]);
                tmp_newmod.transform.position = transform.position;
                tmp_newmod.name = "Melee Leap Mod From: " + gameObject.name;
                TemporaryModDurationHolder.Add(tmp_newmod);
            }

            Invoke(nameof(EndMeleeLeapModifiers), CombatManager.MeleeLeapModDuration);
        }
    }

    private void EndMeleeLeap()
    {
        CombatManager.IsMeleeLeap = false;
        Agent.enabled = true;
    }

    private void EndMeleeLeapModifiers()
    {
        foreach(GameObject Mod in TemporaryModDurationHolder)
        {
            Destroy(Mod);
        }

        TemporaryModDurationHolder = new List<GameObject>();
    }
}
