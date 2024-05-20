using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectionManager : MonoBehaviour
{
    public HeroIconManager PrimarySelectionIcon;
    public Transform AllHeroSelectionButtons;
    private bool UseStickySelect = false;
    public int AcceptSelectionForPlayer = 0;
    public bool AllowMouseSelection = true;
    public bool AllowKeyboardSelection = true;

    public BasicFunctions BF;
    public Camera Cam;
    public RectTransform RT;
    public List<GetIfSelected> SelectableObjects = new List<GetIfSelected>();
    public List<BasicInfo> SelectableObjects_BI = new List<BasicInfo>();
    public List<Health> SelectableObjects_Health = new List<Health>();
    public List<VoiceLineManager> SelectableObjects_VLM = new List<VoiceLineManager>();
    public GameInfo GI;

    private bool IsDragging = false;
    private Vector2 DragStartPos;
    private Vector3 OldMousePos = new Vector3();
    private LayerMask EntityLayer;
    private LayerMask TerrainLayer;
    private List<HeroIconManager> HIM = new List<HeroIconManager>();
    private RTSCameraController RTS_CC;
    private float PreviousSelectTime = 0;
    private KeyCode PreviousSelectKey = KeyCode.Escape;
    private bool IsAutoFollow;

    private KeyCode[] AllHeroSelectionKeys = new KeyCode[10] 
    {
        KeyCode.Alpha1, KeyCode.Alpha2, KeyCode.Alpha3, KeyCode.Alpha4,
        KeyCode.Alpha5, KeyCode.Alpha6, KeyCode.Alpha7, KeyCode.Alpha8,
        KeyCode.Alpha9, KeyCode.Alpha0
    };

    private void Start()
    {
        if (GI == null) { GameObject.FindWithTag("GameController").TryGetComponent(out GI); }
        if (BF == null) { GI.TryGetComponent(out BF); }

        EntityLayer = LayerMask.NameToLayer("Entity");
        TerrainLayer = LayerMask.NameToLayer("Terrain");

        for(int i = 0; i < SelectableObjects.Count; i++)
        {
            AddObjBasicInfo(i);
        }

        foreach (Transform child in AllHeroSelectionButtons) 
        { 
            child.gameObject.SetActive(false);
            child.TryGetComponent(out HeroIconManager tmpHIM);
            if (tmpHIM != null) { HIM.Add(tmpHIM); }
        }

        GI.EUS_UI_CurrentlySelectedParent.SetActive(false);
        GI.MainCamera.TryGetComponent(out RTS_CC);

        InvokeRepeating(nameof(SetIconsForHeroes), 1, 2);
        InvokeRepeating(nameof(UpdateControlGlobals), 0, 1);
    }

    private void UpdateControlGlobals()
    {
        UseStickySelect = PlayerPrefsX.GetBool("IsSticky", false);
        IsAutoFollow = PlayerPrefsX.GetBool("IsAutoFollow", false);
    }

    private void AddObjBasicInfo(int Pos)
    {
        SelectableObjects[Pos].TryGetComponent(out BasicInfo tmpBI);
        SelectableObjects[Pos].TryGetComponent(out VoiceLineManager tmpVLM);
        SelectableObjects[Pos].TryGetComponent(out Health tmpHealth);
        SelectableObjects_BI.Add(tmpBI);
        SelectableObjects_VLM.Add(tmpVLM);
        SelectableObjects_Health.Add(tmpHealth);
    }

    private void SetIconsForHeroes()
    {
        try
        {
            if (SelectableObjects.Count > 0)
            {
                for (int j = 0; j < SelectableObjects.Count; j++)
                {
                    if (HIM.Count >= j)
                    {
                        if (SelectableObjects_BI[j].OwnedByPlayer == 0)
                        {
                            HIM[j].UpdateImageGraphic(SelectableObjects_BI[j].EBPs.UI_Icon, SelectableObjects_Health[j].GetCurrentHP_AsPercentOfMax());
                            HIM[j].gameObject.SetActive(true);
                        }
                        else
                        {
                            SelectableObjects.RemoveAt(j);
                            SelectableObjects_BI.RemoveAt(j);
                            SelectableObjects_Health.RemoveAt(j);
                            SelectableObjects_VLM.RemoveAt(j);
                        }
                    }
                }

                for (int i = SelectableObjects.Count; i < HIM.Count; i++)
                {
                    HIM[i].gameObject.SetActive(false);
                }
            }
        }
        catch
        {
            //PRobably in cutscene...
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(SelectableObjects.Count > SelectableObjects_BI.Count)
        {
            for (int i = SelectableObjects_BI.Count; i < SelectableObjects.Count; i++)
            {
                AddObjBasicInfo(i);
            }
        }

        if (AllowMouseSelection) { CheckForMouseSelection(); }
        if (AllowKeyboardSelection) { CheckForKeyboardSelection(); }
    }

    private void CheckForKeyboardSelection()
    {
        for(int key = 0; key < AllHeroSelectionKeys.Length; key++)
        {
            if (Input.GetKeyDown(AllHeroSelectionKeys[key]) && SelectableObjects.Count > key)
            {
                CheckHeroSelectionViaKeyboard(key);
            }
        }
    }

    public void CheckHeroSelectionViaKeyboard(int pos)
    {
        if ((Time.time - 1.5f <= PreviousSelectTime && PreviousSelectKey == AllHeroSelectionKeys[pos]) || IsAutoFollow)
        {
            RTS_CC.FocusOnEntity(SelectableObjects[pos].transform);
        }

        PreviousSelectTime = Time.time;
        PreviousSelectKey = AllHeroSelectionKeys[pos];
        SingleSelectObjectFromKeyboard(pos, Input.GetKey(KeyCode.LeftShift));
    }

    private void SingleSelectObjectFromKeyboard(int Pos, bool IsShiftClicking)
    {
        if (!IsShiftClicking)
        {
            ResetSelectionStatus(); 
        }         

        SelectableObjects[Pos].IsPrimarySelection = true;
        SelectableObjects_Health[Pos].UI_SetIndicator(true);

        GI.AllPlayers_SM[SelectableObjects_BI[Pos].OwnedByPlayer].SquadSelection_SetSelectedStatus(SelectableObjects_BI[Pos].GetIDs());
        SelectableObjects_VLM[Pos].PlayVoiceLineOfType("SELECTION");
        SelectableObjects[Pos].SetSelectedStatus();
        CheckIfUpdateMiniHeroUI();
    }

    public void ResetSelectionStatus()
    {
        for(int i = 0; i < SelectableObjects.Count; i++)
        {
            SelectableObjects[i].ResetSelectionStatus();
        }
    }

    private void CheckForMouseSelection() 
    {
        Vector3 NewMousePos = Input.mousePosition;
        bool IsMouseClick = Input.GetMouseButtonDown(0);
        bool IsMouseClickHeld = Input.GetMouseButton(0);
        bool IsMouseClick_Up = Input.GetMouseButtonUp(0);

        if (NewMousePos != OldMousePos)
        {
            if (!GI.UITimeout)
            {
                SingleSelectObject(); //Check for Hover
            }

            if (IsMouseClickHeld && !IsDragging && !RT.gameObject.activeInHierarchy)
            {
                RT.gameObject.SetActive(true);
                DragStartPos = NewMousePos;
                IsDragging = true;
            }
        }

        OldMousePos = NewMousePos;

        if (!GI.UITimeout)
        {
            if (IsDragging)
            {
                bool[] MultiSelectKeys = { Input.GetKey("left ctrl"), Input.GetKey("right ctrl"), Input.GetKey("left shift"), Input.GetKey("right shift") };
                bool IsHoldMultiSelectKeys = BF.FindIfArrayContainsTrueOrFalse(MultiSelectKeys);

                if (!IsHoldMultiSelectKeys)
                {
                    ClickDrag();
                }
                else
                {
                    ClickDrag(false, true);
                }

                if (!IsMouseClickHeld)
                {
                    if (!IsHoldMultiSelectKeys)
                    {
                        ClickDrag(true);
                    }
                    else
                    {
                        ClickDrag(true, true);
                    }

                    Vector2 DragStartPos = new Vector2();

                    RT.sizeDelta = new Vector2(Mathf.Abs(0), Mathf.Abs(0));
                    RT.anchoredPosition = new Vector2(DragStartPos.x + 0 / 2, DragStartPos.y + 0 / 2);
                    RT.gameObject.SetActive(false);
                    IsDragging = false;
                }
            }
            else if (IsMouseClick_Up)
            {
                SingleSelectObject(true);
            }
        }
    }

    private void SingleSelectObject(bool IsSelect = false, bool DoNotReset = false)
    {
        RaycastHit hit;
        Ray ray = Cam.ScreenPointToRay(Input.mousePosition);

        Vector2 MousePos = Input.mousePosition;
        if (Physics.Raycast(ray, out hit, ~LayerMask.GetMask("UI")))
        {
            MousePos = hit.point;
        }
 
        Vector2 min = MousePos - new Vector2(12, 12); //If necessary edit these for objects that are bigger. Make the numbers bigger
        Vector2 max = MousePos + new Vector2(12, 12);

        bool tmp_HasAssignedPrimary = false;

        for (int i = 0; i < SelectableObjects.Count; i++)
        {
            GetIfSelected child = SelectableObjects[i];
            if (child != null)
            {
                child.TryGetComponent(out BasicInfo tmpChildBI);
                if (child.enabled && tmpChildBI.OwnedByPlayer == AcceptSelectionForPlayer)
                {
                    Vector2 ChildPos = Cam.WorldToScreenPoint(child.transform.position);

                    if (ChildPos.x > min.x && ChildPos.x < max.x &&
                        ChildPos.y > min.y && ChildPos.y < max.y)
                    {
                        if (IsSelect)
                        {
                            GI.AllPlayers_SM[SelectableObjects_BI[i].OwnedByPlayer].SquadSelection_SetSelectedStatus(SelectableObjects_BI[i].GetIDs());
                            SelectableObjects_VLM[i].PlayVoiceLineOfType("SELECTION");

                            if (!tmp_HasAssignedPrimary)
                            {
                                tmp_HasAssignedPrimary = true;
                                child.IsPrimarySelection = true;
                                SelectableObjects_Health[i].UI_SetIndicator(true);
                            }

                            child.SetSelectedStatus();
                            if (IsAutoFollow)
                            {
                                RTS_CC.FocusOnEntity(SelectableObjects[i].transform);
                            }
                        }
                        else
                        {
                            if ((child.GetSelectedStatus() == "Selected" && IsSelect) || child.GetSelectedStatus() == "None")
                            {
                                GI.AllPlayers_SM[SelectableObjects_BI[i].OwnedByPlayer].SquadSelection_SetHoverStatus(SelectableObjects_BI[i].GetIDs());
                                child.SetHoverSelectionStatus();
                            }
                        }
                    }
                    else if (child.GetSelectedStatus() == "Hover")
                    {
                        GI.AllPlayers_SM[SelectableObjects_BI[i].OwnedByPlayer].SquadSelection_SetNeutralStatus(SelectableObjects_BI[i].GetIDs());
                        child.ResetSelectionStatus();
                    }
                    else if (IsSelect && !Input.GetKey(KeyCode.LeftShift) && !UseStickySelect)
                    {
                        //GI.AllPlayers_SM[SelectableObjects_BI[i].OwnedByPlayer].SquadSelection_SetNeutralStatus(SelectableObjects_BI[i].GetIDs());
                        child.ResetSelectionStatus();
                    }
                }
            }
            else
            {
                SelectableObjects.RemoveAt(i);
                SelectableObjects_BI.RemoveAt(i);
            }
        }

        CheckIfUpdateMiniHeroUI();
    }

    private void ClickDrag(bool IsSelecting = false, bool IsShiftClicking = false)
    {
        //RaycastForObject(IsSelecting, IsShiftClicking);
        CheckIfObjectsExist();
        UpdateBox();

        if (RT.sizeDelta[0] >= 1 || RT.sizeDelta[1] >= 1)
        {
            UpdateSelection(IsSelecting, IsShiftClicking);
        }
    }

    private void UpdateBox()
    {
        Vector2 DragEndPos = Input.mousePosition;

        float width = DragEndPos.x - DragStartPos.x;
        float height = DragEndPos.y - DragStartPos.y;

        RT.sizeDelta = new Vector2(Mathf.Abs(width), Mathf.Abs(height));
        RT.anchoredPosition = new Vector2(DragStartPos.x + width / 2, DragStartPos.y + height / 2);
    }

    private void CheckIfObjectsExist()
    {
        for(int i  = 0; i < SelectableObjects.Count; i++)
        {
            if(SelectableObjects[i] == null)
            {
                SelectableObjects.RemoveAt(i);
            }
        }
    }

    private void UpdateSelection(bool IsSelecting = false, bool IsShiftClicking = false)
    {
        Vector2 min = RT.anchoredPosition - (RT.sizeDelta / 2);
        Vector2 max = RT.anchoredPosition + (RT.sizeDelta / 2);

        bool tmp_HasAssignedPrimary = false;

        for (int i = 0; i < SelectableObjects.Count; i++)
        {
            GetIfSelected child = SelectableObjects[i];
            child.TryGetComponent(out BasicInfo tmpChildBI);
            //IF HAVE TIME COME BACK AND EDIT THIS TO USE POS NOT GETCOMPONENT FOR EFFICIENCY

            //ALSO EDIT FOR PRIORITY SELECTION SO THE FIRST SELECTED ENTITY SPEAKS
            if (child.enabled && tmpChildBI.OwnedByPlayer == AcceptSelectionForPlayer)
            {
                Vector3 ChildPos = Cam.WorldToScreenPoint(child.transform.position);

                if (ChildPos.x > min.x && ChildPos.x < max.x &&
                    ChildPos.y > min.y && ChildPos.y < max.y)
                {
                    if (IsSelecting)
                    {
                        GI.AllPlayers_SM[SelectableObjects_BI[i].OwnedByPlayer].SquadSelection_SetSelectedStatus(SelectableObjects_BI[i].GetIDs());
                        SelectableObjects_VLM[i].PlayVoiceLineOfType("SELECTION");

                        if (!tmp_HasAssignedPrimary)
                        {
                            tmp_HasAssignedPrimary = true;
                            child.IsPrimarySelection = true;
                            SelectableObjects_Health[i].UI_SetIndicator(true);
                        }

                        child.SetSelectedStatus();
                    }
                    else
                    {
                        if ((child.GetSelectedStatus() == "Selected" && !IsShiftClicking) || child.GetSelectedStatus() == "None")
                        {
                            GI.AllPlayers_SM[SelectableObjects_BI[i].OwnedByPlayer].SquadSelection_SetHoverStatus(SelectableObjects_BI[i].GetIDs());
                            child.SetHoverSelectionStatus();
                        }
                    }
                }
                else if ((!IsShiftClicking || child.GetSelectedStatus() == "Hover") && !UseStickySelect)
                {
                    GI.AllPlayers_SM[SelectableObjects_BI[i].OwnedByPlayer].SquadSelection_SetNeutralStatus(SelectableObjects_BI[i].GetIDs());
                    child.ResetSelectionStatus();
                }
            }
        }

        CheckIfUpdateMiniHeroUI();
    }

    private void CheckIfUpdateMiniHeroUI()
    {
        bool AnObjectIsSelected = false;

        for (int i = 0; i < SelectableObjects.Count; i++)
        {
            if (SelectableObjects[i].GetSelectedStatus() == "Selected")
            {
                GI.EUS_UI_CurrentlySelectedParent.SetActive(true);
                AnObjectIsSelected = true;
            }
        }

        if (!AnObjectIsSelected)
        {
            GI.EUS_UI_CurrentlySelectedParent.SetActive(false);
        }
    }
}
