using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoiceLineManager : MonoBehaviour
{
    public GameObject ASPrefab;
    public AudioClip[] MovementLines;
    public AudioClip[] AttackLines;
    public AudioClip[] CaptureLines;
    public AudioClip[] CaptureCompleteLines;
    public AudioClip[] StoppingLines;
    public AudioClip[] SelectionLines;
    public AudioClip[] DieLines;
    public AudioClip[] LevelUpLines;

    public AudioClip[] Revived_SharedLines;
    public AudioClip[] Revived_ByLeaderLines;
    public AudioClip[] Revived_ByPriestLines;
    public AudioClip[] Revived_BySpecialistLines;
    public AudioClip[] Revived_ByWizardLines;
    public AudioClip[] Revived_ByAngelLines;

    public AudioClip[] ReviveTargetSharedLines;
    public AudioClip[] ReviveTarget_LeaderLines;
    public AudioClip[] ReviveTarget_PriestLines;
    public AudioClip[] ReviveTarget_SpecialistLines;
    public AudioClip[] ReviveTarget_WizardLines;

    public float MinDelayBetweenLines = 0;
    public float MaxDelayBetweenLines = 0;
    private float CurrentDesiredTimeBetweenLines = 0;
    private float CurrentDelayBetweenLines = 0;
    private bool CanPlayLines = true;

    private AudioSource GlobalVoiceAS;
    private AudioClip[][] AllSpecialReviveTargetLines;
    private AudioClip[][] AllSpecialRevivedByLines;

    private void Start()
    {
        if (ASPrefab == null) 
        { 
            GameObject.FindGameObjectWithTag("GameController").TryGetComponent(out GameInfo tmpGI);
            tmpGI.GlobalVoicePlayer.TryGetComponent(out GlobalVoiceAS);
        }
        else 
        {
            GameObject tmpAS = Instantiate(ASPrefab);
            tmpAS.transform.position = transform.position;
            AutoFollowObject tmpAS_AF = tmpAS.AddComponent<AutoFollowObject>();

            tmpAS_AF.Obj = transform;
            tmpAS_AF.Vel = 100;
            tmpAS_AF.DestroyOnObjDestroy = true;
            tmpAS_AF.DelayedDeathTime = 8;

            tmpAS.TryGetComponent(out GlobalVoiceAS);
        }

        AllSpecialReviveTargetLines = new AudioClip[4][] { ReviveTarget_LeaderLines, ReviveTarget_PriestLines,  ReviveTarget_WizardLines, ReviveTarget_SpecialistLines };
        AllSpecialRevivedByLines = new AudioClip[5][] { Revived_ByLeaderLines, Revived_ByPriestLines, Revived_ByWizardLines, Revived_BySpecialistLines, Revived_ByAngelLines };
    }

    private void Update()
    {
        if(!CanPlayLines && !GetIfPlaying())
        {
            CurrentDelayBetweenLines += Time.deltaTime;
            if(CurrentDelayBetweenLines >= CurrentDesiredTimeBetweenLines)
            {
                CanPlayLines = true;
            }
        }
    }

    public bool GetIfPlaying()
    {
        return GlobalVoiceAS.isPlaying;
    }

    public void PlaySpecificVoiceLine(AudioClip Line, bool ForcePlayLine = false)
    {
        try
        {
            if ((!GlobalVoiceAS.isPlaying || ForcePlayLine) && Line != null && CanPlayLines)
            {
                GlobalVoiceAS.PlayOneShot(Line);
                CurrentDesiredTimeBetweenLines = Random.Range(MinDelayBetweenLines, MaxDelayBetweenLines);
                CanPlayLines = false;
            }
        }
        catch
        {
            Debug.LogError("GLOBAL VOICE PLAYER NOT FOUND!");
        }
    }

    public void PlayVoiceLineOfType(string Type)
    {
        Type = Type.ToUpper();
        if(Type == "MOVE") { PlaySpecificVoiceLine(GetLineFromArray(MovementLines)); }
        else if (Type == "ATTACK") { PlaySpecificVoiceLine(GetLineFromArray(AttackLines)); }
        else if (Type == "CAPTURE") { PlaySpecificVoiceLine(GetLineFromArray(CaptureLines)); }
        else if (Type == "CAPTURE_COMPLETE") { PlaySpecificVoiceLine(GetLineFromArray(CaptureCompleteLines)); }
        else if (Type == "STOP") { PlaySpecificVoiceLine(GetLineFromArray(StoppingLines)); }
        else if (Type == "SELECTION") { PlaySpecificVoiceLine(GetLineFromArray(SelectionLines)); }
        else if (Type == "DIE") { PlaySpecificVoiceLine(GetLineFromArray(DieLines), true); }
        else if (Type == "LEVELUP") { PlaySpecificVoiceLine(GetLineFromArray(LevelUpLines), true); }
        else { Debug.LogWarning("WARNING: Attempted to play voice line '" + Type + "'. This is an unknown Type! EntityName: " + gameObject.name); }
    }

    public void PlaySpecificReviveTargetLine(int Target)
    {
        if (AllSpecialReviveTargetLines[Target].Length > 0)
        {
            PlaySpecificVoiceLine(GetLineFromArray(AllSpecialReviveTargetLines[Target]));
        }
        else
        {
            PlaySpecificVoiceLine(GetLineFromArray(ReviveTargetSharedLines));
        }
    }

    public void PlaySpecificRevivedByLine(int Target)
    {
        if (AllSpecialRevivedByLines[Target].Length > 0)
        {
            PlaySpecificVoiceLine(GetLineFromArray(AllSpecialRevivedByLines[Target]));
        }
        else
        {
            PlaySpecificVoiceLine(GetLineFromArray(Revived_SharedLines));
        }
    }

    private AudioClip GetLineFromArray(AudioClip[] tmpArray)
    {
        if (tmpArray.Length != 0)
        {
            return tmpArray[Random.Range(0, tmpArray.Length)];
        }

        return null;
    }
}
