using System;

public static class ActionExtensions
{

    //Call action safely
    public static bool InvokeAction(this Action self)
    {

        if (self == null)
        {
            //No subscribers
            return false;

        }

        self.Invoke();
        return true;

    }

    //Call action with 1 arguments safely
    public static bool InvokeAction<A>(this Action<A> self, A arg1)
    {

        if (self == null)
        {

            //No subscribers
            return false;

        }

        self.Invoke(arg1);
        return true;

    }

    //Call action with 2 arguments safely
    public static bool InvokeAction<A, B>(this Action<A, B> self, A arg1, B arg2)
    {

        if (self == null)
        {

            //No subscribers
            return false;

        }

        self.Invoke(arg1, arg2);
        return true;

    }

    //Call action with 3 arguments safely
    public static bool InvokeAction<A, B, C>(this Action<A, B, C> self, A arg1, B arg2, C arg3)
    {

        if (self == null)
        {

            //No subscribers
            return false;

        }

        self.Invoke(arg1, arg2, arg3);
        return true;

    }

    //Call action with 4 arguments safely
    public static bool InvokeAction<A, B, C, D>(this Action<A, B, C, D> self, A arg1, B arg2, C arg3, D arg4)
    {

        if (self == null)
        {

            //No subscribers
            return false;

        }

        self.Invoke(arg1, arg2, arg3, arg4);
        return true;

    }

}

//All static actions that can be called and subscribed to
public static class Actions
{

    //[[Selection]]
    public static Action<GetIfSelected> OnRegisterSelectableObject; //GetIfSelected: Object selection controller
    public static Action<int> OnTrySelectHeroByButton; //int: ID of hero


    //[[Resources]]
    public static Action<int> OnInitiatePlayerResources; //int: ID of player
    public static Action<int, ResourceGroup, bool> OnUpdateResourcesForPlayer; //int: ID of player, ResourceGroup: ammount of resources to change, bool: update ui or not
    public static Action<int,  ResourceGroup> OnAttemptToChargePlayer; //int: ID of player, ResourceGroup: ammount of resources to change
    public static Action<ResourceGroup> OnUpdateResourceCanvasForPlayer; //ResourceGroup: the players actual resources

}

