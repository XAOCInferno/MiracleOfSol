using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class VFXLifetimeManager : MonoBehaviour
{
    public bool IsLooping = true;
    public bool IsActive = true;
    public bool IsACombo = false;
    public bool Combo_SpawnOnceOnly = false;
    public bool IsNotAnEffect = false;

    public float LifetimeMax = 5;
    public float LifetimeMin = 2;
    public float LifetimeRdmness = 1;
    public float DelayedDeathAmmount = 4; //How long after the last effect spawns and the object is destroyed, longer for slow effects

    private float LifetimeCurrent;
    private float LifeTimeCounterCooldown;

    private VisualEffect VE;
    private FXComboManager FXCM;
    private bool HasSpawned = false;

    private void Start()
    {

        gameObject.TryGetComponent(out FXCM);

        if (FXCM != null)
        {
            IsACombo = true;
        }

        gameObject.TryGetComponent(out VE);

        if (VE == null)
        {
            IsNotAnEffect = true;
        }
        else
        {
            VE.SetFloat("ConstantRate", PlayerPrefs.GetFloat("CurrentVFXRateMultiplier", 1) * VE.GetFloat("ConstantRate"));
        }

        ResetLoopCounter();
        if (IsACombo) { LifetimeCurrent = LifeTimeCounterCooldown; }
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive)
        {
            if (LifetimeCurrent >= LifeTimeCounterCooldown)
            {
                if (IsACombo)
                {
                    if (Combo_SpawnOnceOnly)
                    {
                        if (!HasSpawned)
                        {
                            FXCM.SpawnComboEffect();
                        }
                    }
                    else
                    {
                        FXCM.SpawnComboEffect();
                    }
                    HasSpawned = true;
                }

                if (IsLooping)
                {
                    ResetLoopCounter();
                }
                else
                {
                    DelayedDeath();
                }
            }
            else
            {
                LifetimeCurrent += Time.deltaTime;
            }
        }
    }

    public void ResetLoopCounter()
    {
        LifetimeCurrent = 0;
        LifeTimeCounterCooldown = Random.Range(-LifetimeRdmness, LifetimeRdmness) + Random.Range(LifetimeMin, LifetimeMax);

        if (LifetimeCurrent > LifetimeMax)
        {
            LifeTimeCounterCooldown = LifetimeMax;
        }
        else if (LifetimeCurrent < LifetimeMin)
        {
            LifeTimeCounterCooldown = LifetimeMin;
        }
    }

    private void DelayedDeath()
    {
        IsActive = false;

        if (!IsNotAnEffect)
        {
            VE.Stop();
        }

        Invoke("Die", DelayedDeathAmmount);
    }

    private void Die()
    {
        if(transform.parent != null && transform.parent.childCount == 1 && transform.parent.gameObject.tag == "Untagged")
        {
            Destroy(transform.parent.gameObject);
        }
        Destroy(gameObject);
    }
}
