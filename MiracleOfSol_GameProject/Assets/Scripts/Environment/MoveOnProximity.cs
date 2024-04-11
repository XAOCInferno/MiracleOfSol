using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveOnProximity : MonoBehaviour
{
    public bool DoMoveLogic = true;
    public float DesiredEntityDistance = 10;
    public int RaceIndex = 0;
    public bool AllEntitiesRequiredInRange = true;

    public float MoveMultiplier = 5;
    public AnimationCurve MoveOnCapY;
    public float TimeToMove = 5;
    public bool DestroyOnEnd = true;
    public float DestroyTimeout = 2;

    public GameObject[] ChangeActiveStateOnMove;
    public GameObject[] ChangeActiveStateOnComplete;

    private float TickRate = 0.05f;
    private float CurrentTime = 0;
    private Vector3 StartingLocation;
    private Vector3 EndLocation;
    private List<GameObject> AllEntitiesToCheck = new List<GameObject>();
    private SquadManager SM;

    private void Start()
    {
        InvokeRepeating(nameof(TryToSetupLogic), 1, 1);
    }

    private void TryToSetupLogic()
    {
        try
        {
            CancelInvoke();
            SM = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameInfo>().AllPlayers_SM[RaceIndex];
            StartingLocation = transform.position;
            float MaxYChange = Mathf.Clamp(MoveOnCapY.Evaluate(1), 0, 100) * MoveMultiplier;
            EndLocation = new Vector3(StartingLocation[0], StartingLocation[1] + MaxYChange, StartingLocation[2]);

            InvokeRepeating(nameof(CheckProximity), 2, 1);
            InvokeRepeating(nameof(UpdateSquadLists), 1, 1);
        }
        catch
        {
            Debug.LogWarning("MoveOnProximity: Cannot Setup Initial Variables!");
        }
    }

    private void UpdateSquadLists()
    {
        AllEntitiesToCheck = new List<GameObject>();
        List<List<GameObject>> tmp_AllSquads = SM.Get_AllSquadLists();
        for (int i = 0; i < tmp_AllSquads.Count; i++)
        {
            List<GameObject> NewSquad = tmp_AllSquads[i];
            for (int j = 0; j < NewSquad.Count; j++)
            {
                AllEntitiesToCheck.Add(NewSquad[j]);
            }
        }
    }

    public void ForceMove()
    {
        CancelInvoke();
        foreach (GameObject obj in ChangeActiveStateOnMove) { obj.SetActive(!obj.activeSelf); }
        if (DoMoveLogic) { InvokeRepeating(nameof(DoMove), 0, TickRate); } else { Destroy(this); }
    }

    private void CheckProximity()
    {
        if(SM != null)
        {
            bool AnEntityIsInRange = false;
            bool AnEntityIsNotInRange = false;
            bool DoLogic = false;

            for(int i = 0; i < AllEntitiesToCheck.Count; i++)
            {
                float tmpDistance = Vector3.Distance(transform.position, AllEntitiesToCheck[i].transform.position);
                if(tmpDistance > DesiredEntityDistance)
                {
                    AnEntityIsNotInRange = true;
                    if (AllEntitiesRequiredInRange) { break; }
                }
                else
                {
                    AnEntityIsInRange = true;
                    if (!AllEntitiesRequiredInRange) { break; }
                }
            }

            if (AllEntitiesRequiredInRange)
            {
                if (!AnEntityIsNotInRange)
                {
                    DoLogic = true;
                }
            }
            else
            {
                if (AnEntityIsInRange)
                {
                    DoLogic = true;
                }
            }

            if (DoLogic)
            {
                CancelInvoke();
                foreach (GameObject obj in ChangeActiveStateOnMove) { obj.SetActive(!obj.activeSelf); }
                if (DoMoveLogic) { InvokeRepeating(nameof(DoMove), 0, TickRate); } else { Destroy(this); }
            }
        }
    }

    private void DoMove()
    {
        CurrentTime += TickRate;
        if (CurrentTime < TimeToMove)
        {
            float TimeAsPercent = CurrentTime / TimeToMove;
            transform.position = Vector3.Lerp(StartingLocation, new Vector3(StartingLocation[0], StartingLocation[1] + (Mathf.Clamp(MoveOnCapY.Evaluate(TimeAsPercent), 0, 100) * MoveMultiplier), StartingLocation[2]), 1);
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
        foreach (GameObject obj in ChangeActiveStateOnComplete) { obj.SetActive(!obj.activeSelf); }
        Destroy(gameObject);
    }
}
