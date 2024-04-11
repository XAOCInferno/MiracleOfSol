using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBlockerOnCapture : MonoBehaviour
{
    public float MoveMultiplier = 5;
    public AnimationCurve MoveOnCapY;
    public float TimeToMove = 5;
    public CapturePoint CP;
    public int DesiredCapper = 0;
    public bool DestroyOnEnd = true;
    public float DestroyTimeout = 2;

    private float TickRate = 0.05f;
    private float CurrentTime = 0;
    private Vector3 StartingLocation;
    private Vector3 EndLocation;

    private void Start()
    {
        StartingLocation = transform.position;
        float MaxYChange = Mathf.Clamp(MoveOnCapY.Evaluate(1), 0, 100) * MoveMultiplier;
        EndLocation = new Vector3(StartingLocation[0], StartingLocation[1] + MaxYChange, StartingLocation[2]);

        InvokeRepeating(nameof(CheckIfCaptured), 2, 1);
    }

    private void CheckIfCaptured()
    {
        if (CP.gameObject.activeSelf)
        {
            if (CP.GetIfCaptured(DesiredCapper))
            {
                CancelInvoke();
                InvokeRepeating(nameof(DoMove), 0, TickRate);
            }
        }
    }

    private void DoMove()
    {
        CurrentTime += TickRate;
        if (CurrentTime < TimeToMove)
        {
            float TimeAsPercent = CurrentTime / TimeToMove;
            transform.position = Vector3.Lerp(StartingLocation, new Vector3(StartingLocation[0], StartingLocation[1] + (Mathf.Clamp(MoveOnCapY.Evaluate(TimeAsPercent),0,100) * MoveMultiplier), StartingLocation[2]), 1);
        }
        else
        {
            transform.position = EndLocation;
            CancelInvoke();
            if (DestroyOnEnd) { Invoke(nameof(OrderDie), DestroyTimeout); }
        }
    }

    private void OrderDie()
    {
        Destroy(gameObject);
    }
}
