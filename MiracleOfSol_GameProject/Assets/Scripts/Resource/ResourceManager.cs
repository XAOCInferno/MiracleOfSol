using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct ResourceGroup
{
    public float Primary;
    public float Secondary;
    public float Special;

    public ResourceGroup(float _primary, float _secondary, float _special)
    {
        Primary = _primary; Secondary = _secondary; Special = _special;
    }

    public bool HasEnoughResources(ResourceGroup cost)
    {
        if(Primary < cost.Primary || Secondary < cost.Secondary || Special < cost.Special) return false;
        return true;
    }

    public void ChangeResources(ResourceGroup changeBy, float multiplier)
    {
        Primary += changeBy.Primary * multiplier;
        Secondary += changeBy.Secondary * multiplier;
        Special += changeBy.Special * multiplier;
    }
}

public class ResourceManager : MonoBehaviour
{
    public int PlayerCount;
    public ResourceGroup StartingResources = new ResourceGroup(300,10,0);

    private static ResourceGroup[] PlayerResources;


    private void OnEnable()
    {
        Actions.OnUpdateResourcesForPlayer += ChangeResourceCount;
        Actions.OnInitiatePlayerResources += InitPlayerResources;
    }

    private void OnDisable()
    {
        Actions.OnUpdateResourcesForPlayer -= ChangeResourceCount;
        Actions.OnInitiatePlayerResources -= InitPlayerResources;
    }

    public void InitPlayerResources(int NumberPlayers)
    {

        PlayerCount = NumberPlayers;
        PlayerResources = new ResourceGroup[NumberPlayers];

        for(int i = 0; i < PlayerCount; i++)
        {

            PlayerResources[i] = StartingResources;
            Actions.OnUpdateResourceCanvasForPlayer.InvokeAction(PlayerResources[i]);

        }

    }

    public static bool AttemptToChargePlayer(int PlayerNumb, ResourceGroup Cost)
    {

        if(PlayerResources[PlayerNumb].HasEnoughResources(Cost) == false)
        {

            return false;

        }

        PlayerResources[PlayerNumb].ChangeResources(Cost, -1);

        return true;

    }

    public void ChangeResourceCount(int PlayerNumb, ResourceGroup Resource, bool UpdateCanvas)
    {

        PlayerResources[PlayerNumb].ChangeResources(Resource, 1);

        if (UpdateCanvas)
        {

            Actions.OnUpdateResourceCanvasForPlayer.InvokeAction(PlayerResources[PlayerNumb]);

        }

    }
}
