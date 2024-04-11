using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelayedActivate : MonoBehaviour
{
    public float InitialDelayTime = 0;
    public GameObject[] ObjectsToChangeState;

    // Start is called before the first frame update
    void Start()
    {
        Invoke(nameof(ChangeStateAndCleanup), InitialDelayTime);
    }

    private void ChangeStateAndCleanup()
    {
        foreach(GameObject child in ObjectsToChangeState) { child.SetActive(!child.activeSelf); }
        Destroy(gameObject);
    }
}
