using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActorCutscene : MonoBehaviour
{
    public float InitialDelayTime = 0; //4 for cam fade;
    public List<GameObject> ActorsToSpawn = new List<GameObject>();
    public List<float> ActorsSpawnTimestamps = new List<float>();
    public List<Transform> ActorSpawnMkrs = new List<Transform>();

    private List<CutsceneActorManager> Actors = new List<CutsceneActorManager>();
    public List<int> ActorInteractionID = new List<int>();
    public List<float> ActorInteractionTimestamp = new List<float>();
    public List<Transform> ActorMoveToMarker = new List<Transform>();
    public List<Transform> ActorLookAtObj = new List<Transform>();
    public List<int> ActorLookAtAnotherActor = new List<int>();

    private float TimerCurrent = 0;
    private bool InitialCheckOver = false;

    private void Start()
    {
        Invoke(nameof(InitialSetup), 0.5f + InitialDelayTime);
    }
    public List<CutsceneActorManager> GetAllActors()
    {
        return Actors;
    }

    private void InitialSetup()
    {
        CheckForSpawnActors();
        InitialCheckOver = true;
    }

    private void CheckForSpawnActors()
    {
        for (int i = 0; i < ActorsToSpawn.Count; i++) //Spawning new actors
        {
            if (TimerCurrent >= ActorsSpawnTimestamps[i])
            {
                ActorsSpawnTimestamps.RemoveAt(i);

                GameObject tmpObj = Instantiate(ActorsToSpawn[i]);
                ActorsToSpawn.RemoveAt(i);
                tmpObj.TryGetComponent(out UnityEngine.AI.NavMeshAgent tmpAgent);
                tmpAgent.enabled = false; //Enable disable after a transform.move
                tmpObj.transform.position = ActorSpawnMkrs[i].position;
                ActorSpawnMkrs.RemoveAt(i);
                tmpAgent.enabled = true;
                tmpObj.TryGetComponent(out CutsceneActorManager tmp_CAM);
                Actors.Add(tmp_CAM);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
      //  try
      //  {
            if (InitialCheckOver)
            {
                TimerCurrent += Time.deltaTime;

                if (ActorInteractionTimestamp.Count <= 0 && ActorsSpawnTimestamps.Count <= 0)
                {
                    foreach (CutsceneActorManager child in Actors)
                    {
                        if (child != null)
                        {
                            Destroy(child.gameObject);
                        }
                    }
                    Destroy(this);
                }

                CheckForSpawnActors();

                for (int i = 0; i < ActorInteractionTimestamp.Count; i++) //Interacting with existing actors
                {
                    if (ActorInteractionTimestamp[i] <= TimerCurrent)
                    {
                        ActorInteractionTimestamp.RemoveAt(i);

                        if (ActorMoveToMarker[i] != null)
                        {
                            Actors[ActorInteractionID[i]].Actor_MoveToPos(ActorMoveToMarker[i].position);
                            ActorMoveToMarker.RemoveAt(i);
                        }
                        else
                        {
                            ActorMoveToMarker.RemoveAt(i);
                        }

                        if (ActorLookAtAnotherActor[i] >= 0) //Looking at another actor takes priority over looking at world obj
                        {
                            Actors[ActorInteractionID[i]].Set_ActorLookAtEntity(Actors[ActorLookAtAnotherActor[i]].transform);
                            ActorLookAtObj.RemoveAt(i);
                            ActorLookAtAnotherActor.RemoveAt(i);
                        }
                        else if (ActorLookAtAnotherActor[i] == -10)
                        {
                            Actors[ActorInteractionID[i]].Set_ActorLookAtEntity(null);
                            ActorLookAtObj.RemoveAt(i);
                            ActorLookAtAnotherActor.RemoveAt(i);
                        }
                        else
                        {
                            Actors[ActorInteractionID[i]].Set_ActorLookAtEntity(ActorLookAtObj[i]);
                            ActorLookAtAnotherActor.RemoveAt(i);
                            ActorLookAtObj.RemoveAt(i);
                        }

                        ActorInteractionID.RemoveAt(i);
                    }
                }
            }
        }
       // catch
       // {
       //     Debug.LogWarning("ERROR! In ActorCutscene. Cannot do update loop!");
            
       // }
    //}
}
