using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test_WeaponChange : MonoBehaviour
{
    public WeaponInformation weapontochange;
    public float DPS_pertick = 1;

    private GameInfo GI;
    private Weapon_BackupData GI_WeaponData;

    // Start is called before the first frame update
    void Start()
    {
        GI = GameObject.FindWithTag("GameController").GetComponent<GameInfo>().GetComponent<GameInfo>();
        GI_WeaponData = GI.GetComponent<Weapon_BackupData>();
    }

    // Update is called once per frame
    void Update()
    {
        GI_WeaponData.ACTIVE_AllWeapon[weapontochange.PositionInGMData].GetComponent<WeaponInformation>().MaxDamage += DPS_pertick * Time.deltaTime;
    }
}
