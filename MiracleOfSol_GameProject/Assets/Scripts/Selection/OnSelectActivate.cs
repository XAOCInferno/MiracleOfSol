using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnSelectActivate : MonoBehaviour
{
    public List<GameObject> ActivatedObjects = new List<GameObject>();
    public bool ActivatesProductionUI;
    public bool ActivatesJumpUI;
    public bool ActivatesAbilityUI;

    public void SetupInitialVar(bool ProductionState = false, bool JumpState = false, bool AbilityState = false)
    {
        ActivatesProductionUI = ProductionState; ActivatesJumpUI = JumpState; ActivatesAbilityUI = AbilityState;

        if (ActivatedObjects.Count == 0) //FIX ME ;-;
        {
            GameInfo GI = GameObject.FindWithTag("GameController").GetComponent<GameInfo>();

            if (ActivatesProductionUI)
            {
                //print("PRODUCTION");
                ActivatedObjects.Add(GI.ProductionUIMaster);
            }

            if (ActivatesJumpUI)
            {
                //print("JUMP");
                ActivatedObjects.Add(GI.JumpUIMaster);
            }

            if (ActivatesAbilityUI)
            {
                //print("Ability");
                ActivatedObjects.Add(GI.AbilityUIMaster);
            }
        }

    }

    public void ActivateObject(bool state = true)
    {
        if (ActivatedObjects.Count > 0)
        {
            for (int i = 0; i < ActivatedObjects.Count; i++)
            {
                ActivatedObjects[i].SetActive(state);
            }
        }
    }
}
