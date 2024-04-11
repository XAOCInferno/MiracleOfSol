using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutObj_CheckForDistanceToDestination : MonoBehaviour
{
    public GameObject ChangeStateOnComplete;
    public float DesiredDistance = 20;
    public Transform DesiredObjPos;

    private TutorialObjectiveManager TOM;
    private bool IsActive = false;

    // Start is called before the first frame update
    void Start()
    {
        TOM = transform.parent.gameObject.GetComponent<TutorialObjectiveManager>();
        IsActive = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (IsActive)
        {
            if (Vector3.Distance(TOM.SM.SelectableObjects[0].transform.position, DesiredObjPos.position) <= DesiredDistance)
            {
                ChangeStateOnComplete.SetActive(true);
                Destroy(gameObject);
            }
        }
    }
}
