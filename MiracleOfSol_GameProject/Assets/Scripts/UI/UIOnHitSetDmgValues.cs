using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UIOnHitSetDmgValues : MonoBehaviour
{
    public TextMeshProUGUI HP_UI;
    public TextMeshProUGUI Armour_UI;

    public Color[] HPColours = new Color[2] { new Color(0, 150, 0), new Color(190, 0, 0) };
    public Color[] ArmourColours = new Color[2] { new Color(40, 80, 200), new Color(40, 80, 200) };

    private TextMeshPro HP_Prompt;
    private TextMeshPro Armour_Prompt;

    private void Start()
    {
        HP_Prompt = transform.GetChild(0).GetComponent<TextMeshPro>();
        Armour_Prompt = transform.GetChild(1).GetComponent<TextMeshPro>();
    }
    public void SetValues(float HP, float Armour)
    {
        string PreTextIcon = "+";

        if (HP != 0)
        {
            if (HP < 0) { HP_UI.color = HPColours[1]; PreTextIcon = ""; } else { HP_UI.color = HPColours[0]; }
            HP_UI.text = PreTextIcon + HP.ToString();
        }

        if (Armour != 0)
        {
            if (Armour < 0) { Armour_UI.color = ArmourColours[1]; PreTextIcon = ""; } else { Armour_UI.color = ArmourColours[0]; }
            Armour_UI.text = PreTextIcon + Armour.ToString();
        }
    }
}
