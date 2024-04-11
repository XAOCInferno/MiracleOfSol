using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    public ResourceCanvas RC;
    public float[][] PlayerResources;
    public int PlayerCount;
    public float[] StartingResources = new float[3] { 300, 10, 0 };
    public void InitPlayerResources(int NumberPlayers)
    {
        PlayerCount = NumberPlayers;
        PlayerResources = new float[(int) NumberPlayers][];

        for(int i = 0; i < PlayerCount; i++)
        {
            PlayerResources[i] = StartingResources;
            UpdateResourceCanvas(i);
        }
    }

    public bool CheckEnoughCurrency(int PlayerNumb, float[] Cost)
    {
        for(int i = 0; i < PlayerResources[PlayerNumb].Length; i++)
        {
            if(PlayerResources[PlayerNumb][i] - Cost[i] < 0)
            {
                return false;
            }
        }

        for (int i = 0; i < PlayerResources[PlayerNumb].Length; i++)
        {
            PlayerResources[PlayerNumb][i] -= Cost[i];
            UpdateResourceCanvas(PlayerNumb);
        }

        return true;
    }

    public void ChangeResourceCount(int PlayerNumb, float[] Resource, bool UpdateCanvas)
    {

        for (int i = 0; i < PlayerResources[PlayerNumb].Length; i++)
        {
            PlayerResources[PlayerNumb][i] += Resource[i];
        }

        if (UpdateCanvas) { UpdateResourceCanvas(PlayerNumb); }
    }

    private void UpdateResourceCanvas(int PlayerNumb)
    {
        if (PlayerNumb == 1&& RC != null) { RC.ChangeResources(PlayerResources[PlayerNumb]); }//Edit later for AI too.
    }
}
