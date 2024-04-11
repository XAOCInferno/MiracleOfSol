using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EBP_BackupData : MonoBehaviour
{
    public EBP_Info[] AllEBP;
    public List<GameObject> ACTIVE_AllEBP;
    public string StorageName = "ACTIVE_EBP_STORAGE";
    private GameObject EBPStorage;

    // Start is called before the first frame update
    void Start()
    {
        EBPStorage = Instantiate(new GameObject());
        EBPStorage.name = StorageName;

        for(int i = 0; i < AllEBP.Length; i++)
        {
            AllEBP[i].PositionInGMData = i;
            GameObject NewEBP = Instantiate(AllEBP[i].gameObject, EBPStorage.transform);
            NewEBP.name = AllEBP[i].EntityName + "__ACTIVE";
            ACTIVE_AllEBP.Add(NewEBP);
        }
    }
}
