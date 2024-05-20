using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grass3DQuality : MonoBehaviour
{
    public Transform[] ChildGrass;
    private int PreviousValue = 0;
    private MasterSettingsController MSC;

    private float[] PercentageByQuality = new float[3] { 0.333f, 0.666f, 1 };

    private void Start()
    {
        InitLogic();
    }

    private void OnEnable()
    {
        InitLogic();
    }

    private void InitLogic()
    {
        CancelInvoke();
        if (MSC == null) { GameObject.FindGameObjectWithTag("SettingsManager").TryGetComponent(out MSC); }
        InvokeRepeating(nameof(UpdateGrassDetail), 1 + Random.Range(-0.5f, 0.5f), 1 + Random.Range(0.5f, 1f));
    }

    private void UpdateGrassDetail()
    {
        if(PreviousValue != MSC.Global3DQuality)
        {
            PreviousValue = MSC.Global3DQuality;
            int GrassToRemove = (int) (ChildGrass.Length * (1 - PercentageByQuality[MSC.Global3DQuality]));
            int CurrentGrassCount = 0;

            for(int i = 0; i < ChildGrass.Length; i++)
            {
                if(GrassToRemove > CurrentGrassCount)
                {
                    CurrentGrassCount++;
                    if (ChildGrass[i].gameObject.activeSelf)
                    {
                        ChildGrass[i].gameObject.SetActive(false);
                    }
                }
                else
                {
                    ChildGrass[i].gameObject.SetActive(true);
                }
            }
        }
    }
}
