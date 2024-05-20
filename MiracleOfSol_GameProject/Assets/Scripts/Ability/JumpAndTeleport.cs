using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpAndTeleport : MonoBehaviour
{
    public GameObject[] OneShot_VFX_SpawnOnTP;
    public Transform[] OneShot_VFX_SpawnOnTP_SpawnBone;
    public Transform[] OneShot_VFX_SpawnOnTP_AttachBone;
    public GameObject[] VFX_SpawnOnTP;
    public Transform[] VFX_SpawnOnTP_SpawnBone;
    public Transform[] VFX_SpawnOnTP_AttachBone;
    public GameObject[] VFX_SpawnOnArrive;
    public Transform[] VFX_SpawnOnArrive_SpawnBone;
    public Transform[] VFX_SpawnOnArrive_AttachBone;

    public GameObject[] Mod_Target;
    public string[] Mod_TargetType; //NONE [[dummy]], ENEMY, OWN, ALL, ALLIED [[tbc]]
    public float[] Mod_GenericRadius;
    public List<float> Mod_ActiveTime_Target = new List<float>();
    private List<GameObject> AppliedModsHolder_Target = new List<GameObject>();
    private List<float> ModCurrent_Time = new List<float>();

    public bool IsCurrentlyJumping = false;

    private int AcceptSelectionFromPlayer = 0; //TempSolution
    private BasicInfo BI;
    //private GameInfo GI;
    private CustomPhysics CPhys;
    private EntityMovement EM;
    private GetIfSelected GIS;

    private float AcceptedJumpDiscrepency = 1.5f;

    private List<Vector3> PositionsToJumpTo = new List<Vector3>();
    private List<float> DistanceOfOriginFromPosition = new List<float>();
    private List<bool> JumpHasStarted = new List<bool>();
    private List<bool> JumpHasCompleted = new List<bool>();
    private List<bool> SetupTimeComplete = new List<bool>();
    private List<bool> BreakdownTimeComplete = new List<bool>();
    private float InAirTimeMax = 8;
    private float InAirTimer = 0;
    private float SetupTimeCurrent;
    private float BreakdownTimeCurrent;
    private GameInfo GI;
    private bool HasSpawnedTPVFX = false;
    private bool HasReachedTarget = false;
    private Vector3 EntityOffset = new Vector3(0,-1f,0);
    private float InitialJumpOffset = 0;
    private float StartY;

    // Start is called before the first frame update
    void Start()
    {
        BI = gameObject.GetComponent<BasicInfo>();
        CPhys = gameObject.GetComponent<CustomPhysics>();
        EM = gameObject.GetComponent<EntityMovement>(); //FIX THIS sigh why it's not working
        GIS = gameObject.GetComponent<GetIfSelected>();
        GI = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameInfo>();
        SetupTimeCurrent = BI.EBPs.JumpSetupTime; BreakdownTimeCurrent = BI.EBPs.JumpBreakdownTime;
        InitialJumpOffset = transform.position[1];
    }

    public List<Vector3> GetCurrentJumpPositions() { return PositionsToJumpTo; }
    
    public bool GetIfJumpClear() { foreach (bool tmpBool in BreakdownTimeComplete) { if (!tmpBool) { return false; } } return true; }

    private void Update()
    {
        CheckForJumpInput();

        if (PositionsToJumpTo.Count > 0 && !CPhys.IsBeingThrown)
        {
            EM.EnableEM(false);
            if (InAirTimer > InAirTimeMax)
            {
                EndAJump();
                ActivateNextJump();
            }
            else
            {
                InAirTimer += Time.deltaTime;

                if (JumpHasCompleted[0])
                {
                    if (BreakdownTimeComplete[0])
                    {
                        EndAJump();
                        ActivateNextJump();
                    }
                    else
                    {
                        BreakdownTimeCurrent -= Time.deltaTime;
                        if (BreakdownTimeCurrent <= 0)
                        {
                            BreakdownTimeComplete[0] = true;
                        }
                    }
                }
                else
                {
                    if (SetupTimeComplete[0])
                    {
                        if (VFX_SpawnOnTP.Length > 0)
                        {
                            for (int i = 0; i < VFX_SpawnOnTP.Length; i++)
                            {
                                GameObject tmpVFX = Instantiate(VFX_SpawnOnTP[i]);

                                if (VFX_SpawnOnTP_SpawnBone.Length - i > 0)
                                {
                                    if (VFX_SpawnOnTP_SpawnBone[i] != null)
                                    {
                                        tmpVFX.transform.position = VFX_SpawnOnTP_SpawnBone[i].position;
                                    }
                                    else
                                    {
                                        tmpVFX.transform.position = VFX_SpawnOnTP_AttachBone[i].position;
                                        tmpVFX.transform.parent = VFX_SpawnOnTP_AttachBone[i];
                                    }
                                }
                                else if (VFX_SpawnOnTP_AttachBone.Length - i < 0)
                                {
                                    tmpVFX.transform.position = transform.position;
                                }
                                else
                                {
                                    if (VFX_SpawnOnTP_AttachBone[i] != null)
                                    {
                                        tmpVFX.transform.position = VFX_SpawnOnTP_AttachBone[i].position;
                                        tmpVFX.transform.parent = VFX_SpawnOnTP_AttachBone[i];
                                    }
                                    else
                                    {
                                        tmpVFX.transform.position = transform.position;
                                    }
                                }

                            }


                        }

                        if (!HasSpawnedTPVFX && OneShot_VFX_SpawnOnTP.Length != 0)
                        {
                            HasSpawnedTPVFX = true;
                            for (int i = 0; i < OneShot_VFX_SpawnOnTP.Length; i++)
                            {
                                GameObject tmpVFX = Instantiate(OneShot_VFX_SpawnOnTP[i]);

                                if (OneShot_VFX_SpawnOnTP_SpawnBone.Length - i > 0)
                                {
                                    if (OneShot_VFX_SpawnOnTP_SpawnBone[i] != null)
                                    {
                                        tmpVFX.transform.position = OneShot_VFX_SpawnOnTP_SpawnBone[i].position;
                                    }
                                    else
                                    {
                                        tmpVFX.transform.position = OneShot_VFX_SpawnOnTP_AttachBone[i].position;
                                        tmpVFX.transform.parent = OneShot_VFX_SpawnOnTP_AttachBone[i];
                                    }
                                }
                                else if (OneShot_VFX_SpawnOnTP_AttachBone.Length - i < 0)
                                {
                                    tmpVFX.transform.position = transform.position;
                                }
                                else
                                {
                                    if (OneShot_VFX_SpawnOnTP_AttachBone[i] != null)
                                    {
                                        tmpVFX.transform.position = OneShot_VFX_SpawnOnTP_AttachBone[i].position;
                                        tmpVFX.transform.parent = OneShot_VFX_SpawnOnTP_AttachBone[i];
                                    }
                                    else
                                    {
                                        tmpVFX.transform.position = transform.position;
                                    }
                                }
                            }
                        }


                        if (BI.EBPs.JumpIsTP)
                        {
                            transform.position = PositionsToJumpTo[0];
                            JumpHasCompleted[0] = true;
                        }
                        else
                        {
                            JumpProgress();
                        }
                    }
                    else
                    {
                        SetupTimeCurrent -= Time.deltaTime;

                        if (SetupTimeCurrent <= 0)
                        {
                            SetupTimeComplete[0] = true;
                        }
                    }
                }
            }
        }

        for (int i = 0; i < AppliedModsHolder_Target.Count;i++)
        {
            ModCurrent_Time[i] += Time.deltaTime;
            CheckJumpModTime(i);
        }
    }

    private void CheckJumpModTime(int i)
    {
        try 
        {
            if (Mod_ActiveTime_Target[i] <= ModCurrent_Time[i])
            {// Check here if things need to be disabled |||EDIT: 8/8/21, I think this is fixed :)
                if (AppliedModsHolder_Target.Count > 0)
                {
                    if (AppliedModsHolder_Target[i] != null)
                    {
                        AppliedModsHolder_Target[i].SetActive(false);
                        ModCurrent_Time.RemoveAt(i);
                        Destroy(AppliedModsHolder_Target[i]);
                        AppliedModsHolder_Target.RemoveAt(i);
                    }
                    else
                    {
                        AppliedModsHolder_Target.RemoveAt(i);
                    }
                } 
            }
        }
        catch
        {
            //...
        }
    }

    public void IssueAJump(Vector3 JumpDestination)
    {
        if(PositionsToJumpTo.Count == 0)
        {
            StartY = transform.position[1];
        
        }
        EM.EnableEM(false);
        //JumpDestination[1] = transform.position.y;
        if (CPhys == null) { CPhys = gameObject.GetComponent<CustomPhysics>(); }

        PositionsToJumpTo.Add(JumpDestination + EntityOffset);
        JumpHasStarted.Add(false); JumpHasCompleted.Add(false);
        SetupTimeComplete.Add(false); BreakdownTimeComplete.Add(false);

        DistanceOfOriginFromPosition.Add(Vector3.Distance(transform.position, JumpDestination));
    }

    private void EndAJump()
    {
        if (VFX_SpawnOnArrive.Length > 0)
        {
            for (int i = 0; i < VFX_SpawnOnArrive.Length; i++)
            {
                GameObject tmpVFX = Instantiate(VFX_SpawnOnArrive[i]);

                if (VFX_SpawnOnArrive_SpawnBone.Length - i > 0)
                {
                    if (VFX_SpawnOnArrive_SpawnBone[i] != null)
                    {
                        tmpVFX.transform.position = VFX_SpawnOnArrive_SpawnBone[i].position;
                    }
                    else
                    {
                        tmpVFX.transform.position = VFX_SpawnOnArrive_AttachBone[i].position;
                        tmpVFX.transform.parent = VFX_SpawnOnArrive_AttachBone[i];
                    }
                }                
                else if (VFX_SpawnOnArrive_AttachBone.Length - i < 0)
                {
                    tmpVFX.transform.position = transform.position;
                }
                else
                {
                    if (VFX_SpawnOnArrive_AttachBone[i] != null)
                    {
                        tmpVFX.transform.position = VFX_SpawnOnArrive_AttachBone[i].position;
                        tmpVFX.transform.parent = VFX_SpawnOnArrive_AttachBone[i];
                    }
                    else
                    {
                        tmpVFX.transform.position = transform.position;
                    }
                }
            }
        }

        if(Mod_Target.Length != 0)
        {
            for (int mod = 0; mod < Mod_Target.Length; mod++)
            {
                ApplyAModifier(AppliedModsHolder_Target, Mod_Target[mod], Mod_TargetType[mod], Mod_GenericRadius[mod], transform, transform.position);
            }
        }

        EM.EnableEM(true);
        transform.position = PositionsToJumpTo[0];
        if (PositionsToJumpTo.Count > 1)
        {
            EM.EnableEM(false);
        }

        //Remove Last Jump
        PositionsToJumpTo.RemoveAt(0);
        JumpHasStarted.RemoveAt(0);
        JumpHasCompleted.RemoveAt(0);
        SetupTimeComplete.RemoveAt(0);
        BreakdownTimeComplete.RemoveAt(0);
        DistanceOfOriginFromPosition.RemoveAt(0);
        SetupTimeCurrent = BI.EBPs.JumpSetupTime; BreakdownTimeCurrent = BI.EBPs.JumpBreakdownTime;
        InAirTimer = 0;

        IsCurrentlyJumping = false;
        //CPhys.GenericSetMovementStatus(true);
        HasSpawnedTPVFX = false;
    }

    private void ActivateNextJump()
    {
        StartY = transform.position[1];
        InitialJumpOffset = transform.position[1];
        HasReachedTarget = false;
        if (PositionsToJumpTo.Count > 0)
        {
            IsCurrentlyJumping = true;
            //CPhys.GenericSetMovementStatus(false);

            //Setup New Jump
            JumpHasStarted[0] = true;

            float JumpDistance = Vector3.Distance(transform.position, PositionsToJumpTo[0]);

            float TimeInAir = JumpDistance / BI.EBPs.JumpVel;
            float HeightForJump;

            if (JumpDistance >= BI.EBPs.JumpDistanceMax) { HeightForJump = BI.EBPs.JumpHeightMax; }
            else { HeightForJump = (JumpDistance / BI.EBPs.JumpDistanceMax) * BI.EBPs.JumpHeightMax; }
        }
    }

    private void CheckForJumpInput()
    {
        if (BI.OwnedByPlayer == AcceptSelectionFromPlayer && Input.GetKey(KeyCode.LeftControl) && Input.GetMouseButtonUp(1) && GIS.GetSelectedStatus() == "Selected")
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~LayerMask.NameToLayer("UI")))
            {
                if (GI.CheckJumpLocationBlockers(hit.point))
                {
                    if (hit.collider.gameObject.tag == "Terrain")
                    {
                        float temp_distance;
                        if (PositionsToJumpTo.Count == 0)
                        {
                            temp_distance = Vector3.Distance(transform.position, hit.point);
                        }
                        else
                        {
                            temp_distance = Vector3.Distance(PositionsToJumpTo[PositionsToJumpTo.Count - 1], hit.point);
                        }

                        if (temp_distance <= BI.EBPs.JumpDistanceMax)
                        {
                            IssueAJump(hit.point);
                        }
                    }
                }
            }
        }
    }

    private void JumpProgress()
    {
        if (!HasReachedTarget)
        {
            Vector3 tmpNewPos;
            tmpNewPos = Vector3.MoveTowards(transform.position, PositionsToJumpTo[0], BI.EBPs.JumpVel * Time.deltaTime);

            Vector3 CurrentPosNoY = transform.position;
            CurrentPosNoY[1] = PositionsToJumpTo[0][1];

            float DistanceAsPercent = 1 - (Vector3.Distance(CurrentPosNoY, PositionsToJumpTo[0]) / DistanceOfOriginFromPosition[0]);
            DistanceAsPercent = Mathf.Clamp(DistanceAsPercent, 0, 1);
            tmpNewPos[1] = BI.EBPs.JumpCurve.Evaluate(DistanceAsPercent) * BI.EBPs.JumpHeightMax;
            tmpNewPos[1] += Mathf.Lerp(StartY, PositionsToJumpTo[0][1], DistanceAsPercent);
            transform.position = tmpNewPos;

            if (Vector3.Distance(transform.position, PositionsToJumpTo[0]) <= AcceptedJumpDiscrepency)
            {
                HasReachedTarget = true;
                JumpHasCompleted[0] = true;
            }
        }
    }

    private void ApplyAModifier(List<GameObject> AddToList, GameObject ApplyMod, string TargetType, float Size, Transform Target, Vector3 ModPos)
    {
        GameObject NewMod = Instantiate(ApplyMod, ModPos, new Quaternion(), Target);
        ModifierApplier NewMod_MA = NewMod.GetComponent<ModifierApplier>();
        NewMod_MA.TakeDamageFromTarget = BI.EBPs.PositionInLvlHierarchy;
        NewMod_MA.OwnedByPlayer = BI.OwnedByPlayer;
        NewMod_MA.DesiredTarget = TargetType;
        NewMod.transform.localScale = new Vector3(Size, Size, Size);
        NewMod.transform.position = ModPos;
        NewMod.name = "Jump Modifier: " + gameObject.name;
        Actions.OnAddNewModifier.InvokeAction(NewMod_MA);
        AddToList.Add(NewMod);
        ModCurrent_Time.Add(0);
    }
}
