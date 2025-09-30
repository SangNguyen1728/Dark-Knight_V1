using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using UnityEngine.Rendering;
using TMPro;

public class PlayerUIEquipmentManager : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] GameObject menu;

    [Header("Weapon Slots")]
    [SerializeField] Image rightHandSlot01;
    private Button rightHandSlotButton01;
    [SerializeField] Image rightHandSlot02;
    private Button rightHandSlotButton02;
    [SerializeField] Image rightHandSlot03;
    private Button rightHandSlotButton03;
    [SerializeField] Image leftHandSlot01;
    private Button leftHandSlotButton01;
    [SerializeField] Image leftHandSlot02;
    private Button leftHandSlotButton02;
    [SerializeField] Image leftHandSlot03;
    private Button leftHandSlotButton03;

    [Header("Armor")]
    [SerializeField] Image headEquipmentSlot;
    private Button headEquipmentSlotButton;
    [SerializeField] Image bodyEquipmentSlot;
    private Button bodyEquipmentSlotButton;
    [SerializeField] Image legEquipmentSlot;
    private Button legEquipmentSlotButton;
    [SerializeField] Image handEquipmentSlot;
    private Button handEquipmentSlotButton;

    [Header("Quick SLot")]
    [SerializeField] Image quickSlot01EquipmentSlot;
    [SerializeField] TextMeshProUGUI quickSlot01Count;
    private Button quickSlotButton01;
    [SerializeField] Image quickSlot02EquipmentSlot;
    [SerializeField] TextMeshProUGUI quickSlot02Count;
    private Button quickSlotButton02;
    [SerializeField] Image quickSlot03EquipmentSlot;
    [SerializeField] TextMeshProUGUI quickSlot03Count;
    private Button quickSlotButton03;


    [Header("Equipment Inventory")]
    public EquipmentType currentSelectedEquipmentSlot;
    [SerializeField] GameObject equipmentInventoryWindow;
    [SerializeField] GameObject equipmentInventorySlotPrefab;
    [SerializeField] Transform equipmentInventoryContentWindow;
    [SerializeField] Item currentSelectedItem;

    private void Awake()
    {
        rightHandSlotButton01 = rightHandSlot01.GetComponentInParent<Button>(true);
        rightHandSlotButton02 = rightHandSlot02.GetComponentInParent<Button>(true);
        rightHandSlotButton03 = rightHandSlot03.GetComponentInParent<Button>(true);

        leftHandSlotButton01 = leftHandSlot01.GetComponentInParent<Button>(true);
        leftHandSlotButton02 = leftHandSlot02.GetComponentInParent<Button>(true);
        leftHandSlotButton03 = leftHandSlot03.GetComponentInParent<Button>(true);

        headEquipmentSlotButton = headEquipmentSlot.GetComponentInParent<Button>(true);
        bodyEquipmentSlotButton = bodyEquipmentSlot.GetComponentInParent<Button>(true);
        handEquipmentSlotButton = handEquipmentSlot.GetComponentInParent<Button>(true);
        legEquipmentSlotButton= legEquipmentSlot.GetComponentInParent<Button>(true);

        quickSlotButton01 = quickSlot01EquipmentSlot.GetComponentInParent<Button>(true);
        quickSlotButton02 = quickSlot02EquipmentSlot.GetComponentInParent<Button>(true);
        quickSlotButton03 = quickSlot03EquipmentSlot.GetComponentInParent<Button>(true);
    }
    public void OpenEquipmentManagerMenu()
    {
        PlayerUIManager.instance.menuWindowIsOpen = true;
        ToggleEquipmentButtons(true);
        menu.SetActive(true);
        equipmentInventoryWindow.SetActive(false);
        
        ClearEquipmentInventory();
        RefeshEquipmentSlotIcon();
    }

    public void ReshesMenu()
    {
        ClearEquipmentInventory();
        RefeshEquipmentSlotIcon();
    }

    private void ToggleEquipmentButtons(bool isEnabled)
    {
        rightHandSlotButton01.enabled = isEnabled;
        rightHandSlotButton02.enabled = isEnabled;
        rightHandSlotButton03.enabled = isEnabled;

        leftHandSlotButton01.enabled = isEnabled;
        leftHandSlotButton02.enabled = isEnabled;
        leftHandSlotButton03.enabled = isEnabled;

        headEquipmentSlotButton.enabled = isEnabled;
        bodyEquipmentSlotButton.enabled = isEnabled;
        handEquipmentSlotButton.enabled= isEnabled;
        legEquipmentSlotButton.enabled= isEnabled;

        quickSlotButton01.enabled = isEnabled;
        quickSlotButton02.enabled = isEnabled;
        quickSlotButton03.enabled = isEnabled;
    }
    public void SelectLastSelectedEquipmentSlot()
    {
        Button lastSelectedButton = null;
        ToggleEquipmentButtons(true);
        switch (currentSelectedEquipmentSlot)
        {
            case EquipmentType.RightWeapon01:
                lastSelectedButton = rightHandSlotButton01;
                break;
            case EquipmentType.RightWeapon02:
                lastSelectedButton = rightHandSlotButton02;
                break;
            case EquipmentType.RightWeapon03:
                lastSelectedButton = rightHandSlotButton03;
                break;
            case EquipmentType.LeftWeapon01:
                lastSelectedButton = leftHandSlotButton01;
                break;
            case EquipmentType.LeftWeapon02:
                lastSelectedButton = leftHandSlotButton02;
                break;
            case EquipmentType.LeftWeapon03:
                lastSelectedButton = leftHandSlotButton03;
                break;
            case EquipmentType.Head:
                lastSelectedButton = headEquipmentSlotButton;
                break;
            case EquipmentType.Body:
                lastSelectedButton = bodyEquipmentSlotButton;
                break;
            case EquipmentType.Legs:
                lastSelectedButton = legEquipmentSlotButton;
                break;
            case EquipmentType.Hands:
                lastSelectedButton = handEquipmentSlotButton;
                break;
            case EquipmentType.QuickSlot01:
                lastSelectedButton = quickSlotButton01;
                break;
            case EquipmentType.QuickSlot02:
                lastSelectedButton = quickSlotButton02;
                break;
            case EquipmentType.QuickSlot03:
                lastSelectedButton = quickSlotButton03;
                break;

            default:
                break;
        }

        if(lastSelectedButton != null)
        {
            lastSelectedButton.Select();
            lastSelectedButton.OnSelect(null);
        }

        // ToDo: send a player a message that i have none item type in inventory
        equipmentInventoryWindow.SetActive(false);
        
    }
    public void CloseEquipmentManagerMenu()
    {

        PlayerUIManager.instance.menuWindowIsOpen = false;
        menu.SetActive(false);
    }
    private void RefeshEquipmentSlotIcon()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

        // Right weapon 01
        WeaponItem rightHandWeapon01 = player.playerInventoryManager.weaponInRightHandSlot[0];

        if(rightHandWeapon01.itemIcon != null )
        {
            rightHandSlot01.enabled = true;
            rightHandSlot01.sprite = rightHandWeapon01.itemIcon;
        }
        else
        {
            rightHandSlot01.enabled = false;
        }

        // Right weapon 02
        WeaponItem rightHandWeapon02 = player.playerInventoryManager.weaponInRightHandSlot[1];

        if (rightHandWeapon02.itemIcon != null)
        {
            rightHandSlot02.enabled = true;
            rightHandSlot02.sprite = rightHandWeapon02.itemIcon;
        }
        else
        {
            rightHandSlot02.enabled = false;
        }

        // Right weapon 03
        WeaponItem rightHandWeapon03 = player.playerInventoryManager.weaponInRightHandSlot[2];

        if (rightHandWeapon03.itemIcon != null)
        {
            rightHandSlot03.enabled = true;
            rightHandSlot03.sprite = rightHandWeapon03.itemIcon;
        }
        else
        {
            rightHandSlot03.enabled = false;
        }

        // left weapon 01
        WeaponItem leftHandWeapon01 = player.playerInventoryManager.weaponInLeftHandSlot[0];

        if (leftHandWeapon01.itemIcon != null)
        {
            leftHandSlot01.enabled = true;
            leftHandSlot01.sprite = leftHandWeapon01.itemIcon;
        }
        else
        {
            leftHandSlot01.enabled = false;
        }

        // Left weapon 02
        WeaponItem leftHandWeapon02 = player.playerInventoryManager.weaponInLeftHandSlot[1];

        if (leftHandWeapon02.itemIcon != null)
        {
            leftHandSlot02.enabled = true;
            leftHandSlot02.sprite = leftHandWeapon02.itemIcon;
        }
        else
        {
            leftHandSlot02.enabled = false;
        }

        // Left weapon 03
        WeaponItem leftHandWeapon03 = player.playerInventoryManager.weaponInLeftHandSlot[2];

        if (leftHandWeapon03.itemIcon != null)
        {
            leftHandSlot03.enabled = true;
            leftHandSlot03.sprite = leftHandWeapon03.itemIcon;
        }
        else
        {
            leftHandSlot03.enabled = false;
        }

        // Head
        HeadEquipmentItem headEquipment = player.playerInventoryManager.headEquipment;

        if (headEquipment != null)
        {
            headEquipmentSlot.enabled = true;
            headEquipmentSlot.sprite = headEquipment.itemIcon;
        }
        else
        {
            headEquipmentSlot.enabled = false;
        }

        // Body
        BodyEquipmentItem bodyEquipment = player.playerInventoryManager.bodyEquipment;

        if (bodyEquipment != null)
        {
            bodyEquipmentSlot.enabled = true;
            bodyEquipmentSlot.sprite = bodyEquipment.itemIcon;
        }
        else
        {
            bodyEquipmentSlot.enabled = false;
        }

        // Legs
        LegEquipmentItem legEquipment = player.playerInventoryManager.legEquipment;

        if (legEquipment != null)
        {
            legEquipmentSlot.enabled = true;
            legEquipmentSlot.sprite = legEquipment.itemIcon;
        }
        else
        {
            legEquipmentSlot.enabled = false;
        }

        // Hands
        HandEquipmentItem handEquipment = player.playerInventoryManager.handEquipment;

        if (handEquipment != null)
        {
            handEquipmentSlot.enabled = true;
            handEquipmentSlot.sprite = handEquipment.itemIcon;
        }
        else
        {
            handEquipmentSlot.enabled = false;
        }

        // Quick Slot
        QuickSlotItem quickSlotEquipment01 = player.playerInventoryManager.quickSlotItemInQuickSlots[0];

        if(quickSlotEquipment01 != null)
        {
            quickSlot01EquipmentSlot.enabled = true;
            quickSlot01EquipmentSlot.sprite = quickSlotEquipment01.itemIcon;

            if(quickSlotEquipment01.isConsumable)
            {
                quickSlot01Count.enabled = true;
                quickSlot01Count.text = quickSlotEquipment01.GetCurrentAmount(player).ToString();
            }
            else
            {
                quickSlot01Count.enabled = false;
            }
        }
        else
        {
            quickSlot01EquipmentSlot.enabled = false;
            quickSlot01Count.enabled = false;
        }

        QuickSlotItem quickSlotEquipment02 = player.playerInventoryManager.quickSlotItemInQuickSlots[1];

        if (quickSlotEquipment02 != null)
        {
            quickSlot02EquipmentSlot.enabled = true;
            quickSlot02EquipmentSlot.sprite = quickSlotEquipment02.itemIcon;

            if (quickSlotEquipment02.isConsumable)
            {
                quickSlot02Count.enabled = true;
                quickSlot02Count.text = quickSlotEquipment02.GetCurrentAmount(player).ToString();
            }
            else
            {
                quickSlot02Count.enabled = false;
            }
        }
        else
        {
            quickSlot02EquipmentSlot.enabled = false;
            quickSlot02Count.enabled = false;
        }

        QuickSlotItem quickSlotEquipment03 = player.playerInventoryManager.quickSlotItemInQuickSlots[2];

        if (quickSlotEquipment03 != null)
        {
            quickSlot03EquipmentSlot.enabled = true;
            quickSlot03EquipmentSlot.sprite = quickSlotEquipment03.itemIcon;

            if (quickSlotEquipment03.isConsumable)
            {
                quickSlot03Count.enabled = true;
                quickSlot03Count.text = quickSlotEquipment03.GetCurrentAmount(player).ToString();
            }
            else
            {
                quickSlot03Count.enabled = false;
            }
        }
        else
        {
            quickSlot03EquipmentSlot.enabled = false;
            quickSlot03Count.enabled = false;
        }
    }

    private void ClearEquipmentInventory()
    {
        foreach(Transform item in equipmentInventoryContentWindow)
        {
            Destroy(item.gameObject);
        }
    }
    public void LoadEquipmentInventorỵ̣()
    {
        ToggleEquipmentButtons(false);
        equipmentInventoryWindow.SetActive(true);

        switch(currentSelectedEquipmentSlot)
        {
            case EquipmentType.RightWeapon01:
                LoadWeaponInventory();
                break;
            case EquipmentType.RightWeapon02:
                LoadWeaponInventory();
                break;
            case EquipmentType.RightWeapon03:
                LoadWeaponInventory();
                break;
            case EquipmentType.LeftWeapon01:
                LoadWeaponInventory();
                break;
            case EquipmentType.LeftWeapon02:
                LoadWeaponInventory();
                break;
            case EquipmentType.LeftWeapon03:
                LoadWeaponInventory();
                break;
            case EquipmentType.Head:
                LoadHeadEquipmentInventory();
                break;
            case EquipmentType.Body:
                LoadBodyEquipmentInventory();
                break;
            case EquipmentType.Legs:
                LoadLegsInventory();
                break;
            case EquipmentType.Hands:
                LoadHandsInventory();
                break;
            case EquipmentType.QuickSlot01:
                LoadQuickSlotInventory();
                break;
            case EquipmentType.QuickSlot02:
                LoadQuickSlotInventory();
                break;
            case EquipmentType.QuickSlot03:
                LoadQuickSlotInventory();
                break;

            default:
                break;
        }
    }

    private void LoadWeaponInventory()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

        List<WeaponItem> weaponInInventory = new List<WeaponItem>();

        for (int  i = 0; i < player.playerInventoryManager.itemsInInventory.Count; i++)
        {
            WeaponItem weapon = player.playerInventoryManager.itemsInInventory[i] as WeaponItem;

            if(weapon != null)
            {
                weaponInInventory.Add(weapon);
            }
        }

        if(weaponInInventory.Count <= 0)
        {
            // ToDo: send a player a message that i have none item type in inventory
            equipmentInventoryWindow.SetActive(false);
            ToggleEquipmentButtons(true);
            ReshesMenu();
            
            return;
        }

        bool hasSelctedFirstInventorySlot = false;

        for (int i = 0;i < weaponInInventory.Count;i++)
        {
            GameObject inventorySlotGameObject = Instantiate(equipmentInventorySlotPrefab, equipmentInventoryContentWindow);
            UI_EquipmentInventorySlots equipmentInventorySlot = inventorySlotGameObject.GetComponent<UI_EquipmentInventorySlots>();
            equipmentInventorySlot.AddItem(weaponInInventory[i]);

            // This will select the first button in the list
            if(!hasSelctedFirstInventorySlot)
            {
                hasSelctedFirstInventorySlot = true;
                Button inventorySlotButton = inventorySlotGameObject.GetComponent<Button>();
                inventorySlotButton.Select();
                inventorySlotButton.OnSelect(null);
            }
        }
    }

    private void LoadHeadEquipmentInventory()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

        List<HeadEquipmentItem> headEquipmentInInventory = new List<HeadEquipmentItem>();

        for (int i = 0; i < player.playerInventoryManager.itemsInInventory.Count; i++)
        {
            HeadEquipmentItem equipment = player.playerInventoryManager.itemsInInventory[i] as HeadEquipmentItem;

            if (equipment != null)
            {
                headEquipmentInInventory.Add(equipment);
            }
        }

        if (headEquipmentInInventory.Count <= 0)
        {
            // ToDo: send a player a message that i have none item type in inventory
            equipmentInventoryWindow.SetActive(false);
            ToggleEquipmentButtons(true);
            ReshesMenu();
            return;
        }

        bool hasSelctedFirstInventorySlot = false;

        for (int i = 0; i < headEquipmentInInventory.Count; i++)
        {
            GameObject inventorySlotGameObject = Instantiate(equipmentInventorySlotPrefab, equipmentInventoryContentWindow);
            UI_EquipmentInventorySlots equipmentInventorySlot = inventorySlotGameObject.GetComponent<UI_EquipmentInventorySlots>();
            equipmentInventorySlot.AddItem(headEquipmentInInventory[i]);

            // This will select the first button in the list
            if (!hasSelctedFirstInventorySlot)
            {
                hasSelctedFirstInventorySlot = true;
                Button inventorySlotButton = inventorySlotGameObject.GetComponent<Button>();
                inventorySlotButton.Select();
                inventorySlotButton.OnSelect(null);
            }
        }
    }

    private void LoadBodyEquipmentInventory()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

        List<BodyEquipmentItem> bodyEquipmentInInventory = new List<BodyEquipmentItem>();

        for (int i = 0; i < player.playerInventoryManager.itemsInInventory.Count; i++)
        {
            BodyEquipmentItem equipment = player.playerInventoryManager.itemsInInventory[i] as BodyEquipmentItem;

            if (equipment != null)
            {
                bodyEquipmentInInventory.Add(equipment);
            }
        }

        if (bodyEquipmentInInventory.Count <= 0)
        {
            // ToDo: send a player a message that i have none item type in inventory
            equipmentInventoryWindow.SetActive(false);
            ToggleEquipmentButtons(true);
            ReshesMenu();
            return;
        }

        bool hasSelctedFirstInventorySlot = false;

        for (int i = 0; i < bodyEquipmentInInventory.Count; i++)
        {
            GameObject inventorySlotGameObject = Instantiate(equipmentInventorySlotPrefab, equipmentInventoryContentWindow);
            UI_EquipmentInventorySlots equipmentInventorySlot = inventorySlotGameObject.GetComponent<UI_EquipmentInventorySlots>();
            equipmentInventorySlot.AddItem(bodyEquipmentInInventory[i]);

            // This will select the first button in the list
            if (!hasSelctedFirstInventorySlot)
            {
                hasSelctedFirstInventorySlot = true;
                Button inventorySlotButton = inventorySlotGameObject.GetComponent<Button>();
                inventorySlotButton.Select();
                inventorySlotButton.OnSelect(null);
            }
        }
    }

    private void LoadLegsInventory()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

        List<LegEquipmentItem> legsEquipmentInInventory = new List<LegEquipmentItem>();

        for (int i = 0; i < player.playerInventoryManager.itemsInInventory.Count; i++)
        {
            LegEquipmentItem equipment = player.playerInventoryManager.itemsInInventory[i] as LegEquipmentItem;

            if (equipment != null)
            {
                legsEquipmentInInventory.Add(equipment);
            }
        }

        if (legsEquipmentInInventory.Count <= 0)
        {
            // ToDo: send a player a message that i have none item type in inventory
            equipmentInventoryWindow.SetActive(false);
            ToggleEquipmentButtons(true);
            ReshesMenu();
            return;
        }

        bool hasSelctedFirstInventorySlot = false;

        for (int i = 0; i < legsEquipmentInInventory.Count; i++)
        {
            GameObject inventorySlotGameObject = Instantiate(equipmentInventorySlotPrefab, equipmentInventoryContentWindow);
            UI_EquipmentInventorySlots equipmentInventorySlot = inventorySlotGameObject.GetComponent<UI_EquipmentInventorySlots>();
            equipmentInventorySlot.AddItem(legsEquipmentInInventory[i]);

            // This will select the first button in the list
            if (!hasSelctedFirstInventorySlot)
            {
                hasSelctedFirstInventorySlot = true;
                Button inventorySlotButton = inventorySlotGameObject.GetComponent<Button>();
                inventorySlotButton.Select();
                inventorySlotButton.OnSelect(null);
            }
        }
    }

    private void LoadHandsInventory()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

        List<HandEquipmentItem> handsEquipmentInInventory = new List<HandEquipmentItem>();

        for (int i = 0; i < player.playerInventoryManager.itemsInInventory.Count; i++)
        {
            HandEquipmentItem equipment = player.playerInventoryManager.itemsInInventory[i] as HandEquipmentItem;

            if (equipment != null)
            {
                handsEquipmentInInventory.Add(equipment);
            }
        }

        if (handsEquipmentInInventory.Count <= 0)
        {
            // ToDo: send a player a message that i have none item type in inventory
            equipmentInventoryWindow.SetActive(false);
            ToggleEquipmentButtons(true);
            ReshesMenu();
            return;
        }

        bool hasSelctedFirstInventorySlot = false;

        for (int i = 0; i < handsEquipmentInInventory.Count; i++)
        {
            GameObject inventorySlotGameObject = Instantiate(equipmentInventorySlotPrefab, equipmentInventoryContentWindow);
            UI_EquipmentInventorySlots equipmentInventorySlot = inventorySlotGameObject.GetComponent<UI_EquipmentInventorySlots>();
            equipmentInventorySlot.AddItem(handsEquipmentInInventory[i]);

            // This will select the first button in the list
            if (!hasSelctedFirstInventorySlot)
            {
                hasSelctedFirstInventorySlot = true;
                Button inventorySlotButton = inventorySlotGameObject.GetComponent<Button>();
                inventorySlotButton.Select();
                inventorySlotButton.OnSelect(null);
            }
        }
    }

    private void LoadQuickSlotInventory()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();

        List<QuickSlotItem> quickSlotItemsInInventory  = new List<QuickSlotItem>();

        for(int i = 0; i < player.playerInventoryManager.itemsInInventory.Count; i++)
        {
            QuickSlotItem quickSlotItem = player.playerInventoryManager.itemsInInventory[i] as QuickSlotItem;

            if(quickSlotItem != null)
                quickSlotItemsInInventory.Add(quickSlotItem);
        }

        if(quickSlotItemsInInventory.Count <= 0)
        {
            equipmentInventoryWindow.SetActive(false);
            ToggleEquipmentButtons(true);
            ReshesMenu();
            return;
        }

        bool hasSelectedFirstInventorySlot = false;

        for(int i = 0; i < quickSlotItemsInInventory.Count; i++)
        {
            GameObject inventorySlotGameObj = Instantiate(equipmentInventorySlotPrefab, equipmentInventoryContentWindow);
            UI_EquipmentInventorySlots equipmentInventorySlot = inventorySlotGameObj.GetComponent<UI_EquipmentInventorySlots>();
            equipmentInventorySlot.AddItem(quickSlotItemsInInventory[i]);

            if(!hasSelectedFirstInventorySlot)
            {
                hasSelectedFirstInventorySlot = true;
                Button inventorySlotButton = inventorySlotGameObj.GetComponent<Button>();
                inventorySlotButton.Select();
                inventorySlotButton.OnSelect(null);
            }
        }
    }

    public void SelectEquipmentSlot(int equipmentSlot)
    {
        currentSelectedEquipmentSlot = (EquipmentType)equipmentSlot;
    }

    public void UnEquipSelectedItem()
    {
        PlayerManager player = NetworkManager.Singleton.LocalClient.PlayerObject.GetComponent<PlayerManager>();
        Item unequippedItem;

        switch (currentSelectedEquipmentSlot)
        {
            case EquipmentType.RightWeapon01:
                unequippedItem = player.playerInventoryManager.weaponInRightHandSlot[0];

                if(unequippedItem != null)
                {
                    player.playerInventoryManager.weaponInRightHandSlot[0] = Instantiate(WorldItemDatabase.Instance.unarmedWeapon);

                    if(unequippedItem.itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(unequippedItem);
                    }
                }

                if (player.playerInventoryManager.rightHandWeaponIndex == 0)
                    player.playerNetworkManager.currentRightHandWeaponID.Value = WorldItemDatabase.Instance.unarmedWeapon.itemID;

                break;
            case EquipmentType.RightWeapon02:
                unequippedItem = player.playerInventoryManager.weaponInRightHandSlot[1];

                if (unequippedItem != null)
                {
                    player.playerInventoryManager.weaponInRightHandSlot[1] = Instantiate(WorldItemDatabase.Instance.unarmedWeapon);

                    if (unequippedItem.itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(unequippedItem);
                    }
                }

                if (player.playerInventoryManager.rightHandWeaponIndex == 1)
                    player.playerNetworkManager.currentRightHandWeaponID.Value = WorldItemDatabase.Instance.unarmedWeapon.itemID;

                break;
            case EquipmentType.RightWeapon03:
                unequippedItem = player.playerInventoryManager.weaponInRightHandSlot[2];

                if (unequippedItem != null)
                {
                    player.playerInventoryManager.weaponInRightHandSlot[2] = Instantiate(WorldItemDatabase.Instance.unarmedWeapon);

                    if (unequippedItem.itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(unequippedItem);
                    }
                }

                if (player.playerInventoryManager.rightHandWeaponIndex == 2)
                    player.playerNetworkManager.currentRightHandWeaponID.Value = WorldItemDatabase.Instance.unarmedWeapon.itemID;

                break;
            case EquipmentType.LeftWeapon01:
                unequippedItem = player.playerInventoryManager.weaponInLeftHandSlot[0];

                if (unequippedItem != null)
                {
                    player.playerInventoryManager.weaponInLeftHandSlot[0] = Instantiate(WorldItemDatabase.Instance.unarmedWeapon);

                    if (unequippedItem.itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(unequippedItem);
                    }
                }

                if (player.playerInventoryManager.leftHandWeaponIndex == 0)
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = WorldItemDatabase.Instance.unarmedWeapon.itemID;

                break;
            case EquipmentType.LeftWeapon02:
                unequippedItem = player.playerInventoryManager.weaponInLeftHandSlot[1];

                if (unequippedItem != null)
                {
                    player.playerInventoryManager.weaponInLeftHandSlot[1] = Instantiate(WorldItemDatabase.Instance.unarmedWeapon);

                    if (unequippedItem.itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(unequippedItem);
                    }
                }

                if (player.playerInventoryManager.leftHandWeaponIndex == 1)
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = WorldItemDatabase.Instance.unarmedWeapon.itemID;

                break;
            case EquipmentType.LeftWeapon03:

                unequippedItem = player.playerInventoryManager.weaponInLeftHandSlot[2];

                if (unequippedItem != null)
                {
                    player.playerInventoryManager.weaponInLeftHandSlot[2] = Instantiate(WorldItemDatabase.Instance.unarmedWeapon);

                    if (unequippedItem.itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
                    {
                        player.playerInventoryManager.AddItemToInventory(unequippedItem);
                    }
                }

                if (player.playerInventoryManager.leftHandWeaponIndex == 2)
                    player.playerNetworkManager.currentLeftHandWeaponID.Value = WorldItemDatabase.Instance.unarmedWeapon.itemID;

                break;

            case EquipmentType.Head:

                unequippedItem = player.playerInventoryManager.headEquipment;

                if (unequippedItem != null)
                {
                    player.playerInventoryManager.AddItemToInventory(unequippedItem);
                }

                player.playerInventoryManager.headEquipment = null;
                player.playerEquipmentManager.LoadHeadEquipment(player.playerInventoryManager.headEquipment);

                break;

                // Body
            case EquipmentType.Body:

                unequippedItem = player.playerInventoryManager.bodyEquipment;

                if (unequippedItem != null)
                {
                    player.playerInventoryManager.AddItemToInventory(unequippedItem);
                }

                player.playerInventoryManager.bodyEquipment = null;
                player.playerEquipmentManager.LoadBodyEquipment(player.playerInventoryManager.bodyEquipment);

                break;

                // Legs
            case EquipmentType.Legs:

                unequippedItem = player.playerInventoryManager.legEquipment;

                if (unequippedItem != null)
                {
                    player.playerInventoryManager.AddItemToInventory(unequippedItem);
                }

                player.playerInventoryManager.legEquipment = null;
                player.playerEquipmentManager.LoadLegEquipment(player.playerInventoryManager.legEquipment);

                break;


            // Hands
            case EquipmentType.Hands:

                unequippedItem = player.playerInventoryManager.handEquipment;

                if (unequippedItem != null)
                {
                    player.playerInventoryManager.AddItemToInventory(unequippedItem);
                }

                player.playerInventoryManager.handEquipment = null;
                player.playerEquipmentManager.LoadHandEquipment(player.playerInventoryManager.handEquipment);

                break;

            case EquipmentType.QuickSlot01:

                unequippedItem = player.playerInventoryManager.quickSlotItemInQuickSlots[0];

                if (unequippedItem != null)
                {
                    player.playerInventoryManager.AddItemToInventory(unequippedItem);
                }

                player.playerInventoryManager.quickSlotItemInQuickSlots[0] = null;
                if (player.playerInventoryManager.quickSlotItemIndex == 0)
                    player.playerNetworkManager.currentQuickSlotItemID.Value = -1;
                    break;

            case EquipmentType.QuickSlot02:

                unequippedItem = player.playerInventoryManager.quickSlotItemInQuickSlots[1];

                if (unequippedItem != null)
                {
                    player.playerInventoryManager.AddItemToInventory(unequippedItem);
                }

                player.playerInventoryManager.quickSlotItemInQuickSlots[1] = null;
                if (player.playerInventoryManager.quickSlotItemIndex == 1)
                    player.playerNetworkManager.currentQuickSlotItemID.Value = -1;
                break;

            case EquipmentType.QuickSlot03:

                unequippedItem = player.playerInventoryManager.quickSlotItemInQuickSlots[2];

                if (unequippedItem != null)
                {
                    player.playerInventoryManager.AddItemToInventory(unequippedItem);
                }

                player.playerInventoryManager.quickSlotItemInQuickSlots[2] = null;
                if (player.playerInventoryManager.quickSlotItemIndex == 2)
                    player.playerNetworkManager.currentQuickSlotItemID.Value = -1;
                break;


            default:
                break;
        }

        ReshesMenu();
    }
}
