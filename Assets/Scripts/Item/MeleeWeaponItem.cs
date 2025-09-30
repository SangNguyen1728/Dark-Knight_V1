using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Items/ Weapon/ Melee Weapon")]
public class MeleeWeaponItem : WeaponItem
{
    [Header("Attack Notifiers")]
    public float riposte_Attack_01_Modifier = 3.3f;
    public float backstab_Attack_01_Modifier = 3.3f;
    // weapon deflection (if the weapon will bounce off another weapon then it is being guarded against)

    // can be buffed
}
