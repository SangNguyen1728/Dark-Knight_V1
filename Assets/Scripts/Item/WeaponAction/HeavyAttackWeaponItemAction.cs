using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Action/ Weapon Actions/ Heavy Attack Action")]
public class HeavyAttackWeaponItemAction : WeaponItemAction
{
    // Right Hand
    [SerializeField] string heavy_Attack_01 = "Main_Heavy_Attack_01";
    [SerializeField] string heavy_Attack_02 = "Main_Heavy_Attack_02";
    [SerializeField] string heavy_Attack_03 = "Main_Heavy_Attack_03";

    // Two Hand
    [SerializeField] string th_heavy_Attack_01 = "TH_Heavy_Attack_01";
    [SerializeField] string th_heavy_Attack_02 = "TH_Heavy_Attack_02";
    [SerializeField] string th_heavy_Attack_03 = "TH_Heavy_Attack_03";

    public override void AttpemtToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        base.AttpemtToPerformAction(playerPerformingAction, weaponPerformingAction);

        if (!playerPerformingAction.IsOwner)
            return;

        if (playerPerformingAction.playerNetworkManager.currentStamina.Value <= 0)
            return;

        if (playerPerformingAction.playerCombatManager.isUsingItem)
            return;

        if (!playerPerformingAction.characterLocomotionManager.isGrounded)
            return;

        if (playerPerformingAction.IsOwner)
        {
            playerPerformingAction.playerNetworkManager.isAttacking.Value = true;
        }

        PerformHeavyAttack(playerPerformingAction, weaponPerformingAction);
    }
    public void PerformHeavyAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
       if(playerPerformingAction.playerNetworkManager.isTwoHandingWeapon.Value)
        {
            PerformTwoHandHeavyAttack(playerPerformingAction, weaponPerformingAction);
        }
        else
        {
            PerformMainHeavyAttack(playerPerformingAction, weaponPerformingAction);
        }
    }
    private void PerformMainHeavyAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        bool canCombo = playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon;
        Debug.Log("heavy attack 3 :" + playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed);

        if (canCombo && playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

            if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == heavy_Attack_01)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.HeavyAttack02, heavy_Attack_02, true);
            }
            else if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == heavy_Attack_02)
            {
                Debug.Log("heavy attack 2 :" + playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed);
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.HeavyAttack03, heavy_Attack_03, true);

            }
            else
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.HeavyAttack01, heavy_Attack_01, true);
            }

        }

        // otherwise, perform a regular attack
        else if (!playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.HeavyAttack01, heavy_Attack_01, true);
        }
    }
    private void PerformTwoHandHeavyAttack(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        bool canCombo = playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon;
        Debug.Log("heavy attack 3 :" + playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed);

        if (canCombo && playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerCombatManager.canComboWithMainHandWeapon = false;

            if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == th_heavy_Attack_01)
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.HeavyAttack02, th_heavy_Attack_02, true);
            }
            else if (playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed == th_heavy_Attack_02)
            {
                Debug.Log("heavy attack 2 :" + playerPerformingAction.characterCombatManager.lastAttackAnimationPerformed);
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.HeavyAttack03, th_heavy_Attack_03, true);

            }
            else
            {
                playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.HeavyAttack01, th_heavy_Attack_01, true);
            }
        }

        // otherwise, perform a regular attack
        else if (!playerPerformingAction.isPerformingAction)
        {
            playerPerformingAction.playerAnimatorManager.PlayTargetAttackActionAnimtion(weaponPerformingAction, AttackType.HeavyAttack01, th_heavy_Attack_01, true);
        }
    }
}
