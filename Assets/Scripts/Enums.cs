using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enums : MonoBehaviour
{
}

// For Character Saving
public enum CharacterSlot
{
    CharacterSlot_01,
    CharacterSlot_02,
    CharacterSlot_03,
    CharacterSlot_04,   
    CharacterSlot_05,
    CharacterSlot_06,
    CharacterSlot_07,
    CharacterSlot_08,
    CharacterSlot_09,
    CharacterSlot_10,
    NO_SLOT
}

// For processing damage, character target
public enum CharacterGroup
{
    Team01,
    Team02,
    Team03,
    Team04,
    Team05,
}

// For each weapon model instantiation slot
public enum WeaponModelSlot
{
    RightHand,
    LeftHandWeaponSlot,
    LeftHandShieldSlot,
    BackSlot
}

// TO know where to instantiatie weapon model base type
public enum WeaponModelType
{
    Weapon,
    Shield
}

// For any information specific weapon class
public enum WeaponClass
{
    KatanaBlue,
    LightningTwinBlades,
    TwinBlades,
    Shield,
    Fist
}

// For calculating damage base on atk type
public enum AttackType
{
    LightAttack01,
    LightAttack02,
    LightAttack03,
    LightAttack04,
    HeavyAttack01,
    HeavyAttack02,
    HeavyAttack03,
    ChargedAttack01,
    ChargedAttack02,
    ChargedAttack03,
    ChargedAttack04,
    RunningAttack01,
    RollingAttack01,
    BackstepAttack01
}

// For tagging equipment model with specific body parts that will cover
public enum EquipmentModelType
{
    FullHelmet,
    Hat,
    Hood,
    HelmetAccessories,
    FaceCover,
    Torso,
    Back,
    RightShoulder,
    RightUpperArm,
    RightElbow,
    RightLowArm,
    RightHand,
    LeftShoulder,
    LeftUpperArm,
    LeftElbow,
    LeftLowerArm,
    LeftHand,
    Hips,
    HipsAttachment,
    RightLeg,
    RightKnee,
    LeftLeg,
    LeftKnee,
}

// For determine which equipment slot is currrently selected(Armor and weapon)
public enum EquipmentType
{
    RightWeapon01,          // 0
    RightWeapon02,          // 1
    RightWeapon03,          // 2
    LeftWeapon01,           // 3
    LeftWeapon02,           // 4
    LeftWeapon03,           // 5
    Head,                   // 6
    Body,                   // 7
    Legs,                   // 8
    Hands,                  // 9
    MainProjectile,         // 10
    SecondaryProjectile,    // 11
    QuickSlot01,            // 12
    QuickSlot02,            // 13
    QuickSlot03,            // 14
}

// For Helmets type, so sepcific head portion can be hidden during equip process
public enum HeadEquipmentType
{
    FullHelmet, // Hide all face
    Hat,        // Not hide anything
    Hood,       // Hide hair
    FaceCover   // hide lips/ beard
}

// For calculating damage animation intensity
public enum DamageIntensity
{
    Ping,
    Light,
    Medium,
    Heavy,
    Colossal
}

// For determine item pick up type
public enum ItemPickUpType
{
    WorldSpawn,
    CharacterDrop,
}