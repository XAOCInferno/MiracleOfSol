using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneActorManager : MonoBehaviour
{
    private EntityMovement EM;
    private Transform CurrentLookAtObj;
    private float TurnRate = 2.5f;

    private void Start()
    {
        gameObject.TryGetComponent(out EM);
    }

    public void Actor_MoveToPos(Vector3 NewPos)
    {
        EM.SetMoveDestination(NewPos);
    }

    private void Update()
    {
        if(CurrentLookAtObj != null)
        {
            Actor_LookAtEntity();
        }
    }

    private void Actor_LookAtEntity()
    {
        if (CurrentLookAtObj != null)
        {
            Vector3 lookPos = CurrentLookAtObj.transform.position - transform.position;
            Quaternion rotation = Quaternion.LookRotation(lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * TurnRate);
        }
    }

    public void Set_ActorLookAtEntity(Transform NewTarget)
    {
        CurrentLookAtObj = NewTarget;
    }
}
