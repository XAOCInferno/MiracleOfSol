using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturePointArtManager : MonoBehaviour
{
    public Material StartMat;
    public Material EndMat;
    public Renderer TargetRenderer;

    private void Start()
    {
        Material[] Mats = TargetRenderer.materials;
        Mats[1] = StartMat;
        TargetRenderer.materials = Mats;
    }

    public void ChangeMatStatus()
    {
        Material[] Mats = TargetRenderer.materials;
        Mats[1] = EndMat;
        TargetRenderer.materials = Mats;
        Destroy(this);
    }
}
