using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GotoNextGroupCutscene : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        transform.parent.TryGetComponent(out SpecialGroupCutsceneManager tmpSGCM);
        tmpSGCM.DoNextCutscene();
        Destroy(gameObject);        
    }
}
