using System.Collections;
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;

public class FogOfWarController : MonoBehaviour
{
    // Reference this in the Inspector
    public DecalProjector FogProjector;

    private const int HEIGHT = 256;
    private const int WIDTH = 256;
    private byte[,] FogAsByteArray;
    private Texture2D texture;

    private bool MustUpdateTextureThisFrame = false;

    private void OnEnable()
    {
        FogAsByteArray = new byte[WIDTH, HEIGHT];
        texture = new(WIDTH, HEIGHT);

        DemandFogUpdate();

        Actions.OnDrawVisionCircle += DrawCircleCutout;
        Actions.OnDemandFogRedraw += DemandFogUpdate;
    }

    private void OnDisable()
    {
        Actions.OnDrawVisionCircle -= DrawCircleCutout;
        Actions.OnDemandFogRedraw -= DemandFogUpdate;
    }

    private void Start()
    {
        InvokeRepeating(nameof(Loop),0,0.15f);
    }

    private void DemandFogUpdate()
    {
        MustUpdateTextureThisFrame = true;
    }

    private void ClearFogData()
    {
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                FogAsByteArray[x, y] = 1;
            }
        }
    }

    private void Loop()
    {

        if (MustUpdateTextureThisFrame)
        {

            MustUpdateTextureThisFrame = false;

            DisplayFogData();

        }

        ClearFogData();

    }

    private void DrawRandomCircles()
    {
        CircularVisionEmitter tmpCircleEmitter = new(-1, new Vector2Int(Random.Range(80, 130), Random.Range(80, 130)), Random.Range(8, 20));
        DrawCircleCutout(tmpCircleEmitter);
    }

    private void SetFogDataRandom()
    {
        for(int x = 0; x < WIDTH; x++)
        {
            for(int y = 0; y < HEIGHT; y++)
            {
                FogAsByteArray[x, y] = (byte) Random.Range(0, 2);
            }
        }

        DisplayFogData();
    }

    private void DisplayFogData()
    {
        
        for (int x = 0; x < WIDTH; x++)
        {
            for (int y = 0; y < HEIGHT; y++)
            {
                Color color = FogAsByteArray[x, y] == 0 ? Color.clear : Color.black;

                texture.SetPixel(x, y, color);

            }
        }

        texture.Apply();
        FogProjector.material.mainTexture = texture;
        HDMaterial.ValidateMaterial(FogProjector.material);

    }

    private void DrawCircleCutout(CircularVisionEmitter VisionInfo)
    {

        Vector2Int Centre = VisionInfo.Centre;
        int Radius = VisionInfo.Radius;

        for (int x = Centre.x - Radius; x <= Centre.x; x++)
        {
            for (int y = Centre.y - Radius; y <= Centre.y; y++)
            {
                // we don't have to take the square root, it's slow
                if ((x - Centre.x) * (x - Centre.x) + (y - Centre.y) * (y - Centre.y) <= Radius * Radius)
                {
                    int xSym = Centre.x - (x - Centre.x);
                    int ySym = Centre.y - (y - Centre.y);

                    if (x >= 0 && x < WIDTH)
                    {
                        if (y >= 0 && y < HEIGHT)
                        {
                            FogAsByteArray[x, y] = 0;
                        }

                        if (ySym >= 0 && ySym < HEIGHT)
                        {
                            FogAsByteArray[x, ySym] = 0;
                        }
                    }

                    if (xSym >= 0 && xSym < WIDTH)
                    {
                        if (ySym >= 0 && ySym < HEIGHT)
                        {
                            FogAsByteArray[xSym, ySym] = 0;
                        }

                        if (y >= 0 && y < HEIGHT)
                        {
                            FogAsByteArray[xSym, y] = 0;
                        }
                    }
                }
            }
        }

    }

}
