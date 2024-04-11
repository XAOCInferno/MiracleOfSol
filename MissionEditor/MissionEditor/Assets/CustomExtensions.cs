using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ArrayExtensions
{
    public static bool Contains<T>(this T[] Array, T Comparison)
    {
        foreach (T Item in Array) { if (Equals(Item, Comparison)) { return true; } }
        return false;
    }

    public static int ContainsIndex<T>(this T[] Array, T Comparison)
    {
        for (int i = 0; i < Array.Length; i++) { if (Equals(Array[i], Comparison)) { return i; } }
        return -1; //The list is empty!
    }

    public static List<T> ToList<T>(this T[] Array)
    {
        List<T> Tmp = new List<T>(); Tmp.AddRange(Array);
        return Tmp;
    }

    public static T[] ClearNulls<T>(this T[] Array)
    {
        List<T> tmpList = new List<T>();
        tmpList.AddRange(Array);
        tmpList.ClearNulls();
        Array = tmpList.ToArray();
        return Array;
    }
}

public static class ListExtensions
{
    public static bool Contains<T>(this List<T> List, T Comparison)
    {
        foreach (T Item in List) { if (Equals(Item, Comparison)) { return true; } }
        return false;
    }

    public static int ContainsIndex<T>(this List<T> List, T Comparison)
    {
        for (int i = 0; i < List.Count; i++) { if (Equals(List[i], Comparison)) { return i; } }
        return -1; //The list is empty!
    }

    public static List<T> ClearNulls<T>(this List<T> List)
    {
        for(int i = 0; i < List.Count; i++)
        {
            if(List[i] == null) { List.RemoveAt(i); }
        }
        return List;
    }
}
