using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraFade : MonoBehaviour
{
    private static CameraFade _instance;
    public static CameraFade Instance { get { return _instance; } }

    [SerializeField] private float OverrideCameraInitialFadeTime = -1;
    [SerializeField] private float OverrideCameraTimeToMove = -1;

    private float InitialFadeDelayTime = 4;
    private float TimeToMove = 4;
    public bool IsMovingBetweenStates = false;

    private bool State = true;
    private Image selfImage;
    private  Color DesiredColour;
    private Color PreviousColour;
    private float CurrentTime;
    private float TimerInitialDelayTime = 0;
    private bool IsCheckingForInitialDelayTime = true;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }

    private void OnEnable()
    {
        SetupLogic();
    }

    private void SetupLogic()
    {
        gameObject.TryGetComponent(out selfImage);
        selfImage.color = new Color(0, 0, 0, 1);
        DesiredColour = new Color(0, 0, 0, 255);
        PreviousColour = new Color(0, 0, 0, 255);


        float? tmpInitialFadeTime = null;
        float? tmpTimeToMove = null;

        if (OverrideCameraInitialFadeTime != -1)
        {
            tmpInitialFadeTime = OverrideCameraInitialFadeTime;
        }

        if (OverrideCameraTimeToMove != -1)
        {
            tmpTimeToMove = OverrideCameraTimeToMove;
        }

        ChangeState(true, false, tmpInitialFadeTime, tmpTimeToMove);

    }

    private void Update()
    {
        if (_instance.IsMovingBetweenStates)
        {
            _instance.CurrentTime += Time.deltaTime;
            selfImage.color = Color.Lerp(PreviousColour, DesiredColour, _instance.CurrentTime / _instance.TimeToMove);

            if (selfImage.color == DesiredColour) { _instance.IsMovingBetweenStates = false; }
        }

        if (_instance.IsCheckingForInitialDelayTime) 
        {
            _instance.TimerInitialDelayTime += Time.deltaTime;
            if(_instance.TimerInitialDelayTime >= _instance.InitialFadeDelayTime)
            {
                _instance.IsCheckingForInitialDelayTime = false;
                ChangeState(true, false);
            }
        }
    }

    public static void ChangeState(bool ForceState = false, bool NewState = false, float? _InitialFadeDelayTime = null, float? _TimeToMove = null)
    {
        if (_InitialFadeDelayTime.HasValue)
        {
            _instance.InitialFadeDelayTime = _InitialFadeDelayTime.Value;
        }

        if (_TimeToMove.HasValue)
        {
            _instance.TimeToMove = _TimeToMove.Value;
        }

        if (_instance.TimerInitialDelayTime < _instance.InitialFadeDelayTime)
        {
            _instance.IsCheckingForInitialDelayTime = true;
        }
        else
        {

            _instance.PreviousColour = _instance.selfImage.color;
            _instance.IsMovingBetweenStates = true;
            _instance.CurrentTime = 0;

            if (ForceState) 
            {
                _instance.State = NewState; 
            } 
            else 
            {
                _instance.State = !_instance.State; 
            }

            if (_instance.State) 
            {
                _instance.DesiredColour = new Color(0, 0, 0, 1); 
            } 
            else 
            {
                _instance.DesiredColour = new Color(0, 0, 0, 0); 
            }
        }
    }

    public static void ForceChangeColours(Color tmpDesiredColour, bool tmpState)
    {
        _instance.DesiredColour = tmpDesiredColour;
        _instance.selfImage.color = tmpDesiredColour;
        _instance.State = tmpState;
        _instance.IsMovingBetweenStates = false;
    }
}
