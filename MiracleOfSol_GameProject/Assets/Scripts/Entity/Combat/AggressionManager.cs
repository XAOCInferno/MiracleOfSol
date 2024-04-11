using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AggressionManager : MonoBehaviour
{
    private BasicInfo BI;
    private Combat Combat;
    private List<Transform> EnemiesInArea = new List<Transform>();

    private void Start()
    {
        SetCombatMaster(transform.parent.GetComponent<Combat>());
        BI = Combat.GetComponent<BasicInfo>();
    }

    public void SetEnemiesInArea(List<Transform> NewEnemies) => CheckEnemiesInArea(NewEnemies);

    public void SetCombatMaster(Combat combat)
    {
        Combat = combat;
    }

    public GameObject FindClosestEnemyTarget()
    {
        GameObject ClosestTarget = null;
        float ClosestDistance = -1;

        for (int i = 0; i < EnemiesInArea.Count; i++) //  Transform child in EnemiesInArea)
        {
            if (EnemiesInArea[i] != null)
            {
                Vector3 fromPosition = transform.position;
                Vector3 toPosition = EnemiesInArea[i].transform.position;
                float Distance = Vector3.Distance(toPosition, fromPosition);

                if (Distance < ClosestDistance || ClosestDistance == -1)
                {
                    ClosestDistance = Distance;
                    ClosestTarget = EnemiesInArea[i].gameObject;
                }
            }
            else { EnemiesInArea.RemoveAt(i); }
        }

        return ClosestTarget;
    }

    private void CheckEnemiesInArea(List<Transform> newList)
    {
        EnemiesInArea = new List<Transform>();
        int EnemyCount = newList.Count;

        if (EnemyCount == 0)
        {
            Combat.SetCombatStatus();
        }
        else
        {
            for (int z = 0; z < newList.Count; z++)
            {
                Transform entity = newList[z];
                BasicInfo TargetInfo = entity.gameObject.GetComponent<BasicInfo>();

                if (TargetInfo != null)
                {//TargetInfo.OwnedByPlayer != BI.OwnedByPlayer &&
                    if ( TargetInfo.OwnedByPlayer >= 0 && TargetInfo.EBPs.CanBeAttacked == true)
                    {
                        //Debug.Log("Enemy Entity Targeted! Name: " + TargetInfo.EBPs.name);
                        EnemiesInArea.Add(entity.transform);
                        Combat.SetCombatStatus(true);
                    }
                }

            }
        }
    }

    /*private void OnTriggerEnter(Collider collision) [[OLD AND INEFFICIENT, REPLACED WITH COMBATMANAGER]]
    {
        if (collision.gameObject.tag == "Entity")
        {
            BasicInfo TargetInfo = collision.gameObject.GetComponent<BasicInfo>();

            if (TargetInfo != null)
            {
                if (TargetInfo.OwnedByPlayer != BI.OwnedByPlayer && TargetInfo.OwnedByPlayer >= 0 && TargetInfo.EBPs.CanBeAttacked == true)
                {
                    bool IsNotInList = true;
                    for (int i = 0; i < EnemiesInArea.Count; i++)
                    {
                        if (EnemiesInArea[i] != null)
                        {
                            if (EnemiesInArea[i].gameObject == collision.gameObject)
                            {
                                IsNotInList = false;
                                break;
                            }
                        }
                        else
                        {
                            EnemiesInArea.RemoveAt(i);
                        }
                    }

                    if (IsNotInList)
                    {
                        //Debug.Log("Enemy Entity Targeted! Name: " + TargetInfo.EBPs.name);
                        EnemiesInArea.Add(collision.transform);
                        Combat.SetCombatStatus(true);
                    }
                }
            }
        }
    }

    private void OnTriggerExit(Collider collision)
    {
        int EnemyCount = EnemiesInArea.Count;

        if (EnemyCount == 0)
        {
            Combat.SetCombatStatus();
        }
        else
        {
            for (int i = 0; i < EnemyCount; i++)
            {
                if (EnemiesInArea[i] != null)
                {
                    if (EnemiesInArea[i].gameObject == collision.gameObject)
                    {
                        //Debug.Log("Enemy Entity Lost! Name: " + collision.GetComponent<BasicInfo>().EBPs.name);
                        EnemiesInArea.RemoveAt(i);
                        break;
                    }
                }
                else
                {
                    EnemiesInArea.RemoveAt(i);
                }
            }
        }
    }*/
}
