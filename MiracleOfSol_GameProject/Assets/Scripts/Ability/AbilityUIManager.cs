using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AbilityUIManager : MonoBehaviour
{
    public Button ButtonTemplate;
    public Vector2 ButtonOffset = new Vector2(0, 0);
    public Transform BTN_SpawnParent;

    public int NumberButtonsX = 4;
    public int NumberButtonsY = 2;

    private AbilityCaster[] AllAbilities;
    private SpecialAbility_ShiftWeaponry SA_SW;
    private GameInfo GI;
    private bool HasSpecial_SwapWeaponry = false;
    private ButtonTextManager SwapWeaponry_BTM;
    private Button SwapWeaponry_Button;
    private AbilityButtonInfo SwapWeaponry_ABI;
    private GetIfSelected GIS;
    private UIBoneManager UIBM;

    // Start is called before the first frame update
    void Start()
    {
        GIS = gameObject.GetComponent<GetIfSelected>();
        AllAbilities = gameObject.GetComponents<AbilityCaster>();

        SA_SW = gameObject.GetComponent<SpecialAbility_ShiftWeaponry>();
        if(SA_SW != null) { HasSpecial_SwapWeaponry = true; }

        GI = GameObject.FindWithTag("GameController").GetComponent<GameInfo>();

        if (transform.parent != null)
        {
            if (transform.parent.GetComponent<AI_Controller>() == null)
            {
                if (BTN_SpawnParent == null) { BTN_SpawnParent = GI.AbilityUI.transform; }
                UIBM = BTN_SpawnParent.GetComponent<UIBoneManager>();

                foreach (Transform bone in UIBM.AllBones)
                {
                    bone.gameObject.SetActive(false);
                }

                GameObject tmpNewParent = Instantiate(new GameObject());
                tmpNewParent.gameObject.name = "Ability_Canvas: " + gameObject.name;
                tmpNewParent.transform.parent = BTN_SpawnParent;

                BTN_SpawnParent = tmpNewParent.transform;

                RectTransform RT_NewButton = ButtonTemplate.GetComponent<RectTransform>();
                ButtonOffset = new Vector2(ButtonOffset[0] + (RT_NewButton.sizeDelta[0] / 2), ButtonOffset[1] + (RT_NewButton.sizeDelta[0] / 2));

                if (HasSpecial_SwapWeaponry)
                {//Create button and assign its position
                    SwapWeaponry_Button = Instantiate(ButtonTemplate, BTN_SpawnParent);
                    SwapWeaponry_Button.image.sprite = SA_SW.UI_Icon;
                    RT_NewButton = SwapWeaponry_Button.GetComponent<RectTransform>();
                    RT_NewButton.position = UIBM.AllBones[0].position;
                    //RT_NewButton.position = BTN_SpawnParent.transform.GetChild(0).position; //new Vector2(RT_NewButton.sizeDelta[0] * SA_SW.PositionInAbilityUI[0], RT_NewButton.sizeDelta[1] * (NumberButtonsY + 1) - RT_NewButton.sizeDelta[1] - (RT_NewButton.sizeDelta[1] * SA_SW.PositionInAbilityUI[1])) + ButtonOffset;
                    //

                    //Give 'OnClick' Input for the button
                    SwapWeaponry_Button.TryGetComponent(out SwapWeaponry_ABI);
                    SwapWeaponry_ABI.AbilityToCast = -1;
                    SwapWeaponry_ABI.SetHotkey("Q");
                    SwapWeaponry_ABI.SetImage(SA_SW.UI_Icon);
                    SwapWeaponry_Button.onClick.AddListener(delegate { CastSpecialAbility_WeaponSwap(); });
                    //

                    //Create name and description for ability
                    SwapWeaponry_BTM = SwapWeaponry_Button.GetComponent<ButtonTextManager>();
                    string[] TotalCostStr = new string[3] { "", "", "" };

                    for (int j = 0; j < SA_SW.FireCost.Length; j++)
                    {
                        TotalCostStr[j] = ((int)SA_SW.FireCost[j]).ToString();
                    }

                    SwapWeaponry_Button.image.sprite = SA_SW.UI_Icon;
                    SwapWeaponry_BTM.BDBI.SetDescValues(SA_SW.AbilityDisplayName, SA_SW.AbilityDesc, SA_SW.EntityBusyTime.ToString(), TotalCostStr);
                    Invoke(nameof(UpdateSpecialAbilityButtonArt), 0.1f);
                }

                for (int i = 0; i < AllAbilities.Length; i++)
                {
                    CreateNewAbilityButton(RT_NewButton, i);
                }

                BTN_SpawnParent.gameObject.SetActive(false);
                InvokeRepeating(nameof(ResetOnSelectAbilityUI), 0.11f, 0.11f);
            }
        }
    }

    private void ResetOnSelectAbilityUI()
    {
        OnSelectActivate tmpOnSelect = gameObject.GetComponent<OnSelectActivate>();
        if(tmpOnSelect != null)
        {
            tmpOnSelect.ActivatedObjects = new List<GameObject>(1) { BTN_SpawnParent.gameObject };
            CancelInvoke();
        }
    }

    private void Update()
    {
        if (HasSpecial_SwapWeaponry && GIS.GetSelectedStatus() == "Selected")
        {
            if (Input.GetKeyDown(SA_SW.AbilityHK)) { CastSpecialAbility_WeaponSwap(); }
            if (Input.GetMouseButtonUp(0)) { GI.UITimeout = false; }
        }
    }

    private void CreateNewAbilityButton(RectTransform RT_NewButton, int i)
    {
        //Create button and assign its position
        Button Temp_NewButton = Instantiate(ButtonTemplate, BTN_SpawnParent);
        if (AllAbilities[i].UI_Icon != null) { Temp_NewButton.image.sprite = AllAbilities[i].UI_Icon; }
        RT_NewButton = Temp_NewButton.GetComponent<RectTransform>();
        int tmpBoneIndex = (int)(AllAbilities[i].PositionInAbilityUI[0] + (AllAbilities[i].PositionInAbilityUI[1] * 3));
        RT_NewButton.position = UIBM.AllBones[tmpBoneIndex].position; 
        //

        //Give 'OnClick' Input for the button
        AbilityButtonInfo ABI = Temp_NewButton.GetComponent<AbilityButtonInfo>();
        ABI.AbilityToCast = i;
        ABI.SetHotkey(AllAbilities[i].AbilityHK.ToString());
        AllAbilities[i].LinkedButton = Temp_NewButton;

        if (AllAbilities[i].UI_Icon != null)
        {
            ABI.SetImage(AllAbilities[i].UI_Icon);
        }

        Temp_NewButton.onClick.AddListener(delegate { CastAnAbility(ABI.AbilityToCast); });
        //

        //Create name and description for ability
        ButtonTextManager BTM = Temp_NewButton.GetComponent<ButtonTextManager>();
        string[] TotalCostStr = new string[3] { "", "", "" };

        for (int j = 0; j < AllAbilities[i].FireCost.Length; j++)
        {
            TotalCostStr[j] = ((int)AllAbilities[i].FireCost[j]).ToString();
        }

        BTM.BDBI.SetDescValues(AllAbilities[i].AbilityDisplayName, AllAbilities[i].AbilityDesc, AllAbilities[i].EntityBusyTime.ToString(), TotalCostStr);
        //

        //Set if ability is an alternative for shift clicking (enable/disable the button)
        if (AllAbilities[i].IsAltAbility)
        {
            Temp_NewButton.GetComponent<AbilityButtonVisibility>().IsAlt = true;
        }
        //
    }

    public void CastAnAbility(int AbilityToCast)
    {
        GI.UITimeout = true;
        foreach(AbilityCaster AC in AllAbilities) { AC.DisableAbilityCasting(); AC.AbilityIsActiveThroughButton = false; }

        AllAbilities[AbilityToCast].AbilityIsActiveThroughButton = true;
        AllAbilities[AbilityToCast].PreventButtonCasting(0.5f);
    }

    public void CastSpecialAbility_WeaponSwap()
    {
        GI.UITimeout = true;
        foreach (AbilityCaster AC in AllAbilities) { AC.DisableAbilityCasting(); AC.AbilityIsActiveThroughButton = false; }
        SA_SW.ShiftWeapons();

        UpdateSpecialAbilityButtonArt();
    }

    private void UpdateSpecialAbilityButtonArt()
    {
        string[] TotalCostStr = new string[3] { "", "", "" };
        for (int j = 0; j < SA_SW.FireCost.Length; j++)
        {
            TotalCostStr[j] = ((int)SA_SW.FireCost[j]).ToString();
        }

        SwapWeaponry_Button.image.sprite = SA_SW.UI_Icon;
        SwapWeaponry_BTM.BDBI.SetDescValues(SA_SW.AbilityDisplayName, SA_SW.AbilityDesc, SA_SW.EntityBusyTime.ToString(), TotalCostStr);
        SwapWeaponry_ABI.SetImage(SA_SW.UI_Icon);
    }
}
