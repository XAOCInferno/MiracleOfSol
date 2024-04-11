using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExtendedUnitStatusUI : MonoBehaviour
{
    public UnityEngine.UI.Image CurrentlySelectedEntityImg;
    public HealthBar HP_Bar;
    public HealthBar Armour_Bar;
    public GameObject DamagePrompt;
    public UIChangeImage UI_ArmourTypeIndicator;
    public UIChangeImage UI_DamageTypeIndicator;
    public TMPro.TextMeshProUGUI QuoteText;    

    public void UI_SetIndicator(GameInfo GI, int Armour, Sprite ImgDamage, string Quote)
    {
        UI_ArmourTypeIndicator.SetNewImage(GI.UI_ArmourTypeIdentifiers[Armour]);
        UI_DamageTypeIndicator.SetNewImage(ImgDamage);
        QuoteText.text = Quote;
    }
}
