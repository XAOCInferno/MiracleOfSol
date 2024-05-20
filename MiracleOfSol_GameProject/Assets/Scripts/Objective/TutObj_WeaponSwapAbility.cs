using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutObj_WeaponSwapAbility : MonoBehaviour
{
    public GameObject ChangeStateOnComplete;

    private TutorialObjectiveManager TOM;
    private bool IsActive = false;
    private SpecialAbility_ShiftWeaponry AC;

    private void Start()
    {
        TOM = transform.parent.gameObject.GetComponent<TutorialObjectiveManager>();
        AC = TOM.GI.AllPlayers_SM[0].Get_AllSquadLists()[0][0].GetComponent<SpecialAbility_ShiftWeaponry>();
        IsActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive && AC.IsSwapped)
        {
            ChangeStateOnComplete.SetActive(!ChangeStateOnComplete.activeSelf);
            Destroy(gameObject);
        }
    }
}
