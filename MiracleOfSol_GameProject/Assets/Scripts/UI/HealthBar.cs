using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public RectTransform Bar;
    public Image BarImage;

    public float MaxHP = 100;
    public float CurrentHP = 100;
    public Color32 MaxHP_Colour = new Color32(0, 255, 0, 255);
    public Color32 MinHP_Colour = new Color32(255, 0, 0, 255);

    private Vector2 StartingDimmensions;

    private void Start()
    {
        if(Bar == null)
        {
            Bar = gameObject.GetComponent<RectTransform>();
        }

        if(BarImage == null)
        {
            BarImage = gameObject.GetComponent<Image>();
        }

        Bar.pivot = new Vector2(0, 0.5f);
        StartingDimmensions = Bar.sizeDelta;
    }

    private void Update()
    {
            UpdateBarGraphic();

            

    }

    private void UpdateBarGraphic()
    {
        try
        {
            float HP_Percent = CurrentHP / MaxHP;

            Bar.sizeDelta = new Vector2(StartingDimmensions.x * HP_Percent, Bar.sizeDelta.y);

            BarImage.color = Color32.Lerp(MinHP_Colour, MaxHP_Colour, HP_Percent);
        }
        catch
        {
            Debug.LogWarning("Error: HealthBar, Cannot update graphic");
        }
    }

    public void SetHP_Percentage(float HP)
    {
        SetHP(HP, "percent");
    }

    public void SetHP(float HP, string type = "set")
    {
        if(type.ToLower() == "raw")
        {
            CurrentHP += HP;

            if(CurrentHP < 0)
            {
                CurrentHP = 0;
            }
            else if(CurrentHP > MaxHP)
            {
                CurrentHP = MaxHP;
            }
        }
        else if(type.ToLower() == "percent")
        {
            if(HP > 1)
            {
                HP = 1;
            }else if (HP < 0)
            {
                HP = 0;
            }

            CurrentHP = MaxHP * HP;
        }
        else
        {
            CurrentHP = HP;
        }

        UpdateBarGraphic();
    }

    public float GetHP()
    {
        return CurrentHP;
    }
}
