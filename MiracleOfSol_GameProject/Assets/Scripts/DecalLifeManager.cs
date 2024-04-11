using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class DecalLifeManager : MonoBehaviour
{
    public bool FollowParent = false;
    public bool ForceNotChangePos = false;
    public bool SetRandomRotationOnInit = true;
    public bool IsTerrainDecal = false;
    public bool ForceLogic = false;
    public float Lifetime = 30;
    public bool DestroyOnComplete = true;
    public AnimationCurve SizeOverLife = AnimationCurve.Linear(0, 1, 1, 1);
    public AnimationCurve OpacityOverLife = AnimationCurve.Linear(0, 1, 1, 0);
    public AnimationCurve XOffsetOverLife = AnimationCurve.Linear(0, 1, 1, 1);
    public AnimationCurve YOffsetOverLife = AnimationCurve.Linear(0, 1, 1, 1);
    public float MaxXOffset = 1;
    public float MaxYOffset = 1;

    public float MaxSizeOverLife = 1;
    public float MaxOpacity = 1;


    private float StartingXOffset;
    private float StartingYOffset;

    private float CurrentLifetime = 0;
    private DecalProjector DP;
    private Vector2 StartingDimmensions;
    private bool EnableDecals = true;

    private void Start()
    {
        gameObject.TryGetComponent(out DP);
        DP.enabled = false;
        EnableDecals = PlayerPrefsX.GetBool("EnableScarring", true);

        if (!IsTerrainDecal)
        {
            if (!EnableDecals) { DP.enabled = false; if (DestroyOnComplete) { Destroy(gameObject); } else { DP.enabled = true; this.enabled = false; } }
            if (!FollowParent) { transform.parent = null; }
            if (!ForceNotChangePos) { transform.position = new Vector3(transform.position[0], transform.position[1] + 2, transform.position[2]); }
        }
        if (SetRandomRotationOnInit) { transform.rotation = Quaternion.Euler(90, Random.Range(0, 360), 0); }
        if (IsTerrainDecal && !ForceLogic) { DP.enabled = true; this.enabled = false; }
        StartingDimmensions = DP.size;
        StartingXOffset = DP.uvBias[0];
        StartingYOffset = DP.uvBias[1];
    }

    private void Update()
    {
        CurrentLifetime += Time.deltaTime;
        float LifeAsPercentage = CurrentLifetime / Lifetime;
        LifeAsPercentage = Mathf.Clamp(LifeAsPercentage, 0f, 1f);

        float SizeScale = MaxSizeOverLife * SizeOverLife.Evaluate(LifeAsPercentage);
        DP.size = new Vector3(StartingDimmensions[0] * SizeScale, StartingDimmensions[1] * SizeScale, DP.size[2]); 

        float Opacity = MaxOpacity * OpacityOverLife.Evaluate(LifeAsPercentage);
        DP.fadeFactor = Opacity;
        DP.uvBias = new Vector2(StartingXOffset + XOffsetOverLife.Evaluate(LifeAsPercentage) * MaxXOffset, StartingYOffset + YOffsetOverLife.Evaluate(LifeAsPercentage) * MaxYOffset);

        if(CurrentLifetime >= Lifetime) 
        {
            if (DestroyOnComplete)
            {
                Destroy(gameObject);
            }
            else
            {
                CurrentLifetime = 0;
            }
        }
        DP.enabled = true;
    }
}
