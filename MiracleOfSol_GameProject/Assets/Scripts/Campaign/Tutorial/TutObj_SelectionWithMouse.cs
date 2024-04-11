using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutObj_SelectionWithMouse : MonoBehaviour
{
    public GameObject[] ChangeStateOnComplete;

    private TutorialObjectiveManager TOM;
    private bool IsActive = false;

    // Start is called before the first frame update
    void Start()
    {
        TOM = transform.parent.gameObject.GetComponent<TutorialObjectiveManager>();
        TOM.SM.AllowKeyboardSelection = false;
        TOM.SM.ResetSelectionStatus();
        IsActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (TOM.SM.SelectableObjects[0].GetSelectedStatus() == "Selected" && IsActive)
        {
            TOM.SM.AllowKeyboardSelection = true;

            foreach(GameObject tmp in ChangeStateOnComplete)
            {
                tmp.SetActive(true);
            }

            Destroy(gameObject);
        }
    }
}
