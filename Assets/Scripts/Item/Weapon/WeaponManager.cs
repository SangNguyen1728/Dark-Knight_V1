using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class WeaponManager : MonoBehaviour
{
   public MeleeWeaponDamageColider meleeDamageCollider;

    private void Awake()
    {
        meleeDamageCollider =  GetComponentInChildren<MeleeWeaponDamageColider>();
    }

    public void SetWeaponDamage(CharacterManager characterWieldingWeapon,WeaponItem weapon)
    {
        meleeDamageCollider.characterCausingDamage = characterWieldingWeapon;
        meleeDamageCollider.physicalDamage = weapon.physicalDamage;
        meleeDamageCollider.magicDamage = weapon.magicDamage;
        meleeDamageCollider.fireDamage = weapon.fireDamage;
        meleeDamageCollider.lightningDamage = weapon.lightningDamage;
        meleeDamageCollider.holyDamage = weapon.hollyDamage;
        meleeDamageCollider.poiseDamage = weapon.poiseDamage;

        // Light Attack
        meleeDamageCollider.light_Attack_01_Modifier = weapon.light_Attack_01_notifier;
        meleeDamageCollider.light_Attack_02_Modifier = weapon.light_Attack_02_notifier;
        meleeDamageCollider.light_Attack_03_Modifier = weapon.light_Attack_03_notifier;
        meleeDamageCollider.light_Attack_04_Modifier = weapon.light_Attack_04_notifier;

        // Heavy 
        meleeDamageCollider.heavy_Attack_01_Modifier = weapon.heavy_Attack_01_notifier;
        meleeDamageCollider.heavy_Attack_02_Modifier = weapon.heavy_Attack_02_notifier;
        meleeDamageCollider.heavy_Attack_03_Modifier = weapon.heavy_Attack_03_notifier;

        // Charge Damage
        meleeDamageCollider.charge_Attack_01_Modifier = weapon.charge_Attack_01_notifier;
        meleeDamageCollider.charge_Attack_02_Modifier = weapon.charge_Attack_02_notifier;
        meleeDamageCollider.charge_Attack_03_Modifier = weapon.charge_Attack_03_notifier;
        //meleeDamageCollider.charge_Attack_04_Modifier = weapon.charge_Attack_04_notifier;

        // Running
        meleeDamageCollider.running_Attack_01_Modifier = weapon.running_Attack_01_Modifier;

        // Rolling
        meleeDamageCollider.rolling_Attack_01_Modifier = weapon.rolling_Attack_01_Modifier;

        // Backstep
        meleeDamageCollider.backstep_Attack_01_Modifier = weapon.backstep_Attack_01_Modifier;
    }
}
