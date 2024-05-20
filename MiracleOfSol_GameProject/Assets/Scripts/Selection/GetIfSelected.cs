using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GetIfSelected : MonoBehaviour
{
    public bool IsInDevMode = false;
    public Transform UI_Canvas;
    public GameObject DevWhitePixel;
    public SelectionArtManager SAM;
    public bool ForceNoSelection = false;
    public bool IsPrimarySelection = false;

    //Don't edit these in inspector
    private List<Vector2[]> StickyArea = new List<Vector2[]> { new Vector2[] { new Vector2(0, 0), new Vector2(1510, 310) } };
    private string SelectedStatus = "None";
    private OnSelectActivate SelectActivate;

    private void Start()
    {
        GameInfo GI = GameObject.Find("GAME_MANAGER").GetComponent<GameInfo>();
        if(UI_Canvas == null) { UI_Canvas = GI.UI_Canvas.transform; };

        SelectActivate = gameObject.GetComponent<OnSelectActivate>();

        if(SelectActivate == null)
        {
            if (transform.parent != null)
            {
                if (transform.parent.GetComponent<AI_Controller>() == null)
                {
                    BasicInfo BI = gameObject.GetComponent<BasicInfo>();
                    SelectActivate = gameObject.AddComponent<OnSelectActivate>();

                    SelectActivate.SetupInitialVar(BI.EBPs.IsProduction, BI.EBPs.HasJump, BI.EBPs.HasAbility);
                }
            }
        }

        if (IsInDevMode)
        {
            if(UI_Canvas == null)
            {
                UI_Canvas = GI.UI_Canvas.transform;
            }

            DevDisplaySticky();
        }

        Actions.OnRegisterSelectableObject(this);

    }

    private void DevDisplaySticky()
    {
        for (int i = 0; i < StickyArea.Count; i++)
        {
            GameObject DevImage = Instantiate(DevWhitePixel, UI_Canvas);
            DevImage.GetComponent<Image>().color = new Color(125, 125, 125);

            RectTransform DevImage_RT = DevImage.GetComponent<RectTransform>();

            DevImage_RT.position = StickyArea[i][0];
            DevImage_RT.sizeDelta = (StickyArea[i][0] + StickyArea[i][1]) / 2;
        }        
    }

    public bool CheckIfClickInSticky()
    {
        bool IsSticky = false; //EWWWW!

        for (int i = 0; i < StickyArea.Count; i++)
        {
            Vector2 MousePos = Input.mousePosition;

            if (MousePos.x >= StickyArea[i][0][0] && MousePos.x <= StickyArea[i][1][0] / 2 && MousePos.y >= StickyArea[i][0][1] && MousePos.y <= StickyArea[i][1][1] / 2)
            {
                IsSticky = true;
                break;
            }
        }

        return IsSticky;
    }

    public void ResetSelectionStatus()
    {
        IsPrimarySelection = false;
        SelectedStatus = "None";
        SAM.EnableArt();

        try
        {
            SelectActivate.ActivateObject(false);
        }
        catch
        {
            Debug.LogWarning("ERROR! In GIS/ResetSelectionStatus, Cannot Activate the special UI!");
        }
    }

    public void SetHoverSelectionStatus()
    {
        IsPrimarySelection = false;

        if (!ForceNoSelection)
        {
            SelectedStatus = "Hover";
            SAM.EnableArt(false, false, true);

            SelectActivate.ActivateObject(false);
        }
        else
        {
            ResetSelectionStatus();
        }
    }

    public void SetSelectedStatus()
    {
        if (!ForceNoSelection)
        {
            SelectedStatus = "Selected";
            SAM.EnableArt(false, true, false);

            if (IsPrimarySelection) { SelectActivate.ActivateObject(true); } else { SelectActivate.ActivateObject(false); }
        }
        else
        {
            ResetSelectionStatus();
        }
    }

    public string GetSelectedStatus()
    {
        return SelectedStatus;
    }
}
