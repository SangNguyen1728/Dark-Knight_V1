using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuickSlotItem : Item
{
    [Header("Item Model")]
    [SerializeField] protected GameObject itemModel;

    [Header("Animtion")]
    [SerializeField] protected string useItemAnimation;

    [Header("Consumable")]
    public bool isConsumable = true;
    public int itemAmount = 1;

    public virtual void AttemptToUseItem(PlayerManager player)
    {
        if (!CanIUseThisItem(player))
            return;

        player.playerAnimatorManager.PlayTargetActionAnimtion(useItemAnimation, true); ;
    }

    public virtual void SuccessfullyUseItem(PlayerManager player)
    {

    }
    public virtual bool  CanIUseThisItem(PlayerManager player)
    {
        return true;
    }

    public virtual int GetCurrentAmount(PlayerManager player)
    {
        return 0;
    }
}
