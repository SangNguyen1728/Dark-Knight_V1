using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Item/ Ash Of War/Parry")]
public class ParryAshOfWar : AshOfWar
{
    public override void AttempToPerformAction(PlayerManager playerPerformAction)
    {
        base.AttempToPerformAction(playerPerformAction);    

        if(!CanIUseThisAbility(playerPerformAction))
        {
            return;
        }

        DeductStaminaCost(playerPerformAction);
        DeductFocusPointCost(playerPerformAction);
        PerformParryTypeBasedOnWeapon(playerPerformAction);
    }

    public override bool CanIUseThisAbility(PlayerManager playerPerformAction)
    {
        if (playerPerformAction.isPerformingAction)
        {
            Debug.Log("Cannot Perform Ash Of War: Already Perfomning Action");
            return false;
        }

        if (playerPerformAction.playerNetworkManager.isJumping.Value)
        {
            Debug.Log("Cannot Perform Ash Of War: Jumping");
            return false;
        }

        if (!playerPerformAction.playerLocomotionManager.isGrounded)
        {
            Debug.Log("Cannot Perform Ash Of War: Not Grounded");
            return false;
        }

        if (playerPerformAction.playerNetworkManager.currentStamina.Value <= 0)
        {
            Debug.Log("Cannot Perform Ash Of War: Out Of Stamina");
            return false;
        }

        return true;
    }
    private void PerformParryTypeBasedOnWeapon(PlayerManager playerPerformAction)
    {
        WeaponItem weaponBeingUsed = playerPerformAction.playerCombatManager.currentWeaponBeingUsed;

        switch (weaponBeingUsed.weaponClass)
        {
            case WeaponClass.KatanaBlue:
                playerPerformAction.playerAnimatorManager.PlayTargetActionAnimtion("Slow_Parry_01", true);
                break;
            case WeaponClass.LightningTwinBlades:
                playerPerformAction.playerAnimatorManager.PlayTargetActionAnimtion("Fast_Parry_01", true);
                break;
            case WeaponClass.TwinBlades:
                playerPerformAction.playerAnimatorManager.PlayTargetActionAnimtion("Fast_Parry_01", true);
                break;
            case WeaponClass.Fist:
                break;
            case WeaponClass.Shield:
                playerPerformAction.playerAnimatorManager.PlayTargetActionAnimtion("Slow_Parry_01", true);
                break;
            default:
                break;
        }
    }
}
