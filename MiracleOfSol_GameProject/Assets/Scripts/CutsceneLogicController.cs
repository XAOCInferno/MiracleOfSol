using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CutsceneLogicController : MonoBehaviour
{
    public List<GameObject> ObjToSpawn;
    public List<Transform> MkrToSpawnAt;
    public List<Quaternion> ObjSpawnRotation;
    public List<float> TimeToSpawnAt;

    private float TimerCurrent;


    private void Update()
    {
        if(TimeToSpawnAt.Count == 0)
        {
            Destroy(this);
        }

        TimerCurrent += Time.deltaTime;

        for (int i = 0; i < TimeToSpawnAt.Count; i++)
        {
            if (TimerCurrent >= TimeToSpawnAt[i])
            {
                SpawnANewObject(i);
            }
        }
    }

    private void SpawnANewObject(int Pos)
    {
        GameObject tmpObj = Instantiate(ObjToSpawn[Pos]);
        tmpObj.transform.position = MkrToSpawnAt[Pos].position;
        tmpObj.transform.rotation = ObjSpawnRotation[Pos];
        ObjToSpawn.RemoveAt(Pos); MkrToSpawnAt.RemoveAt(Pos); ObjSpawnRotation.RemoveAt(Pos); TimeToSpawnAt.RemoveAt(Pos);
    }
}
