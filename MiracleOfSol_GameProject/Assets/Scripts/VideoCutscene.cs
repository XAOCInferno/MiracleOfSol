using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VideoCutscene : MonoBehaviour
{
    public HolderCameraAnimators TargetHolder;
    public bool IsIntro = false;
    public float InitialDelayTime = 0;
    public GameObject MainCam;
    public GameObject[] ActivateOnComplete;
    public Transform CameraAnimatorParent;
    private Animator[] AllCameraAnimators;
    public bool PlayOnStart = false;
    public bool CleanGoIntoCamera = false;

    private GameInfo GI;
    private int CurrentClipToPlay = -1;
    private bool IsPlaying = false;
    private bool WaitingForCutsceneFade = true;
    private float CurrentClipTime = 0;
    private AnimatorClipInfo[] CurrentClipInfo;
    private bool DelayedIsPlaying = false;

    private bool HasDoneDelayedInit = false;

    // Start is called before the first frame update
    void Start()
    {
        AllCameraAnimators = TargetHolder.AllCamAnimators;
        GI = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameInfo>();
        //foreach (Transform child in CameraAnimatorParent) { child.gameObject.SetActive(true); }
        //AllCameraAnimators = CameraAnimatorParent.GetComponentsInChildren<Animator>();
        //foreach (Transform child in CameraAnimatorParent) { child.gameObject.SetActive(false); }
        GI.UI_Canvas.SetActive(false);
        foreach(GameObject child in ActivateOnComplete) { child.SetActive(false); }
        if (PlayOnStart) { InitCutsceneClips(); }
    }

    private void OnEnable()
    {
        StartLogic();
    }

    private void StartLogic()
    {
        AllCameraAnimators = TargetHolder.AllCamAnimators;
        GI = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameInfo>();
        GI.UI_Canvas.SetActive(false);

        //THIS ISN'T WORKING, PUT THIS ON A SCRIPT FOR THE PARENT OF CAMERA. 
        //foreach (Transform child in CameraAnimatorParent) { child.gameObject.SetActive(true); }
        //AllCameraAnimators = CameraAnimatorParent.GetComponentsInChildren<Animator>();
        //foreach (Animator child in AllCameraAnimators) { child.gameObject.SetActive(false); }


        foreach (GameObject child in ActivateOnComplete) { child.SetActive(false); }

        if (PlayOnStart) { InitCutsceneClips(); }
    }

    private void Update()
    {
        PlayingLoop();
        if (Input.GetKeyDown(KeyCode.Escape) && IsPlaying) { DoNextClip(); }
    }

    public bool GetIfCheckingForFade()
    {
        return WaitingForCutsceneFade;
    }

    private void PlayingLoop()
    {
        if (IsPlaying)
        {
            foreach (SquadManager SM in GI.AllPlayers_SM)
            {
                SM.gameObject.SetActive(false);
            }

            CurrentClipTime += Time.deltaTime;
            if (CurrentClipTime >= CurrentClipInfo[0].clip.length)
            {
                DoNextClip();
            }
        }
        else if (WaitingForCutsceneFade && !DelayedIsPlaying)
        {
            DelayedIsPlaying = true;
            if (!GI.CamFade.IsMovingBetweenStates) { Invoke(nameof(DelayedInitCutscene), InitialDelayTime); GI.CamFade.gameObject.SetActive(false); }
        }
    }

    private void DelayedInitCutscene()
    {
        if (!HasDoneDelayedInit)
        {
            GI.CamFade.gameObject.SetActive(false);
            HasDoneDelayedInit = true;
            WaitingForCutsceneFade = false;

            foreach (Animator AnimObj in AllCameraAnimators)
            {
                AnimObj.gameObject.SetActive(false);
            }

            AllCameraAnimators[0].gameObject.SetActive(true);
            MainCam.SetActive(false);
            IsPlaying = true;
            DoNextClip();
        }
    }

    public void InitCutsceneClips()
    {
        GI.EnableCutsceneLogic(IsIntro);
        Invoke(nameof(DelayedInitCutscene), 4);
        //WaitingForCutsceneFade = true;
        //FIX THIS STUPID BROKEN SHIT
    }

    private void DoNextClip()
    {
        CurrentClipTime = 0;
        if (CurrentClipToPlay >= AllCameraAnimators.Length-1)
        {
            EndCutscene();
        }
        else
        {
            CurrentClipToPlay++; 
            try
            {
                if (IsPlaying)
                {
                    foreach (Animator AnimObj in AllCameraAnimators)
                    {
                        AnimObj.gameObject.SetActive(false);
                    }
                    AllCameraAnimators[CurrentClipToPlay].gameObject.SetActive(true);
                }

                CurrentClipInfo = AllCameraAnimators[CurrentClipToPlay].GetCurrentAnimatorClipInfo(0);
            }
            catch
            {
                Debug.LogError("ERROR! In 'VideoCutscene': 'Do Next Clip' cannot play next clip!");
            }
        }
    }

    private void EndCutscene()
    {
        IsPlaying = false;
        GI.EndCutsceneLogic(CleanGoIntoCamera);
        
        foreach (SquadManager SM in GI.AllPlayers_SM)
        {
            SM.gameObject.SetActive(true);
        }

        MainCam.SetActive(true);

        foreach (Animator AnimObj in AllCameraAnimators)
        {
            AnimObj.gameObject.SetActive(false);
        }

        foreach (GameObject child in ActivateOnComplete) { child.SetActive(true); }
        IsPlaying = false;
        GI.UI_Canvas.SetActive(true);

        if (gameObject.GetComponent<SpeechCutscene>().LoadSceneOnComplete)
        {
            gameObject.GetComponent<SpeechCutscene>().LoadScene();
        }

        ActorCutscene tmpActors = gameObject.GetComponent<ActorCutscene>();
        if(tmpActors != null)
        {
            foreach(CutsceneActorManager child in tmpActors.GetAllActors())
            {
                Destroy(child.gameObject);
            }

            Destroy(tmpActors);
        }

        if (IsIntro)
        {
            GI.EnableCutsceneLogic(false);
        }

        Invoke(nameof(CleanupCutscene), 0.25f);
    }

    private void CleanupCutscene()
    {
        foreach(Animator child in AllCameraAnimators)
        {
            Destroy(child.gameObject);
        }

        Destroy(this);
    }
}
