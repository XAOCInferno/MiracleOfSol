using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetTextAsFPS : MonoBehaviour
{
    int frameCount = 0;
    float nextUpdate = 0.0f;
    float fps = 0.0f;
    float updateRate = 4.0f;  // 4 updates per sec.

    private TMPro.TextMeshProUGUI self_text;
    private float[] ColourPercentageThreshold = new float[3] { 0.333f, 0.6666f, 1 };
    private Color[] ColourValuesFromPercent = new Color[3] { Color.red, Color.yellow, Color.green };

    private void Start()
    {
        gameObject.TryGetComponent(out self_text);
        nextUpdate = Time.time;
    }

    private void Update()
    {
        frameCount++;
        if (Time.time > nextUpdate)
        {
            nextUpdate += 1.0f / updateRate;
            fps = frameCount * updateRate;
            frameCount = 0;
            self_text.text = ((int) fps).ToString();

            float FPS_AsPercentage = fps / Application.targetFrameRate;

            for (int i = 0; i < ColourPercentageThreshold.Length; i++) 
            {
                if (FPS_AsPercentage <= ColourPercentageThreshold[i])
                {
                    self_text.color = ColourValuesFromPercent[i];
                    break;
                } 
            }
        }
    }
}
