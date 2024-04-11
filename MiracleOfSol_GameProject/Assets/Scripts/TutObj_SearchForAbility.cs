using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutObj_SearchForAbility : MonoBehaviour
{
    public GameObject ChangeStateOnComplete;
    public string DesiredAbilityName = "Sprint";

    private TutorialObjectiveManager TOM;
    private bool IsActive = false;
    private AbilityCaster AC;

    // Start is called before the first frame update
    void Start()
    {
        TOM = transform.parent.gameObject.GetComponent<TutorialObjectiveManager>();
        AbilityCaster[] tmpAC = TOM.GI.AllPlayers_SM[0].Get_SquadList()[0].GetComponents<AbilityCaster>();

        foreach(AbilityCaster tmp in tmpAC)
        {
            if(tmp.AbilityName == "Sprint")
            {
                AC = tmp;
                break;
            }
        }

        IsActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (AC.GetAbilityStatus() && IsActive)
        {
            ChangeStateOnComplete.SetActive(true);
            Destroy(gameObject);
        }
    }
}
