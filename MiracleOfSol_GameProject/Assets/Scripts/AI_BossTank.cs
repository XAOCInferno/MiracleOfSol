using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_BossTank : MonoBehaviour
{
    public Transform OnSpawnMoveToLocation;
    public Transform OnInitialDieMoveToLocation;
    public Transform[] DesiredMoveLocsStartingArea;
    public GameObject ActivateOnRetreat;

    public float HealthPercentForEndFirstStage = 0.5f;
    private EntityMovement EM;
    private Combat selfCombat;
    private Health selfHealth;
    private BasicInfo BI;
    private bool FirstStageDone = false;
    private Vector3 DesiredPos;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.TryGetComponent(out EM);
        gameObject.TryGetComponent(out selfHealth);
        gameObject.TryGetComponent(out BI);
        gameObject.TryGetComponent(out selfCombat);

        selfHealth.CanPassiveHeal = false;
        selfHealth.CanDie = false;
    }

    public void ActivateTankBoss(Transform NewSpawnMove, Transform NewDieMove, Transform[] AllMoveLocsInStartArea, GameObject NewRetreatActivate, bool ForceSecondPhase)
    {
        if (!ForceSecondPhase)
        {
            OnSpawnMoveToLocation = NewSpawnMove;
            OnInitialDieMoveToLocation = NewDieMove;
            DesiredMoveLocsStartingArea = AllMoveLocsInStartArea;
            ActivateOnRetreat = NewRetreatActivate;

            DesiredPos = OnSpawnMoveToLocation.position;

            Invoke(nameof(ForceMoveToDesiredPos), 4.5f + Random.Range(-1f, 2f));
        }
        else
        {
            gameObject.TryGetComponent(out AI_BossTankSecondPhase tmpSecondPhase);
            tmpSecondPhase.ActivateTankBoss(AllMoveLocsInStartArea);
            Destroy(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!FirstStageDone)
        {
            if(selfHealth.GetCurrentArmour_AsPercentOfMax() <= HealthPercentForEndFirstStage)
            {
                FirstStageDone = true;
                DesiredPos = OnInitialDieMoveToLocation.position;
                EM.SetAttackTarget();
                selfCombat.Target = null;
                selfCombat.DesiredTarget = null;
                selfCombat.CombatIsEnabled = false;
                ActivateOnRetreat.SetActive(true);
                ForceMoveToDesiredPos();
                selfHealth.UpdateCurrentHealth(-1, true, BI.EBPs.MaxHP / 2, BI.EBPs.MaxArmour / 2);
                selfHealth.CanTakeHealthDamage = false;
            }
        }
    }

    private void ForceMoveToDesiredPos()
    {
        EM.SetMoveDestination(DesiredPos);

        if (!FirstStageDone)
        {
            DesiredPos = DesiredMoveLocsStartingArea[Random.Range(0, DesiredMoveLocsStartingArea.Length)].position;
            Invoke(nameof(ForceMoveToDesiredPos), 4.5f + Random.Range(-1f, 2f));
        }
        else
        {
            Invoke(nameof(TankBossKillSelf), 10);
        }
    }

    private void TankBossKillSelf()
    {
        Destroy(gameObject);
    }
}
