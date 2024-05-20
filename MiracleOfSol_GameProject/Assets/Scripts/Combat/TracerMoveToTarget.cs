using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TracerMoveToTarget : MonoBehaviour
{
    public float TracerVel = 20;

    private Transform Target;
    private bool IsActive = false;

    public void EnableTracer(Transform newTarget)
    {
        Target = newTarget;
        transform.LookAt(Target);
        IsActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (Target != null && IsActive)
        {
            if(Vector3.Distance(transform.position, Target.position) <= 0.25f)
            {
                InitDestroySelf();
            }
            else
            {
                transform.position = Vector3.MoveTowards(transform.position, Target.position, TracerVel * Time.deltaTime);
            }
        }
        else
        {
            transform.position = Vector3.MoveTowards(transform.position, Vector3.forward, Time.deltaTime);
            InitDestroySelf();
        }
    }

    private void InitDestroySelf()
    {
        IsActive = false;
        UnityEngine.VFX.VisualEffect[] tmpVFX = gameObject.GetComponentsInChildren<UnityEngine.VFX.VisualEffect>();
        foreach(UnityEngine.VFX.VisualEffect VFX in tmpVFX) { VFX.SetFloat("ConstantRate", 0); }
        Invoke(nameof(DestroySelf), 0.2f);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
    }
}
