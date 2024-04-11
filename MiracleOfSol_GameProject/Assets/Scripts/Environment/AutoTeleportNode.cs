using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTeleportNode : MonoBehaviour
{
    public int NodeIndex;

    private AutoTeleportNodeMaster MasterNode;
    private GameObject FX;
    private GameObject Deactive_FX;

    private void Start()
    {
        MasterNode = transform.parent.GetComponent<AutoTeleportNodeMaster>();
        FX = transform.GetChild(0).gameObject;
        Deactive_FX = transform.GetChild(1).gameObject;

        /*if (MasterNode.transform.GetChild(0).gameObject == gameObject)
        {
            NodeIndex = 0;
        }
        else
        {
            NodeIndex = 1;
        }*/
        if (MasterNode.NodesCanDecay) { InvokeRepeating(nameof(CheckPortalStatus), 1, 1); }

        FX.SetActive(true);
        Deactive_FX.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (MasterNode.NodesAreActive)
        {
            MasterNode.CheckForTeleportation(other.gameObject, NodeIndex);
        }
    }

    private void CheckPortalStatus()
    {
        if (MasterNode.NodesAreActive)
        {
            FX.SetActive(true);
            Deactive_FX.SetActive(false);
        }
        else
        {
            FX.SetActive(false);
            Deactive_FX.SetActive(true);
        }
    }
}
