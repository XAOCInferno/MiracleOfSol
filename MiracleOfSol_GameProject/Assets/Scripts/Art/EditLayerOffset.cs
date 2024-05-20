using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditLayerOffset : MonoBehaviour
{

    public Vector2[] scrollDimmensions;
    public TerrainLayer[] LayerToEdit;

    void Update()
    {
        for (int i = 0; i < LayerToEdit.Length; i++)
        {
            LayerToEdit[i].tileOffset = Time.time * scrollDimmensions[i];
        }
    }
}
