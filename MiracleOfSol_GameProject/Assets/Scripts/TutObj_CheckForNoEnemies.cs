using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutObj_CheckForNoEnemies : MonoBehaviour
{
    public GameObject ChangeStateOnComplete;

    private TutorialObjectiveManager TOM;
    private bool IsActive = false;
    private SquadManager HumanPlayer;

    private Combat HumanCombat;
    private GameObject EnemyObject;

    private void Start()
    {
        TOM = transform.parent.gameObject.GetComponent<TutorialObjectiveManager>();
        HumanPlayer = TOM.GI.AllPlayers_SM[0];
        HumanCombat = HumanPlayer.Get_AllSquadLists()[0][0].GetComponent<Combat>();//.CombatIsEnabled = false;//  Get_AllCombatLists()[0][0].CombatIsEnabled = false;
        EnemyObject = TOM.GI.AllPlayers_SM[1].Get_AllSquadLists()[0][0].gameObject;
        HumanCombat.DisableCombat();
        HumanCombat.enabled = false;
        IsActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive)
        {
            if (EnemyObject == null)
            {
                HumanCombat.enabled = true;
                HumanCombat.CombatIsEnabled = true;
                ChangeStateOnComplete.SetActive(!ChangeStateOnComplete.activeSelf);
                Destroy(gameObject);
            }
        }
    }
}
