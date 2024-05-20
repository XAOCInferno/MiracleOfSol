using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechCutscene : MonoBehaviour
{
    public bool DEV_SKIPME = false;
    public bool LoadSceneOnComplete = false;
    public bool SceneIsMainMenu = false;
    public bool DoSindyrEndCutsceneOnComplete = false;

    public float DelayBeforeLoadScene = 5;
    public float InitialDelayTime = 0; //Set to 4 for Cam fade
    public AudioClip[] AllVoice;
    public float[] SpeechDelays;
    public Sprite[] SpeechIcon;
    public string[] ActorName;
    public string[] SpeechScript;
    public bool PlayOnStart = false;
    public AudioSource AS;
    public GameObject[] ChangeStateOnActivate;
    public GameObject[] ChangeStateOnComplete;
    public string CreateObjectiveOnStart = "";
    public string CreateObjectiveOnComplete = "";

    private bool IsPlaying = false;
    private bool IsCheckingForCamFade = false;
    private int CurrentClipToPlay = -1;
    private bool IsDoingDelay = false;
    private VideoCutscene VC;
    private SpeechBubbleManager SBM;
    private TutorialBubbleManager TBM;
    private bool IsActive = false;
    private GameInfo GI;

    private void Start()
    {
        GI = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameInfo>();
        VC = gameObject.GetComponent<VideoCutscene>();
        if (DEV_SKIPME){ AllVoice = new AudioClip[0]; SpeechDelays = new float[0]; }
        SBM = GI.SpeechCutsceneBubble.GetComponent<SpeechBubbleManager>();
        TBM = GI.SpeechCutsceneBubble.GetComponent<TutorialBubbleManager>();

        if (AS == null)
        {
            AS = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameInfo>().GlobalVoicePlayer.GetComponent<AudioSource>();
        }

        if (PlayOnStart) { Invoke(nameof(InitVoiceCutscene),InitialDelayTime); };

        IsActive = true;
    }

    private void OnEnable()
    {
        InitVoiceCutscene();
    }

    private void Update()
    {
        if (IsActive)
        {
            if (!DEV_SKIPME)
            {
                GI.IsInCutscene = true;
            }
            if (!IsCheckingForCamFade)
            {
                CheckForNewClip();
                if (Input.GetKeyDown(KeyCode.Escape)) { OrderNextSpeech(); }
            }
            else
            {
                IsCheckingForCamFade = VC.GetIfCheckingForFade();
            }
        }
    }

    private void CheckStartObj()
    {
        if (TBM != null)
        {
            if (CreateObjectiveOnStart != "")
            {
                TBM.UpdateSpeechBubbleArt(CreateObjectiveOnStart);
            }
            else
            {
                TBM.ClearSpeechBubble();
            }
        }
    }

    public void InitVoiceCutscene()
    {
        if (VC != null)
        {
            IsCheckingForCamFade = true;
            foreach (GameObject tmpObj in ChangeStateOnActivate) { tmpObj.SetActive(!tmpObj.activeSelf); }
            ChangeStateOnActivate = new GameObject[0];
            CheckStartObj();
        }
        else
        {
            DelayedInitVoiceCutscene();
        }
    }

    private void DelayedInitVoiceCutscene()
    {
        IsPlaying = true;
        foreach (GameObject tmpObj in ChangeStateOnActivate) { tmpObj.SetActive(!tmpObj.activeSelf); }
        ChangeStateOnActivate = new GameObject[0];
        CheckStartObj();
    }

    private void OrderNextSpeech()
    {
        AS.Stop();
        CurrentClipToPlay++;
        IsDoingDelay = true;

        if (CurrentClipToPlay < SpeechDelays.Length)
        {
            Invoke(nameof(EndVoiceDelay), SpeechDelays[CurrentClipToPlay]);
        }
        else
        {
            SBM.ClearSpeechBubble();
            GI.IsInCutscene = false;
            if (CreateObjectiveOnComplete != "")
            {
                TBM.UpdateSpeechBubbleArt(CreateObjectiveOnComplete);
            }
            else
            {
                TBM.ClearSpeechBubble();
            }
            foreach (GameObject tmpObj in ChangeStateOnComplete) { if (tmpObj != null) { tmpObj.SetActive(!tmpObj.activeSelf); } }
            IsPlaying = false;

            if (DoSindyrEndCutsceneOnComplete)
            {
                GameObject.FindWithTag("GameController").TryGetComponent(out GameInfo GI);
                GI.SpecialSindyrSpeechIsDone = true;
            }

            if(LoadSceneOnComplete)
            {
                Invoke(nameof(LoadScene), DelayBeforeLoadScene);
            }
        }
    }

    public void LoadScene()
    {
        GameObject.FindWithTag("GameController").TryGetComponent(out GameInfo GI);
        GI.TryGetComponent(out LootManager LM);

        PlayerPrefs.SetInt("WeaponPartsCount", LM.GetWeaponParts());
        GI.AllPlayers_SM[0].SetXPToPlayerPrefs();

        if (SceneIsMainMenu)
        {
            PlayerPrefs.SetInt("CurrentLevel", 0);
            PlayerPrefs.SetString("NextLevelToRun", "MainMenu");
        }
        else
        {
            int NextLevel = PlayerPrefs.GetInt("CurrentLevel", -1) + 1;
            PlayerPrefs.SetInt("CurrentLevel", NextLevel);

            if (NextLevel == 0)
            {
                PlayerPrefs.SetString("NextLevelToRun", "TutorialMission");
            }
            else if (NextLevel == 1)
            {
                PlayerPrefs.SetString("NextLevelToRun", "Mission00");
            }
            else if (NextLevel == 2)
            {
                PlayerPrefs.SetString("NextLevelToRun", "Mission01");
            }
            else if (NextLevel == 3)
            {
                PlayerPrefs.SetString("NextLevelToRun", "Mission02");
            }
            else if (NextLevel == 4)
            {
                PlayerPrefs.SetString("NextLevelToRun", "MainMenu");
            }
            else
            {
                PlayerPrefs.SetString("NextLevelToRun", "Mission00");
            }

        }



        UnityEngine.SceneManagement.SceneManager.LoadScene("LevelUpScene");
    }

    private void EndVoiceDelay()
    {
        AS.Stop();
        SBM.UpdateSpeechBubbleArt(SpeechIcon[CurrentClipToPlay], ActorName[CurrentClipToPlay], SpeechScript[CurrentClipToPlay]);
        AS.PlayOneShot(AllVoice[CurrentClipToPlay]);
        IsDoingDelay = false;
    }

    private void CheckForNewClip()
    {
        if (IsPlaying)
        {
            if (!AS.isPlaying)
            {
                if (!IsDoingDelay)
                {
                    OrderNextSpeech();
                }
            }
        }
        else
        {
            GI.IsInCutscene = false;
        }
    }
}
