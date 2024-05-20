using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : MonoBehaviour
{
    private BasicInfo BI;

    // Start is called before the first frame update
    void Start()
    {
        BI = gameObject.GetComponent<BasicInfo>();
        InvokeRepeating(nameof(GenerateResource), 1, 1);
    }

    private void GenerateResource()
    {
        if (BI.OwnedByPlayer > -1)
        {
            Actions.OnUpdateResourcesForPlayer.InvokeAction(BI.OwnedByPlayer, BI.EBPs.ResourcesToGenerate, BI.EBPs.UpdateCanvas);
        }
    }

#if UNITY_EDITOR
    //Debug that lets you get infinite resources
    private void Update()
    {
        if (Input.GetKey("p") && BI.OwnedByPlayer > -1)
        {
            Actions.OnUpdateResourcesForPlayer.InvokeAction(BI.OwnedByPlayer, new(1000,1000,1000), BI.EBPs.UpdateCanvas);
        }
    }
#endif

}
