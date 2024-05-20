using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HeroIconManager : MonoBehaviour
{
    public int HeroID = -1;

    private Image EntityImage;
    private Image EntityStatusBorder;
    private Color HealthyColour = new Color(0, 255, 0);
    private Color DeathColour = new Color(255, 0, 0);

    private void Start()
    {
        GameObject.FindWithTag("GameController").TryGetComponent(out GameInfo tmpGI);
        EntityImage = transform.GetChild(0).GetComponent<Image>();
        EntityStatusBorder = transform.GetChild(1).GetComponent<Image>();
    }

    public void SelectEntityViaButton()
    {

        Actions.OnTrySelectHeroByButton.InvokeAction(HeroID);

    }

    public void UpdateImageGraphic(Sprite NewEntityImage, float HealthStatus = -1)
    {
        try
        {
            if (NewEntityImage != null) { EntityImage.sprite = NewEntityImage; }
            if (HealthStatus != -1)
            {
                HealthStatus = Mathf.Clamp(HealthStatus, 0f, 1f);
                EntityStatusBorder.color = Color.Lerp(DeathColour, HealthyColour, HealthStatus);
            }
        }
        catch
        {
            Debug.LogWarning("HIM: Cannot assign health to Hero icon, game is probably in a cutscene");
        }
    }
}
