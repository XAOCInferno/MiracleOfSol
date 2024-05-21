using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct CircularVisionEmitter
{
    public int ID;
    public Vector2Int Centre;
    public int Radius;

    public CircularVisionEmitter(int _id, Vector2Int _centre, int _radius)
    {
        ID = _id;
        Centre = _centre;
        Radius = _radius;
    }
}

public class VisionEmitter : CachedObject
{
    [SerializeField] private int VisionRadius = 10;
    private Vector3 previousPosition = new();

    private void OnEnable()
    {
        StartCoroutine(nameof(CheckIfMovedAndUpdateVision));
    }
    private void OnDisable()
    {
        StopCoroutine(nameof(CheckIfMovedAndUpdateVision));
    }

    private IEnumerator CheckIfMovedAndUpdateVision()
    {
        while (true)
        {
            if(transform.position != previousPosition)
            {
                previousPosition = transform.position;
                Actions.OnDemandFogRedraw.InvokeAction();
            }
            Actions.OnDrawVisionCircle.InvokeAction(new(ID, new((int) transform.position.x, (int) transform.position.z ), VisionRadius));
            yield return new WaitForSeconds(0.25f);
        }
    }
}
