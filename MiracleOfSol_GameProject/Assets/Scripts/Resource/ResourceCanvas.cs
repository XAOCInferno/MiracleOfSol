using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

[Serializable]
public struct ResourceCanvasEntries
{
    public TextMeshProUGUI Primary;
    public TextMeshProUGUI Secondary;
    public TextMeshProUGUI Special;
}

public class ResourceCanvas : MonoBehaviour
{
    [SerializeField] private ResourceCanvasEntries ResourceCanvasTextFields;

    private void OnEnable()
    {
        Actions.OnUpdateResourceCanvasForPlayer += ChangeResources;
    }

    private void OnDisable()
    {
        Actions.OnUpdateResourceCanvasForPlayer -= ChangeResources;
    }

    public void ChangeResources(ResourceGroup NewResources)
    {

        ResourceCanvasTextFields.Primary.text = NewResources.Primary.ToString();
        ResourceCanvasTextFields.Secondary.text = NewResources.Secondary.ToString();
        ResourceCanvasTextFields.Special.text = NewResources.Special.ToString();

    }
}
