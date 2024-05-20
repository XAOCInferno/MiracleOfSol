using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicFunctions : MonoBehaviour
{
    public bool FindIfArrayContainsTrueOrFalse(bool[] Array, bool State = true)
    {
        foreach(bool Item in Array) { if (Item == State) { return true; } }
        return false;
    }

    public bool FindIfListContainsTrueOrFalse(List<bool> List, bool State = true)
    {
        foreach (bool Item in List) { if (Item == State) { return true; } }
        return false;
    }

    public int ReturnTrueFalsePositionInArray(bool[] Array, bool State = true)
    {
        for (int i = 0; i < Array.Length;i++) { if (Array[i] == State) { return i; } }
        return -1; //Error!
    }

    public int ReturnTrueFalsePositionInList(List<bool> List, bool State = true)
    {
        for (int i = 0; i < List.Count; i++) { if (List[i] == State) { return i; } }
        return -1; //Error!
    }
}
