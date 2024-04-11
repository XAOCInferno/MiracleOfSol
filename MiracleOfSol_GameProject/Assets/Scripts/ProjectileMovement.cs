using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileMovement : MonoBehaviour
{
    private Transform ProjectileTargetEntity;
    private Vector3 ProjectileTargetPos;
    private float Vel;
    private float DesiredDistanceFromTarget;
    public bool HasReachedTarget = false;
    private bool IsEnabled = false;
    private bool UseAnimationCurve = false;
    private AnimationCurve AC;
    private Vector3 CasterSpawnPos;
    private float ACMaxY;
    private float MaxDistance;

    public void SetupProjectile(Transform Target, Vector3 TargetPos, float NewVel, float DesiredDistanceForCol, bool NewUseAnimationCurve, AnimationCurve tmpAC, float NewACMaxY, Vector3 NewCasterSpawnPos)
    {
        ProjectileTargetEntity = Target; ProjectileTargetPos = TargetPos; Vel = NewVel; DesiredDistanceFromTarget = DesiredDistanceForCol;
        UseAnimationCurve = NewUseAnimationCurve; AC = tmpAC; CasterSpawnPos = NewCasterSpawnPos; ACMaxY = NewACMaxY;
        IsEnabled = true;

        if (UseAnimationCurve) { MaxDistance = Vector3.Distance(ProjectileTargetPos, CasterSpawnPos); }

        gameObject.TryGetComponent(out ProjectileFaceTowardsTarget tmpPFTT);
        if(tmpPFTT != null) { tmpPFTT.SetupProjectile(Target, TargetPos); }
    }

    // Update is called once per frame
    void Update()
    {
        if (!HasReachedTarget && IsEnabled)
        {
            Vector3 tmpNewPos;
            if (ProjectileTargetEntity != null)
            {
                //MoveTowardsEntity;
                ProjectileTargetPos = ProjectileTargetEntity.position;
                tmpNewPos = Vector3.MoveTowards(transform.position, ProjectileTargetEntity.position, Vel * Time.deltaTime);
            }
            else
            {
                //MoveTowardsTargetPos;
                tmpNewPos = Vector3.MoveTowards(transform.position, ProjectileTargetPos, Vel * Time.deltaTime);
            }


            if (UseAnimationCurve)
            {
                Vector3 CurrentPosNoY = transform.position;
                CurrentPosNoY[1] = ProjectileTargetPos[1];

                float DistanceAsPercent = 1 - (Vector3.Distance(CurrentPosNoY, ProjectileTargetPos) / MaxDistance);
                DistanceAsPercent = Mathf.Clamp(DistanceAsPercent, 0, 1);
                tmpNewPos[1] = AC.Evaluate(DistanceAsPercent) * ACMaxY + CasterSpawnPos[1] - ((CasterSpawnPos[1] - ProjectileTargetPos[1]) * DistanceAsPercent);

                //tmpNewPos[1] = AC.Evaluate(DistanceAsPercent) * ACMaxY + CasterSpawnPos[1];
            }

            try
            {
                transform.position = tmpNewPos;
            }
            catch
            {
                //Invalid move pos, not important tbh
            }


            if (Vector3.Distance(transform.position, ProjectileTargetPos) <= DesiredDistanceFromTarget)
            {
                HasReachedTarget = true;
            }
        }
    }

    public bool GetProgress()
    {
        return HasReachedTarget;
    }
}
