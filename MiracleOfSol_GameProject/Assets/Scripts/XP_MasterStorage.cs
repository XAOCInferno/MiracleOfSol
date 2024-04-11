using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class XP_MasterStorage : MonoBehaviour
{
    private int[] LevelXPRequired = new int[10] { 0, 75, 125, 200, 325, 550, 925, 1500, 2000, 3000 };

    public int GetXPRequirement(int NextLevel)
    {
        if (NextLevel < 10)
        {
            return LevelXPRequired[NextLevel];
        }

        return 0;
    }
}
