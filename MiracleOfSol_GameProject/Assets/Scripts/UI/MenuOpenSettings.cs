using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuOpenSettings : MonoBehaviour
{
    public Button[] MMBtns;
    public GameObject SettingsParent;
    public GameObject CreditsParent;

    private bool SettingsCurrentActiveState = false;

    public void ToggleSettings()
    {
        SettingsCurrentActiveState = !SettingsCurrentActiveState;
        ToggleMMInteractivity(!SettingsCurrentActiveState);
        SettingsParent.SetActive(SettingsCurrentActiveState);
    }

    public void ToggleCredits()
    {
        SettingsCurrentActiveState = !SettingsCurrentActiveState;
        ToggleMMInteractivity(!SettingsCurrentActiveState);
        CreditsParent.SetActive(SettingsCurrentActiveState);
    }

    private void ToggleMMInteractivity(bool state)
    {
        foreach (Button tmpBtn in MMBtns)
        {
            tmpBtn.interactable = state;
        }
    }
}
