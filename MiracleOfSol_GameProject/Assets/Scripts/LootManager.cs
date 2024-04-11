using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LootManager : MonoBehaviour
{
    public GameObject WeaponPartsPrefab;
    public GameObject VFX_WeaponPartSpawn;
    public GameObject VFX_WeaponPartPickup;

    private int ExtraPartSpawnedMargin = 15;
    private float MinTimeBeforePickup = 2;

    private float GravitationRange = 8;
    private float DeleteRange = 0.2f;
    private float MaxMoveVel = 8;
    private float MaxWeaponPartsToSpawnInOneGo = 25;
    private Vector2 MaxThrowVel = new Vector2(1.25f, 2);
    private int CurrentWeaponParts;
    private List<Transform> AllWeaponParts = new List<Transform>();
    private List<float> WeaponPartsLifetime = new List<float>();
    private GameInfo GI;

    private void Start()
    {
        CurrentWeaponParts = PlayerPrefs.GetInt("WeaponPartsCount", 0);
        gameObject.TryGetComponent(out GI);
    }

    public int GetWeaponParts() { return CurrentWeaponParts; }

    public void AddWeaponParts(int ChangeBy)
    {
        CurrentWeaponParts += ChangeBy;
    }

    public void AddWeaponPartsAtLocation(int ChangeBy, Vector3 Location)
    {
        AddWeaponParts(ChangeBy);
        if(ChangeBy > ExtraPartSpawnedMargin)
        {
            int tmpNumber = ChangeBy;
            int factor = 0;
            while(tmpNumber >= ExtraPartSpawnedMargin && factor < MaxWeaponPartsToSpawnInOneGo)
            {
                tmpNumber -= ExtraPartSpawnedMargin;
                factor++;
            }
            for(int i = 0; i < factor; i++) { SpawnAPart(Location); }
        }
        else
        {
            SpawnAPart(Location);
        }
    }

    private void SpawnAPart(Vector3 Location)
    {
        GameObject tmpObj = Instantiate(WeaponPartsPrefab);
        tmpObj.transform.position = Location + new Vector3(Random.Range(-1.25f,1.25f), Random.Range(0.1f, 0.25f), Random.Range(-1.25f, 1.25f));
        tmpObj.transform.rotation = Random.rotation;
        WeaponPartsLifetime.Add(0);
        AllWeaponParts.Add(tmpObj.transform);

        SpawnPickupVFX(tmpObj.transform.position, VFX_WeaponPartSpawn);
    }

    private void Update()
    {
        UpdateLifetimes();

        try
        {
            if (GI.AllPlayers_SM.Count > 0 && AllWeaponParts.Count > 0)
            {
                List<List<GameObject>> tmpSquads = GI.AllPlayers_SM[0].Get_AllSquadLists();
                for (int i = 0; i < AllWeaponParts.Count; i++)
                {
                    if (WeaponPartsLifetime[i] >= MinTimeBeforePickup)
                    {
                        for (int j = 0; j < tmpSquads.Count; j++)
                        {
                            float tmpDistance = Vector3.Distance(tmpSquads[j][0].transform.position, AllWeaponParts[i].transform.position);
                            if (tmpDistance <= DeleteRange)
                            {
                                SpawnPickupVFX(AllWeaponParts[i].transform.position, VFX_WeaponPartPickup);
                                DestroyLootAtPos(i);
                            }
                            else if (tmpDistance <= GravitationRange)
                            {
                                AllWeaponParts[i].transform.position = Vector3.MoveTowards(AllWeaponParts[i].transform.position, tmpSquads[j][0].transform.position, Mathf.Lerp(0, MaxMoveVel, GravitationRange / tmpDistance) * Time.deltaTime);
                            }
                        }
                    }
                }
            }
        }
        catch
        {
            Debug.LogWarning("ERROR! In LootManager/Update. Cannot Spawn OR Gravitate Weapon Part");
        }
    }

    private void UpdateLifetimes()
    {
        for (int i = 0; i < WeaponPartsLifetime.Count; i++)
        {
            WeaponPartsLifetime[i] += Time.deltaTime;
        }
    }

    private void SpawnPickupVFX(Vector3 Location, GameObject VFX)
    {
        GameObject tmpObj = Instantiate(VFX);
        tmpObj.transform.position = Location;
    }

    private void DestroyLootAtPos(int i)
    {
        Destroy(AllWeaponParts[i].gameObject);
        AllWeaponParts.RemoveAt(i);
        WeaponPartsLifetime.RemoveAt(i);
    }
}
