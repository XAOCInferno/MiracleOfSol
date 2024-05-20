using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TextFadeInFadeOut : MonoBehaviour
{
    public float TimeToMove = 4;
    public bool IsMovingBetweenStates = false;
    public Color InitialColour = new Color(1, 1, 1, 0);
    public Color AimColour = new Color(1, 1, 1, 1);

    private bool State = true;
    private TextMeshProUGUI selfGUI;
    private Color DesiredColour;
    private Color PreviousColour;
    private float CurrentTime;
    private bool LogicSetup = false;
    private int ChangeCount = 0;

    private void Start()
    {
        if (!LogicSetup) { SetupLogic(); }
    }

    private void SetupLogic()
    {
        LogicSetup = true;
        selfGUI = gameObject.GetComponent<TextMeshProUGUI>();
        selfGUI.color = InitialColour;
        DesiredColour = InitialColour;
        PreviousColour = AimColour;
        ChangeState(true, false);
    }

    private void Update()
    {
        if (IsMovingBetweenStates)
        {
            CurrentTime += Time.deltaTime;
            selfGUI.color = Color.Lerp(PreviousColour, DesiredColour, CurrentTime / TimeToMove);

            if (selfGUI.color == DesiredColour) { if (ChangeCount < 3) { ChangeState(); } else { IsMovingBetweenStates = false; Destroy(gameObject); } }
        }
    }

    public void ChangeState(bool ForceState = false, bool NewState = false)
    {
        ChangeCount++;
        if (selfGUI == null)
        {
            SetupLogic();
        }

        PreviousColour = selfGUI.color;
        IsMovingBetweenStates = true;
        CurrentTime = 0;

        if (ForceState) { State = NewState; } else { State = !State; }
        if (State) { DesiredColour = AimColour; } else { DesiredColour = InitialColour; }
    }
}
