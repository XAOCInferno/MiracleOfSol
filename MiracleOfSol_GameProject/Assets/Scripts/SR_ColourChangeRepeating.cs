using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SR_ColourChangeRepeating : MonoBehaviour
{
    public float TimeForWholeAnimation = 5;
    public AnimationCurve RateOfChange = new AnimationCurve(new Keyframe(0,1), new Keyframe(0.5f,0), new Keyframe(1,1));
    public Color32 ColourStart;
    public Color32 ColourEnd;

    private SpriteRenderer SR;

    private float TimerCurrent;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.TryGetComponent(out SR);
    }

    // Update is called once per frame
    void Update()
    {
        TimerCurrent += Time.deltaTime;
        if (TimerCurrent > TimeForWholeAnimation) { TimerCurrent = 0f; }

        float Percent = RateOfChange.Evaluate(TimerCurrent / TimeForWholeAnimation);
        SR.color = Color.Lerp(ColourStart, ColourEnd, Percent);

    }
}
