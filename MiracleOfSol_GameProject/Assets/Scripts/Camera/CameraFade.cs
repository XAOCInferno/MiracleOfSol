using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFade : MonoBehaviour
{
    public float InitialFadeDelayTime = 4;
    public float TimeToMove = 4;
    public bool IsMovingBetweenStates = false;

    private bool State = true;
    private Image selfImage;
    private Color DesiredColour;
    private Color PreviousColour;
    private float CurrentTime;
    private bool LogicSetup = false;
    private float TimerInitialDelayTime = 0;
    private bool IsCheckingForInitialDelayTime = true;

    private void Start()
    {
        if (!LogicSetup) { SetupLogic(); }
    }

    private void SetupLogic()
    {
        LogicSetup = true;
        gameObject.TryGetComponent(out selfImage);
        selfImage.color = new Color(0, 0, 0, 1);
        DesiredColour = new Color(0, 0, 0, 255);
        PreviousColour = new Color(0, 0, 0, 255);
        ChangeState(true, false);
    }

    private void Update()
    {
        if (IsMovingBetweenStates)
        {
            CurrentTime += Time.deltaTime;
            selfImage.color = Color.Lerp(PreviousColour, DesiredColour, CurrentTime / TimeToMove);

            if (selfImage.color == DesiredColour) { IsMovingBetweenStates = false; }
        }

        if (IsCheckingForInitialDelayTime) 
        { 
            TimerInitialDelayTime += Time.deltaTime;
            if(TimerInitialDelayTime >= InitialFadeDelayTime)
            {
                IsCheckingForInitialDelayTime = false;
                ChangeState(true, false);
            }
        }
    }

    public void ChangeState(bool ForceState = false, bool NewState = false)
    {
        if (TimerInitialDelayTime < InitialFadeDelayTime)
        {
            IsCheckingForInitialDelayTime = true;
        }
        else
        {
            if (selfImage == null)
            {
                SetupLogic();
            }

            PreviousColour = selfImage.color;
            IsMovingBetweenStates = true;
            CurrentTime = 0;
            if (ForceState) { State = NewState; } else { State = !State; }
            if (State) { DesiredColour = new Color(0, 0, 0, 1); } else { DesiredColour = new Color(0, 0, 0, 0); }
        }
    }

    public void ForceChangeColours(Color tmpDesiredColour, bool tmpState)
    {
        DesiredColour = tmpDesiredColour;
        selfImage.color = tmpDesiredColour;
        State = tmpState;
        IsMovingBetweenStates = false;
    }
}
