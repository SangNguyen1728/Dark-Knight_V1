using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Action/ Weapon Actions/ Off Hand Melee Action")]
public class OffHandMeleeAction : WeaponItemAction
{
    // In future, if character is wielding a main hand and off hand weapon of the same weapon class, the off hand action will not be block
    // The off hand's action becomes a dual attack

    public override void AttpemtToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
        base.AttpemtToPerformAction(playerPerformingAction, weaponPerformingAction);

        // Check attack status
        if (!playerPerformingAction.playerCombatManager.canBlock)
            return;

        if (playerPerformingAction.playerCombatManager.isUsingItem)
            return;

        if (playerPerformingAction.playerNetworkManager.isAttacking.Value)
        {
            if(playerPerformingAction.IsOwner)
            {
                playerPerformingAction.playerNetworkManager.isBlocking.Value = false;
            }

            return;
        }

        if (playerPerformingAction.playerNetworkManager.isBlocking.Value)
            return;

        if(playerPerformingAction.IsOwner)
        {
            playerPerformingAction.playerNetworkManager.isBlocking.Value = true;
        }
    }
}
