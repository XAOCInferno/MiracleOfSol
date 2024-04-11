using System.Collections.Generic;

public static class BasicFunctions
{
    public static bool GetArrayItem<T>(this T[] SearchArray, out bool _DoesContain, out int _ContainPos, T DesiredItem)
    {
        for(int i = 0; i < SearchArray.Length; i++)
        {
            if (EqualityComparer<T>.Default.Equals(DesiredItem, SearchArray[i]))
            { 
                _ContainPos = i; 
                _DoesContain = true; 
                return true; 
            }
        }
        _DoesContain = false;
        _ContainPos = -1;
        return false;
    }
}
