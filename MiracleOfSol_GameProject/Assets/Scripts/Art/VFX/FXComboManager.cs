using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FXComboManager : MonoBehaviour
{
    public Transform VFXStorage;
    public bool StorageIsSelf = false;
    public bool IsLooping = false;
    public bool IgnoreComboLogic = false;
   
    [SerializeField] private GameObject[] AllEffects;
    [SerializeField] private List<Vector3> PositionOffset;
    [SerializeField] private List<Quaternion> RotationOffset;
    [SerializeField] private List<Vector3> SizeScale;

    private void Start()
    {
        if (StorageIsSelf) { VFXStorage = transform; }
        for(int i = RotationOffset.Count; i < AllEffects.Length; i++)
        {
            RotationOffset.Add(new Quaternion());
        }

        for (int i = PositionOffset.Count; i < AllEffects.Length; i++)
        {
            PositionOffset.Add(new Vector3());
        }

        for (int i = SizeScale.Count; i < AllEffects.Length; i++)
        {
            SizeScale.Add(new Vector3(1,1,1));
        }
    }

    public void SpawnComboEffect()
    {
        for (int i = 0; i < AllEffects.Length; i++) //foreach (GameObject Effect in AllEffects)
        {
            try
            {
                GameObject TemporaryEffect = Instantiate(AllEffects[i], transform.position, transform.rotation, VFXStorage);
                TemporaryEffect.transform.rotation = RotationOffset[i];
                TemporaryEffect.transform.position += PositionOffset[i];
                TemporaryEffect.transform.localScale = SizeScale[i];

                if (!IgnoreComboLogic)
                {
                    TemporaryEffect.TryGetComponent(out VFXLifetimeManager TemporaryEffect_VFXLM);

                    if (TemporaryEffect_VFXLM != null)
                    {
                        TemporaryEffect_VFXLM.IsLooping = IsLooping;
                    }
                }

                TemporaryEffect.TryGetComponent(out VFXSpawner TemporaryEffect_VFXS);

                if (TemporaryEffect_VFXS != null)
                {
                    TemporaryEffect_VFXS.VFXStorage = VFXStorage;
                }
            }
            catch
            {
                Debug.LogWarning("ERROR! In FXComboManager/SpawnComboEffect. Cannot instantiate new VFX");
            }
        }
    }
}
