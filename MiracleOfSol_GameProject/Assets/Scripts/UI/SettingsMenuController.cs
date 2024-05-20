using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SettingsMenuController : MonoBehaviour
{
    public UnityEngine.UI.Slider FoVSlider;

    public UnityEngine.UI.Slider Vol_MasterSlider;
    public UnityEngine.UI.Slider Vol_MusicSlider;
    public UnityEngine.UI.Slider Vol_SFXSlider;
    public UnityEngine.UI.Slider Vol_VoiceSlider;
    public UnityEngine.UI.Slider Vol_AmbienceSlider;

    public TMP_Dropdown PresetDropdown;
    public TMP_Dropdown ResolutionDropdown;
    public TMP_Dropdown FPSDropdown;
    public TMP_Dropdown AudioQualityDropdown;
    public TMP_Dropdown FullscreenDropdown;
    public TMP_Dropdown Detail3DDropdown;
    public TMP_Dropdown TerrainDropdown;
    public TMP_Dropdown VFXDropdown;
    public TMP_Dropdown ShadowsDropdown;
    public TMP_Dropdown BloomDropdown;
    public TMP_Dropdown AADropdown;
    public TMP_Dropdown RDDropdown;
    public TMP_Dropdown ColourblindDropdown;

    public UnityEngine.UI.Toggle VSyncBtn;
    public UnityEngine.UI.Toggle ScarringBtn;
    public UnityEngine.UI.Toggle DoFBtn;
    public UnityEngine.UI.Toggle StickyBtn;
    public UnityEngine.UI.Toggle Full3DCamBtn;
    public UnityEngine.UI.Toggle AutoFollowBtn;
    public UnityEngine.UI.Toggle DyslexicBtn;

    public GameObject[] Pages;

    private Vector2 MinMaxFoV = new Vector2(40, 80);
    private readonly int[] FPS_Quality = new int[5] { 30, 40, 60, 80, 120 };
    private MasterSettingsController MSC;
    private bool HasDoneStartLogic = false;

    // Start is called before the first frame update
    void Start()
    {
        InitLogic();
    }

    private void OnEnable()
    {
        InitLogic();
    }

    private void InitLogic()
    {
        if (!HasDoneStartLogic)
        {
            if (MSC == null)
            {
                GameObject tmpCC = GameObject.FindGameObjectWithTag("SettingsManager");
                tmpCC.TryGetComponent(out MSC);
            }

            SetDropdownStrings();
            SetSliderDefaults();
            SetToggleDefaults();
        }
    }

    private void SetSliderDefaults()
    {
        float tmpFoV = PlayerPrefs.GetFloat("FoVSetting", 60);
        FoVSlider.maxValue = MinMaxFoV[1];
        FoVSlider.minValue = MinMaxFoV[0];
        FoVSlider.value = tmpFoV;
        Vol_MasterSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1);
        Vol_MusicSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1);
        Vol_SFXSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1);
        Vol_VoiceSlider.value = PlayerPrefs.GetFloat("VoiceVolume", 1);
        Vol_AmbienceSlider.value = PlayerPrefs.GetFloat("AmbienceVolume", 1);
    }

    private void SetToggleDefaults()
    {
        VSyncBtn.isOn = PlayerPrefsX.GetBool("VSyncSetting", true);
        ScarringBtn.isOn = PlayerPrefsX.GetBool("EnableScarring", true);
        DoFBtn.isOn = PlayerPrefsX.GetBool("EnableDoF", true);
        StickyBtn.isOn = PlayerPrefsX.GetBool("IsSticky", false);
        Full3DCamBtn.isOn = PlayerPrefsX.GetBool("Is3DCamera", true);
        AutoFollowBtn.isOn = PlayerPrefsX.GetBool("IsAutoFollow", false);
        DyslexicBtn.isOn = PlayerPrefsX.GetBool("IsDyslexic", false);
    }

    private void SetDropdownStrings()
    {
        FPSDropdown.ClearOptions();
        ResolutionDropdown.ClearOptions();

        int CurrentResolutionIndex = 0;
        List<string> tmpResStrings = new List<string>();
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            string option = Screen.resolutions[i].width + " x " + Screen.resolutions[i].height;
            tmpResStrings.Add(option);

            if(Screen.resolutions[i].width == Screen.currentResolution.width 
            && Screen.resolutions[i].height == Screen.currentResolution.height)
            {
                CurrentResolutionIndex = i;
            }
        }

        ResolutionDropdown.AddOptions(tmpResStrings);
        ResolutionDropdown.value = CurrentResolutionIndex;


        List<string> tmpFPSStrings = new List<string>();
        for (int i = 0; i < FPS_Quality.Length; i++)
        {
            string option = FPS_Quality[i].ToString();
            tmpFPSStrings.Add(option);
        }

        FPSDropdown.AddOptions(tmpFPSStrings);        
        FPSDropdown.value = PlayerPrefs.GetInt("FPSSetting", 1);

        AudioQualityDropdown.value = PlayerPrefs.GetInt("SoundQualitySetting", 2);
        FullscreenDropdown.value = PlayerPrefs.GetInt("FullscreenMode", 2);
        Detail3DDropdown.value = PlayerPrefs.GetInt("3DQuality", 2);
        TerrainDropdown.value = PlayerPrefs.GetInt("TerrainQuality", 2);
        VFXDropdown.value = PlayerPrefs.GetInt("VFXQuality", 2);
        ShadowsDropdown.value = PlayerPrefs.GetInt("ShadowQuality", 2);
        BloomDropdown.value = PlayerPrefs.GetInt("BloomQuality", 2);
        AADropdown.value = PlayerPrefs.GetInt("AASetting", 2);
        RDDropdown.value = PlayerPrefs.GetInt("RenderDistanceQuality", 2);
        ColourblindDropdown.value = PlayerPrefs.GetInt("ColourblindMode", 0);
        HasDoneStartLogic = true;
    }

    public void Navigation_MoveToPage(int PageNumber)
    {
        for(int i = 0; i < Pages.Length; i++)
        {
            if(PageNumber == i)
            {
                Pages[i].SetActive(true);
            }
            else
            {
                Pages[i].SetActive(false);
            }
        }
    }

    public void SetRenderQuality(int RD)
    {
        RDDropdown.value = RD;
        PlayerPrefs.SetInt("RenderDistanceQuality", RD);
        MSC.SetRenderQuality();
    }

    public void SetAA(int AA_Mode)
    {
        AADropdown.value = AA_Mode;
        PlayerPrefs.SetInt("AASetting", AA_Mode);
        MSC.SetAA();
    }

    public void SetFullscreenMode(int FS_Mode)
    {
        PlayerPrefs.SetInt("FullscreenMode", FS_Mode);
        MSC.SetFullscreen();
    }

    public void SetResolution(int Resolution)
    {
        PlayerPrefs.SetInt("DesiredResolutionIndex", Resolution);
        MSC.SetResolution();
    }

    public void SetFPS(int FPSPos)
    {
        PlayerPrefs.SetInt("FPSSetting", FPSPos);
        MSC.SetDesiredFPS();
    }

    public void SetVSync(bool state)
    {
        int tmpState = 1;
        if (state)
        {
            tmpState = 0;
        }

        PlayerPrefs.SetInt("VSyncSetting", tmpState);
        MSC.SetDesiredVSync();
    }

    public void SetMasterVolume(float VolumePercent)
    {
        PlayerPrefs.SetFloat("MasterVolume", VolumePercent);
        MSC.SetMasterVolume();
    }

    public void SetSFXVolume(float VolumePercent)
    {
        PlayerPrefs.SetFloat("SFXVolume", VolumePercent);
        MSC.SetSFXVolume();
    }

    public void SetVoiceVolume(float VolumePercent)
    {
        PlayerPrefs.SetFloat("VoiceVolume", VolumePercent);
        MSC.SetVoiceVolume();
    }

    public void SetMusicVolume(float VolumePercent)
    {
        PlayerPrefs.SetFloat("MusicVolume", VolumePercent);
        MSC.SetMusicVolume();
    }

    public void SetAmbienceVolume(float VolumePercent)
    {
        PlayerPrefs.SetFloat("AmbienceVolume", VolumePercent);
        MSC.SetAmbienceVolume();
    }

    public void SetAudioQuality(int Quality)
    {
        PlayerPrefs.SetInt("SoundQualitySetting", Quality);
        MSC.SetAudioQuality();
    }

    public void SetFoV(float FoV)
    {
        PlayerPrefs.SetFloat("FoVSetting", FoV);
        MSC.SetFoV();
    }

    public void EnableSticky(bool State)
    {
        PlayerPrefsX.SetBool("IsSticky", State);
    }

    public void Enable3DCamera(bool State)
    {
        PlayerPrefsX.SetBool("Is3DCamera", State);
        MSC.Set3DCameraMode();
    }

    public void EnableAutoFollow(bool State)
    {
        PlayerPrefsX.SetBool("IsAutoFollow", State);
        MSC.SetAutoFollowMode();
    }

    public void EnableDyslexic(bool State)
    {
        if (!State) 
        {
            PlayerPrefs.SetInt("CurrentFont", 0);
            PlayerPrefsX.SetBool("IsDyslexic", false);
        }
        else
        {
            PlayerPrefs.SetInt("CurrentFont", 1);
            PlayerPrefsX.SetBool("IsDyslexic", true);
        }

        MSC.SetDyslexicFont();
    }

    public void SetColourblindMode(int Quality)
    {
        PlayerPrefs.SetInt("ColourblindMode", Quality);
        MSC.SetColourblindMode();
    }

    public void EnableScarring(bool State)
    {
        ScarringBtn.isOn = State;
        PlayerPrefsX.SetBool("EnableScarring", State);
        MSC.SetBattleScarring();
    }
    
    public void EnableDoF(bool State)
    {
        DoFBtn.isOn = State;
        PlayerPrefsX.SetBool("EnableDoF", State);
        MSC.SetDoF();
    }
   
    public void Set3DQuality(int Quality)
    {
        Detail3DDropdown.value = Quality;
        PlayerPrefs.SetInt("3DQuality", Quality);
        MSC.Set3DQuality();
    }

    public void SetTerrainQuality(int Quality)
    {
        TerrainDropdown.value = Quality;
        PlayerPrefs.SetInt("TerrainQuality", Quality);
        MSC.SetTerrainQuality();
    }

    public void SetVFXQuality(int Quality)
    {
        VFXDropdown.value = Quality;
        PlayerPrefs.SetInt("VFXQuality", Quality);
        MSC.SetVFXQuality();
    }

    public void SetShadowQuality(int Quality)
    {
        ShadowsDropdown.value = Quality;
        PlayerPrefs.SetInt("ShadowQuality", Quality);
        MSC.SetShadowQuality();
    }

    public void SetBloomQuality(int Quality)
    {
        BloomDropdown.value = Quality;
        PlayerPrefs.SetInt("BloomQuality", Quality);
        MSC.SetBloomQuality();
    }

    public void SetPresetQuality(int Quality)
    {
        if (Quality != 3)
        {
            SetTerrainQuality(Mathf.Clamp(Quality, 0, TerrainDropdown.options.Count - 1));
            SetRenderQuality(Mathf.Clamp(Quality, 0, RDDropdown.options.Count - 1));

            if (Quality <= 1) { EnableDoF(false); } else { EnableDoF(true); }
            if (Quality == 0) { EnableScarring(false); } else { EnableScarring(true); }

            if (Quality == 1)
            {
                SetShadowQuality(2);
                SetBloomQuality(2);
                SetAA(2);
                Set3DQuality(1);
                SetVFXQuality(1);
            }
            else if(Quality == 2)
            {
                SetShadowQuality(ShadowsDropdown.options.Count - 1);
                SetBloomQuality(BloomDropdown.options.Count - 1);
                SetAA(AADropdown.options.Count - 1);
                Set3DQuality(Detail3DDropdown.options.Count - 1);
                SetVFXQuality(VFXDropdown.options.Count - 1);
            }
            else
            {
                SetShadowQuality(0);
                SetBloomQuality(0);
                SetAA(0);
                Set3DQuality(0);
                SetVFXQuality(0);
            }
        }

        PresetDropdown.value = Quality;
    }

    public void SetPresetAsCustom()
    {
        PresetDropdown.value = 3;
    }
}
