using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu_EnableDisableManager : MonoBehaviour
{
    public GameObject[] LinkedObjects;

    private List<UnityEngine.UI.Image> LinkedObjImgs = new List<UnityEngine.UI.Image>();

    private string EnableType = "DEFAULT";
    private bool IsFading = false;

    private void Start()
    {
        foreach(GameObject tmp in LinkedObjects)
        {
            LinkedObjImgs.Add(tmp.GetComponent<UnityEngine.UI.Image>());
        }
    }

    public void SetStatus(bool Value, string _OverloadType = "null", float _Overload_AnimationTimeActive = 1, float _Overload_AnimationTimeDisabled = 0, int _Overload_RepeatRate = 8, bool IsRepeating = false)
    {
        string Type;
        if(_OverloadType != null)
        {
            Type = _OverloadType.ToUpper();
        }
        else
        {
            Type = EnableType;
        }

        if (Type == "DEFAULT")
        {
            SetAllObjectStatus(Value);
        }
        else if(Type == "FLASH")
        {
            DoFlash(_Overload_AnimationTimeActive, _Overload_AnimationTimeDisabled, _Overload_RepeatRate);
        }
        else if (Type == "FADE")
        {
            FadeAllObjects(_Overload_AnimationTimeActive, new Color(55, 55, 55), new Color(255, 255, 255), IsRepeating);
        }
    }

    private void SetAllObjectStatus(bool state)
    {
        foreach (GameObject tmp in LinkedObjects)
        {
            SetObjectStatus(tmp, state);
        }
    }

    private void SetObjectStatus(GameObject tmp, bool State)
    {
        tmp.SetActive(State);
    }

    private void DoFlash(float AnimationTimeActive = 1f, float AnimationTimeDisabled = 0.5f, int RepeatRate = 8)
    {
        foreach(GameObject tmp in LinkedObjects)
        {
            FlashUpdate(tmp, AnimationTimeActive, AnimationTimeDisabled, RepeatRate);
        }
    }

    IEnumerator FlashUpdate(GameObject TargetObj, float AnimationTimeActive = 1f, float AnimationTimeDisabled = 0.5f, int RepeatRate = 8)
    {
        if(RepeatRate % 2 != 0) { RepeatRate++; } //Must be even so that the end state is the same as start state.

        for (int i = 0; i < RepeatRate; i++)
        {
            SetObjectStatus(TargetObj, !TargetObj.activeSelf);
            if (TargetObj.activeSelf) { yield return new WaitForSeconds(AnimationTimeActive); } else { yield return new WaitForSeconds(AnimationTimeDisabled); }
            
        }
    }

    IEnumerator FadeAllObjects(float AnimTime, Color MinFade, Color MaxFade, bool IsRepeating)
    {
        foreach(UnityEngine.UI.Image tmp in LinkedObjImgs)
        {
            FadeAnObject(tmp, AnimTime, MinFade, MaxFade, IsRepeating);
        }
        yield break;
    }

    IEnumerator FadeAnObject(UnityEngine.UI.Image TargetImg, float AnimTime, Color MinFade, Color MaxFade, bool IsRepeating)
    {
        Color TargetFade = MinFade;
        Color FromFade = MaxFade;
        bool InitialFadeComplete = false;
        float TimeAsPercent = 0;
        float Currentime = 0;
        float TickRate = 0.1f;

        while (IsFading || (InitialFadeComplete && !IsRepeating))
        {
            Currentime += TickRate;
            if (Currentime < AnimTime)
            {                
                TargetImg.color = Color.Lerp(FromFade, TargetFade, TimeAsPercent);
            }
            else if(IsRepeating)
            {
                TargetImg.color = TargetFade;
                Color tmpColour = TargetFade;
                TargetFade = FromFade;
                FromFade = tmpColour;
            }
            else
            {
                TargetImg.color = TargetFade;
                yield break;
            }
            yield return new WaitForSeconds(TickRate);
        }

        yield break;
    }
}
