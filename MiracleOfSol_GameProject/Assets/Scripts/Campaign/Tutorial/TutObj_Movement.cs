using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutObj_Movement : MonoBehaviour
{
    public GameObject ChangeStateOnComplete;

    private TutorialObjectiveManager TOM;
    private bool IsActive = false;
    private EntityMovement EM;

    // Start is called before the first frame update
    void Start()
    {
        TOM = transform.parent.gameObject.GetComponent<TutorialObjectiveManager>();
        EM = TOM.GI.AllPlayers_SM[0].Get_SquadList()[0].GetComponent<EntityMovement>();
        EM.StopCommands(false, false, false);
        IsActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (EM.GetIfMoving() && IsActive)
        {
            TOM.SM.AllowMouseSelection = true;
            ChangeStateOnComplete.SetActive(true);
            Destroy(gameObject);
        }
    }
}
