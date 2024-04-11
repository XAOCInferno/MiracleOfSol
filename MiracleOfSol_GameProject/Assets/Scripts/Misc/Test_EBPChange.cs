using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_EBPChange : MonoBehaviour
{
    public EBP_Info ebptochange;
    public float HP_PerTick = 1;

    private GameInfo GI;
    private EBP_BackupData GI_EBP;

    // Start is called before the first frame update
    void Start()
    {
        GI = GameObject.FindWithTag("GameController").GetComponent<GameInfo>().GetComponent<GameInfo>();
        GI_EBP = GI.GetComponent<EBP_BackupData>();
    }

    // Update is called once per frame
    void Update()
    {
        GI_EBP.ACTIVE_AllEBP[ebptochange.PositionInGMData].GetComponent<EBP_Info>().MaxHP += HP_PerTick * Time.deltaTime;
    }
}
