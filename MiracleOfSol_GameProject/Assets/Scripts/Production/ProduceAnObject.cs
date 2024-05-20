using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProduceAnObject : MonoBehaviour
{
    public float ProductionRate = 1;

    public Vector2 ButtonOffset = new Vector2(0, 0);

    public Transform SpawnParent;
    public Transform SpawnLocation;
    public Transform BTN_SpawnParent;

    public GameObject[] AllProducedObjs;

    public Button ButtonTemplate;

    private BasicInfo BI;

    public int NumberButtonsX = 4;
    public int NumberButtonsY = 2;
    private List<Button> ProductionButtons = new List<Button>();
    private List<GameObject> BuildQueue = new List<GameObject>();
    private List<float> TimeUntilBuilt = new List<float>();

    private GameInfo GI;

    private void Start()
    {
        GI = GameObject.FindWithTag("GameController").GetComponent<GameInfo>();
        BI = gameObject.GetComponent<BasicInfo>();
        ButtonTemplate.GetComponent<RectTransform>().sizeDelta = new Vector2(128, 128);

        //OnSelectActivate Temp_OSA = gameObject.AddComponent<OnSelectActivate>();
        //Temp_OSA.SetupInitialVar(true, BI.EBPs.HasJump);

        if (transform.parent.GetComponent<AI_Controller>() == null)
        {
            if (BTN_SpawnParent == null) { BTN_SpawnParent = GI.ProductionUI.transform; }

            if (SpawnParent == null) { SpawnParent = transform.parent; }

            RectTransform RT_NewButton = ButtonTemplate.GetComponent<RectTransform>();
            ButtonOffset = new Vector2(ButtonOffset[0] + (RT_NewButton.sizeDelta[0] / 2), ButtonOffset[1] + (RT_NewButton.sizeDelta[0] / 2));
            for (int i = 0; i < AllProducedObjs.Length; i++)
            {
                BasicInfo CurrentEntity = AllProducedObjs[i].GetComponent<BasicInfo>();
                Button Temp_NewButton = Instantiate(ButtonTemplate, BTN_SpawnParent);
                if (CurrentEntity.EBPs.UI_Icon != null) { Temp_NewButton.image.sprite = CurrentEntity.EBPs.UI_Icon; }
                RT_NewButton = Temp_NewButton.GetComponent<RectTransform>();

                RT_NewButton.position = new Vector2(RT_NewButton.sizeDelta[0] * CurrentEntity.EBPs.PositionInBuildUI[0], RT_NewButton.sizeDelta[1] * (NumberButtonsY + 1) - RT_NewButton.sizeDelta[1] - (RT_NewButton.sizeDelta[1] * CurrentEntity.EBPs.PositionInBuildUI[1])) + ButtonOffset;

                ProductionButtonInfo PBI = Temp_NewButton.GetComponent<ProductionButtonInfo>();
                PBI.EntityToProduce = i;

                Temp_NewButton.onClick.AddListener(delegate { BuildEBP(PBI.EntityToProduce); });
                //Temp_NewButton.transform.GetChild(0).GetComponent<Text>().text = CurrentEntity.EBPs.EntityName + " (" + CurrentEntity.EBPs.BuildTimePerEntity * CurrentEntity.SBPs.SquadMin + "s)";
                ButtonTextManager BTM = Temp_NewButton.GetComponent<ButtonTextManager>();
                string[] TotalCostStr = new string[3] { "", "", "" };

                for(int j = 0; j < CurrentEntity.EBPs.CostPerEntity.Length; j++)
                {
                    TotalCostStr[j] =  ((int) (CurrentEntity.EBPs.CostPerEntity[j] * CurrentEntity.SBPs.SquadMin)).ToString();
                }

                BTM.BDBI.SetDescValues(CurrentEntity.EBPs.EntityName, CurrentEntity.EBPs.EntityDesc, CurrentEntity.EBPs.BuildTimePerEntity * CurrentEntity.SBPs.SquadMin + "s", TotalCostStr);

                ProductionButtons.Add(Temp_NewButton);
            }
        }

        /*float y = temp_rt.sizeDelta[1] * NumberButtonsY;
        float x = 0;
        int x_reducer = 0;
        for (int i = 0; i < AllProducedObjs.Length; i++)
        {
            BasicInfo CurrentEntity = AllProducedObjs[i].GetComponent<BasicInfo>();
            Button Temp_NewButton = Instantiate(ButtonTemplate, BTN_SpawnParent);
            RectTransform RT_NewButton = Temp_NewButton.GetComponent<RectTransform>();

            y = temp_rt.sizeDelta[1] * NumberButtonsY - temp_rt.sizeDelta[1] * CurrentEntity.EBPs.BuildTier;
            if (i == NumberButtonsY) { y -= RT_NewButton.sizeDelta.y; x_reducer += 3; }//TEMP
            x = RT_NewButton.sizeDelta.x * (i - x_reducer);
            RT_NewButton.position = new Vector2(x, y) + ButtonOffset;

            ProductionButtonInfo PBI = Temp_NewButton.GetComponent<ProductionButtonInfo>();
            PBI.EntityToProduce = i;

            Temp_NewButton.onClick.AddListener(delegate { BuildEBP(PBI.EntityToProduce); } );
            Temp_NewButton.transform.GetChild(0).GetComponent<Text>().text = CurrentEntity.EBPs.EntityName + " (" + CurrentEntity.EBPs.BuildTimePerEntity * CurrentEntity.SBPs.SquadMin + "s)";

            ProductionButtons.Add(Temp_NewButton);
        }*/

    }

    private void Update()
    {
        if (BuildQueue.Count > 0)
        {
            if (TimeUntilBuilt[0] <= 0)
            {
                CompleteProduction();
            }
            else
            {
                TimeUntilBuilt[0] -= Time.deltaTime * ProductionRate;
            }
        }
    }

    public void BuildEBP(int ButtonIdentifier)
    {
        GI.UITimeout = true;
        Invoke(nameof(ReturnSelectionAbility), 0.1f);

        BasicInfo CurrentUnit = AllProducedObjs[ButtonIdentifier].GetComponent<BasicInfo>();
        EBP_Info CurrentEntity = CurrentUnit.EBPs;
        SBP_Info SBP = CurrentUnit.SBPs;

        if (SBP == null)
        {
            //The Object is a single entity and not a squad
            CurrentEntity.IsASquad = false;
            CurrentEntity.TotalBuildTime = CurrentEntity.BuildTimePerEntity;
        }
        else
        {
            CurrentEntity.TotalBuildTime = SBP.SquadMin * CurrentEntity.BuildTimePerEntity;
        }

        string tmp_name = CurrentEntity.EntityName;
        float tmp_time = CurrentEntity.TotalBuildTime;
        ResourceGroup tmp_cost = new ( CurrentEntity.CostPerEntity[0] * SBP.SquadMin, CurrentEntity.CostPerEntity[1] * SBP.SquadMin, CurrentEntity.CostPerEntity[2] * SBP.SquadMin );

        if (ResourceManager.AttemptToChargePlayer(BI.OwnedByPlayer, tmp_cost))
        {
            BuildQueue.Add(CurrentUnit.gameObject);
            TimeUntilBuilt.Add(tmp_time);

            Debug.Log("Building Entity... '" + tmp_name + "' " + "(ID: " + ButtonIdentifier + ") with... " + SBP.SquadMin + " Squad Members in... " + tmp_time + "s");
        }
        else
        {
            Debug.Log("Attempted to Produce Entity: '" + tmp_name + "' " + "(ID: " + ButtonIdentifier + ") with... " + SBP.SquadMin + " Squad Members has failed due to not enough Resources!");
        }
    }

    private void ReturnSelectionAbility() { GI.UITimeout = false; }

    private void CompleteProduction()
    {
        BasicInfo tmp_BI = BuildQueue[0].GetComponent<BasicInfo>();
        SBP_Info Squad = tmp_BI.SBPs;
        bool IsNewSquad = true;
        GameObject NewSquad = Instantiate(new GameObject(), SpawnLocation.position, new Quaternion(), SpawnParent);

        Vector3 Offset = new Vector3(0,0,0);
        int x_Offset = 0;
        int z_Offset = 0;
        int OffsetAtModel = (int) Squad.SquadMin / 3;

        for (int i = 0; i < Squad.SquadMin; i++)
        {
            if (i > Squad.SquadMin)
            {
                break;
            }
            else
            {
                if(i >= OffsetAtModel)
                {
                    OffsetAtModel += (int)Squad.SquadMin / 3;
                    x_Offset = 0;
                    z_Offset++;
                }
                GameObject TempObj = Instantiate(BuildQueue[0], SpawnLocation.position + Offset, new Quaternion(), NewSquad.transform);
                TempObj.GetComponent<BoxCollider>().size = tmp_BI.EBPs.EntityScale;
                //EBP_Info TempEBP = TempObj.GetComponent<EBP_Info>();

                //Setup_EBPs_OSA(TempObj, TempEBP);
                Setup_EBPs_BI_and_Squad(TempObj, IsNewSquad);
                IsNewSquad = false;
                Offset += new Vector3(x_Offset, 0, z_Offset);
                x_Offset++;
            }
        }

        BuildQueue.RemoveAt(0);
        TimeUntilBuilt.RemoveAt(0);
    }

    private void Setup_EBPs_OSA(GameObject TempObj, EBP_Info TempEBP) //FIX ME :(
    {
        bool HasProduction = false; bool HasJump = false;

        if (TempEBP.HasJump) { TempObj.AddComponent<JumpAndTeleport>(); }
        if (TempObj.GetComponent<ProduceAnObject>() != null) { HasProduction = true; }

        OnSelectActivate Temp_OSA = TempObj.GetComponent<OnSelectActivate>();
        if(Temp_OSA == null){ Temp_OSA = TempObj.AddComponent<OnSelectActivate>(); }

        Temp_OSA.SetupInitialVar(HasProduction, HasJump);
    }

    private void Setup_EBPs_BI_and_Squad(GameObject TempObj, bool IsNewSquad)
    {
        BasicInfo TempBI = TempObj.GetComponent<BasicInfo>();
        TempBI.OwnedByPlayer = BI.OwnedByPlayer;

        SquadManager CurrentSquadManager = GI.AllPlayers[BI.OwnedByPlayer].GetComponent<SquadManager>();
        int[] NewIDs = CurrentSquadManager.CalculateNewIDs(IsNewSquad);
        CurrentSquadManager.Set_NewEntity(TempObj, NewIDs[1]);

        TempBI.SetID(NewIDs[0], NewIDs[1]);
    }
}
