using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AshOfWar : Item
{
    [Header("Ash of War Information")]
    public WeaponClass[] usableWeaponClasses;

    [Header("Costs")]
    public int focusPointCost = 20;
    public int staminaCost = 20;

    // The funtion attempting to perform the ash of war
    public virtual void AttempToPerformAction(PlayerManager playerPerformAction)
    {
        Debug.Log("Performed!");
    }

    // A helper funtion used to determine if i can in this moment use this ash of war
    public virtual bool CanIUseThisAbility(PlayerManager playerPerformAction)
    {
        return false;
    }

    public virtual void DeductStaminaCost(PlayerManager playerPerformAction)
    {
        playerPerformAction.playerNetworkManager.currentStamina.Value -= staminaCost;
    }

    public virtual void DeductFocusPointCost(PlayerManager playerPerformAction)
    {

    }
}
