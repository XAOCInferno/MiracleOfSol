using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CustomPhysics : MonoBehaviour
{
    public float Mass = 20;
    public int WeightType = 0; // 0 = Light Infantry, 1 = Med Infantry, 2 = Heavy Infantry, 3 = Light Tank, 4 = Unshakeable
    public bool IsBeingThrown = false;

    private GameInfo GI;

    private EntityMovement EM;
    private Health HealthManager;
    private Combat CombatManager;
    private JumpAndTeleport JumpManager;


    private Vector3 MovePerSec;
    private Vector3 StartPos;
    private Vector3 EndPos;

    private float TimeLeftInAir;
    private float TimeBeforeDirectionSwitch;
    private bool HasSwitched;

    private void Start()
    {
        GI = GameObject.Find("GAME_MANAGER").GetComponent<GameInfo>();

        if (WeightType < 4)
        {
            EM = gameObject.GetComponent<EntityMovement>();
        }

        HealthManager = gameObject.GetComponent<Health>();
        CombatManager = gameObject.GetComponent<Combat>();
        JumpManager = gameObject.GetComponent<JumpAndTeleport>();
    }

    private void Update()
    {
        if (IsBeingThrown)
        {
            if (TimeLeftInAir > 0)
            {
                if(TimeLeftInAir <= TimeBeforeDirectionSwitch && !HasSwitched)
                {
                    MovePerSec[1] *= -1;
                    HasSwitched = true;
                }

                TimeLeftInAir -= Time.deltaTime;
                transform.position += MovePerSec * Time.deltaTime;
            }
            else
            {
                IsBeingThrown = false;
                SetIsBeingThrown(false);
            }
        }
    }

    public void ApplyAForce(Vector3 Force, float KnockbackTime, bool ForceActivate, Transform ForceOrigin)
    {
        bool JumpIsValid = false;
        if(JumpManager != null)
        {
            if (!JumpManager.IsCurrentlyJumping)
            {
                JumpIsValid = true;
            }
        }
        else { JumpIsValid = true; }

        if (EM != null && JumpIsValid)
        {
            if (!ForceActivate)
            {
                Force *= 1 - (Mass / 100);
            }

            if (!IsBeingThrown || ForceActivate)
            {
                CalculateThrowVector3(Force, KnockbackTime);
                SetIsBeingThrown(true, ForceOrigin);
            }
        }
    }

    private void CalculateThrowVector3(Vector3 Force, float KnockbackTime)
    {
        TimeLeftInAir = KnockbackTime;
        TimeBeforeDirectionSwitch = KnockbackTime / 2;
        StartPos = transform.position;
        EndPos = transform.position + Force;

        for(int i = 0; i < 3; i++)
        {
            MovePerSec[i] = EndPos[i] / StartPos[i] / KnockbackTime;
        }

        
        float TempYMove = MovePerSec[1] / 2;

        MovePerSec = Vector3.Scale(transform.forward, MovePerSec);
        MovePerSec[1] = TempYMove;
    }

    public void SetIsBeingThrown(bool SetTo = true, Transform FaceTowards = null) //TEMPORARY!!!!!!!
    {
        if (EM != null)
        {
            GenericSetMovementStatus(!SetTo);
            IsBeingThrown = SetTo;
            HasSwitched = false;
            if (FaceTowards != null) { transform.LookAt(FaceTowards, transform.up); }
        }
    }

    public void GenericSetMovementStatus(bool status = true)
    {
        EM.EnableEM(status);
    }
}
