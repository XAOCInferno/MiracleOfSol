using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_BackupData : MonoBehaviour
{
    public WeaponInformation[] AllWeapon;
    public List<GameObject> ACTIVE_AllWeapon;
    public string StorageName = "ACTIVE_Weapon_STORAGE";
    private GameObject WeaponStorage;

    // Start is called before the first frame update
    void Start()
    {
        WeaponStorage = Instantiate(new GameObject());
        WeaponStorage.name = StorageName;

        for (int i = 0; i < AllWeapon.Length; i++)
        {
            if (AllWeapon[i] != null)
            {
                AllWeapon[i].PositionInGMData = i;
                GameObject NewWeapon = Instantiate(AllWeapon[i].gameObject, WeaponStorage.transform);
                NewWeapon.name = AllWeapon[i].WeaponName + "__ACTIVE";
                ACTIVE_AllWeapon.Add(NewWeapon);
            }
        }
    }
}
