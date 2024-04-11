using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceUIManager : MonoBehaviour
{
    GameInfo GI;

    private Camera MainCam;
    // Start is called before the first frame update
    void Start()
    {
        GI = GameObject.FindWithTag("GameController").GetComponent<GameInfo>();
        MainCam = GI.MainCamera.GetComponent<Camera>();
        gameObject.GetComponent<Canvas>().worldCamera = MainCam;
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.LookRotation(transform.position - MainCam.transform.position);
    }
}
