using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Action/ Weapon Actions/ Test Actions")]
public class WeaponItemAction : ScriptableObject
{
    public int actionID;

    public virtual void AttpemtToPerformAction(PlayerManager playerPerformingAction, WeaponItem weaponPerformingAction)
    {
       if(playerPerformingAction.IsOwner)
        {
            playerPerformingAction.playerNetworkManager.currentWeaponBeingUsed.Value = weaponPerformingAction.itemID;
        }
    }
}
