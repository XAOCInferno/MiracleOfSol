using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileFaceTowardsTarget : MonoBehaviour
{
    public float RotationRate = 5;
    private Transform TargetEntity;
    private Vector3 TargetPos;
    private bool IsActive = false;
    public Vector3 MoveRotation;

    public void SetupProjectile(Transform NewTarget, Vector3 NewPosition)
    {
        MoveRotation = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1));
        TargetEntity = NewTarget; TargetPos = NewPosition; IsActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive)
        {
            if(TargetEntity != null)
            {
                TargetPos = TargetEntity.position;
            }

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation((TargetPos - transform.position + MoveRotation).normalized), Time.deltaTime * RotationRate);
        }
    }
}
