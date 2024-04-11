using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat_WeaponFaceAtTarget : MonoBehaviour
{
    public Combat selfCombat;
    public Quaternion RotationOffset;
    public float RotationSpeed = 5;
    private Quaternion StartingRotation;
    private Quaternion DesiredRotation;
    private Transform DesiredTarget;

    private void Start()
    {
        if (selfCombat = null) { selfCombat = transform.parent.GetComponent<Combat>(); }
        StartingRotation = transform.rotation;
        DesiredRotation = StartingRotation;
        InvokeRepeating(nameof(TryToFindDesiredTarget), 4, 1);
        InvokeRepeating(nameof(RotateRandomly), 1 + Random.Range(-1f,1f), 10 + Random.Range(-2f, 2f));
    }

    private void Update()
    {
        if (DesiredTarget != null)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(DesiredTarget.position - transform.position)*RotationOffset, RotationSpeed * Time.deltaTime);
        }
        else
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, DesiredRotation*RotationOffset, ((RotationSpeed / 2) * Time.deltaTime));
        }
    }

    private void TryToFindDesiredTarget()
    {
        if (selfCombat != null)
        {
            if (selfCombat.Target != null)
            {
                DesiredTarget = selfCombat.Target.transform;
            }
            else
            {
                DesiredTarget = null;
            }
        }
        else
        {
            Debug.LogWarning("Combat_WeaponFaceAtTarget: Entity '" + gameObject.name + "' Has No Combat Module. Attempting to add one.");
            try
            {
                selfCombat = transform.parent.GetComponent<Combat>();
                Debug.LogWarning("Adding Combat Module: Success!");
            }
            catch
            {
                Debug.LogError("Combat_WeaponFaceAtTarget: Entity '" + gameObject.name + "' Has No Combat Module |and can't find one!!");
                Destroy(this);
            }
        }

    }

    private void RotateRandomly()
    {
        //DesiredRotation = transform.rotation * new Quaternion(0, Random.Range(-180, 180), 0, transform.rotation.w);
    }
}
