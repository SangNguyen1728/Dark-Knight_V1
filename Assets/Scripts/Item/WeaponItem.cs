using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponItem : EquipmentItem
{
    // animator controller override to change attack animation based on weapon i am using
    [Header("Animator")]
    public AnimatorOverrideController weaponAnimator;

    [Header("Model Instantiation")]
    public WeaponModelType weaponModelType;

    [Header("Weapon Model")]
    public GameObject weaponModel;

    [Header("Weapon Class")]
    public WeaponClass weaponClass;

    [Header("Weapon Requirements")]
    public int strengthREQ = 0;
    public int dexREQ = 0;
    public int intREQ = 0;
    public int faithREQ = 0;

    [Header("Weapon Base Damage")]
    public int physicalDamage = 0;
    public int magicDamage = 0;
    public int fireDamage = 0;
    public int hollyDamage = 0;
    public int lightningDamage = 0;

    // weapon guard absorption(block power)

    [Header("Weapon Poise Damage")]
    public float poiseDamage = 10;

    [Header("Attack Notifiers")]
    // weapon notifiers
    public float light_Attack_01_notifier = 1.0f;
    public float light_Attack_02_notifier = 1.1f;
    public float light_Attack_03_notifier = 1.2f;
    public float light_Attack_04_notifier = 1.3f;
    public float heavy_Attack_01_notifier = 1.4f;
    public float heavy_Attack_02_notifier = 1.5f;
    public float heavy_Attack_03_notifier = 1.6f;
    public float charge_Attack_01_notifier = 2.0f;
    public float charge_Attack_02_notifier = 2.1f;
    public float charge_Attack_03_notifier = 2.2f;
    public float running_Attack_01_Modifier = 1.1f;
    public float rolling_Attack_01_Modifier = 1.1f;
    public float backstep_Attack_01_Modifier = 1.1f;
    



    [Header("Stamina Cost Notifiers")]
    public int baseStaminaCost = 20;
    // Light attack stamina cost notifier
    public float lightAttackStamiaCostMultiplier = 1.0f;
    // Heavy attaack stamina cost notifer
    public float heavyAttackStamiaCostMultiplier = 1.3f;
    // Charged Attack stamina cost notifier
    public float chargedAttackStamiaCostMultiplier = 1.5f;
    // Running attack stamina cost notifier
    public float runningAttackStamiaCostMultiplier = 1.1f;
    // Rolling attack stamina cost notifier
    public float rollingAttackStamiaCostMultiplier = 1.1f;
    //  Backstep attack stamina cost notifier
    public float backstepAttackStamiaCostMultiplier = 1.1f;

    [Header("Weapon Blocking Absorption")]
    public float physicalBaseDamageAbsorption = 50;
    public float magicBaseDamageAbsorption = 50;
    public float fireBaseDamageAbsorption = 50;
    public float holyBaseDamageAbsorption = 50;
    public float lightningBaseDamageAbsorption = 50;
    public float stability = 50; // Redusces stamina lost form block


    // item base action
    [Header("Action")]
    public WeaponItemAction oh_RB_Action;       // 1 hand right bumper action (will fix when find animation 1 hand)
    public WeaponItemAction oh_RT_Action;       // 1 hand right trigger action (will fix when find animation 1 hand)
    public WeaponItemAction oh_LB_Action;
    public AshOfWar ashOfWarAction;             // ashes of war

    // blocking sound
    [Header("SFX")]
    public AudioClip[] whooshes;
    public AudioClip[] blockingSFX;
}
