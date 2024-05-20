using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CapturePoint : MonoBehaviour
{
    public float TimeToCapture = 15;
    public float DegradeRate = 0.5f;
    public int CurrentCappingPlayer = -1;
    public int PreviousCappingPlayer = -1;

    public float AcceptableCapDistance = 1.5f;
    public Transform CurrentCappingEntity;
    public HealthBar CapBarArt;

    public AudioClip PlayOnStartCapture;
    public AudioClip PlayOnRevertCapture;
    public AudioClip PlayOnEndCapture;
    public AudioClip LoopDuringCapture;

    public GameObject[] ChangeActiveStateOnCapture;

    private BasicInfo CurrentCappingEntityBI;
    private float CurrentCapTime;
    private bool IsCaptured = false;

    private BasicInfo BI;
    private AudioSourceController ASC;
    private bool HasActivated = false;

    // Start is called before the first frame update
    void Start()
    {
        BI = gameObject.GetComponent<BasicInfo>();
        ASC = gameObject.GetComponent<AudioSourceController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (CurrentCappingPlayer != PreviousCappingPlayer)
        {
            ResetCapState();
            PreviousCappingPlayer = CurrentCappingPlayer;
        }

        if (CurrentCappingEntity != null)
        {
            if (!IsCaptured)
            {
                if (Vector3.Distance(transform.position, CurrentCappingEntity.position) <= AcceptableCapDistance)
                {
                    CaptureRepeating(CurrentCappingEntityBI.EBPs.CaptureRate);
                }
                else
                {
                    CaptureRepeating(-DegradeRate);
                }
            }
        }
        else
        {
            CaptureRepeating(-DegradeRate);
        }
    }

    public void SetupNewCapper(Transform CappingEntity, BasicInfo CappingEntityBI)
    {
        if (Vector3.Distance(transform.position, CappingEntity.position) <= AcceptableCapDistance)
        {
            CurrentCappingEntity = CappingEntity;
            CurrentCappingEntityBI = CappingEntityBI;
            CurrentCappingPlayer = CurrentCappingEntityBI.OwnedByPlayer;

            if (CappingEntityBI.OwnedByPlayer != BI.OwnedByPlayer)
            {
                if (CurrentCapTime == 0)
                {
                    //print("PLAYING FROM HERE 1");
                    ASC.OrderNewSound(PlayOnStartCapture, new float[0], true, false, false);
                }
                else if (CurrentCapTime == TimeToCapture)
                {
                    //print("PLAYING FROM HERE 2");
                    ASC.OrderNewSound(PlayOnStartCapture, new float[0], true, false, false);
                }
                else
                {
                    //print("PLAYING FROM HERE 3");
                    ASC.OrderNewSound(LoopDuringCapture, new float[0], true, true, false);
                }
            }
        }
    }

    public bool GetIfCaptured(int DesiredCaptureRace)
    {
        if(BI.OwnedByPlayer == DesiredCaptureRace)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void CaptureRepeating(float CapRate = 1)
    {
        if (!IsCaptured)
        {
            CurrentCapTime += CapRate * Time.deltaTime;
        }
        else
        {
            CurrentCapTime -= CapRate * Time.deltaTime;
        }

        CheckForCap();
    }

    private void CheckForCap()
    {
        if (!IsCaptured)
        {
            if (CurrentCapTime >= TimeToCapture)
            {
                FinishCap(); 
            }
            else if (CurrentCapTime <= 0 && BI.OwnedByPlayer != -1)
            {
                ResetCapState();
            }
        }
        else
        {
            if (CurrentCapTime <= 0)
            {
                FinishCap(); 
            }
            else if (CurrentCapTime > TimeToCapture)
            {
                ResetCapState();
            }
        }

        CapBarArt.SetHP_Percentage(CurrentCapTime / TimeToCapture);
    }

    private void FinishCap()
    {
        if (!IsCaptured)
        {
            BI.OwnedByPlayer = CurrentCappingPlayer;
            IsCaptured = true;
            CurrentCapTime = TimeToCapture;
            ASC.OrderNewSound(PlayOnEndCapture, new float[0], true, false, false);
            ASC.StopLoopingSound();
            if (!HasActivated && ChangeActiveStateOnCapture.Length > 0) { foreach (GameObject obj in ChangeActiveStateOnCapture) { if (obj != null) { obj.SetActive(!obj.activeSelf); } } }
            HasActivated = true;

            //This is causing a null error, refactor
            gameObject.TryGetComponent(out CapturePointArtManager CPAM);
            CPAM.ChangeMatStatus();
        }
        else
        {
            BI.OwnedByPlayer = -1;
            IsCaptured = false;
            CurrentCapTime = 0;
        }
    }

    private void ResetCapState()
    {
        if (!IsCaptured) { CurrentCapTime = 0; ASC.OrderNewSound(PlayOnRevertCapture, new float[0], true, false, false); } 
        else { CurrentCapTime = TimeToCapture; ASC.OrderNewSound(PlayOnRevertCapture, new float[0], true, false, false); }        
    }
}
