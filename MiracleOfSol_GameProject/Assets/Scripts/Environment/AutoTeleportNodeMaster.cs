using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoTeleportNodeMaster : MonoBehaviour
{
    public string TargetType = "ALL"; 
    public int[] AcceptedArmourTypes = new int[14] { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
    public float SquadJumpCD = 3;

    public bool NodesAreActive = true;
    public bool NodesCanDecay = true;
    public float TimeUntilNodeDecay = 10;
    public float DurationOfNodeDecay = 30;
    public List<Transform> ChildNodes = new List<Transform>();

    private List<int> NodeTo = new List<int>() { 1, 0 };

    private bool ChargingDecay = false;
    private BasicInfo BI;
    private List<GameObject> TPObj = new List<GameObject>();
    private List<float> TPObjCD = new List<float>();
    private List<SquadManager> SM = new List<SquadManager>();
    private Color MMColour = new Color(128, 0, 128);

    // Start is called before the first frame update
    void Start()
    {
        BI = gameObject.GetComponent<BasicInfo>();
        SM = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameInfo>().AllPlayers_SM;
        TargetType = TargetType.ToUpper();

        foreach (Transform child in ChildNodes)
        {
            GameObject tmp_mm_obj = Instantiate(new GameObject());
            tmp_mm_obj.layer = LayerMask.NameToLayer("MinimapDot");
            tmp_mm_obj.name = "MM_Dot_TP_Node";
            tmp_mm_obj.transform.position = child.position;
            tmp_mm_obj.transform.parent = child;
            tmp_mm_obj.transform.localScale = new Vector3(10, 10, 10);
            tmp_mm_obj.transform.rotation = Quaternion.Euler(90, 0, 0);

            SpriteRenderer tmp_mm_sr = tmp_mm_obj.AddComponent<SpriteRenderer>();
            tmp_mm_sr.color = MMColour;
            tmp_mm_sr.sprite = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameInfo>().DefaultMMDot;
        }
    }

    private void Update()
    {
        if(TPObj.Count > 0) { UpdateObjTPCD(); }
    }

    private void UpdateObjTPCD()
    {
        for (int i = 0; i < TPObj.Count; i++)
        {
            if (TPObjCD[i] > 0)
            {
                TPObjCD[i] -= Time.deltaTime;
            }
            else
            {
                TPObj.RemoveAt(i);
                TPObjCD.RemoveAt(i);

                if (i != TPObj.Count)
                {
                    i--;
                }
            }
        }
    }

    public void CheckForTeleportation(GameObject Entity, int NodeNumber)
    {
        BasicInfo tmp_BI = Entity.GetComponent<BasicInfo>();
        if (tmp_BI != null)
        {
            bool EntityIsValid = true;

            for (int i = 0; i < TPObj.Count; i++)
            {
                if (Entity == TPObj[i])
                {
                    EntityIsValid = false;
                    break;
                }
            }

            if (EntityIsValid)
            {
                EntityIsValid = false;
                for (int i = 0; i < AcceptedArmourTypes.Length; i++)
                {
                    if (tmp_BI.EBPs.PiercingArmourType == AcceptedArmourTypes[i])
                    {
                        EntityIsValid = true; 
                        break;
                    }
                }
            }

            if (EntityIsValid && CheckValidPlayer(tmp_BI))
            {
                DoATeleport(tmp_BI.OwnedByPlayer, tmp_BI.GetIDs(), NodeNumber);
            }
        }
    }

    private bool CheckValidPlayer(BasicInfo hitBI)
    {
        if (TargetType == "ALL")
        {
            return true;
        }
        if ((TargetType == "ALLIED" || TargetType == "OWN") && hitBI.OwnedByPlayer == BI.OwnedByPlayer)
        {
            return true;
        }
        else if (TargetType == "ENEMY" && hitBI.OwnedByPlayer != BI.OwnedByPlayer && hitBI.OwnedByPlayer != -1)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void DoATeleport(int EntityPlayer, int[] EntityIDs, int NodeFrom)
    {
        List<GameObject> Squad = SM[EntityPlayer].Get_SquadList(EntityIDs[1]);

        foreach (GameObject Entity in Squad)
        {
            TPObj.Add(Entity);
            TPObjCD.Add(SquadJumpCD);
        }

        SM[EntityPlayer].SquadMove_TeleportToLocation(EntityIDs, ChildNodes[NodeTo[NodeFrom]].transform.position);

        if (!ChargingDecay && NodesCanDecay)
        {
            ChargingDecay = true;
            Invoke(nameof(DisableTeleportation), TimeUntilNodeDecay);
            Invoke(nameof(EnableTeleportation), DurationOfNodeDecay + TimeUntilNodeDecay);
        }
    }

    private void DisableTeleportation()
    {
        NodesAreActive = false;
    }

    private void EnableTeleportation()
    {
        NodesAreActive = true;
    }
}
