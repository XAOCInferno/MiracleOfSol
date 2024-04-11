using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class BtnDescBoxInfo : MonoBehaviour
{
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI DescText;
    public TextMeshProUGUI[] CostTexts;
    public TextMeshProUGUI TimeCost;

    private Transform CostParent;

    public void SetDescValues(string NewName, string NewDesc, string NewTime, string[] NewCost)
    {
        NameText.text = NewName; DescText.text = NewDesc; TimeCost.text = NewTime;
        CostParent = CostTexts[0].transform.parent;

        if (NewCost == null)
        {
            CostParent.gameObject.SetActive(false);
        }
        else
        {
            for(int i = 0; i < NewCost.Length; i++)
            {
                CostTexts[i].text = NewCost[i];
            }
        }
    }
}
