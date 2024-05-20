using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialGroupCutsceneManager : MonoBehaviour
{
    public List<GameObject> AllCutscenes;

    public void DoNextCutscene()
    {
        AllCutscenes[0].SetActive(true);
        AllCutscenes.RemoveAt(0);
        if(AllCutscenes.Count == 0) { Destroy(gameObject); }
    }
}
