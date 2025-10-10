using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class WorldItemDatabase : MonoBehaviour
{
    public static WorldItemDatabase Instance;

    public WeaponItem unarmedWeapon;

    public GameObject pickUpItemPrefab;

    [Header("Weapons")]
    [SerializeField] List<WeaponItem> weapons = new List<WeaponItem>();

    [Header("Head Equipment")]
    [SerializeField] List<HeadEquipmentItem> headEquipment = new List<HeadEquipmentItem>();

    [Header("Body Equipment")]
    [SerializeField] List<BodyEquipmentItem> bodyEquipment = new List<BodyEquipmentItem>();

    [Header("Hand Equipment")]
    [SerializeField] List<HandEquipmentItem> handEquipment = new List<HandEquipmentItem>();

    [Header("Leg Equipment")]
    [SerializeField] List<LegEquipmentItem> legEquipment = new List<LegEquipmentItem>();

    [Header("Ashes Of War")]
    [SerializeField] List<AshOfWar> ashesOfWar = new List<AshOfWar>();

    [Header("Quick Slot")]
    [SerializeField] List<QuickSlotItem> quickSlotItems = new List<QuickSlotItem>();

    //list of every items in game
    [Header("Items")]
    private List<Item> items = new List<Item>();
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        // Add all of weapons to the list of items
        foreach(var weapon in weapons)
        {
            items.Add(weapon);
        }

        // Add all of equipment (full set)
        foreach (var item in headEquipment)
        {
            items.Add(item);
        }

        foreach (var item in bodyEquipment)
        {
            items.Add(item);
        }

        foreach (var item in handEquipment)
        {
            items.Add(item);
        }

        foreach (var item in legEquipment)
        {
            items.Add(item);
        }

        foreach (var item in ashesOfWar)
        {
            items.Add(item);
        }

        // assign all of items a unique item id
        for (int i =0; i < items.Count; i++)
        {
            items[i].itemID = i;
        }

        foreach(var item in quickSlotItems)
        {
            items.Add(item);
        }
    }

    // Item Database
    public Item GetItemByID(int ID)
    {
        return items.FirstOrDefault(item => item.itemID == ID);
    }
    public WeaponItem GetWeaponByID(int ID)
    {
        return weapons.FirstOrDefault(weapons => weapons.itemID == ID);
    }

    public HeadEquipmentItem GetHeadEquipmentByID(int ID)
    {
        return headEquipment.FirstOrDefault(equipment => equipment.itemID == ID);
    }

    public BodyEquipmentItem GetBodyEquipmentByID(int ID)
    {
        return bodyEquipment.FirstOrDefault(equipment => equipment.itemID == ID);
    }

    public HandEquipmentItem GetHandEquipmentByID(int ID)
    {
        return handEquipment.FirstOrDefault(equipment => equipment.itemID == ID);
    }

    public LegEquipmentItem GetLegEquipmentByID(int ID)
    {
        return legEquipment.FirstOrDefault(equipment => equipment.itemID == ID);
    }

    public AshOfWar GetAshOfWarByID(int ID)
    {
        return ashesOfWar.FirstOrDefault(item => item.itemID == ID);
    }

    public QuickSlotItem GetQuickSlotItemByID(int ID)
    {
        return quickSlotItems.FirstOrDefault(item => item.itemID == ID);
    }

    // Item Serialization

    public WeaponItem GetWeaponFromSerializedData(SerializzableWeapon serializableWeapon)
    {
        WeaponItem weapon = null;
       
        if(GetWeaponByID(serializableWeapon.itemID))
            weapon = Instantiate(GetWeaponByID(serializableWeapon.itemID));

        if (weapon == null)
            return Instantiate(unarmedWeapon);

        if(GetAshOfWarByID(serializableWeapon.ashOfWarID))
        {
            AshOfWar ashOfWar = Instantiate(GetAshOfWarByID(serializableWeapon.ashOfWarID));
            weapon.ashOfWarAction = ashOfWar;
        }

        return weapon;
    }

    public FlaskItem GetFlaskFromSerializedData(SerializableFlasks serializableFlask)
    {
        FlaskItem flask = null;

        if (GetQuickSlotItemByID(serializableFlask.itemID))
        {
            flask = Instantiate(GetQuickSlotItemByID(serializableFlask.itemID)) as FlaskItem; 
        }

        return flask;
    }

    public QuickSlotItem GetQuickSlotItemFromSerializedData(SerializableQuickSlotItem serializableQuickSlotItem)
    {
        QuickSlotItem quickSlotItem = null;

        if (GetQuickSlotItemByID(serializableQuickSlotItem.itemID))
        {
            quickSlotItem = Instantiate(GetQuickSlotItemByID(serializableQuickSlotItem.itemID));
            quickSlotItem.itemAmount = serializableQuickSlotItem.itemAmount;
        }

        return quickSlotItem;
    }


}
