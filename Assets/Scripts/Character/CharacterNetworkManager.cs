using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class CharacterNetworkManager : NetworkBehaviour
{
    CharacterManager character;

    [Header("IsActive")]
    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Position")]
    public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<Quaternion> networkRotaion = new NetworkVariable<Quaternion>(Quaternion.identity, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public Vector3 networkPositionVelocity;
    public float networkPositionSmoothTime = 0.1f;
    public float networkRotationSmoothTime = 0.1f;

    [Header("Animator")]
    public NetworkVariable<bool> isMoving = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> horizontalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> verticalMovement = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> moveAmount = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Target")]
    public NetworkVariable<ulong> currentTargetNetworkObjectID = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Flags")]
    public NetworkVariable<bool> isBlocking = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isParrying = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isParryable = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isAttacking = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isInvulnerable = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isLockedOn = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isSprinting = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isJumping = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isChargingAttack = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isRipostable = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isBeingCriticallyDamaged = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Resources")]
    public NetworkVariable<int> currentHealth = new NetworkVariable<int>(400, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxhealth = new NetworkVariable<int>(400, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<float> currentStamina = new NetworkVariable<float>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> maxStamina = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Stats")]
    public NetworkVariable<int> vitality = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> endurance = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> strength = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Stats Modifiers")]
    public NetworkVariable<int> strengthModifier = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }
    // Send request from client to server(in case: host): serverrpc
    // Send information from the server(in case: host) to all clients: clientrpc
    // Bandwidth Limits
    // Do not send too much data continuously through ServerRpc to avoid lag.

    // a client rpc í sent to all client present, from the server
    public virtual void CheckHP(int oldValue, int newValue)
    {
        if(currentHealth.Value <= 0)
        {
            StartCoroutine(character.ProcessDeathEvent());
        }

        // prevent me form over 
        if(character.IsOwner)
        {
            if(currentHealth.Value > maxhealth.Value)
            {
                currentHealth.Value = maxhealth.Value;
                //maxhealth.Value = currentHealth.Value;
            }
           
            
        }
    }

    public virtual void OnIsDeadChanged(bool oldStatus, bool newStatus)
    {
        character.animator.SetBool("IsDead", character.isDead.Value);
    }

    public void OnLockOnTargetIDChange(ulong oldID, ulong newID)
    {
        if(!IsOwner)
        {
            character.characterCombatManager.currentTarget = NetworkManager.Singleton.SpawnManager.SpawnedObjects[newID].gameObject.GetComponent<CharacterManager>();
        }
    }

    public void OnIsLockedOnChanged(bool old, bool isLockOn)
    { 
        if(!isLockOn)
        {
            character.characterCombatManager.currentTarget = null;
        }
    }
    public void OnIsChargingAttackCharged(bool oldStatus, bool newStatus)
    {
        character.animator.SetBool("isChargingAttack", isChargingAttack.Value);
    }
    public void OnIsMovingChanged(bool oldStatus, bool newStatus)
    {
        character.animator.SetBool("isMoving", isMoving.Value);
    }
    public virtual void OnIsActiveChanged(bool oldStatus, bool newStatus)
    {
        gameObject.SetActive(isActive.Value);
    }

    public virtual void OnIsBlockingChanged(bool oldStatus, bool newStatus)
    {
        //if (isBlocking.Value)
        //{
        //    character.animator.SetBool("isBlocking", isBlocking.Value);
        //}

        character.animator.SetBool("isBlocking", isBlocking.Value);

       
    }

    [ServerRpc]
    public virtual void DestroyAllCurrentActionFXServerRpc()
    {
        if(IsServer)
        {
            DestroyAllCurrentActionFXClientRpc();
        }

    }

    [ClientRpc]

    public virtual void  DestroyAllCurrentActionFXClientRpc()
    {
        if(character.characterEffectsManager.activeQuickSlotItemFX != null)
            Destroy(character.characterEffectsManager.activeQuickSlotItemFX);
    }

    [ServerRpc] //server
    // ulong: unsigned long
    public void NotifyTheServerOfActionAnimationServerRpc(ulong clientID, string aniamtionID, bool applyRootMotion)
    {
        // if this character is the host/ server, then activate the client RPC
        if(IsServer)
        {
            PlayerActionAnimationForAllClientsClientRpc(clientID, aniamtionID, applyRootMotion);
        }
    }

    [ClientRpc] // client
    public void PlayerActionAnimationForAllClientsClientRpc(ulong clientID, string aniamtionID, bool applyRootMotion)
    {

        // we make sure to not run the function on the character who sent it (so we dont play aniamtion clip twice)
        if(clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformActionAanimationFromServer(aniamtionID, applyRootMotion);
        }
    }
    private void PerformActionAanimationFromServer(string aniamtionID, bool applyRootMotion)
    {
        character.characterAnimatorManager.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(aniamtionID, 0.2f);
    }

    // Animation Attack
    
    [ServerRpc] //server
    // ulong: unsigned long
    public void NotifyTheServerOfAttackActionAnimationServerRpc(ulong clientID, string aniamtionID, bool applyRootMotion)
    {
        // if this character is the host/ server, then activate the client RPC
        if (IsServer)
        {
            PlayerAttackActionAnimationForAllClientsClientRpc(clientID, aniamtionID, applyRootMotion);
        }
    }

    [ClientRpc] // client
    public void PlayerAttackActionAnimationForAllClientsClientRpc(ulong clientID, string aniamtionID, bool applyRootMotion)
    {

        // we make sure to not run the function on the character who sent it (so we dont play aniamtion clip twice)
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformAttackActionAanimationFromServer(aniamtionID, applyRootMotion);
        }
    }
    private void PerformAttackActionAanimationFromServer(string aniamtionID, bool applyRootMotion)
    {
        character.characterAnimatorManager.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(aniamtionID, 0.2f);
    }



    // Animation Attack

    [ServerRpc] //server
    // ulong: unsigned long
    public void NotifyTheServerOfInstantAttackActionAnimationServerRpc(ulong clientID, string aniamtionID, bool applyRootMotion)
    {
        // if this character is the host/ server, then activate the client RPC
        if (IsServer)
        {
            PlayerInstantAttackActionAnimationForAllClientsClientRpc(clientID, aniamtionID, applyRootMotion);
        }
    }

    [ClientRpc] // client
    public void PlayerInstantAttackActionAnimationForAllClientsClientRpc(ulong clientID, string aniamtionID, bool applyRootMotion)
    {

        // we make sure to not run the function on the character who sent it (so we dont play aniamtion clip twice)
        if (clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformInstantAttackActionAanimationFromServer(aniamtionID, applyRootMotion);
        }
    }
    private void PerformInstantAttackActionAanimationFromServer(string aniamtionID, bool applyRootMotion)
    {
        character.characterAnimatorManager.applyRootMotion = applyRootMotion;
        character.animator.Play(aniamtionID);
    }


    // Damage
    [ServerRpc(RequireOwnership = false)]
    public void NotifyTheServerOfCharacterDamageServerRpc(
        ulong damagedCharacter,
        ulong characterCausingDamage,
        float physicalDamage,
        float magicDamage,
        float fireDamage,
        float holyDamage,
        float poiseDamage,
        float anglehitFrom,
        float contactPointX,
        float contactPointY,
        float contactPointZ)
    {
        if(IsServer)
        {
            NotifyTheServerOfCharacterDamageClientRpc(damagedCharacter, characterCausingDamage, physicalDamage, magicDamage, fireDamage, holyDamage, poiseDamage, anglehitFrom, contactPointX, contactPointY, contactPointZ);
        }
    }

    [ClientRpc]
    public void NotifyTheServerOfCharacterDamageClientRpc(
        ulong damagedCharacterID,
        ulong characterCausingDamage,
        float physicalDamage,
        float magicDamage,
        float fireDamage,
        float holyDamage,
        float poiseDamage,
        float anglehitFrom,
        float contactPointX,
        float contactPointY,
        float contactPointZ)
    {
        ProcessCharacterDamageFromServer(
            damagedCharacterID, characterCausingDamage, physicalDamage, magicDamage, fireDamage, holyDamage, poiseDamage, anglehitFrom, contactPointX, contactPointY, contactPointZ);
    }

    public void ProcessCharacterDamageFromServer(
        ulong damagedCharacterID,
        ulong characterCausingDamageID,
        float physicalDamage,
        float magicDamage,
        float fireDamage,
        float holyDamage,
        float poiseDamage,
        float anglehitFrom,
        float contactPointX,
        float contactPointY,
        float contactPointZ)
    {
        CharacterManager damagedCharacter = NetworkManager.Singleton.SpawnManager.SpawnedObjects[damagedCharacterID].gameObject.GetComponent<CharacterManager>();
        CharacterManager characterCausingDamage = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterCausingDamageID].gameObject.GetComponent<CharacterManager>(); ;
        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeDamageEffect);

        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.poiseDamage = poiseDamage;
        damageEffect.angleHitFrom = anglehitFrom;
        damageEffect.contactPoint = new Vector3(contactPointX, contactPointY, contactPointZ);
        damageEffect.characterCausingDamage = characterCausingDamage;

        damagedCharacter.characterEffectsManager.ProcessInstantEffects(damageEffect);
    }

    // Critical Damage (Riposte)
    [ServerRpc(RequireOwnership = false)]
    public void NotifyTheServerOfRiposteServerRpc(
        ulong damagedCharacter,
        ulong characterCausingDamage,
        string criticalDamageAnimation,
        int weaponID,
        float physicalDamage,
        float magicDamage,
        float fireDamage,
        float holyDamage,
        float poiseDamage
        )
    {
        if (IsServer)
        {
            NotifyTheServerOfRiposteClientRpc(
                damagedCharacter, 
                characterCausingDamage, 
                criticalDamageAnimation,
                weaponID,
                physicalDamage, 
                magicDamage, 
                fireDamage, 
                holyDamage, 
                poiseDamage);
        }
    }

    [ClientRpc]
    public void NotifyTheServerOfRiposteClientRpc(
        ulong damagedCharacterID,
        ulong characterCausingDamage,
        string criticalDamageAnimation,
        int weaponID,
        float physicalDamage,
        float magicDamage,
        float fireDamage,
        float holyDamage,
        float poiseDamage
        )
    {
        ProcessRiposteFromServer(
            damagedCharacterID, 
            characterCausingDamage, 
            criticalDamageAnimation,
            weaponID,
            physicalDamage, 
            magicDamage, 
            fireDamage, 
            holyDamage, 
            poiseDamage);
    }

    public void ProcessRiposteFromServer(
        ulong damagedCharacterID,
        ulong characterCausingDamageID,
        string criticalDamageAnimation,
        int weaponID,
        float physicalDamage,
        float magicDamage,
        float fireDamage,
        float holyDamage,
        float poiseDamage
        )
    {
        CharacterManager damagedCharacter = NetworkManager.Singleton.SpawnManager.SpawnedObjects[damagedCharacterID].gameObject.GetComponent<CharacterManager>();
        CharacterManager characterCausingDamage = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterCausingDamageID].gameObject.GetComponent<CharacterManager>(); ;
        WeaponItem weapon = WorldItemDatabase.Instance.GetWeaponByID(weaponID);
        TakeCriticalDamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeCriticalDamageEffect);

        if (damagedCharacter.IsOwner)
            damagedCharacter.characterNetworkManager.isBeingCriticallyDamaged.Value = true;

        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.poiseDamage = poiseDamage;
       
        damageEffect.characterCausingDamage = characterCausingDamage;

        damagedCharacter.characterEffectsManager.ProcessInstantEffects(damageEffect);
        damagedCharacter.characterAnimatorManager.PlayTargetActionAnimtionInstantly(criticalDamageAnimation, true);

        
        // Move the enemy to the proper Riposte position
        StartCoroutine(damagedCharacter.characterCombatManager.ForceMoveEnemyCharacterToRipostePosition(
            characterCausingDamage, WorldUtilityManager.Instance.GetRipostingPositionBasedOnWeaponClass(weapon.weaponClass)));
        // Todo: Get different position depending on weapon for animation versions
    }

    // Critical Damage (Backstab)
    [ServerRpc(RequireOwnership = false)]
    public void NotifyTheServerOfBackstabServerRpc(
        ulong damagedCharacter,
        ulong characterCausingDamage,
        string criticalDamageAnimation,
        int weaponID,
        float physicalDamage,
        float magicDamage,
        float fireDamage,
        float holyDamage,
        float poiseDamage
        )
    {
        if (IsServer)
        {
            NotifyTheServerOfBackstabClientRpc(
                damagedCharacter,
                characterCausingDamage,
                criticalDamageAnimation,
                weaponID,
                physicalDamage,
                magicDamage,
                fireDamage,
                holyDamage,
                poiseDamage);
        }
    }

    [ClientRpc]
    public void NotifyTheServerOfBackstabClientRpc(
        ulong damagedCharacterID,
        ulong characterCausingDamage,
        string criticalDamageAnimation,
        int weaponID,
        float physicalDamage,
        float magicDamage,
        float fireDamage,
        float holyDamage,
        float poiseDamage
        )
    {
        ProcessBackstabFromServer(
            damagedCharacterID,
            characterCausingDamage,
            criticalDamageAnimation,
            weaponID,
            physicalDamage,
            magicDamage,
            fireDamage,
            holyDamage,
            poiseDamage);
    }

    public void ProcessBackstabFromServer(
        ulong damagedCharacterID,
        ulong characterCausingDamageID,
        string criticalDamageAnimation,
        int weaponID,
        float physicalDamage,
        float magicDamage,
        float fireDamage,
        float holyDamage,
        float poiseDamage
        )
    {
        CharacterManager damagedCharacter = NetworkManager.Singleton.SpawnManager.SpawnedObjects[damagedCharacterID].gameObject.GetComponent<CharacterManager>();
        CharacterManager characterCausingDamage = NetworkManager.Singleton.SpawnManager.SpawnedObjects[characterCausingDamageID].gameObject.GetComponent<CharacterManager>(); ;
        WeaponItem weapon = WorldItemDatabase.Instance.GetWeaponByID(weaponID);
        TakeCriticalDamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeCriticalDamageEffect);

        if (damagedCharacter.IsOwner)
            damagedCharacter.characterNetworkManager.isBeingCriticallyDamaged.Value = true;

        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.poiseDamage = poiseDamage;

        damageEffect.characterCausingDamage = characterCausingDamage;

        damagedCharacter.characterEffectsManager.ProcessInstantEffects(damageEffect);
        damagedCharacter.characterAnimatorManager.PlayTargetActionAnimtionInstantly(criticalDamageAnimation, true);


        // Move backstab target to the postiton of the back stabber
        StartCoroutine(characterCausingDamage.characterCombatManager.ForceMoveEnemyCharacterToBackstabPosition(
            damagedCharacter, WorldUtilityManager.Instance.GetBackstabbPositionBasedOnWeaponClass(weapon.weaponClass)));
        // Todo: Get different position depending on weapon for animation versions
    }

    // Parry
    [ServerRpc(RequireOwnership = false)]
    public void NotifyServerOfParryServerRpc(ulong parriedClientID)
    {
        if(IsServer)
        {
            NotifyServerOfParryClientRpc(parriedClientID);
        }
    }

    [ClientRpc]
    protected void NotifyServerOfParryClientRpc(ulong parriedClientID)
    {
        ProcessParryFromServer(parriedClientID);
    }

    protected void ProcessParryFromServer(ulong parriedClientID)
    {
        CharacterManager parriedCharacter = 
            NetworkManager.Singleton.SpawnManager.SpawnedObjects[parriedClientID].gameObject.GetComponent<CharacterManager>();

        if (parriedCharacter == null)
            return;

       if(parriedCharacter.IsOwner)
        {
            parriedCharacter.characterAnimatorManager.PlayTargetActionAnimtionInstantly("Parried_01", true);
        }
    }
}
