using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct DeathExplosionInfo
{
    public bool IsExclusive;
    public GameObject NewExplosion;
    public float ModLifetime;
    public bool ModIsTimed;
    public string ExplosionName;
    public bool DestroyOnUse;

    public DeathExplosionInfo(bool _isExclusive, GameObject _newExplosion, float _modLifetime, bool _modIsTimed, string _explosionName, bool _destroyOnUse)
    {
        IsExclusive = _isExclusive;
        NewExplosion = _newExplosion;
        ModLifetime = _modLifetime;
        ModIsTimed = _modIsTimed;
        ExplosionName = _explosionName;
        DestroyOnUse = _destroyOnUse;
    }
}

public class DeathExplosionManager : MonoBehaviour
{
    private static List<GameObject> EntityHolder = new List<GameObject>();
    private static List<GameObject> DeathExplosionsToSpawn = new List<GameObject>();
    private static List<float> CurrentDeathExplosionTimers = new List<float>();
    private static List<bool> DeathExplosionIsTimed = new List<bool>();
    private static List<string> DeathExplosionName = new List<string>();
    private static List<bool> DestroyModOnActivate = new List<bool>();
    private static float TickRate = 0.15f;

    private void OnEnable()
    {
        Actions.OnCreateDeathExplosion += SetNewDeathExplosion;
        StartCoroutine(nameof(InternalUpdate));
    }

    private void OnDisable()
    {
        Actions.OnCreateDeathExplosion -= SetNewDeathExplosion;
        StopCoroutine(nameof(InternalUpdate));
    }


    private IEnumerator InternalUpdate()
    {
        while (true)
        {
            Internal_CheckForLifetimes();
            yield return new WaitForSeconds(TickRate);
        }
    } 

    private static void CheckForLifetimes(bool ChangeTime)
    {
        for (int i = 0; i < CurrentDeathExplosionTimers.Count; i++)
        {
            if (DeathExplosionIsTimed[i])
            {
                if (ChangeTime) { CurrentDeathExplosionTimers[i] -= TickRate; }

                if(CurrentDeathExplosionTimers[i] <= 0)
                {
                    RemoveDEAtPosition(i);
                }
            }
        }
    }

    public static void External_CheckForLifetimes()
    {
        CheckForLifetimes(false);
    }
    private static void Internal_CheckForLifetimes()
    {
        CheckForLifetimes(true);
    }

    public void SetNewDeathExplosion(DeathExplosionInfo ExplosionInfo, GameObject ExplosionHolder)
    {
        if (ExplosionInfo.NewExplosion != null && ExplosionHolder != null)
        {
            if (ExplosionInfo.ExplosionName != "" && ExplosionInfo.IsExclusive)
            {
                for (int i = 0; i < DeathExplosionName.Count; i++)
                {
                    if (DeathExplosionName[i] == ExplosionInfo.ExplosionName && ExplosionHolder == EntityHolder[i])
                    {
                        RemoveDEAtPosition(i);
                    }
                }
            }

            EntityHolder.Add(ExplosionHolder);
            DeathExplosionsToSpawn.Add(ExplosionInfo.NewExplosion);
            CurrentDeathExplosionTimers.Add(ExplosionInfo.ModLifetime);
            DeathExplosionIsTimed.Add(ExplosionInfo.ModIsTimed);
            DeathExplosionName.Add(ExplosionInfo.ExplosionName);
            DestroyModOnActivate.Add(ExplosionInfo.DestroyOnUse);
        }
    }

    private static void RemoveDEAtPosition(int pos)
    {
        EntityHolder.RemoveAt(pos);
        DeathExplosionsToSpawn.RemoveAt(pos);
        CurrentDeathExplosionTimers.RemoveAt(pos);
        DeathExplosionIsTimed.RemoveAt(pos);
        DeathExplosionName.RemoveAt(pos);
        DestroyModOnActivate.RemoveAt(pos);
    }

    public static List<GameObject> GetEntityDeathExplosions(string EntityName)
    {
        List<GameObject> tmpList = new List<GameObject>();
        try
        {
            if (EntityName != null)
            {
                for (int i = 0; i < EntityHolder.Count; i++)
                {
                    if (EntityName == EntityHolder[i].name)
                    {
                        tmpList.Add(DeathExplosionsToSpawn[i]);
                        if (DestroyModOnActivate[i]) { RemoveDEAtPosition(i); };
                    }
                }
            }
        }
        catch
        {
            Debug.LogWarning("ERROR! In DeathExplosionManager/GetEntityDeathExplosion. Cannot find a death explosion!");
        }

        return tmpList;
    }
}
