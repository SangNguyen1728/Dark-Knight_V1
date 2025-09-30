using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using Unity.Netcode;

public class PlayerCombatManager : CharacterCombatManager
{
    PlayerManager player;

    public WeaponItem currentWeaponBeingUsed;

    [Header("Flags")]
    public bool canComboWithMainHandWeapon = false;
    public bool isUsingItem = false;
    public bool canRoll = true;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }
    public void PerformingWeaponBasedAction(WeaponItemAction weaponAction, WeaponItem weaponPeformingAction)
    {
        if(player.IsOwner)
        {
            // perform action
            weaponAction.AttpemtToPerformAction(player, weaponPeformingAction);

            player.playerNetworkManager.NotifyTheServerOfWeaponActionServerRpc(NetworkManager.Singleton.LocalClientId , weaponAction.actionID, weaponPeformingAction.itemID);
        }
        
    }

    public override void CloseAllDamageColliders()
    {
        base.CloseAllDamageColliders();

        player.playerEquipmentManager.rightWeaponManager.meleeDamageCollider.DisableDamageCollider();
        player.playerEquipmentManager.leftWeaponManager.meleeDamageCollider.DisableDamageCollider();
    }

    // Critical Attack
    public override void AttemptRiposte(RaycastHit hit)
    {
        CharacterManager targetCharacter = hit.transform.GetComponent<CharacterManager>();

        // If the character is null => return
        if (targetCharacter == null)
            return;

        // If some how since the initial check the character can no longer be riposted =>return
        if (!targetCharacter.characterNetworkManager.isRipostable.Value)
            return;

        // If sombody else is already perform a critical strike on the character (or i already am) => return
        if (targetCharacter.characterNetworkManager.isBeingCriticallyDamaged.Value)
            return;

        MeleeWeaponItem riposteWeapon;
        MeleeWeaponDamageColider riposteCollider;

        if (player.playerNetworkManager.isTwoHandingLeftWeapon.Value)
        {
            riposteWeapon = player.playerInventoryManager.currentLeftHandWeapon as MeleeWeaponItem;
            riposteCollider = player.playerEquipmentManager.leftWeaponManager.meleeDamageCollider;
        }
        else
        {

            riposteWeapon = player.playerInventoryManager.currentRightHandWeapon as MeleeWeaponItem;
            riposteCollider = player.playerEquipmentManager.rightWeaponManager.meleeDamageCollider;
        }
        //riposteWeapon = player.playerInventoryManager.currentRightHandWeapon as MeleeWeaponItem;
        //riposteCollider = player.playerEquipmentManager.rightWeaponManager.meleeDamageCollider;

        // The Animation of Ripsote will change based on weapon's animation controller, so can be choosen 
        character.characterAnimatorManager.PlayTargetActionAnimtionInstantly("Riposte_01", true);

        // Can not be Damaged when making a critical damage
        if(character.IsOwner)
            character.characterNetworkManager.isInvulnerable.Value = true;

        // 1. Cteate a new damage effect for this type of đamage
        TakeCriticalDamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeCriticalDamageEffect);

        // 2. Apply all of the damage stats from the collider to the damage effect
        damageEffect.physicalDamage = riposteCollider.physicalDamage;
        damageEffect.holyDamage = riposteCollider.holyDamage;
        damageEffect.fireDamage = riposteCollider.fireDamage;
        damageEffect.lightningDamage = riposteCollider.lightningDamage;
        damageEffect.magicDamage = riposteCollider.magicDamage;
        damageEffect.poiseDamage = riposteCollider.poiseDamage;

        // 3. Using a server RPC send the riposte to the target, where they will play the proper animations on their end, and take the damage 
        damageEffect.physicalDamage *= riposteWeapon.riposte_Attack_01_Modifier;
        damageEffect.holyDamage *= riposteWeapon.riposte_Attack_01_Modifier;
        damageEffect.fireDamage *= riposteWeapon.riposte_Attack_01_Modifier;
        damageEffect.lightningDamage *= riposteWeapon.riposte_Attack_01_Modifier;
        damageEffect.magicDamage *= riposteWeapon.riposte_Attack_01_Modifier;
        damageEffect.poiseDamage *= riposteWeapon.riposte_Attack_01_Modifier;

        // 4. Using a server Rpc send the Riposte to the target, where they will play the proper animations on their end, and take the damage
        targetCharacter.characterNetworkManager.NotifyTheServerOfRiposteServerRpc(
            targetCharacter.NetworkObjectId, 
            character.NetworkObjectId,
            "Riposted_01",
            riposteWeapon.itemID,
            damageEffect.physicalDamage,
            damageEffect.magicDamage,
            damageEffect.fireDamage,
            damageEffect.holyDamage,
            damageEffect.poiseDamage);
    }
    public override void AttemptBackStab(RaycastHit hit)
    {
        CharacterManager targetCharacter = hit.transform.GetComponent<CharacterManager>();

        // If the character is null => return
        if (targetCharacter == null)
            return;

        // If some how since the initial check the character can no longer be riposted =>return
        if (!targetCharacter.characterCombatManager.canBeBackStabbed)
            return;

        // If sombody else is already perform a critical strike on the character (or i already am) => return
        if (targetCharacter.characterNetworkManager.isBeingCriticallyDamaged.Value)
            return;

        MeleeWeaponItem backstabWeapon;
        MeleeWeaponDamageColider backstabCollider;

        if(player.playerNetworkManager.isTwoHandingLeftWeapon.Value)
        {
            backstabWeapon = player.playerInventoryManager.currentLeftHandWeapon as MeleeWeaponItem;
            backstabCollider = player.playerEquipmentManager.leftWeaponManager.meleeDamageCollider;
        }
        else
        {

            backstabWeapon = player.playerInventoryManager.currentRightHandWeapon as MeleeWeaponItem;
            backstabCollider = player.playerEquipmentManager.rightWeaponManager.meleeDamageCollider;
        }


            // The Animation of Ripsote will change based on weapon's animation controller, so can be choosen 
            character.characterAnimatorManager.PlayTargetActionAnimtionInstantly("Backstab_01", true);

        // Can not be Damaged when making a critical damage
        if (character.IsOwner)
            character.characterNetworkManager.isInvulnerable.Value = true;

        // 1. Cteate a new damage effect for this type of đamage
        TakeCriticalDamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeCriticalDamageEffect);

        // 2. Apply all of the damage stats from the collider to the damage effect
        damageEffect.physicalDamage = backstabCollider.physicalDamage;
        damageEffect.holyDamage = backstabCollider.holyDamage;
        damageEffect.fireDamage = backstabCollider.fireDamage;
        damageEffect.lightningDamage = backstabCollider.lightningDamage;
        damageEffect.magicDamage = backstabCollider.magicDamage;
        damageEffect.poiseDamage = backstabCollider.poiseDamage;

        // 3. Using a server RPC send the riposte to the target, where they will play the proper animations on their end, and take the damage 
        damageEffect.physicalDamage *= backstabWeapon.backstab_Attack_01_Modifier;
        damageEffect.holyDamage *= backstabWeapon.backstab_Attack_01_Modifier;
        damageEffect.fireDamage *= backstabWeapon.backstab_Attack_01_Modifier;
        damageEffect.lightningDamage *= backstabWeapon.backstab_Attack_01_Modifier;
        damageEffect.magicDamage *= backstabWeapon.backstab_Attack_01_Modifier;
        damageEffect.poiseDamage *= backstabWeapon.backstab_Attack_01_Modifier;

        // 4. Using a server Rpc send the Riposte to the target, where they will play the proper animations on their end, and take the damage
        targetCharacter.characterNetworkManager.NotifyTheServerOfBackstabServerRpc(
            targetCharacter.NetworkObjectId,
            character.NetworkObjectId,
            "Backstabbed_01",
            backstabWeapon.itemID,
            damageEffect.physicalDamage,
            damageEffect.magicDamage,
            damageEffect.fireDamage,
            damageEffect.holyDamage,
            damageEffect.poiseDamage);
    }
    public virtual void DrainStaminaBaseOnAttack()
    {
       
        if (!player.IsOwner)
            return;

        if(currentWeaponBeingUsed == null)
            return;

        float staminaDeducted = 0;

        //switch(currentAttackTpye)
        //{
        //    case AttackType.LightAttack01:
        //        staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStamiaCostMultiplier;
        //        break;
        //    default:
        //        break;
        //}

        switch(currentAttackTpye)
        {
            case AttackType.LightAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStamiaCostMultiplier;
                break;
            case AttackType.LightAttack02:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStamiaCostMultiplier;
                break;
            case AttackType.LightAttack03:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStamiaCostMultiplier;
                break;
            case AttackType.LightAttack04:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.lightAttackStamiaCostMultiplier;
                break;
            case AttackType.HeavyAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStamiaCostMultiplier;
                break;
            case AttackType.HeavyAttack02:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStamiaCostMultiplier;
                break;
            case AttackType.HeavyAttack03:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.heavyAttackStamiaCostMultiplier;
                break;
            case AttackType.ChargedAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargedAttackStamiaCostMultiplier;
                break;
            case AttackType.ChargedAttack02:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargedAttackStamiaCostMultiplier;
                break;
            case AttackType.ChargedAttack03:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.chargedAttackStamiaCostMultiplier;
                break;
            case AttackType.RunningAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.runningAttackStamiaCostMultiplier;
                break;
            case AttackType.RollingAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.rollingAttackStamiaCostMultiplier;
                break;
            case AttackType.BackstepAttack01:
                staminaDeducted = currentWeaponBeingUsed.baseStaminaCost * currentWeaponBeingUsed.backstepAttackStamiaCostMultiplier;
                break;
            default:
                break;
        }
      
        player.playerNetworkManager.currentStamina.Value -= Mathf.RoundToInt(staminaDeducted);
    }
    public override void SetTarget(CharacterManager newTarget)
    {
        base.SetTarget(newTarget);

        if(player.IsOwner)
        {
            PlayerCamera.instance.SetLockOnCameraTarget();
        }
    }
    //public void EnableCanDoCombo()
    //{
    //    if(player.playerNetworkManager.isUsingRightHand.Value)
    //    {
    //        canComboWithMainHandWeapon = true;
    //    }
    //    else
    //    {
    //        // do for off hand (To Do Later)
    //    }
    //}
    //public void DisableCanDoCombo()
    //{
    //    canComboWithMainHandWeapon = false;
    //}

    public WeaponItem SelectWeaponToPerformAshOfWar()
    {
        // ToDo: Select weapon depending on setup
         WeaponItem selectWeapon = player.playerInventoryManager.currentRightHandWeapon;

        player.playerNetworkManager.SetCharacterActionHand(false);
        player.playerNetworkManager.currentWeaponBeingUsed.Value = selectWeapon.itemID;

        return selectWeapon;
    }

    // Quick Slot
    public void SuccessfullyUseQuickSlotItem()
    {
       if(player.playerInventoryManager.currentQuickSlotItem != null)
            player.playerInventoryManager.currentQuickSlotItem.SuccessfullyUseItem(player);
    }
}
