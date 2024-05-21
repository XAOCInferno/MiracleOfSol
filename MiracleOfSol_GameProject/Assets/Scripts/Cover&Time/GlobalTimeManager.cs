using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class GlobalTimeManager : MonoBehaviour
{
    public AudioClip[] SunQuotes;
    public AudioClip[] FullMoonQuotes;
    public AudioClip[] MoonQuotes;
    public AudioClip[] BloodMoonQuotes;

    public AudioClip[] InBossSpecialQuotes_BloodMoon;
    public AudioClip[] InBossSpecialQuotes_FullMoon;
    public AudioClip[] InBossSpecialQuotes_Moon;
    public AudioClip[] InBossSpecialQuotes_Sun;

    private AudioClip[][] InBossSpecialQuotes_All;
    private AudioClip[][] AllQuotes;

    public UnityEngine.Rendering.Volume FogVolume;
    public AnimationCurve DaySkyColourOverLifeR;
    public AnimationCurve DaySkyColourOverLifeG;
    public AnimationCurve DaySkyColourOverLifeB;
    public AnimationCurve DaySkyColourOverLifeA;

    public AnimationCurve NightSkyColourOverLifeR;
    public AnimationCurve NightSkyColourOverLifeG;
    public AnimationCurve NightSkyColourOverLifeB;
    public AnimationCurve NightSkyColourOverLifeA;

    public Color32[] DayNightSkyTint = new Color32[4] { new Color32(), new Color32(), new Color32(), new Color32() };
    public float ExposureDuringDay = 1000;
    public float ExposureDuringNight = 350;
    public float ExposureChangeRate = 25;

    public GameObject MainDirectionalLight;
    public GameObject SunriseObj;
    public GameObject SunsetObj;
    public GameObject CentreOfMapObj;
    public float TickRateModifier = 1;
    public float YTickRateModifier = 4;
    public float CurrentTime = 0;
    public float RepeatMoonPenalty = 0.1f;

    private int CelestialType = 0;
    private int PreviousCelestialType = 0;
    private string[] CelestialBodyNames = new string[4] { "Sun", "Full Moon", "Partial Moon", "Blood Moon" };
    private float[] BaseChanceForMoonType = new float[3] { 0.15f, 0.65f, 0.20f };
    private float[] Mod_ChanceForMoonType = new float[3] { 0, 0, 0 };
    private int[] CelestialType_ChangeShadowTo = new int[4] { 2, 2, 3, 4 };
    private float[] ShadowScaleByCelestialTypeAndMoonType = new float[] { 0.5f, 0.75f, 1, 1.25f };

    private Light DirectionalLightAsLight;
    private Vector3 CentreOfMapPosition;
    private Vector3 SunrisePos;
    private Vector3 SunsetPos;
    private float DayTime = -125;//-133; //250?
    private float NightTime = 125;//133; //300 (5min)
    private bool IsDay = true;
    private float TimeAsPercent;
    private GameInfo GI;
    //private UnityEngine.Rendering.HighDefinition.Fog CurrentFog;


    // Start is called before the first frame update
    void Start()
    {
        InBossSpecialQuotes_All = new AudioClip[4][] { InBossSpecialQuotes_Sun, InBossSpecialQuotes_FullMoon, InBossSpecialQuotes_Moon, InBossSpecialQuotes_BloodMoon };
        AllQuotes = new AudioClip[4][] { SunQuotes, FullMoonQuotes, MoonQuotes, BloodMoonQuotes };

        gameObject.TryGetComponent(out GI);

        if (NightTime < DayTime) { NightTime *= -1; }
        if (IsDay) { CurrentTime = DayTime; } else { CurrentTime = NightTime; }
        if (MainDirectionalLight == null) { MainDirectionalLight = GameObject.FindGameObjectWithTag("MainDirectionalLight"); }
        if (CentreOfMapObj == null)
        {
            CentreOfMapObj = Instantiate(new GameObject());
            CentreOfMapObj.name = "CentreOfMapPosition";

            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(transform.position, -Vector3.up, out hit, 1000, LayerMask.GetMask("Terrain")))
            {
                CentreOfMapPosition = hit.point;
            }
            else
            {
                CentreOfMapPosition = new Vector3(Random.Range(0, 100), 0, Random.Range(0, 100));
            }
        }
        else
        {
            CentreOfMapPosition = CentreOfMapObj.transform.position;
        }

        DirectionalLightAsLight = MainDirectionalLight.GetComponent<Light>();
        SunrisePos = SunriseObj.transform.position;
        Destroy(SunriseObj);
        SunsetPos = SunsetObj.transform.position;
        Destroy(SunsetObj);

        CentreOfMapPosition[1] = 0;
        //FogVolume.profile.TryGet<UnityEngine.Rendering.HighDefinition.Fog>(out CurrentFog);
        CheckForInvalidKeyframes(DaySkyColourOverLifeR); CheckForInvalidKeyframes(NightSkyColourOverLifeR);
        CheckForInvalidKeyframes(DaySkyColourOverLifeG); CheckForInvalidKeyframes(NightSkyColourOverLifeG);
        CheckForInvalidKeyframes(DaySkyColourOverLifeB); CheckForInvalidKeyframes(NightSkyColourOverLifeB);
        CheckForInvalidKeyframes(DaySkyColourOverLifeA); CheckForInvalidKeyframes(NightSkyColourOverLifeA);
    }

    // Update is called once per frame
    void Update()
    {
        float NewYPos = MainDirectionalLight.transform.parent.position[1]; 
        Color NewColour;

        if (IsDay) 
        {
            if (DirectionalLightAsLight.intensity != ExposureDuringDay) //FIX ME!
            {
                DirectionalLightAsLight.intensity = Mathf.MoveTowards(DirectionalLightAsLight.intensity, ExposureDuringDay, ExposureChangeRate * Time.deltaTime);
            }

            if (CurrentTime >= 0)
            {
                IsDay = false;
                GetNewMoonType();
                NewYPos = 0;
                TimeAsPercent = 0;
            } 
            else
            {
                CurrentTime += Time.deltaTime * TickRateModifier;
                TimeAsPercent = CurrentTime / DayTime;
            }

            if (CurrentTime > DayTime / 2)
            {
                NewYPos -= YTickRateModifier * Time.deltaTime;
            }
            else
            {
                NewYPos += YTickRateModifier * Time.deltaTime;
            }

            if (CurrentTime == 0) { CurrentTime += 0.1f; }
        }
        else
        {
            if (DirectionalLightAsLight.intensity != ExposureDuringNight) //FIX ME!
            {
                DirectionalLightAsLight.intensity = Mathf.MoveTowards(DirectionalLightAsLight.intensity, ExposureDuringNight, ExposureChangeRate * Time.deltaTime);
            }

            if (CurrentTime >= NightTime)
            {
                IsDay = true;
                CelestialType = 0;
                CurrentTime = DayTime;
                NewYPos = 0;
                TimeAsPercent = 0;
                PlaySpeechComment();
            }
            else
            {
                CurrentTime += Time.deltaTime * TickRateModifier;
                TimeAsPercent = 1 - (CurrentTime / NightTime);
            }

            if (CurrentTime > NightTime / 2)
            {
                NewYPos -= YTickRateModifier * Time.deltaTime;
            }
            else
            {
                NewYPos += YTickRateModifier * Time.deltaTime;
            }

            if (CurrentTime == 0) { CurrentTime += 0.1f; }
        }
        if (IsDay)
        {
            NewColour = new Color(Mathf.Clamp(DaySkyColourOverLifeR.Evaluate(TimeAsPercent),0, 255), Mathf.Clamp(DaySkyColourOverLifeG.Evaluate(TimeAsPercent),0, 255), Mathf.Clamp(DaySkyColourOverLifeB.Evaluate(TimeAsPercent),0, 255), Mathf.Clamp(DaySkyColourOverLifeA.Evaluate(TimeAsPercent),0,255));
        }
        else
        {
            NewColour = new Color(Mathf.Clamp(NightSkyColourOverLifeR.Evaluate(TimeAsPercent),0, 255), Mathf.Clamp(NightSkyColourOverLifeG.Evaluate(TimeAsPercent),0, 255), Mathf.Clamp(NightSkyColourOverLifeB.Evaluate(TimeAsPercent),0, 255), Mathf.Clamp(NightSkyColourOverLifeA.Evaluate(TimeAsPercent),0, 255));
        }

        NewColour += DayNightSkyTint[CelestialType];
        MainDirectionalLight.transform.parent.position = Vector3.Lerp(SunsetPos, SunrisePos, TimeAsPercent) + new Vector3(0, NewYPos, 0);
        MainDirectionalLight.transform.parent.LookAt(CentreOfMapPosition);

        DirectionalLightAsLight.color = NewColour;
    }

    private void CheckForInvalidKeyframes(AnimationCurve AC)
    {
        for (int i = 0; i < AC.keys.Length; i++)// Keyframe KF in DaySkyColourOverLifeR.keys)
        {
            if (AC.keys[i].value > 1) { AC.keys[i].value = 1; }
            else if (AC.keys[i].value <= 0) { AC.keys[i].value = 0.01f; }
        }

    }

    public float GetTimeAsPercent()
    {
        return TimeAsPercent;
    }

    private void GetNewMoonType()
    {
        float BestRandom = 99999;
        int CurrentMoonType = 0;

        for(int i = 0; i < BaseChanceForMoonType.Length; i++)
        {
            float compare_rdm = Random.Range(0, 100);
            compare_rdm /= 100;

            if (compare_rdm <= (BaseChanceForMoonType[i] + Mod_ChanceForMoonType[i]))
            {
                if(compare_rdm < BestRandom)
                {
                    BestRandom = compare_rdm;
                    CurrentMoonType = i;
                }
            }
        }

        CelestialType = CurrentMoonType + 1;


        List<float> tmp_MoonChangeAlter = new List<float>();
        for (int j = 0; j < BaseChanceForMoonType.Length; j++)
        {
            if (j == CurrentMoonType)
            {
                tmp_MoonChangeAlter.Add(-RepeatMoonPenalty);
            }
            else
            {
                tmp_MoonChangeAlter.Add(RepeatMoonPenalty);
            }
        }

        PlaySpeechComment();
        ChangeMoonTypeChance(tmp_MoonChangeAlter, "ADD");
    }

    public void ChangeMoonTypeChance(List<float> MoonChance, string Type = "ADD")
    {
        if(Type == "ADD")
        {
            Mod_ChanceForMoonType = new float[3] {MoonChance[0] + Mod_ChanceForMoonType[0], MoonChance[1] + Mod_ChanceForMoonType[1], MoonChance[2] + Mod_ChanceForMoonType[2] };
        }
        else if(Type == "SET")
        {
            Mod_ChanceForMoonType = new float[3] { MoonChance[0], MoonChance[1], MoonChance[2] };
        }
    }

    private void PlaySpeechComment()
    {
        if (PreviousCelestialType != CelestialType)
        {
            PreviousCelestialType = CelestialType;

            if (!GI.IsInCutscene)
            {
                GlobalVoicePlayer.StopPlaying();
                if (!MusicManager.Instance.IsInBoss)
                {
                    GlobalVoicePlayer.PlayVoiceLine(AllQuotes[CelestialType][Random.Range(0, AllQuotes[CelestialType].Length)]);
                }
                else
                {
                    GlobalVoicePlayer.PlayVoiceLine(InBossSpecialQuotes_All[CelestialType][Random.Range(0, InBossSpecialQuotes_All[CelestialType].Length)]);
                }
            }
        }
    }

    public int GetShadowType()
    {
        return CelestialType_ChangeShadowTo[CelestialType];
    }

    public float GetShadowScale()
    {
        return ShadowScaleByCelestialTypeAndMoonType[CelestialType];
    }

    public int GetMoonType() { return CelestialType; }
}

/*
SunBeam:
Generic Good (HP Regen + 5)
Sun Worshipper (Damage + 20%, HP Regen +10)
Sun/Moon (Damage +10%, HPRegen +5)
Generic Evil (Accuracy -0.2)
Demon (HP Degen of 5/s, speed -50%)

Daytime Shadow:
Generic Good (Accuracy -0.2)
Generic Evil (Damage Resist + 20%)
Demon (Damage + 20%, HP Regen +5)

Nighttime Shadow:
Generic Good (Accuracy -0.2)
Sun Worshipper (Accuracy -0.2)
Moon Follower (Accuracy +0.2)
Sun/Moon (Accuracy + 0.1)
Generic Evil (Damage Resist + 20%, HP Regen + 5)
Demon (Damage + 40%, HP Regen +10)

Blood Shadow:
Generic Good (Accuracy -0.2, HP Degen of 10HP/s)
Sun Worshipper (Accuracy -0.4, HP Degen of 20HP/s)
Moon Follower (HP Degen of 5HP/s)
Sun/Moon (HP Degen of 10HP/s)
Generic Evil (Damage Resist + 20%, HP Regen + 5, Speed + 20%)
Demon (Damage + 40%, HP Regen +10, Speed + 40%)
*/
