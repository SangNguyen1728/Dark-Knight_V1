using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;

public class UI_EquipmentInventorySlots : MonoBehaviour
{
    public Image itemIcon;
    public Image hightlightedIcon;
    [SerializeField] public Item currentItem;

    public void AddItem(Item item)
    {
        if(item == null)
        {
            itemIcon.enabled = false;
            return;
        }

        itemIcon.enabled = true;

        currentItem = item;
        itemIcon.sprite = item.itemIcon;
    }

    public void SelectSlot()
    {
        hightlightedIcon.enabled = true;
    }

    public void DeselectSlot()
    {
        hightlightedIcon.enabled = false;
    }

    public void EquipItem()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        Item equipItem;

        switch (PlayerUIManager.instance.playerUIEquipmentManager.currentSelectedEquipmentSlot)
        {
            case EquipmentType.RightWeapon01:

                // If my current weapon in this slot, is not an unarmed item, add it to my inventory
                equipItem = player.playerInventoryManager.weaponInRightHandSlot[0];
               
                if(equipItem.itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
                {
                    player.playerInventoryManager.AddItemToInventory(equipItem);
                }

                // Then Replace the weapon in that slot with my new weapon
                player.playerInventoryManager.weaponInRightHandSlot[0] = currentItem as WeaponItem;
                // Then remove the new weapon from my inventory
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                // Re-Equip new weapon
                if (player.playerInventoryManager.rightHandWeaponIndex == 0)
                {
                    player.playerNetworkManager.currentRightHandWeaponID.Value = currentItem.itemID;
                }

                // Refeshes equipment window
                PlayerUIManager.instance.playerUIEquipmentManager.ReshesMenu();
                break;
            case EquipmentType.RightWeapon02:

                // If my current weapon in this slot, is not an unarmed item, add it to my inventory
                equipItem = player.playerInventoryManager.weaponInRightHandSlot[1];

                if (equipItem.itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
                {
                    player.playerInventoryManager.AddItemToInventory(equipItem);
                }

                // Then Replace the weapon in that slot with my new weapon
                player.playerInventoryManager.weaponInRightHandSlot[1] = currentItem as WeaponItem;
                // Then remove the new weapon from my inventory
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                // Re-Equip new weapon
                if (player.playerInventoryManager.rightHandWeaponIndex == 1)
                {
                    player.playerNetworkManager.currentRightHandWeaponID.Value = currentItem.itemID;
                }

                // Refeshes equipment window
                PlayerUIManager.instance.playerUIEquipmentManager.ReshesMenu();
                break;
            case EquipmentType.RightWeapon03:

                equipItem = player.playerInventoryManager.weaponInRightHandSlot[2];

                if (equipItem.itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
                {
                    player.playerInventoryManager.AddItemToInventory(equipItem);
                }

                // Then Replace the weapon in that slot with my new weapon
                player.playerInventoryManager.weaponInRightHandSlot[2] = currentItem as WeaponItem;
                // Then remove the new weapon from my inventory
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                // Re-Equip new weapon
                if (player.playerInventoryManager.rightHandWeaponIndex == 2)
                {
                    player.playerNetworkManager.currentRightHandWeaponID.Value = currentItem.itemID;
                }

                // Refeshes equipment window
                PlayerUIManager.instance.playerUIEquipmentManager.ReshesMenu();
                break;
            case EquipmentType.LeftWeapon01:

                equipItem = player.playerInventoryManager.weaponInLeftHandSlot[0];

                if (equipItem.itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
                {
                    player.playerInventoryManager.AddItemToInventory(equipItem);
                }

                // Then Replace the weapon in that slot with my new weapon
                player.playerInventoryManager.weaponInLeftHandSlot[0] = currentItem as WeaponItem;
                // Then remove the new weapon from my inventory
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                // Re-Equip new weapon
                if (player.playerInventoryManager.leftHandWeaponIndex == 0)
                {
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = currentItem.itemID;
                }

                // Refeshes equipment window
                PlayerUIManager.instance.playerUIEquipmentManager.ReshesMenu();
                break;
            case EquipmentType.LeftWeapon02:

                equipItem = player.playerInventoryManager.weaponInLeftHandSlot[1];

                if (equipItem.itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
                {
                    player.playerInventoryManager.AddItemToInventory(equipItem);
                }

                // Then Replace the weapon in that slot with my new weapon
                player.playerInventoryManager.weaponInLeftHandSlot[1] = currentItem as WeaponItem;
                // Then remove the new weapon from my inventory
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                // Re-Equip new weapon
                if (player.playerInventoryManager.leftHandWeaponIndex == 1)
                {
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = currentItem.itemID;
                }

                // Refeshes equipment window
                PlayerUIManager.instance.playerUIEquipmentManager.ReshesMenu();
                break;
            case EquipmentType.LeftWeapon03:

                equipItem = player.playerInventoryManager.weaponInLeftHandSlot[2];

                if (equipItem.itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
                {
                    player.playerInventoryManager.AddItemToInventory(equipItem);
                }

                // Then Replace the weapon in that slot with my new weapon
                player.playerInventoryManager.weaponInLeftHandSlot[2] = currentItem as WeaponItem;
                // Then remove the new weapon from my inventory
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                // Re-Equip new weapon
                if (player.playerInventoryManager.leftHandWeaponIndex == 2)
                {
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = currentItem.itemID;
                }

                // Refeshes equipment window
                PlayerUIManager.instance.playerUIEquipmentManager.ReshesMenu();
                break;

            case EquipmentType.Head:

                equipItem = player.playerInventoryManager.headEquipment;

                if (equipItem!= null)
                {
                    player.playerInventoryManager.AddItemToInventory(equipItem);
                }

                // Then assign the slot my new item
                player.playerInventoryManager.headEquipment = currentItem as HeadEquipmentItem;
                // Then remove the new item from my inventory
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                // Re-Equip new item
                player.playerEquipmentManager.LoadHeadEquipment(player.playerInventoryManager.headEquipment);

                // Refeshes equipment window
                PlayerUIManager.instance.playerUIEquipmentManager.ReshesMenu();
                break;

            case EquipmentType.Body:

                equipItem = player.playerInventoryManager.bodyEquipment;

                if (equipItem != null)
                {
                    player.playerInventoryManager.AddItemToInventory(equipItem);
                }

                // Then assign the slot my new item
                player.playerInventoryManager.bodyEquipment = currentItem as BodyEquipmentItem;
                // Then remove the new item from my inventory
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                // Re-Equip new item
                player.playerEquipmentManager.LoadBodyEquipment(player.playerInventoryManager.bodyEquipment);

                // Refeshes equipment window
                PlayerUIManager.instance.playerUIEquipmentManager.ReshesMenu();
                break;

            case EquipmentType.Legs:

                equipItem = player.playerInventoryManager.legEquipment;

                if (equipItem != null)
                {
                    player.playerInventoryManager.AddItemToInventory(equipItem);
                }

                // Then assign the slot my new item
                player.playerInventoryManager.legEquipment = currentItem as LegEquipmentItem;
                // Then remove the new item from my inventory
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                // Re-Equip new item
                player.playerEquipmentManager.LoadLegEquipment(player.playerInventoryManager.legEquipment);

                // Refeshes equipment window
                PlayerUIManager.instance.playerUIEquipmentManager.ReshesMenu();
                break;

            case EquipmentType.Hands:

                equipItem = player.playerInventoryManager.handEquipment;

                if (equipItem != null)
                {
                    player.playerInventoryManager.AddItemToInventory(equipItem);
                }

                // Then assign the slot my new item
                player.playerInventoryManager.handEquipment = currentItem as HandEquipmentItem;
                // Then remove the new item from my inventory
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                // Re-Equip new item
                player.playerEquipmentManager.LoadHandEquipment(player.playerInventoryManager.handEquipment);

                // Refeshes equipment window
                PlayerUIManager.instance.playerUIEquipmentManager.ReshesMenu();
                break;

            case EquipmentType.QuickSlot01:

                equipItem = player.playerInventoryManager.quickSlotItemInQuickSlots[0];

                if (equipItem != null)
                {
                    player.playerInventoryManager.AddItemToInventory(equipItem);
                }

                // Then assign the slot my new item
                player.playerInventoryManager.quickSlotItemInQuickSlots[0] = currentItem as QuickSlotItem;
                // Then remove the new item from my inventory
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                // Re-Equip new item
                if (player.playerInventoryManager.quickSlotItemIndex == 0)
                    player.playerNetworkManager.currentQuickSlotItemID.Value = currentItem.itemID;

                // Refeshes equipment window
                PlayerUIManager.instance.playerUIEquipmentManager.ReshesMenu();
                break;

            case EquipmentType.QuickSlot02:

                equipItem = player.playerInventoryManager.quickSlotItemInQuickSlots[1];

                if (equipItem != null)
                {
                    player.playerInventoryManager.AddItemToInventory(equipItem);
                }

                // Then assign the slot my new item
                player.playerInventoryManager.quickSlotItemInQuickSlots[1] = currentItem as QuickSlotItem;
                // Then remove the new item from my inventory
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                // Re-Equip new item
                if (player.playerInventoryManager.quickSlotItemIndex == 1)
                    player.playerNetworkManager.currentQuickSlotItemID.Value = currentItem.itemID;

                // Refeshes equipment window
                PlayerUIManager.instance.playerUIEquipmentManager.ReshesMenu();
                break;

            case EquipmentType.QuickSlot03:

                equipItem = player.playerInventoryManager.quickSlotItemInQuickSlots[2];

                if (equipItem != null)
                {
                    player.playerInventoryManager.AddItemToInventory(equipItem);
                }

                // Then assign the slot my new item
                player.playerInventoryManager.quickSlotItemInQuickSlots[2] = currentItem as QuickSlotItem;
                // Then remove the new item from my inventory
                player.playerInventoryManager.RemoveItemFromInventory(currentItem);

                // Re-Equip new item
                if (player.playerInventoryManager.quickSlotItemIndex == 2)
                    player.playerNetworkManager.currentQuickSlotItemID.Value = currentItem.itemID;

                // Refeshes equipment window
                PlayerUIManager.instance.playerUIEquipmentManager.ReshesMenu();
                break;

            default:
                break;
        }

        PlayerUIManager.instance.playerUIEquipmentManager.SelectLastSelectedEquipmentSlot();
    }
}
