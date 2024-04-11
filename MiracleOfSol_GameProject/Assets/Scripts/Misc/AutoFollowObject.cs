using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoFollowObject : MonoBehaviour
{
    public Transform Obj;
    public float Vel = 1;
    public bool DestroyOnObjDestroy = false;

    public bool StartAtObj = true;
    public float DelayedDeathTime = 0;

    private bool IsBeingDestroyed = false;

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(Setup), 0.1f);
    }

    private void Setup()
    {
        if (Obj == null) { Obj = Camera.main.transform; }
        if (StartAtObj) { transform.position = Obj.transform.position; }
        if (transform.parent = Obj) { transform.parent = null; }
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsBeingDestroyed)
        {
            if (Obj == null)
            {
                if (DestroyOnObjDestroy) { IsBeingDestroyed = true; Invoke(nameof(DelayedDeath), DelayedDeathTime); }
            }
            else
            {
                if (transform.position != Obj.position) { transform.position = Vector3.MoveTowards(transform.position, Obj.position, Vel * Time.deltaTime); }
            }
        }
    }

    private void DelayedDeath()
    {
        Destroy(gameObject);
    }
}
