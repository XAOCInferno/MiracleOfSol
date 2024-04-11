using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPlayer : MonoBehaviour
{
    public Animator AnimMaster;

    private bool IsAiming = false;
    private bool IsMoving = false;
    private bool AbilityCastGrenade = false;
    private bool IsShooting = false;
    private bool IsInMelee = false;
    private bool IsDoingSpecialAttack = false;
    private float NumberMeleeAnimations = 5;
    private float NumberSpecialAttacks = 2; 


    public void SetAnimationState(bool[] NewAim = null, bool[] NewMove = null, bool[] NewAbilityCastGrenade = null, bool[] NewShooting = null, bool[] NewMelee = null, bool[] NewSpecialAttack = null)
    {
        if (AnimMaster != null)
        {
            if (NewAim != null) { IsAiming = NewAim[0]; }
            if (NewMove != null) { IsMoving = NewMove[0]; }
            if (NewAbilityCastGrenade != null) { AbilityCastGrenade = NewAbilityCastGrenade[0]; }
            if (NewShooting != null) { IsShooting = NewShooting[0]; }
            if (NewMelee != null) { IsInMelee = NewMelee[0]; }
            if (NewSpecialAttack != null) { IsDoingSpecialAttack = NewSpecialAttack[0]; }

            UpdateAnimationState();
        }
    }

    private void UpdateAnimationState()
    {
        if (AnimMaster != null)
        {
            AnimMaster.SetBool("IsMoving", IsMoving);
            AnimMaster.SetBool("IsAiming", IsAiming);
            AnimMaster.SetBool("IsShooting", IsShooting);
            AnimMaster.SetBool("AbilityCastGrenade", AbilityCastGrenade);
            AnimMaster.SetBool("IsInMelee", IsInMelee);
            AnimMaster.SetBool("IsDoingSpecialAttack", IsDoingSpecialAttack);

            float tmpRdmSeed = 0;
            if (AnimMaster.GetCurrentAnimatorStateInfo(1).IsName("IdleLooping"))
            {
                if (IsDoingSpecialAttack)
                {
                    tmpRdmSeed = ((int)Random.Range(0, NumberSpecialAttacks)) + 0.1f;
                }
                else if (IsInMelee)
                {
                    tmpRdmSeed = ((int)Random.Range(0, NumberMeleeAnimations)) + 0.1f;
                }
                AnimMaster.SetFloat("AnimationSeed", tmpRdmSeed);
                //if (AnimMaster.GetFloat("AnimationSeed") != 0) { print(AnimMaster.GetFloat("AnimationSeed")); }
            }
        }
    }


    //Test function used to test new animations
    /*private void Update() 
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SetAnimationState(null, new bool[1] { true }, null);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            SetAnimationState(new bool[1] { true }, null, null);
        }
        if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            SetAnimationState(null, null, new bool[1] { true });
        }
        if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            SetAnimationState(new bool[1] { false }, null, null, new bool[1] { false }, new bool[1] { true }, null);
        }
    }*/
}
