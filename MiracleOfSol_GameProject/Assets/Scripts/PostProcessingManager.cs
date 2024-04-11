using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

public class PostProcessingManager : MonoBehaviour
{
    private VolumeProfile GV_Profile;

    private DepthOfField DoF;
    private HDShadowSettings Shadows;
    private AmbientOcclusion AO;
    private Bloom gBloom;
    public NoInterpMinFloatParameter[] ShadowDistances;
    public MinFloatParameter[] BloomThreshold;
    public ClampedFloatParameter[] BloomIntensity;
    public ClampedFloatParameter[] BloomScatter;

    // Start is called before the first frame update
    void Start()
    {
        GameObject tmpGO = GameObject.FindGameObjectWithTag("GlobalVolume");
        tmpGO.TryGetComponent(out Volume GlobalVolume);
        GV_Profile = GlobalVolume.profile;


        GV_Profile.TryGet(out DoF);
        GV_Profile.TryGet(out Shadows);
        GV_Profile.TryGet(out AO);
        GV_Profile.TryGet(out gBloom);
    }

    public void SetDoF()
    {
        if(PlayerPrefsX.GetBool("EnableDoF", true))
        {
            DoF.focusMode.value = DepthOfFieldMode.Manual;
        }
        else
        {
            DoF.focusMode.value = DepthOfFieldMode.Off;
        }
    }

    public void SetShadowQualities()
    {
        int tmpQuality = PlayerPrefs.GetInt("ShadowQuality");
        Shadows.maxShadowDistance = ShadowDistances[tmpQuality];

        if(tmpQuality == 0)
        {
            AO.active = false;
        }
        else
        {
            AO.active = true;
            AO.quality.value = tmpQuality -1;
        }
    }

    public void SetBloomQuality()
    {
        int tmpQuality = PlayerPrefs.GetInt("BloomQuality", 2);

        gBloom.threshold = BloomThreshold[tmpQuality];
        gBloom.intensity = BloomIntensity[tmpQuality];
        gBloom.scatter = BloomScatter[tmpQuality];

        if (tmpQuality == 0)
        {
            gBloom.quality.value = 0;
            gBloom.active = false;
        }
        else
        {
            gBloom.quality.value = tmpQuality -1;
            gBloom.active = true;
        }
    }
}
