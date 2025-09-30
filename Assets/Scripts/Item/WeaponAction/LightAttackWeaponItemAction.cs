using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Action/ Weapon Actions/ Light Attack Action")]
public class LightAttackWeaponItemAction : WeaponItemAction
{
    // Right Hand
    [Header("Lights Attack")]
    [SerializeField] string light_Attack_01 = "Main_Light_Attack_01";
    [SerializeField] string light_Attack_02 = "Main_Light_Attack_02";
    [SerializeField] string light_Attack_03 = "Main_Light_Attack_03";
    [SerializeField] string light_Attack_04 = "Main_Light_Attack_04";

    [Header("Running Attack")]
    [SerializeField] string run_Attack_01 = "Main_Run_Attack_01";

    [Header("Rolling Attack")]
    [SerializeField] string roll_Attack_01 = "Main_Roll_Attack_01";

    [Header("BackStep Attack")]
    [SerializeField] string backstep_Attack_01 = "Main_BackStep_Attack_01";

    // Two Hand
    [Header("Lights Attack")]
    [SerializeField] string th_light_Attack_01 = "TH_Light_Attack_01";
    [SerializeField] string th_light_Attack_02 = "TH_Light_Attack_02";
    [SerializeField] string th_light_Attack_03 = "TH_Light_Attack_03";
    [SerializeField] string th_light_Attack_04 = "TH_Light_Attack_04";

    [Header("Running Attack")]
    [SerializeField] string th_run_Attack_01 = "TH_Run_Attack_01";

    [Header("Rolling Attack")]
    [SerializeField] string th_roll_Attack_01 = "TH_Roll_Attack_01";

    [Header("BackStep Attack")]
    [SerializeField] string th_backstep_Attack_01 = "TH_BackStep_Attack_01";


    public override void AttpemtToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        base.AttpemtToPerformAction(playerPerformingAction, weaponPerformingAction);

        if (!playerPerformingAction.IsOwner)
            return;

        if (playerPerformingAction.playerCombatManager.isUsingItem)
            return;

        if (playerPerformingAction.playerNetworkManager.currentStamina.Value <=0 )
            return;

        if (!playerPerformingAction.characterLocomotionManager.isGrounded)
            return;

        if(playerPerformingAction.IsOwner)
        {
            playerPerformingAction.playerNetworkManager.isAttacking.Value = true;
        }

        // If Springting, perform running attack
        if(playerPerformingAction.characterNetworkManager.isSprinting.Value)
        {
            PerformRunningtAttack(playerPerformingAction,weaponPerformingAction);
            return;
        }

        // If Rolling, perform rolling attack
        if (playerPerformingAction.characterCombatManager.canPerformRollingAttack)
        {
            PerformRollingAttack(playerPerformingAction, weaponPerformingAction);
            return;
        }

        // if Backstep, perform backstep attack
        if (playerPerformingAction.characterCombatManager.canPerformBackStepAttack)
        {
            PerformBackStepAttack(playerPerformingAction, weaponPerformingAction);
            return;
        }

        playerPerformingAction.characterCombatManager.AttemptCriticalAttack();

        PerformLightAttack(playerPerformingAction,weaponPerformingAction);
    }
    private void PerformLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        if(playerPerformingAction.playerNetworkManager.isTwoHandingWeapon.Value)
        {
            PerformTwoHandLightAttack(playerPerformingAction, weaponPerformingAction);
        }
        else
        {
            PerformMainHandLightAttack(playerPerformingAction, weaponPerformingAction);
        }
    }
    private void PerformMainHandLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        // if i am attacking, and can do combo => perform combo attack
        if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

            if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == light_Attack_01)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.LightAttack02, light_Attack_02, true);
            }
            else if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == light_Attack_02)
            {

                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.LightAttack03, light_Attack_03, true);
            }
            else if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == light_Attack_03)
            {

                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.LightAttack04, light_Attack_04, true);
            }
            else
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.LightAttack01, light_Attack_01, true);
            }
        }
        // otherwise, perform a regular attack
        else if (!playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.LightAttack01, light_Attack_01, true);
        }
        
    }
    private void PerformTwoHandLightAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        Debug.Log("Is Two Handing: " + playerPerformingAction.playerNetworkManager.isTwoHandingWeapon.Value);

        // if i am attacking, and can do combo => perform combo attack
        if (playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon && playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

            if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == th_light_Attack_01)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.LightAttack02, th_light_Attack_02, true);
            }
            else if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == th_light_Attack_02)
            {

                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.LightAttack03, th_light_Attack_03, true);
            }
            else if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == th_light_Attack_03)
            {

                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.LightAttack04, th_light_Attack_04, true);
            }
            else
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.LightAttack01, th_light_Attack_01, true);
            }
        }
        // otherwise, perform a regular attack
        else if (!playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.LightAttack01, th_light_Attack_01, true);
        }
    }
    private void PerformRunningtAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        if(playerPerformingAction.playerNetworkManager.isTwoHandingWeapon.Value)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.RunningAttack01, th_run_Attack_01, true);
        }
        else
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.RunningAttack01, run_Attack_01, true);
        }
    }
    private void PerformRollingAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        playerPerformingAction.playerCombatManager.canPerformRollingAttack = false;

        if (playerPerformingAction.playerNetworkManager.isTwoHandingWeapon.Value)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.RollingAttack01, th_roll_Attack_01, true);
        }
        else
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.RollingAttack01, roll_Attack_01, true);
        }
    }
    private void PerformBackStepAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        playerPerformingAction.playerCombatManager.canPerformBackStepAttack = false;


        if (playerPerformingAction.playerNetworkManager.isTwoHandingWeapon.Value)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.BackstepAttack01, th_backstep_Attack_01, true);
        }
        else
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.BackstepAttack01, backstep_Attack_01, true);
        }

        
    } 
   
}
