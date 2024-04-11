using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupBar : MonoBehaviour
{
    public bool BarIsUnique = false;
    public int BarToGet = 0; //0 = HP, 1 = F.A, 2 = DmgPromptHP, 3 = Armour, 4 = CapBar
    private HealthBar HB;
    private UIOnHitSetDmgValues UI_OHSDV;
    private GameInfo GI;

    // Start is called before the first frame update
    void Start()
    {
        if (!BarIsUnique)
        {
            GI = GameObject.FindWithTag("GameController").GetComponent<GameInfo>().GetComponent<GameInfo>();

            if (BarToGet < 4)
            {
                if (BarToGet < 3)
                {
                    if (BarToGet < 2)
                    {
                        HB = gameObject.GetComponent<HealthBar>();
                        HB.MaxHP_Colour = GI.Bar_MaxColourInfo[BarToGet];
                        HB.MinHP_Colour = GI.Bar_MinColourInfo[BarToGet];
                    }
                    else
                    {
                        BarToGet = 2;

                        UI_OHSDV = gameObject.GetComponent<UIOnHitSetDmgValues>();
                        UI_OHSDV.HPColours[0] = GI.Bar_MaxColourInfo[BarToGet];
                        UI_OHSDV.HPColours[1] = GI.Bar_MinColourInfo[BarToGet];

                        UI_OHSDV.ArmourColours[0] = GI.Bar_MaxColourInfo[BarToGet + 1];
                        UI_OHSDV.ArmourColours[1] = GI.Bar_MinColourInfo[BarToGet + 1];
                    }
                }
            }
            else
            {
                HB = gameObject.GetComponent<HealthBar>();
                HB.MaxHP_Colour = GI.Bar_MaxColourInfo[BarToGet];
                HB.MinHP_Colour = GI.Bar_MinColourInfo[BarToGet];
            }
        }
    }
}
