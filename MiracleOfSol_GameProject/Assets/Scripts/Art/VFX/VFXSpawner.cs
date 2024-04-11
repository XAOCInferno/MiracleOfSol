using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VFXSpawner : MonoBehaviour
{
    public GameObject VFX;
    public Transform VFXStorage;

    public bool SpawnAtPoint = false;
    public bool IsActive = true;
    public bool SpawnOnStart = true; //Start spawning
    public bool IsOneShot = false; //Spawn 1 effect then disable the spawner
    public bool Cluster_SpawnAtSameSpot = true; //Spawn at different places or all ontop eachother
    public bool LoopEffects = false; //NOT RECOMMENDED!!!!! Only use on a one-shot spawner, or spawner who will become disabled!

    public float Spawn_MaxCooldown = 6; //Longest cooldown
    public float Spawn_MinCooldown = 3; //Shortest cooldown
    public float Spawn_Rdmness = 1.5f; 

    public int Cluster_NumberEffectsToSpawn = 3; //How many of the effects to spawn. Careful of performance with higher numbers


    private Transform SpawnArea;

    private float Spawn_CurrentCooldown;
    private float Spawn_CounterCooldown;

    // Start is called before the first frame update
    void Start()
    {
        //gameObject.GetComponent<MeshRenderer>().enabled = false;
        IsActive = SpawnOnStart;
        SpawnArea = transform;

        if(VFXStorage == null)
        {
            VFXStorage = GameObject.FindWithTag("VFX_Storage").transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive)
        {
            if (Spawn_CurrentCooldown >= Spawn_CounterCooldown)
            {
                SpawnVFX();
                ResetSpawnCounter();
            }
            else
            {
                Spawn_CurrentCooldown += Time.deltaTime;
            }
        }
    }

    private void SpawnVFX()
    {
        if (IsOneShot)
        {
            IsActive = false;
            Destroy(gameObject);
        }

        Vector3 TemporarySpawnLocation = GetSpawnLocation();

        for (int CurrentSpawnedFX = 0; CurrentSpawnedFX <= Cluster_NumberEffectsToSpawn; CurrentSpawnedFX++)
        {
            GameObject TemporaryFX = Instantiate(VFX, TemporarySpawnLocation, new Quaternion(), VFXStorage);
            VFXLifetimeManager TemporaryFX_VFXLM = TemporaryFX.GetComponent<VFXLifetimeManager>();
            TemporaryFX_VFXLM.IsLooping = LoopEffects;

            if (TemporaryFX_VFXLM.IsACombo)
            {
                FXComboManager TemporaryFX_FXCM = TemporaryFX.GetComponent<FXComboManager>();
                TemporaryFX_FXCM.IsLooping = LoopEffects;
                TemporaryFX_FXCM.VFXStorage = VFXStorage;
            }

            if (!Cluster_SpawnAtSameSpot)
            {
                TemporarySpawnLocation = GetSpawnLocation();
            }
        } 
    }

    private Vector3 GetSpawnLocation()
    {
        if (SpawnAtPoint)
        {
            return transform.position;
        }
        else
        {
            float Tmp_XPos = Random.Range(SpawnArea.position.x - (SpawnArea.localScale.x / 2), SpawnArea.position.x + (SpawnArea.localScale.x / 2));
            float Tmp_YPos = Random.Range(SpawnArea.position.y - (SpawnArea.localScale.y / 2), SpawnArea.position.y + (SpawnArea.localScale.y / 2));
            float Tmp_ZPos = Random.Range(SpawnArea.position.z - (SpawnArea.localScale.z / 2), SpawnArea.position.z + (SpawnArea.localScale.z / 2));
            return new Vector3(Tmp_XPos, Tmp_YPos, Tmp_ZPos);
        }
    }

    private void ResetSpawnCounter()
    {
        Spawn_CurrentCooldown = 0;
        Spawn_CounterCooldown = Random.Range(-Spawn_Rdmness, Spawn_Rdmness) + Random.Range(Spawn_MinCooldown, Spawn_MaxCooldown);

        if (Spawn_CounterCooldown > Spawn_MaxCooldown)
        {
            Spawn_CounterCooldown = Spawn_MaxCooldown;
        }
        else if (Spawn_CurrentCooldown < Spawn_MinCooldown)
        {
            Spawn_CounterCooldown = Spawn_MinCooldown;
        }
    }
}
