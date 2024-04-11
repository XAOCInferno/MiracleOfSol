using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicInfo : MonoBehaviour
{
    public EBP_Info EBPs;
    public SBP_Info SBPs;
    public int OwnedByPlayer = -1;

    private int UniqueID;
    private int SquadID;

    private bool[] CoverStatus = new bool[5] { true, false, false, false, false }; //None,Sunbeam,DayShadow,NightShadow,BloodShadow

    private Rigidbody rb;
    private Vector3 StartLoc;
    private GlobalTimeManager GTM;


    private void Start()
    {
        GameObject.FindGameObjectWithTag("GameController").GetComponent<GameInfo>().TryGetComponent(out GTM);
        rb = gameObject.GetComponent<Rigidbody>();
        StartLoc = transform.position;

        if(rb != null)
        {
            InvokeRepeating(nameof(SetRBSleep), 0 + Random.Range(0,1), 8 + Random.Range(-2,2));
        }

        if (EBPs != null)
        {
            if (OwnedByPlayer >= 0 || EBPs.GeneratesResources)
            {
                GameObject tmp_mm_obj = Instantiate(new GameObject());
                tmp_mm_obj.layer = LayerMask.NameToLayer("MMDot");
                tmp_mm_obj.name = "MM_Dot: " + EBPs.EntityName;
                tmp_mm_obj.transform.position = transform.position;
                tmp_mm_obj.transform.parent = transform;
                tmp_mm_obj.transform.localScale = new Vector3(10, 10, 10);
                tmp_mm_obj.transform.rotation = Quaternion.Euler(90, 0, 0);

                SpriteRenderer tmp_mm_sr = tmp_mm_obj.AddComponent<SpriteRenderer>();
                if (OwnedByPlayer == 0) { tmp_mm_sr.color = new Color(0, 255, 0); }
                else if (OwnedByPlayer == 1) { tmp_mm_sr.color = new Color(255, 0, 0); }
                else { tmp_mm_sr.color = new Color(255, 0, 255); }
                tmp_mm_sr.sprite = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameInfo>().DefaultMMDot;
            }
        }

        InvokeRepeating(nameof(GetLuxCoverFromGlobal), 1, 1);
    }

    private void GetLuxCoverFromGlobal()
    {
        SetCoverStatus(GTM.GetMoonType());
    }

    public void SetID(int NewUniqueID, int NewSquadID)
    {
        UniqueID = NewUniqueID; SquadID = NewSquadID;
       // Debug.Log("New ID For Entity: '" + gameObject.name + "' Has Been Set! (Entity: " + UniqueID + ", Squad: " + NewSquadID + ")");
    }

    public int[] GetIDs() 
    {
        return new int[2] { UniqueID, SquadID };
    }

    public void SetCoverStatus(int Pos = 0)
    {
        for (int i = 0; i < CoverStatus.Length; i++)
        {
            if(i == Pos) { CoverStatus[i] = true; }
            else { CoverStatus[i] = false; }
        }
    }

    public bool[] GetCoverStatus()
    {
        return CoverStatus;
    }

    public Vector3 GetStartLoc()
    {
        return StartLoc;
    }

    private void SetRBSleep()
    {
        if (rb != null) { if (!rb.IsSleeping()) { rb.Sleep(); } } //Is this necessary?
    }
}
