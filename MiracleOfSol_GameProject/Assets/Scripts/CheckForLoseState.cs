using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckForLoseState : MonoBehaviour
{
    public GameObject GameWorld;
    public GameObject FadeToBlackParent;
    public CameraFade FadeToBlack;
    public GameObject MenuCanvas;
    private bool HasLost = false;//false
    private bool HasDoneLoseLogic = false;

    // Update is called once per frame
    void Update()
    {// work on this shit
        if (Input.GetKey(KeyCode.P))
        {
            HasLost = true;
        }
        if (HasLost && !HasDoneLoseLogic)
        {
            HasDoneLoseLogic = true;
            FadeToBlackParent.SetActive(true);

            Invoke(nameof(DestroyScene), 1f);
        }
    }

    void DestroyScene()
    {
        MenuCanvas.SetActive(true);
        Destroy(GameWorld);
    }


}
