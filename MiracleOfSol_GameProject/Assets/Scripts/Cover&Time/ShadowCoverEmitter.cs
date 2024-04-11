using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShadowCoverEmitter : MonoBehaviour
{
    public Material Mat_NoShadow;
    public Material Mat_Sunbeam;
    public Material Mat_LightShadow;
    public Material Mat_DarkShadow;
    public Material Mat_BloodShadow;

    public int StartingState = 2;
    public int CurrentState;
    public bool FollowDayNight = true;
    public Vector3 NonDayNight_Scale = new Vector3(4, 4, 4);

    public float MaxShadowGrowth = 1.4f;

    private float[] MinMaxHeightToShadowScale = { 0, 2 }; //CONTINUE ON THIS LATER, BIGGER OBJECTS BIGGER SHADOWS
    private float[] MinMaxHeightScale = { 0, 10 };
    private GlobalTimeManager GTM;
    private Material[] AllShadowMats;
    private int[] PossibleImproveCoverValues;
    private bool[] CoverIsShadowType;

    private Transform EmitterParent;
    private Transform RotationJoint;

    private Renderer g_Renderer;
    private LayerMask EntityLayer;
    private List<GameObject> AllActiveObjects = new List<GameObject>();
    private List<BasicInfo> AllActiveObj_BI = new List<BasicInfo>();
    private Transform ObjectArt;

    // Start is called before the first frame update
    void Start()
    {
        RotationJoint = transform.parent;
        EmitterParent = RotationJoint.parent;
        ObjectArt = EmitterParent.parent;
        GTM = GameObject.FindGameObjectWithTag("GameController").GetComponent<GlobalTimeManager>();

        AllShadowMats = new Material[5] { Mat_NoShadow, Mat_Sunbeam, Mat_LightShadow, Mat_DarkShadow, Mat_BloodShadow };
        PossibleImproveCoverValues = new int[5] { 0, 0, 2, 1, 0 };
        CoverIsShadowType = new bool[5] { false, false, true, true, true };

        EntityLayer = LayerMask.NameToLayer("Entity"); 
        g_Renderer = transform.GetChild(0).gameObject.GetComponent<Renderer>();

        if (!FollowDayNight)
        {
            RotationJoint.transform.localPosition = new Vector3(RotationJoint.localPosition[0], RotationJoint.localPosition[1], RotationJoint.localPosition[2]);
            transform.localPosition = new Vector3(transform.localPosition[0], transform.localPosition[1], transform.localPosition[2]);
            RotationJoint.localScale = NonDayNight_Scale;
        }

        ResetToDefaultState();

        InvokeRepeating(nameof(UpdateShadows), 0 + Random.Range(0.1f, 0.5f), 0.5f + Random.Range(-0.05f, 0.05f));
    }

    private void UpdateShadows()
    {
        if (FollowDayNight)
        {
            float TimePercent = GTM.GetTimeAsPercent();
            float MaxZ = EmitterParent.transform.localScale[2] * MaxShadowGrowth;
            MaxZ *= GTM.GetShadowScale();

            RotationJoint.rotation = Quaternion.Euler(RotationJoint.eulerAngles[0], 360 * TimePercent, RotationJoint.eulerAngles[2]);
            RotationJoint.localScale = new Vector3(RotationJoint.localScale[0], 1, MaxZ * TimePercent);
            int NewShadowType = GTM.GetShadowType();

            if (NewShadowType != CurrentState)
            {
                SetNewShadowMat(NewShadowType);
            }
        }

        foreach(BasicInfo tmp_BI in AllActiveObj_BI)
        {
            tmp_BI.SetCoverStatus(CurrentState);
        }
    }

    public void SetNewShadowMat(int NewState = 0, int ImproveCoverPower = 0, bool ForceSetShadow = false)
    {
        if (ForceSetShadow) 
        {
            if (!CoverIsShadowType[CurrentState])
            {
                for(int i = 0; i < CoverIsShadowType.Length; i++)
                {
                    if(CoverIsShadowType[i] == true)
                    {
                        NewState = i;
                        break;
                    }
                }
            }
            else
            {
                NewState = CurrentState;
            }
        }

        CurrentState = NewState + Mathf.Min(new int[2] { PossibleImproveCoverValues[CurrentState], ImproveCoverPower });
        ChangeShadowMat();
    }

    public void ResetToDefaultState()
    {
        CurrentState = StartingState;
        ChangeShadowMat();
    }

    private void ChangeShadowMat()
    {
        g_Renderer.sharedMaterial = AllShadowMats[CurrentState];
    }

    
    private void OnMouseOver()
    {
        if (Input.GetMouseButtonUp(0) && FollowDayNight)
        {
            Dev_GoThroughCoverType();
        }
    }

    private void Dev_GoThroughCoverType()
    {
        int tmp_ChangeByState = 1;
        if (CurrentState + tmp_ChangeByState >= AllShadowMats.Length)
        {
            SetNewShadowMat(0);
        }
        else
        {
            SetNewShadowMat(CurrentState + tmp_ChangeByState);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == EntityLayer)
        {
            ApplyActiveObject(other.gameObject); 
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == EntityLayer)
        {
            RemoveActiveObject(other.gameObject);
        }
    }

    public void ApplyActiveObject(GameObject Target)
    {
        bool IsInList = false;

        for (int i = 0; i < AllActiveObjects.Count; i++)
        {
            if (Target == AllActiveObjects[i])
            {
                IsInList = true;
                break;
            }
        }

        if (!IsInList) 
        {
            BasicInfo tmp_BI = Target.GetComponent<BasicInfo>();

            if (tmp_BI != null)
            {
                AllActiveObj_BI.Add(tmp_BI);
                AllActiveObjects.Add(Target);
            }
        }
    }

    public void RemoveActiveObject(GameObject Target, int IfNull_ItemPos = 0)
    {
        bool IsInList = true;
        int ListPos = IfNull_ItemPos;

        if (Target != null)
        {
            IsInList = false;

            for (int i = 0; i < AllActiveObjects.Count; i++)
            {
                if (Target == AllActiveObjects[i])
                {
                    ListPos = i;
                    IsInList = true;
                    break;
                }
            }
        }

        if (IsInList)
        {
            AllActiveObj_BI[ListPos].SetCoverStatus(0);
            AllActiveObjects.RemoveAt(ListPos); 
            AllActiveObj_BI.RemoveAt(ListPos); 
        }
    }
}
