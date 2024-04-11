using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceGenerator : MonoBehaviour
{
    private BasicInfo BI;
    private ResourceManager RM;

    // Start is called before the first frame update
    void Start()
    {
        BI = gameObject.GetComponent<BasicInfo>();
        RM = GameObject.FindWithTag("GameController").GetComponent<GameInfo>().ResourceManager.GetComponent<ResourceManager>();
        InvokeRepeating(nameof(GenerateResource), 1, 1);
    }

    private void GenerateResource()
    {
        if (BI.OwnedByPlayer > -1)
        {
            RM.ChangeResourceCount(BI.OwnedByPlayer, BI.EBPs.ResourcesToGenerate, BI.EBPs.UpdateCanvas);
        }
    }

    private void Update()
    {
        if (Input.GetKey("p") && BI.OwnedByPlayer > -1)
        {
            RM.ChangeResourceCount(BI.OwnedByPlayer, new float[3] { 1000, 1000, 1000 }, BI.EBPs.UpdateCanvas);
        }
    }
}
