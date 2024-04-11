using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class SkinnedMeshToMesh : MonoBehaviour
{
    public SkinnedMeshRenderer SkinnedMesh;
    public Mesh StandardMesh;
    public bool IsActive = true;
    public float TickRate = 0.05f;


    private VisualEffect[] VFXGraph;
    private List<float> StoredVFXRates = new List<float>();
    private bool IsDisabled = false;
    //private Mesh FinalSkinnedMesh;

    // Start is called before the first frame update
    void Start()
    {
        VFXGraph = gameObject.GetComponentsInChildren<VisualEffect>();

        foreach (VisualEffect VFX in VFXGraph)
        {
            StoredVFXRates.Add(VFX.GetFloat("Rate"));
        }
        UpdateVFXMeshInChildren();
        //InvokeRepeating(nameof(UpdateVFXMeshInChildren), TickRate, TickRate);
    }

    private void UpdateVFXMeshInChildren()
    {
        if (IsActive)
        {
            if (IsDisabled)
            {
                EnableVFX();
            }

            Mesh tmpMesh = new Mesh();
            SkinnedMesh.BakeMesh(tmpMesh);
            Vector3[] Verts = tmpMesh.vertices;
            Mesh tmpMeshCorrected = new Mesh();
            tmpMeshCorrected.vertices = Verts;

            foreach (VisualEffect VFX in VFXGraph)
            {
                if (SkinnedMesh != null)
                {
                    VFX.SetMesh("Mesh", tmpMeshCorrected);
                }
                else
                {
                    VFX.SetMesh("Mesh", StandardMesh);
                    CancelInvoke();
                }
            }
        }
        else if (!IsDisabled)
        {
            DisableVFX();
        }
    }

    public void DisableVFX()
    {
        CancelInvoke();

        if (!IsDisabled)
        {
            IsDisabled = true;
            StoredVFXRates.Clear();
            foreach (VisualEffect VFX in VFXGraph)
            {
                StoredVFXRates.Add(VFX.GetFloat("Rate"));
                VFX.SetFloat("Rate", 0);
            }
        }
    }

    public void EnableVFX()
    {
        CancelInvoke();

        if (IsDisabled)
        {
            IsDisabled = false;
            for (int i = 0; i < StoredVFXRates.Count; i++)
            {
                VFXGraph[i].SetFloat("Rate", StoredVFXRates[i]);
            }
            StoredVFXRates.Clear();
        }

        InvokeRepeating(nameof(UpdateVFXMeshInChildren), TickRate, TickRate);
    }
}
