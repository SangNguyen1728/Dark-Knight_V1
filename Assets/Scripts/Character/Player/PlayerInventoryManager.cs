using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInventoryManager : CharacterInventoryManager
{
    [Header("Weapon")]
    public WeaponItem currentRightHandWeapon;
    public WeaponItem currentLeftHandWeapon;
    public WeaponItem currentTwoHandWeapon;

    [Header("Quick Slots")]
    public WeaponItem[] weaponInRightHandSlot = new WeaponItem[7];
    public int rightHandWeaponIndex = 0;
    public WeaponItem[] weaponInLeftHandSlot = new WeaponItem[7];
    public int leftHandWeaponIndex = 0;
    public QuickSlotItem[] quickSlotItemInQuickSlots = new QuickSlotItem[5];
    public int quickSlotItemIndex = 0;
    public QuickSlotItem currentQuickSlotItem;

    [Header("Amor")]
    public HeadEquipmentItem headEquipment;
    public BodyEquipmentItem bodyEquipment;
    public LegEquipmentItem legEquipment;
    public HandEquipmentItem handEquipment;

    [Header("Inventory")]
    public List<Item> itemsInInventory;

    public void AddItemToInventory(Item item)
    {
        itemsInInventory.Add(item);
    }
    public void RemoveItemFromInventory(Item item)
    {
        itemsInInventory.Remove(item);

        for(int i = itemsInInventory.Count -1; i > 1; i --)
        {
            if(itemsInInventory[i] == null)
            {
                itemsInInventory.RemoveAt(i);
            }
        }
    }
}
