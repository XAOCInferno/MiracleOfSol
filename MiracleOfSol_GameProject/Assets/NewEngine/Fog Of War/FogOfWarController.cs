using System.Collections;
using System.Collections.Generic;
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

    private Dictionary<Transform, HideInFog> AllHideInFogEntities = new();

    private void OnEnable()
    {
        FogAsByteArray = new byte[WIDTH, HEIGHT];
        texture = new(WIDTH, HEIGHT);

        DemandFogUpdate();

        Actions.OnDrawVisionCircle += DrawCircleCutout;
        Actions.OnDemandFogRedraw += DemandFogUpdate;
        Actions.OnRegisterHideInFogEntity += RegisterHideInFogEntity;
        Actions.OnDeRegisterHideInFogEntity += DeRegisterHideInFogEntity;
    }

    private void OnDisable()
    {
        Actions.OnDrawVisionCircle -= DrawCircleCutout;
        Actions.OnDemandFogRedraw -= DemandFogUpdate;
        Actions.OnRegisterHideInFogEntity -= RegisterHideInFogEntity;
        Actions.OnDeRegisterHideInFogEntity -= DeRegisterHideInFogEntity;
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
            GetEntitiesInVision();

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
        double rad2 = System.Math.Pow(Radius, 2);

        //Calculate top and left of the circle
        int left = Centre.x - Radius;
        int top = Centre.y - Radius;

        //Iterate over each point in the range of far left/top to centre
        for (int x = left; x <= Centre.x; x++)
        {
            for (int y = top; y <= Centre.y; y++)
            {

                //Calculate left and top 1 whole circle away
                int farLeft = x - Centre.x;
                int farTop = y - Centre.y;

                //x2 + y2 <= r2 (in circle) [Pythagorus] 
                if (System.Math.Pow(farLeft, 2) + System.Math.Pow(farTop, 2) <= rad2)
                {
                    int xFlipped = Centre.x - farLeft;
                    int yFlipped = Centre.y - farTop;

                    //Convert points into positions in the byte array
                    Vector2Int CoordXY = PointToBytePosition(new(x, y));
                    Vector2Int CoordXYFlipped = PointToBytePosition(new(xFlipped, yFlipped));
                    Vector2Int CoordXFlippedYFlipped = PointToBytePosition(new(x, yFlipped));
                    Vector2Int CoordXFlippedY = PointToBytePosition(new(xFlipped, y));

                    //Set the positions in array as false (visible area)
                    FogAsByteArray[CoordXY.x, CoordXY.y] = 0;
                    FogAsByteArray[CoordXYFlipped.x, CoordXYFlipped.y] = 0;
                    FogAsByteArray[CoordXFlippedYFlipped.x, CoordXFlippedYFlipped.y] = 0;
                    FogAsByteArray[CoordXFlippedY.x, CoordXFlippedY.y] = 0;
                    
                }
            }
        }
    }

    private void DrawSquareCutout(CircularVisionEmitter VisionInfo)
    {

        Vector2Int Centre = VisionInfo.Centre;
        int Radius = VisionInfo.Radius;
        double rad2 = System.Math.Pow(Radius, 2);

        for (int x = Centre.x - Radius; x <= Centre.x; x++)
        {
            for (int y = Centre.y - Radius; y <= Centre.y; y++)
            {
                if (x - Centre.x + y - Centre.y <= Radius)
                {
                    int xSym = Centre.x - (x - Centre.x);
                    int ySym = Centre.y - (y - Centre.y);

                    Vector2Int CoordXY = PointToBytePosition(new(x, y));
                    Vector2Int CoordXYSym = PointToBytePosition(new(xSym, ySym));
                    Vector2Int CoordXSymYSym = PointToBytePosition(new(x, ySym));
                    Vector2Int CoordXSymY = PointToBytePosition(new(xSym, y));

                    FogAsByteArray[CoordXY.x, CoordXY.y] = 0;
                    FogAsByteArray[CoordXYSym.x, CoordXYSym.y] = 0;
                    FogAsByteArray[CoordXSymYSym.x, CoordXSymYSym.y] = 0;
                    FogAsByteArray[CoordXSymY.x, CoordXSymY.y] = 0;

                }
            }
        }
    }

    private Vector2Int PointToBytePosition(Vector2Int point)
    {
        Vector2Int bytePosition = new(0, 0);

        if (point.x >= 0 && point.x < WIDTH)
        {
            if (point.y >= 0 && point.y < HEIGHT)
            {
                bytePosition.x = point.x;
                bytePosition.y = point.y;
            }
        }

        return bytePosition;

    }

    private Vector2Int WorldPositionToBytePosition(Vector3 worldPosition)
    {
        return PointToBytePosition(new((int)worldPosition.x, (int)worldPosition.z));
    }

    private void GetEntitiesInVision()
    {
        foreach(KeyValuePair<Transform, HideInFog> keyValuePair in AllHideInFogEntities)
        {
            keyValuePair.Value.SetArtView(GetIfInVision(keyValuePair.Key.transform.position));
        }
    }

    private bool GetIfInVision(Vector3 Position)
    {

        Vector2Int bytePosition = WorldPositionToBytePosition(Position);

        return FogAsByteArray[bytePosition.x, bytePosition.y] == 0;

    }

    private void RegisterHideInFogEntity(HideInFogEntity EntityStruct)
    {
        try
        {
            AllHideInFogEntities.Add(EntityStruct.EntityTransform, EntityStruct.HideController);
        }
        catch
        {
            Dbg.Log("Cannot add Hide In Fog entity, likely null.", eLogType.Warning, null);
        }
    }

    private void DeRegisterHideInFogEntity(HideInFogEntity EntityStruct)
    {
        try
        {
            AllHideInFogEntities.Remove(EntityStruct.EntityTransform);
        }
        catch
        {
            Dbg.Log("Cannot remove Hide In Fog entity, likely null.", eLogType.Warning, null);
        }
    }


}
