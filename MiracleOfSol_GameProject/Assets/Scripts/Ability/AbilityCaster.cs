using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AbilityCaster : MonoBehaviour
{
    public UnityEngine.UI.Button LinkedButton;
    public GameObject[] ActivateObjOnActivate;
    public Vector2 PositionInAbilityUI = new Vector2(0, 0);
    public Sprite UI_Icon;

    public KeyCode AbilityHK = KeyCode.Q;
    public bool IsAltAbility = false;
    public bool AutoCastWhenReady = false;
    public string AbilityDisplayName = "Name Here";
    public string AbilityName = "__Default";
    public string AbilityDesc = "__DefaultDesc";
    public int AbilityRepeatRate = 0;

    public bool UseProjectile = false;
    public bool ProjectileSpawnAtTarget = false;
    public bool ProjectileUseAnimCurve = false;
    public float ProjectileACMaxY = 2;
    public AnimationCurve ProjectileMovementCurve = AnimationCurve.Linear(0,0,1,0);
    public Vector3 ProjectileSpawnOffset = new Vector3(0, 0, 0);
    public Vector3 ProjectileRandomSpawnOffset = new Vector3(0, 0, 0);
    public GameObject ProjectilePrefab;
    public bool ProjectileCanCollide = false;
    public float ProjectileSpeed = 15;
    public float ProjectileRandomSpeedOffset = 0;
    public float ProjectileDesiredColDistance = 1;
    public bool ProjectileIsHoming = false;
    public GameObject[] ProjectileVFX;
    public GameObject[] ProjectileVFXWarning;

    public string TargetType = "NONE"; //NONE [[dummy]], ENEMY, OWN, ALL, ALLIED [[tbc]]
    public bool TargetTypeOverrideModifier = true;
    public string ActivationType = "TIMED"; //TIMED, TARGETED, ENABLED, PASSIVE
    public bool IsRampage = false;
    public float RampageSpeedBonus = 1;
    public float Cooldown = 60;
    public float Duration = 30;
    public float GenericRadius = 10;
    public float FX_Scale = 1;
    public bool FX_Scale_IsRadius = false;
    public bool FireFXOnce = false;
    public float GenericActivationDelayTime = 0;
    public bool TargetGround = false;
    public float[] FireCost = new float[3] { 0, 0, 0 };
    public AudioClip[] VoiceLineOnCast;
    public AudioClip[] VoiceLineOnRampageDuration;
    private VoiceLineManager VLM;

    public GameObject EntityToSpawn;
    public bool EntitySpawnAsChild = true;

    public GameObject VFX_Cursor;
    public float VFX_Cursor_SizeMultiplier = 1;

    public float EntityBusyTime = 0;
    public bool PersistAfterCasterDeath = true;
    public bool FX_TargetGround = true;

    public GameObject[] Mod_Target;
    public List<bool> KillVFXOnAbilityEnd_Target = new List<bool>();
    public GameObject[] VFX_Target;
    private List<float> Mod_Radius_Target = new List<float>();
    public List<float> Mod_DelayTime_Target = new List<float>(); //Leave blank for 0
    public List<float> Mod_ActiveTime_Target = new List<float>(); //Leave blank for ability duration
    private List<bool> Mod_Target_IsCasted = new List<bool>();
    private List<GameObject> AppliedModsHolder_Target = new List<GameObject>();
    private List<float> AppliedModsHolder_TargetLifetime = new List<float>();
    private List<Vector3> Mod_Offset_Target = new List<Vector3>();

    public GameObject[] Mod_Caster;
    public GameObject[] VFX_Caster;
    private List<float> Mod_Radius_Caster = new List<float>();
    public List<float> Mod_DelayTime_Caster = new List<float>(); //Leave blank for 0
    public List<float> Mod_ActiveTime_Caster = new List<float>(); //Leave blank for ability duration


    private List<bool> Mod_Caster_IsCasted = new List<bool>();
    private List<GameObject> AppliedModsHolder_Caster = new List<GameObject>();
    private List<float> AppliedModsHolder_CasterLifetime = new List<float>();
    private List<Vector3> Mod_Offset_Caster = new List<Vector3>();

    public bool AbilityIsActiveThroughButton = false;
    private bool AbilityIsActive = false;
    private float CurrentAbilityCD = 0;
    private float CurrentAbilityActiveTime = 0;
    private Transform CurrentTarget;
    private Vector3 AbilityImpactPos;

    private LayerMask FloorLayer;
    private LayerMask CapPointLayer;
    private LayerMask EntityLayer;
    private LayerMask UILayer;

    private GameObject VFX_Cursor_Current;
    private Transform VFX_Storage;
    private BasicInfo BI;
    private GetIfSelected GIS;
    private bool IsAnAI;
    private List<GameObject> ActiveVFXObj = new List<GameObject>();
    private AbilityCaster LinkedAbility;
    private bool HasCanceledAbility = false;
    private KeyCode CancelAbilityKey = KeyCode.Escape;
    private GameInfo GI;
    private bool PreventAbilityCasting_ThroughButtonInput = false;
    private RampageController RC;
    private EntityMovement EM;
    private bool FXHasFired = false;
    private bool HasDoneVoiceLine = false;

    private List<ProjectileMovement> AllPM = new List<ProjectileMovement>();
    private List<ProjectileInfo> AllPI = new List<ProjectileInfo>();
    private List<bool> ProjectileHasReachedTarget = new List<bool>();

    private void Start()
    {
        GameObject.Find("GAME_MANAGER").TryGetComponent(out GI);
        gameObject.TryGetComponent(out BI);
        gameObject.TryGetComponent(out GIS);

        CheckIfAI();
        FindAltAbility();

        if (IsRampage)
        {
            RC = gameObject.AddComponent<RampageController>();
            gameObject.TryGetComponent(out EM);
            RC.SetRampageValues(this, RampageSpeedBonus, EM);
            TargetGround = true;
        }

        if(VoiceLineOnCast.Length > 0) { gameObject.TryGetComponent(out VLM); }

        VFX_Storage = GI.VFX_Storage.transform;
        if (VFX_Cursor == null) { VFX_Cursor = GI.GenericAbilityCursorVFX; }
        TargetType = TargetType.ToUpper();
        ActivationType = ActivationType.ToUpper();

        FloorLayer = LayerMask.GetMask("Terrain");
        CapPointLayer = LayerMask.GetMask("CapturePoint");
        EntityLayer = LayerMask.GetMask("Entity");
        UILayer = LayerMask.GetMask("UI");

        if (FX_Scale_IsRadius)
        {
            FX_Scale = GenericRadius;
        }

        for (int i = 0; i < Mod_Target.Length; i++) { Mod_Target_IsCasted.Add(false); }
        for (int i = Mod_DelayTime_Target.Count; i < Mod_Target.Length; i++)
        {
            Mod_DelayTime_Target.Add(GenericActivationDelayTime);
        }
        for (int i = Mod_ActiveTime_Target.Count; i < Mod_Target.Length; i++)
        {
            Mod_ActiveTime_Target.Add(Duration);
        }
        for (int i = Mod_Radius_Target.Count; i < Mod_Target.Length; i++)
        {
            Mod_Radius_Target.Add(GenericRadius);
        }

        for (int i = 0; i < Mod_Caster.Length; i++) { Mod_Caster_IsCasted.Add(false); }
        for (int i = Mod_DelayTime_Caster.Count; i < Mod_Caster.Length; i++)
        {
            Mod_DelayTime_Caster.Add(GenericActivationDelayTime);
        }
        for (int i = Mod_ActiveTime_Caster.Count; i < Mod_Caster.Length; i++)
        {
            Mod_ActiveTime_Caster.Add(Duration);
        }
        for (int i = Mod_Radius_Caster.Count; i < Mod_Caster.Length; i++)
        {
            Mod_Radius_Caster.Add(GenericRadius);
        }

        for (int i = KillVFXOnAbilityEnd_Target.Count; i < VFX_Target.Length; i++)
        {
            KillVFXOnAbilityEnd_Target.Add(true);
        }
        if (AutoCastWhenReady) { InvokeRepeating(nameof(CheckAutoCastStatus), Cooldown, Cooldown); }
    }

    public bool GetAbilityStatus()
    {
        return AbilityIsActive;
    }
    
    private void CheckAutoCastStatus()
    {
        if (CurrentAbilityCD <= 0)
        {
            ActivateAbilityExternally(transform, transform.position);
        }
    }

    public void ActivateAbilityExternally(Transform NewTarget, Vector3 NewImpactLoc)
    {
        FXHasFired = false;
        for (int i = 0; i <= AbilityRepeatRate; i++)
        {
            CurrentTarget = NewTarget; AbilityImpactPos = NewImpactLoc;
            if (!UseProjectile)
            {
                //KillAllAbilitiesExternally();
                ActivateAbility();
            }
            else
            {
                //KillAllAbilitiesExternally();
                ActivateAbilityAsProjectile();
            }
            HasDoneVoiceLine = true;
        }
        HasDoneVoiceLine = false;
    }

    public void KillAllAbilitiesExternally()
    {
        for (int i = 0; i < AppliedModsHolder_Target.Count; i++)
        {
            AppliedModsHolder_Target[i].SetActive(false);
            Destroy(AppliedModsHolder_Target[i]);
            AppliedModsHolder_Target.RemoveAt(i);

            Kill_VFX(false, true);
        }
    }

    private void FindAltAbility()
    {
        AbilityCaster[] tmp_AC = gameObject.GetComponents<AbilityCaster>();
        string DesiredLinkName;
        string CurrentName = AbilityName;

        if (IsAltAbility)
        {
            DesiredLinkName = CurrentName.Replace("_alt", "");
        }
        else
        {
            DesiredLinkName = CurrentName + "_alt";
        }

        foreach (AbilityCaster AC in tmp_AC)
        {
            string tmp_name = AC.AbilityName;
            if (DesiredLinkName == tmp_name)
            {
                LinkedAbility = AC;
                break;
            }
        }
    }

    private void CheckIfAI()
    {
        gameObject.TryGetComponent(out AI_Controller tmpAI);
        if (tmpAI != null)
        {
            IsAnAI = true;
        }
        else { IsAnAI = false; }
    }

    private void Update()
    {
        UpdateAbilityCD();
    }

    private void UpdateAbilityCD()
    {
        try
        {
            if (CurrentAbilityCD > 0) { CurrentAbilityCD -= Time.deltaTime; LinkedButton.interactable = false; } else { LinkedButton.interactable = true; }
        }
        catch
        {
            //more cutscene stuff..
        }

        StandardAbilityInputCheck();

        if(UseProjectile)
        {
            for(int i = 0; i < AllPM.Count; i++)
            {
                if(ProjectileCanCollide && AllPI[i].IHaveCollided)
                {
                    CurrentAbilityActiveTime = 0;
                    for (int mod = 0; mod < Mod_Target.Length; mod++)
                    {
                        ApplyAModifier(AppliedModsHolder_Target, AppliedModsHolder_TargetLifetime, Mod_Target[mod], Mod_Radius_Target[mod], AllPI[i].CollidedObj.transform, AllPM[i].transform.position, Mod_ActiveTime_Target[mod]);
                        CreateStandardVFXFromArray(VFX_Target, "VFX_Ability; Target FX for: ", AllPI[i].CollidedObj.transform.position, AllPI[i].CollidedObj.transform);
                        Mod_Target_IsCasted[mod] = true;
                    }

                    for (int mod = 0; mod < Mod_Caster.Length; mod++)
                    {
                        ApplyAModifier(AppliedModsHolder_Caster, AppliedModsHolder_CasterLifetime, Mod_Caster[mod], Mod_Radius_Caster[mod], transform, AllPM[i].transform.position, Mod_ActiveTime_Caster[mod]);
                        CreateStandardVFXFromArray(VFX_Caster, "VFX_Ability; Caster FX for: ", transform.position, transform);
                        Mod_Caster_IsCasted[mod] = true;
                    }

                    Destroy(AllPM[i].gameObject);
                    AllPM.RemoveAt(i);
                    AllPI.RemoveAt(i);
                }
                else if(AllPM[i].GetProgress())
                {
                    CurrentAbilityActiveTime = 0;
                    for (int mod = 0; mod < Mod_Target.Length; mod++)
                    {
                        ApplyAModifier(AppliedModsHolder_Target, AppliedModsHolder_TargetLifetime, Mod_Target[mod], Mod_Radius_Target[mod], CurrentTarget, AllPM[i].transform.position, Mod_ActiveTime_Target[mod]);
                        CreateStandardVFXFromArray(VFX_Target, "VFX_Ability; Target FX for: ", AllPM[i].transform.position, CurrentTarget);
                        Mod_Target_IsCasted[mod] = true;
                    }

                    for (int mod = 0; mod < Mod_Caster.Length; mod++)
                    {
                        ApplyAModifier(AppliedModsHolder_Caster, AppliedModsHolder_CasterLifetime, Mod_Caster[mod], Mod_Radius_Caster[mod], transform, AllPM[i].transform.position, Mod_ActiveTime_Caster[mod]);
                        CreateStandardVFXFromArray(VFX_Caster, "VFX_Ability; Caster FX for: ", transform.position, transform);
                        Mod_Caster_IsCasted[mod] = true;
                    }

                    Destroy(AllPM[i].gameObject);
                    AllPM.RemoveAt(i);
                    AllPI.RemoveAt(i);
                }
            }
        }
    }

    private void StandardAbilityInputCheck()
    {
        if (!AbilityIsActive)
        {
            CheckAbilityInput();
        }
        else
        {
            CheckForAbilityCooldown();
            ApplyTargetMods();
            ApplyCasterMods();            
        }

        if (UseProjectile)
        {
            CheckProjectileModDurations();
        }
    }

    private void CheckProjectileModDurations()
    {
        for (int i = 0; i < AppliedModsHolder_Target.Count; i++)
        {
            for (int mod = 0; mod < Mod_DelayTime_Target.Count; mod++)
            {
                if (AppliedModsHolder_TargetLifetime[i] - Mod_DelayTime_Target[mod] >= Mod_ActiveTime_Target[mod])
                {
                    if (AppliedModsHolder_Target[i] != null)
                    {
                        AppliedModsHolder_Target[i].SetActive(false);
                        AppliedModsHolder_TargetLifetime.RemoveAt(i);
                        Destroy(AppliedModsHolder_Target[i]);
                        AppliedModsHolder_Target.RemoveAt(i);
                    }
                    else
                    {
                        AppliedModsHolder_Target.RemoveAt(i);
                        AppliedModsHolder_TargetLifetime.RemoveAt(i);
                    }

                    Kill_VFX(false, true);
                }
            }
        } 

        for (int i = 0; i < AppliedModsHolder_Caster.Count; i++)
        {
            for (int mod = 0; mod < Mod_DelayTime_Target.Count; mod++)
            {
                if (AppliedModsHolder_CasterLifetime[i] - Mod_DelayTime_Caster[mod] >= Mod_ActiveTime_Caster[mod])
                {
                    if (AppliedModsHolder_Caster[i] != null)
                    {
                        AppliedModsHolder_Caster[i].SetActive(false);
                        AppliedModsHolder_CasterLifetime.RemoveAt(i);
                        Destroy(AppliedModsHolder_Caster[i]);
                        AppliedModsHolder_Caster.RemoveAt(i);
                    }
                    else
                    {
                        AppliedModsHolder_Caster.RemoveAt(i);
                        AppliedModsHolder_CasterLifetime.RemoveAt(i);
                    }

                    Kill_VFX(false, true);
                }
            }
        }

        for (int i = 0; i < AppliedModsHolder_TargetLifetime.Count; i++) { AppliedModsHolder_TargetLifetime[i] += Time.deltaTime; }
        for (int i = 0; i < AppliedModsHolder_CasterLifetime.Count; i++) { AppliedModsHolder_CasterLifetime[i] += Time.deltaTime; }
        CurrentAbilityActiveTime += Time.deltaTime;
    }

    private void CheckForAbilityCooldown()
    {
        CurrentAbilityActiveTime += Time.deltaTime; //Tracks how long the ability has been going for, resets on casting the ability agian to 0

        if (CurrentAbilityCD <= 0)
        {//EDIT: 8/8/21 this is probably fixed, IF ABILITY CASTING IS WEIRD CHECK HERE PLEASE! ALSO CHECK HERE FOR "YOUR ABILITY IS READY" PROMPT
            //FXHasFired = false;
            AbilityIsActive = false;
            CurrentAbilityCD = 0;
            CurrentAbilityActiveTime = 0;
        }
    }

    private void ApplyTargetMods()
    {
        for (int i = 0; i < Mod_Target.Length; i++)
        {
            if (!Mod_Target_IsCasted[i])
            {
                if (CurrentAbilityActiveTime >= Mod_DelayTime_Target[i])
                {
                    Transform NewTarget = CurrentTarget;
                    if (TargetGround) { NewTarget = null; }

                    if (!TargetGround)
                    {
                        ApplyAModifier(AppliedModsHolder_Target, AppliedModsHolder_TargetLifetime, Mod_Target[i], Mod_Radius_Target[i], NewTarget, CurrentTarget.position, Mod_ActiveTime_Target[i]);
                        CreateStandardVFXFromArray(VFX_Target, "VFX_Ability; Target FX for: ", CurrentTarget.position, CurrentTarget);
                    }
                    else
                    {
                        ApplyAModifier(AppliedModsHolder_Target, AppliedModsHolder_TargetLifetime, Mod_Target[i], Mod_Radius_Target[i], NewTarget, AbilityImpactPos, Mod_ActiveTime_Target[i]);
                        if (FX_TargetGround)
                        {
                            CreateStandardVFXFromArray(VFX_Target, "VFX_Ability; Target FX for: ", AbilityImpactPos, VFX_Storage);
                        }
                        else
                        {
                            CreateStandardVFXFromArray(VFX_Target, "VFX_Ability; Target FX for: ", AbilityImpactPos, CurrentTarget);
                        }
                    }

                    Mod_Target_IsCasted[i] = true;
                }
            }
            else
            {
                if (CurrentAbilityActiveTime - Mod_DelayTime_Target[i] >= Mod_ActiveTime_Target[i])
                {// Check here if things need to be disabled |||EDIT: 8/8/21, I think this is fixed :)
                    if (AppliedModsHolder_Target.Count > 0)
                    {
                        if (AppliedModsHolder_Target[i] != null)
                        {
                            AppliedModsHolder_Target[i].SetActive(false);
                            Destroy(AppliedModsHolder_Target[i]);
                            AppliedModsHolder_Target.RemoveAt(i);
                        }
                        else
                        {
                            AppliedModsHolder_Target.RemoveAt(i);
                        }
                    }
                    else
                    {
                        SetActiveStateForOtherObjects(false);
                    }
                    Kill_VFX(false, true);
                }
            }
        }
    }

    private void ApplyCasterMods()
    {
        for (int i = 0; i < Mod_Caster.Length; i++)
        {
            try
            {
                if (!Mod_Target_IsCasted[i])
                {
                    if (CurrentAbilityActiveTime >= Mod_DelayTime_Caster[i])
                    {
                        ApplyAModifier(AppliedModsHolder_Caster, AppliedModsHolder_CasterLifetime, Mod_Caster[i], Mod_Radius_Caster[i], transform, transform.position, Mod_ActiveTime_Caster[i]);

                        if (FX_TargetGround)
                        {
                            CreateStandardVFXFromArray(VFX_Caster, "VFX_Ability; Caster FX for: ", transform.position, transform);
                        }
                        else
                        {
                            CreateStandardVFXFromArray(VFX_Caster, "VFX_Ability; Caster FX for: ", transform.position, VFX_Storage);
                        }

                        Mod_Caster_IsCasted[i] = true;
                    }
                }
                else
                {
                    if (CurrentAbilityActiveTime - Mod_DelayTime_Caster[i] >= Mod_ActiveTime_Caster[i])
                    {// Check here if things need to be disabled |||EDIT: 8/8/21, I think this is fixed :)
                        if (AppliedModsHolder_Caster.Count > 0)
                        {
                            AppliedModsHolder_Caster[i].SetActive(false);
                            Destroy(AppliedModsHolder_Caster[i]);
                            AppliedModsHolder_Caster.RemoveAt(i);
                        }
                        Kill_VFX(false, true);
                    }
                }
            }
            catch
            {
                Debug.LogWarning("ERROR! In AbilityCaster/ApplyCasterMods. Cannot apply mods!");
            }
        }
    }

    private void CreateStandardVFXFromArray(GameObject[] FX_List, string NameHelpText, Vector3 FX_Pos, Transform ParentObj)
    {
        if (FireFXOnce && !FXHasFired || !FireFXOnce)
        {
            foreach (GameObject tmp_obj in FX_List)
            {
                GameObject tmp_VFX = Instantiate(tmp_obj, ParentObj);
                tmp_VFX.transform.localScale = new Vector3(FX_Scale, FX_Scale, FX_Scale);
                tmp_VFX.transform.position = FX_Pos;
                tmp_VFX.name = NameHelpText + AbilityName;
                ActiveVFXObj.Add(tmp_VFX);
            }
            FXHasFired = true;
        }
    }

    private bool CheckAbility()
    {
        if (ActivationType == "TARGETED")
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (TargetGround)
            {
                if (Physics.Raycast(ray, out hit, 1000, FloorLayer))
                {
                    CurrentTarget = hit.collider.transform;
                    AbilityImpactPos = hit.point;
                    return true;
                }
            }
            else if (Physics.Raycast(ray, out hit, 1000, EntityLayer))
            {
                CurrentTarget = hit.collider.transform;
                AbilityImpactPos = hit.point;
                return true;
            }
        }
        else
        {
            CurrentTarget = transform;
            AbilityImpactPos = transform.position;
            return true;
        }

        return false;
    }

    private bool CheckTargetAbility()
    {
        if (TargetType == "NONE" || TargetGround)
        {
            return true;
        }
        else if (TargetType == "ALLIED" || TargetType == "ENEMY" || TargetType == "ALL" || TargetType == "OWN")
        {
            return RaycastForEntityAndOwner(TargetType);
        }
        else
        {
            Debug.LogError("ERROR: In AbilityCaster/CheckTargetAbility | 'TargetType = " + TargetType + "' Is Unknown! Defaulting to None Target.");
            return true;
        }
    }

    private bool CompareTargetToTargetType(BasicInfo hitBI)
    {
        if (hitBI != null)
        {
            if (TargetType == "ALL" || TargetGround)
            {
                return true;
            }
            else if ((TargetType == "ALLIED" || TargetType == "OWN") && hitBI.OwnedByPlayer == BI.OwnedByPlayer)
            {
                return true;
            }
            else if ((TargetType == "ENEMY") && (hitBI.OwnedByPlayer != BI.OwnedByPlayer) && (hitBI.OwnedByPlayer != -1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private bool RaycastForEntityAndOwner(string DesiredTarget)
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        Physics.Raycast(ray, out hit, 1000, EntityLayer);
        hit.collider.gameObject.TryGetComponent(out BasicInfo hitBI);

        if (hitBI != null)
        {
            if (DesiredTarget == "ALL" || TargetGround)
            {
                return true;
            }
            else if ((DesiredTarget == "ALLIED" || DesiredTarget == "OWN") && hitBI.OwnedByPlayer == BI.OwnedByPlayer)
            {
                return true;
            }
            else if ((DesiredTarget == "ENEMY") && (hitBI.OwnedByPlayer != BI.OwnedByPlayer) && (hitBI.OwnedByPlayer != -1))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    private void CheckAbilityInput()
    {
        if (Input.GetKeyDown(CancelAbilityKey))
        {
            if (AbilityIsActiveThroughButton) { AbilityIsActiveThroughButton = false; GI.UITimeout = false; }
            HasCanceledAbility = true; Kill_VFX(true, false);
        }

        bool DoAbility = false;
        if (GIS != null)
        {
            if (GIS.GetSelectedStatus() == "Selected" && CurrentAbilityCD <= 0 && !HasCanceledAbility && !IsAnAI) //Necessary for ability to be allowed
            {
                Ray ray;
                RaycastHit hit;
                bool ActivatedThroughRampageSpecial = false;
                if(IsRampage && Input.GetKey(KeyCode.LeftControl))
                {
                    if (Input.GetMouseButtonUp(1))
                    {
                        ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        Physics.Raycast(ray, out hit, 1000, FloorLayer);//~UILayer);
                        ActivatedThroughRampageSpecial = true;
                    }
                }

                if (Input.GetKey(AbilityHK) || AbilityIsActiveThroughButton || ActivatedThroughRampageSpecial)//Player wants to cast bc of input
                {
                    DoAbility = true;

                    if (!AbilityIsActiveThroughButton)
                    {
                        if (IsAltAbility)
                        {
                            if (!Input.GetKey(KeyCode.LeftShift) || ActivatedThroughRampageSpecial)
                            {
                                DoAbility = false;
                                Kill_VFX(true);
                            }
                        }
                        else if (Input.GetKey(KeyCode.LeftShift) || ActivatedThroughRampageSpecial)
                        {
                            DoAbility = false;
                            Kill_VFX(true);
                        }
                    }

                    if (DoAbility && !ActivatedThroughRampageSpecial)
                    {
                        if (ActivationType == "TARGETED")
                        {
                            if (VFX_Cursor_Current == null)
                            {
                                VFX_Cursor_Current = Instantiate(VFX_Cursor, VFX_Storage);
                                VFX_Cursor_Current.name = "VFX_Cursor; Casting Ability: " + AbilityName;
                                VFX_Cursor_Current.transform.localScale = new Vector3(GenericRadius * VFX_Cursor_SizeMultiplier, GenericRadius * VFX_Cursor_SizeMultiplier, GenericRadius * VFX_Cursor_SizeMultiplier);
                            }
                            else
                            {
                                ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                                Physics.Raycast(ray, out hit, 1000, FloorLayer);//~UILayer);

                                VFX_Cursor_Current.transform.position = hit.point + new Vector3(0, 1, 0);
                                //print(VFX_Cursor_Current.transform.position);
                            }
                        }
                    }
                }

                bool CastAbility = false;
                if (AbilityIsActiveThroughButton)
                {
                    if (ActivationType == "TARGETED")
                    {
                        if (Input.GetMouseButtonUp(0) && !PreventAbilityCasting_ThroughButtonInput)
                        {
                            CastAbility = true;
                            Kill_VFX();
                        }
                    }
                    else
                    {
                        CastAbility = true;
                    }
                }
                else if (Input.GetKeyUp(AbilityHK) || ActivatedThroughRampageSpecial)//(GIS.GetSelectedStatus() == "Selected") && (!IsAnAI) && CurrentAbilityCD <= 0 && !HasCanceledAbility)
                {
                    CastAbility = true;
                    if (IsAltAbility)
                    {
                        if (!Input.GetKey(KeyCode.LeftShift))
                        {
                            CastAbility = false;
                        }
                    }
                    else if (Input.GetKey(KeyCode.LeftShift))
                    {
                        CastAbility = false;
                    }

                    Kill_VFX();
                }

                if (CheckAbility() && CastAbility)
                {
                    if (ActivationType == "TIMED")
                    {
                        AbilityImpactPos = transform.position;
                        GI.UITimeout = false;
                        
                        for (int i = 0; i < AbilityRepeatRate+1; i++)
                        {
                            if (!UseProjectile)
                            {
                                ActivateAbility();
                            }
                            else
                            {
                                ActivateAbilityAsProjectile();
                            }
                        }
                    }
                    else
                    {
                        if (CheckTargetAbility() || TargetGround)
                        {
                            for (int i = 0; i < AbilityRepeatRate+1; i++)
                            {
                                if (IsRampage)
                                {
                                    EM.SetMoveDestination(AbilityImpactPos);
                                    RC.EnableRampage();
                                }
                                else
                                {
                                    if (!UseProjectile)
                                    {
                                        ActivateAbility();
                                    }
                                    else
                                    {
                                        ActivateAbilityAsProjectile();
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (HasCanceledAbility)
            {
                if (Input.GetKeyDown(AbilityHK) || Input.GetKeyUp(AbilityHK))
                {
                    HasCanceledAbility = false;
                }
            }
        }
    }

    private void CreateNewProjectile()
    {
        GameObject tmpProjectile = Instantiate(ProjectilePrefab);
        Vector3 RandomOffset = new Vector3(Random.Range(-ProjectileRandomSpawnOffset[0], ProjectileRandomSpawnOffset[0]), Random.Range(-ProjectileRandomSpawnOffset[1], ProjectileRandomSpawnOffset[1]), Random.Range(-ProjectileRandomSpawnOffset[2], ProjectileRandomSpawnOffset[2]));
        Vector3 InitialOffset = ProjectileSpawnOffset + RandomOffset;
        if (!ProjectileSpawnAtTarget) { InitialOffset = new Vector3(); }

        Vector3 tmpProjectileOffset = InitialOffset + new Vector3(Random.Range(-ProjectileRandomSpawnOffset[0], ProjectileRandomSpawnOffset[0]), Random.Range(-ProjectileRandomSpawnOffset[1], ProjectileRandomSpawnOffset[1]), Random.Range(-ProjectileRandomSpawnOffset[2], ProjectileRandomSpawnOffset[2]));
        
        if (ProjectileSpawnAtTarget) 
        {            
            if (TargetGround) 
            {
                tmpProjectile.transform.position = AbilityImpactPos + tmpProjectileOffset; 
            }
            else
            {
                tmpProjectile.transform.position = CurrentTarget.transform.position + tmpProjectileOffset;
            }
        }
        else
        {
            tmpProjectile.transform.position = transform.position + tmpProjectileOffset;
        }

        foreach (GameObject tmp_obj in ProjectileVFX)
        {
            GameObject tmp_VFX = Instantiate(tmp_obj, tmpProjectile.transform);
            tmp_VFX.transform.localScale = new Vector3(FX_Scale, FX_Scale, FX_Scale);
            tmp_VFX.transform.position = tmpProjectile.transform.position;
            tmp_VFX.name = "ProjectileVFX" + Time.time + ": " + AbilityName;
        }

        ProjectileMovement tmpPM = tmpProjectile.AddComponent<ProjectileMovement>();

        if (ProjectileIsHoming)
        {
            CurrentTarget.TryGetComponent(out BasicInfo tmpTargBI);
            if (!CompareTargetToTargetType(tmpTargBI))
            {
                CurrentTarget = null;
            }
        }
        else
        {
            CurrentTarget = null;
        }

        InitialOffset[1] = 0; RandomOffset[1] = 0;

        foreach (GameObject tmpVFX in ProjectileVFXWarning)
        {
            Instantiate(tmpVFX, AbilityImpactPos + InitialOffset + RandomOffset, new Quaternion(), null);
        }

        tmpPM.SetupProjectile(CurrentTarget,AbilityImpactPos + InitialOffset + RandomOffset, ProjectileSpeed + Random.Range(-ProjectileRandomSpeedOffset, ProjectileRandomSpeedOffset), ProjectileDesiredColDistance, ProjectileUseAnimCurve, ProjectileMovementCurve, ProjectileACMaxY, transform.position);
        AllPI.Add(tmpProjectile.AddComponent<ProjectileInfo>());
        AllPM.Add(tmpPM);
    }

    private void Kill_VFX(bool IsCursor = true, bool IsAbility = false)
    {
        if (IsCursor)
        {
            if (VFX_Cursor_Current != null)
            {
                Destroy(VFX_Cursor_Current);
            }
        }

        if (IsAbility && ActiveVFXObj != null && AppliedModsHolder_Target != null && AppliedModsHolder_Caster != null && KillVFXOnAbilityEnd_Target != null)
        {
            if (AppliedModsHolder_Target.Count == 0 && AppliedModsHolder_Caster.Count == 0)
            {
                for (int i = 0; i < ActiveVFXObj.Count; i++)
                {
                    if (KillVFXOnAbilityEnd_Target.Count > i)
                    {
                        if (KillVFXOnAbilityEnd_Target[i])
                        {
                            if (ActiveVFXObj[i] != null)
                            {
                                Destroy(ActiveVFXObj[i]);
                            }
                        }
                    }
                    else
                    {
                        if (ActiveVFXObj[i] != null)
                        {
                            ActiveVFXObj.RemoveAt(i);
                            //Destroy(ActiveVFXObj[i]);
                        }
                    }
                }

                ActiveVFXObj.Clear();
            }
        }
    }

    public void SetIsAI()
    {
        IsAnAI = true;
    }

    private void SetActiveStateForOtherObjects(bool state)
    {
        foreach(GameObject obj in ActivateObjOnActivate) { obj.SetActive(state); }
    }

    private void ActivateAbility()
    {
        ResetAbilityCD();
        SetActiveStateForOtherObjects(true);

        if (!IsAnAI)
        {
            GI.UITimeout = false;
        }

        AbilityIsActive = true;
        AbilityIsActiveThroughButton = false;

        if (LinkedAbility != null) { LinkedAbility.ResetAbilityCD(); }
        if (EntityToSpawn != null)
        {
            GameObject tmpEntity = Instantiate(EntityToSpawn);
            tmpEntity.transform.localPosition = AbilityImpactPos;
            if (EntitySpawnAsChild) { tmpEntity.transform.parent = transform; }
        }

        if(VoiceLineOnCast.Length > 0 && !HasDoneVoiceLine)
        {
            VLM.PlaySpecificVoiceLine(VoiceLineOnCast[Random.Range(0, VoiceLineOnCast.Length)], true, false);
        }

        if (IsRampage) { PlayDelayedRampageLine(); }
    }

    private void ActivateAbilityAsProjectile()
    {
        ResetAbilityCD();
        SetActiveStateForOtherObjects(true);

        if (!IsAnAI)
        {
            GI.UITimeout = false;
        }

        //AbilityIsActive = true;
        //AbilityIsActiveThroughButton = false;

        if (LinkedAbility != null) { LinkedAbility.ResetAbilityCD(); }
        if (EntityToSpawn != null)
        {
            GameObject tmpEntity = Instantiate(EntityToSpawn);
            tmpEntity.transform.localPosition = AbilityImpactPos;
            if (EntitySpawnAsChild) { tmpEntity.transform.parent = transform; }
        }

        if (VoiceLineOnCast.Length > 0)
        {
            VLM.PlaySpecificVoiceLine(VoiceLineOnCast[Random.Range(0, VoiceLineOnCast.Length)], false, false);
        }

        if (IsRampage) { PlayDelayedRampageLine(); }
        CreateNewProjectile();
    }

    private void PlayDelayedRampageLine()
    {
        if (VoiceLineOnRampageDuration.Length > 0)
        {
            VLM.PlaySpecificVoiceLine(VoiceLineOnRampageDuration[Random.Range(0, VoiceLineOnRampageDuration.Length)], false, true);
        }
    }

    private void ResetAbilityCD()
    {
        CurrentAbilityCD = Cooldown;
        CurrentAbilityActiveTime = 0;
        for (int i = 0; i < Mod_Target_IsCasted.Count; i++) { Mod_Target_IsCasted[i] = false; }
        for (int i = 0; i < Mod_Caster_IsCasted.Count; i++) { Mod_Caster_IsCasted[i] = false; }
    }

    private void ApplyAModifier(List<GameObject> AddToList, List<float> AddToListLifetime, GameObject ApplyMod, float Size, Transform Target, Vector3 ModPos, float AutoDestTime)
    {
        GameObject NewMod = Instantiate(ApplyMod, ModPos, new Quaternion(), Target);
        NewMod.TryGetComponent(out ModifierApplier NewMod_MA);
        AutoDestroyObject tmpDest = NewMod.AddComponent<AutoDestroyObject>();
        tmpDest.ResetDestroyTime(AutoDestTime);
        NewMod_MA.TakeDamageFromTarget = BI.EBPs.PositionInLvlHierarchy;
        if (!TargetGround) { NewMod_MA.ApplyActiveObject(Target.gameObject); }
        NewMod_MA.OwnedByPlayer = BI.OwnedByPlayer;
        if (TargetTypeOverrideModifier) { NewMod_MA.DesiredTarget = TargetType; }
        NewMod.transform.localScale = new Vector3(Size, Size, Size);
        NewMod.transform.position = ModPos;
        NewMod.name = "Modifier: " + AbilityName;
        Actions.OnAddNewModifier.InvokeAction(NewMod_MA);
        AddToList.Add(NewMod);
        AddToListLifetime.Add(0);
    }

    private void AllowForButtonCasting()
    {
        PreventAbilityCasting_ThroughButtonInput = false;
    }

    public void PreventButtonCasting(float TimeUntilReset = 0)
    {
        PreventAbilityCasting_ThroughButtonInput = true;
        Invoke(nameof(AllowForButtonCasting), TimeUntilReset);
    }

    public void DisableAbilityCasting() { AbilityIsActive = false; Kill_VFX(); }
}
