using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathExplosion : MonoBehaviour
{
    public GameObject[] Mod_Target;
    public string[] Mod_TargetType; //NONE [[dummy]], ENEMY, OWN, ALL, ALLIED [[tbc]]
    public float[] Mod_GenericRadius;
    public List<float> Mod_ActiveTime_Target = new List<float>();
    public GameObject[] VFXToSpawn;
    private List<GameObject> AppliedModsHolder_Target = new List<GameObject>();
    private List<float> ModCurrent_Time = new List<float>();

    private int PositionInLvlHierarchy = -1;
    private int OwnedByPlayer = -1;
    private bool IsActive = false;

    public void DoDeathExplosion(int tmpLvlHierarchy, int tmpOwnedBy, CombatManager tmpCM)
    {
        PositionInLvlHierarchy = tmpLvlHierarchy;
        OwnedByPlayer = tmpOwnedBy;

        for (int mod = 0; mod < Mod_Target.Length; mod++)
        {
            ApplyAModifier(AppliedModsHolder_Target, Mod_Target[mod], Mod_TargetType[mod], Mod_GenericRadius[mod], transform, transform.position);
        }

        for (int fx = 0; fx < VFXToSpawn.Length; fx++)
        {
            Instantiate(VFXToSpawn[fx], transform.position, new Quaternion(), null);
        }

        IsActive = true;
    }

    private void ApplyAModifier(List<GameObject> AddToList, GameObject ApplyMod, string TargetType, float Size, Transform Target, Vector3 ModPos)
    {
        GameObject NewMod = Instantiate(ApplyMod, ModPos, new Quaternion(), Target);
        NewMod.TryGetComponent(out ModifierApplier NewMod_MA);
        NewMod_MA.TakeDamageFromTarget = PositionInLvlHierarchy;
        NewMod_MA.OwnedByPlayer = OwnedByPlayer;
        NewMod_MA.DesiredTarget = TargetType;
        NewMod.transform.localScale = new Vector3(Size, Size, Size);
        NewMod.transform.position = ModPos;
        NewMod.name = "Death Explosion Modifier: " + gameObject.name;
        Actions.OnAddNewModifier.InvokeAction(NewMod_MA);
        AddToList.Add(NewMod);
        ModCurrent_Time.Add(0);
    }

    private void Update()
    {
        if (IsActive) { CheckModDuration(); }
    }

    private void CheckModDuration()
    {
        for(int i = 0; i < AppliedModsHolder_Target.Count; i++)
        {
            if(ModCurrent_Time[i] > Mod_ActiveTime_Target[i])
            {
                RemoveModAtPosition(i);
            }
            else { ModCurrent_Time[i] += Time.deltaTime; }
        }

        CheckForNoMods();
    }

    private void RemoveModAtPosition(int pos)
    {
        Destroy(AppliedModsHolder_Target[pos].gameObject);
        AppliedModsHolder_Target.RemoveAt(pos);
        ModCurrent_Time.RemoveAt(pos);
    }

    private void CheckForNoMods()
    {
        if(AppliedModsHolder_Target.Count == 0) { Destroy(gameObject); }
    }
}
