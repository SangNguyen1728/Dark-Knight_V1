using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
//since we want to reference this data for every save file
// this script is not a monobehavior and is instance serializable
public class CharacterSaveData 
{
    [Header("Scene Index")]
    public int SenceIndex = 1;

    [Header("Character Name")]
    public string CharacterName = "Character";

    [Header("Body Type")]
    public bool isMale = true;

    [Header("Time Played")]
    public float secondPlayed;

    // can not use vector3 for saving because only save data form basic variable type (float, int, string, bool)
    [Header("World Coordinates")]
    public float xPosition;
    public float yPosition;
    public float zPosition;

    [Header("Resources")]
    public int currentHealth;
    public float currentStamina;

    [Header("Stats")]
    public int vitality;
    public int endurance;


    [Header("Sites Of Grace")]
    public SerializableDictionary<int, bool> siteOfGrace; // int for site of grace ID. bool for "Activaved" status

    [Header("Bosses")]
    public SerializableDictionary<int, bool> bossesAwakened; // int for Boss ID, bool for Awakened status
    public SerializableDictionary<int, bool> bossesDefeated; // int for Boss ID, bool for Defeated status

    [Header("World Items")]
    public SerializableDictionary<int, bool> worldItemsLooted; // int for Item ID, bool for Looted status

    [Header("Equipment")]
    public int headEquipment;
    public int bodyEquipment;
    public int legEquipment;
    public int handEquipment;

    public int rightWeaponIndex;
    public SerializzableWeapon rightWeapon01;
    public SerializzableWeapon rightWeapon02;
    public SerializzableWeapon rightWeapon03;

    public int leftWeaponIndex;
    public SerializzableWeapon leftWeapon01;
    public SerializzableWeapon leftWeapon02;
    public SerializzableWeapon leftWeapon03;

    public int quickSlotIndex;
    public SerializableQuickSlotItem quickSlotItem01;
    public SerializableQuickSlotItem quickSlotItem02;
    public SerializableQuickSlotItem quickSlotItem03;

    [Header("Inventory")]
    public List<SerializzableWeapon> weaponsInInventory;
    public List<SerializableQuickSlotItem> quickSlotItemInInventory;
    public List<int> headEquipmentInInventory;
    public List<int> bodyEquipmentInInventory;
    public List<int> handEquipmentInInventory;
    public List<int> legEquipmentInInventory;
    


    public int currentHealthFlaskRemaining = 3;           // For Health 
    //public int currentForcusPointFlaskRemaining = 1;    // For Forcus Point(ToDo in future)

    public CharacterSaveData()
    {
        siteOfGrace = new SerializableDictionary<int, bool>(); // int => Site of Grace Id, bool for site of grace "Activated" status
        bossesAwakened = new SerializableDictionary<int, bool>();
        bossesDefeated = new SerializableDictionary<int, bool>();
        worldItemsLooted = new SerializableDictionary<int, bool>();

        weaponsInInventory = new List<SerializzableWeapon>();
        quickSlotItemInInventory = new List<SerializableQuickSlotItem>();
        headEquipmentInInventory = new List<int>();
        bodyEquipmentInInventory = new List<int>();
        handEquipmentInInventory = new List<int>();
        legEquipmentInInventory = new List<int>();
    }
}
