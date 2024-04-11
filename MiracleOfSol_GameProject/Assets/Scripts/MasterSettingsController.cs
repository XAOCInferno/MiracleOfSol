using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MasterSettingsController : MonoBehaviour
{
    public GameObject[] EnableOnSettingsComplete;
    public AudioSource MusicPlayer;
    public Camera MainCamera;

    public UnityEngine.Audio.AudioMixer MasterAM;
    public UnityEngine.Audio.AudioMixer SFXAM;
    public UnityEngine.Audio.AudioMixer VoiceAM;
    public UnityEngine.Audio.AudioMixer VoiceAMGlobal;
    public UnityEngine.Audio.AudioMixer MusicAM;
    public UnityEngine.Audio.AudioMixer AmbienceAM;

    public TMPro.TMP_FontAsset CurrentFont;
    public TMPro.TMP_FontAsset[] AllFonts;

    private ColourblindController CC;
    private PostProcessingManager PPM;

    private Vector2 MinMaxFoV = new Vector2(40, 80);

    private int[] FPS_Quality = new int[5] { 30, 40, 60, 80, 120 };

    private int[] Audio_Quality_rate = new int[3] { 22050, 42000, 42000 };
    private int[] Audio_Quality_numRealVoice = new int[3] { 16, 32, 64 };
    private int[] Audio_Quality_numVirutalVoice = new int[3] { 128, 256, 512 };
    private int[] Audio_Quality_dpsBufferSize = new int[3] { 1024, 512, 256 };
    private AudioSpeakerMode[] Audio_Quality_SpeakerMode = new AudioSpeakerMode[3] { AudioSpeakerMode.Mono, AudioSpeakerMode.Stereo, AudioSpeakerMode.Stereo };

    private FullScreenMode[] AllFullScreenModes = new FullScreenMode[3] { FullScreenMode.Windowed, FullScreenMode.ExclusiveFullScreen, FullScreenMode.ExclusiveFullScreen };
    private float[] VFX_DensityMultiplier = new float[4] { 0.25f, 0.5f, 0.75f, 1 };

    public Terrain[] AllTerrains;
    private float[] Terrain_PixelErrorDistance = new float[4] { 200, 100, 50, 10 };
    private float[] Terrain_TextureMapDistance = new float[4] { 0, 50, 100, 300 };
    private float[] MaxRenderDistances = new float[3] { 80, 150, 250 };

    public int Global3DQuality = 2;
    private UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData HD_ACD;

    private UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData.AntialiasingMode[] AAModes =
        new UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData.AntialiasingMode[4]
        {
            UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData.AntialiasingMode.None,
            UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData.AntialiasingMode.FastApproximateAntialiasing,
            UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData.AntialiasingMode.TemporalAntialiasing,
            UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData.AntialiasingMode.SubpixelMorphologicalAntiAliasing
        };

    private bool[] EnableDithering = new bool[4] { false, false, true, true };

    private void Start()
    {
        gameObject.TryGetComponent(out CC);
        gameObject.TryGetComponent(out PPM);
        if (MainCamera != null)
        {
            MainCamera.TryGetComponent(out HD_ACD);
        }
        UpdateAllSettings();
    }
    
    private void OpenScene()
    {
        foreach (GameObject GO in EnableOnSettingsComplete)
        {
            GO.SetActive(true);
        }
    }

    private void UpdateAllSettings()
    {
        Invoke(nameof(SetBasicAASettings), 1f + Random.Range(-0.5f, 1)); 
        Invoke(nameof(SetRenderQuality), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(SetAA), 1f + Random.Range(-0.5f, 1));        
        Invoke(nameof(SetMasterVolume), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(SetSFXVolume), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(SetVoiceVolume), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(SetMusicVolume), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(SetAmbienceVolume), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(SetFullscreen), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(SetColourblindMode), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(SetDesiredFPS), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(SetDesiredVSync), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(SetResolution), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(Set3DCameraMode), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(SetAutoFollowMode), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(SetTerrainQuality), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(SetDoF), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(SetShadowQuality), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(SetBloomQuality), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(SetVFXQuality), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(SetAudioQuality), 0);
        Invoke(nameof(SetDyslexicFont), 0);
        Invoke(nameof(SetBattleScarring), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(Set3DQuality), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(SetFoV), 1f + Random.Range(-0.5f, 1));
        Invoke(nameof(OpenScene), 2.25f);
    }

    private void SetBasicAASettings()
    {
        if (HD_ACD != null)
        {
            HD_ACD.TAAQuality = UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData.TAAQualityLevel.Medium;
            HD_ACD.SMAAQuality = UnityEngine.Rendering.HighDefinition.HDAdditionalCameraData.SMAAQualityLevel.High;
        }
    }

    public void SetRenderQuality()
    {
        if (MainCamera != null)
        {
            MainCamera.farClipPlane = MaxRenderDistances[PlayerPrefs.GetInt("RenderDistanceQuality", 1)];
        }
    }

    public void SetAA()
    {
        if (HD_ACD != null)
        {
            int AASetting = PlayerPrefs.GetInt("AASetting", 2);
            HD_ACD.antialiasing = AAModes[AASetting];
            HD_ACD.dithering = EnableDithering[AASetting];
        }
    }

    public void SetMasterVolume()
    {
        float VolumePercent = PlayerPrefs.GetFloat("MasterVolume", 1);
        AudioListener.volume = VolumePercent;
        VolumePercent = Mathf.Clamp(Mathf.Log10(VolumePercent) * 20, -80, 0);
        MasterAM.SetFloat("Volume", VolumePercent);
    }

    public void SetSFXVolume()
    {
        SFXAM.SetFloat("Volume", Mathf.Clamp(Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume", 1)) * 20, -80, 0));
    }

    public void SetVoiceVolume()
    {
        VoiceAM.SetFloat("Volume", Mathf.Clamp(Mathf.Log10(PlayerPrefs.GetFloat("VoiceVolume", 1)) * 20, -80, 0));
        VoiceAMGlobal.SetFloat("Volume", Mathf.Clamp(Mathf.Log10(PlayerPrefs.GetFloat("VoiceVolume", 1)) * 20, -80, 0));
    }

    public void SetMusicVolume()
    {
        MusicAM.SetFloat("Volume", Mathf.Clamp(Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", 1)) * 20, -80, 0));
    }

    public void SetAmbienceVolume()
    {
        AmbienceAM.SetFloat("Volume", Mathf.Clamp(Mathf.Log10(PlayerPrefs.GetFloat("AmbienceVolume", 1)) * 20, -80, 0));
    }

    public void SetFullscreen()
    {
        int tmpMode = PlayerPrefs.GetInt("FullscreenMode");
        Screen.fullScreenMode = AllFullScreenModes[tmpMode];
    }

    public void SetColourblindMode()
    {
        CC.UpdateColourblindMode();
    }

    public void SetDesiredFPS()
    {
        Application.targetFrameRate = FPS_Quality[PlayerPrefs.GetInt("FPSSetting", 1)];
    }

    public void SetDesiredVSync()
    {
        QualitySettings.vSyncCount = PlayerPrefs.GetInt("VSyncSetting", 0);
    }

    public void SetResolution()
    {
        Resolution tmpRes = Screen.resolutions[PlayerPrefs.GetInt("DesiredResolutionIndex", 0)];
        Screen.SetResolution(tmpRes.width, tmpRes.height, Screen.fullScreen);
    }

    public void Set3DCameraMode()
    {

    }

    public void SetAutoFollowMode()
    {

    }

    public void SetTerrainQuality()
    {
        int TerrainDetail = PlayerPrefs.GetInt("TerrainQuality", 0);
        foreach (Terrain tmpTerrain in AllTerrains)
        {
            tmpTerrain.heightmapPixelError = Terrain_PixelErrorDistance[TerrainDetail];
            tmpTerrain.basemapDistance = Terrain_TextureMapDistance[TerrainDetail];
        }
    }

    public void SetDoF()
    {
        PPM.SetDoF();
    }

    public void SetShadowQuality()
    {
        PPM.SetShadowQualities();
    }

    public void SetBloomQuality()
    {
        PPM.SetBloomQuality();
    }

    public void SetVFXQuality()
    {
        PlayerPrefs.SetFloat("CurrentVFXRateMultiplier", VFX_DensityMultiplier[PlayerPrefs.GetInt("VFXQuality", 2)]);
    }

    public void SetAudioQuality()
    {
        int CurrentSoundQuality = PlayerPrefs.GetInt("SoundQualitySetting", 2);
        AudioConfiguration tmpConfig = AudioSettings.GetConfiguration();

        tmpConfig.sampleRate = Audio_Quality_rate[CurrentSoundQuality];
        tmpConfig.numRealVoices = Audio_Quality_numRealVoice[CurrentSoundQuality]; ;
        tmpConfig.numVirtualVoices = Audio_Quality_numVirutalVoice[CurrentSoundQuality]; ;
        tmpConfig.dspBufferSize = Audio_Quality_dpsBufferSize[CurrentSoundQuality]; ;
        tmpConfig.speakerMode = Audio_Quality_SpeakerMode[CurrentSoundQuality]; ;

        AudioSettings.Reset(tmpConfig);
        MusicPlayer.Stop();
        MusicPlayer.Play();
    }

    public void SetDyslexicFont()
    {
        CurrentFont = AllFonts[PlayerPrefs.GetInt("CurrentFont", 0)];

        TMPro.TextMeshProUGUI[] textComponents = Component.FindObjectsOfType<TMPro.TextMeshProUGUI>();
        foreach (TMPro.TextMeshProUGUI component in textComponents) 
        {
            component.font = CurrentFont; 
        }
    }

    public void SetBattleScarring()
    {

    }

    public void Set3DQuality()
    {
        Global3DQuality = PlayerPrefs.GetInt("3DQuality", 2);
    }

    public void SetFoV()
    {
        if (MainCamera != null)
        {
            MainCamera.fieldOfView = Mathf.Clamp(PlayerPrefs.GetFloat("FoVSetting", 60), MinMaxFoV[0], MinMaxFoV[1]);
        }
    }
}
