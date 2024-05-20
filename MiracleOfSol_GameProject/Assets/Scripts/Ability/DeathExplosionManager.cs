using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathExplosionManager : MonoBehaviour
{
    public List<GameObject> EntityHolder = new List<GameObject>();
    public List<GameObject> DeathExplosionsToSpawn = new List<GameObject>();
    public List<float> CurrentDeathExplosionTimers = new List<float>();
    public List<bool> DeathExplosionIsTimed = new List<bool>();
    public List<string> DeathExplosionName = new List<string>();
    public List<bool> DestroyModOnActivate = new List<bool>();
    public float TickRate = 0.15f;

    private void Start() { InvokeRepeating(nameof(Internal_CheckForLifetimes), TickRate, TickRate); }

    public void CheckForLifetimes(bool ChangeTime)
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

    public void External_CheckForLifetimes()
    {
        CheckForLifetimes(false);
    }
    public void Internal_CheckForLifetimes()
    {
        CheckForLifetimes(true);
    }

    public void SetNewDeathExplosion(bool IsExclusive, GameObject ExplosionHolder, GameObject NewExplosion, float ModLifetime, bool ModIsTimed, string ExplosionName, bool DestroyOnUse)
    {
        if (NewExplosion != null && ExplosionHolder != null)
        {
            if (ExplosionName != "" && IsExclusive)
            {
                for (int i = 0; i < DeathExplosionName.Count; i++)
                {
                    if (DeathExplosionName[i] == ExplosionName && ExplosionHolder == EntityHolder[i])
                    {
                        RemoveDEAtPosition(i);
                    }
                }
            }

            EntityHolder.Add(ExplosionHolder);
            DeathExplosionsToSpawn.Add(NewExplosion);
            CurrentDeathExplosionTimers.Add(ModLifetime);
            DeathExplosionIsTimed.Add(ModIsTimed);
            DeathExplosionName.Add(ExplosionName);
            DestroyModOnActivate.Add(DestroyOnUse);
        }
    }

    private void RemoveDEAtPosition(int pos)
    {
        EntityHolder.RemoveAt(pos);
        DeathExplosionsToSpawn.RemoveAt(pos);
        CurrentDeathExplosionTimers.RemoveAt(pos);
        DeathExplosionIsTimed.RemoveAt(pos);
        DeathExplosionName.RemoveAt(pos);
        DestroyModOnActivate.RemoveAt(pos);
    }

    public List<GameObject> GetEntityDeathExplosions(string EntityName)
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
