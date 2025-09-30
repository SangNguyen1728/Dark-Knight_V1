using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using Unity.Collections;
using UnityEngine.Rendering.Universal;

public class PlayerNetworkManager : CharacterNetworkManager
{
    PlayerManager player;

    public NetworkVariable<FixedString64Bytes> characterName =new NetworkVariable<FixedString64Bytes>("character",NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    [Header("Flasks")]
    public NetworkVariable<int> remainingHealthFlasks = new NetworkVariable<int>(3, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> remainingManaFlasks = new NetworkVariable<int>(3, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isChugging = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Equipment")]
    public NetworkVariable<int> currentWeaponBeingUsed = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> currentRightHandWeaponID= new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> currentLeftHandWeaponID= new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isUsingRightHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isUsingLeftHand = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> currentQuickSlotItemID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Two Handing")]
    public NetworkVariable<int> currentWeaponBeingTwoHanded = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isTwoHandingWeapon = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isTwoHandingRightWeapon = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> isTwoHandingLeftWeapon = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("Armor")]
    public NetworkVariable<bool> isMale = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> headEquipmentID = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> bodyEquipmentID = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> handEquipmentID = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<int> legEquipmentID = new NetworkVariable<int>(-1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }
    public void SetCharacterActionHand(bool rightHandAction)
    {
        if(rightHandAction)
        {
            isUsingLeftHand.Value = false;
            isUsingRightHand.Value = true;
        }
        else
        {
            isUsingRightHand.Value = false;
            isUsingLeftHand.Value = true;
        }
    }

    public void SetNewMaxHealthValue(int oldVitality, int newVitality)
    {
        maxhealth.Value = player.playerStatsManager.CalculateHealthBaseOnVitalityLevel(newVitality);
        PlayerUIManager.instance.playerHudManager.SetMaxHealthValue(maxhealth.Value);
        currentHealth.Value = maxhealth.Value;
    }
   
    public void SetNewMaxStaminaValue(int oldEndurance, int newEndurance)
    {
        maxStamina.Value = player.playerStatsManager.CalculateStaminaBaseOnEnduranceLevel(newEndurance);
        PlayerUIManager.instance.playerHudManager.SetMaxStaminaValue(maxStamina.Value);
        currentStamina.Value = maxStamina.Value;
    }

    public void OnCurrentRightHandWeaponIDChange(int oldID, int  newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID));
        player.playerInventoryManager.currentRightHandWeapon = newWeapon;
        player.playerEquipmentManager.LoadRightHandWeapon();

        if(player.IsOwner)
        {
            PlayerUIManager.instance.playerHudManager.SetRightWeaponQuickSlotIcon(newID);
        }
    }
    public void OnCurrentLeftHandWeaponIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID));
        player.playerInventoryManager.currentLeftHandWeapon = newWeapon;
        player.playerEquipmentManager.LoadLeftHandWeapon();

        if (player.IsOwner)
        {
            PlayerUIManager.instance.playerHudManager.SetLeftWeaponQuickSlotIcon(newID);
        }
    }
    public void OnCurrentWeaponBeingUsedIDChange(int oldID, int newID)
    {
        WeaponItem newWeapon = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(newID));
        player.playerCombatManager.currentWeaponBeingUsed = newWeapon;

        // i do not need to run this if i am the owner because i've already done so locally
        if (player.IsOwner)
            return;

        if (player.playerCombatManager.currentWeaponBeingUsed != null)
            player.playerAnimatorManager.UpdateAnimatorController(player.playerCombatManager.currentWeaponBeingUsed.weaponAnimator); 
    }
    public override void OnIsBlockingChanged(bool oldStatus, bool newStatus)
    {
        base.OnIsBlockingChanged(oldStatus, newStatus);

        if (IsOwner)
        {
            player.playerStatsManager.blockingPhysicalAbsorption = player.playerCombatManager.currentWeaponBeingUsed.physicalBaseDamageAbsorption;
            player.playerStatsManager.blockingMagicAbsorption = player.playerCombatManager.currentWeaponBeingUsed.magicBaseDamageAbsorption;
            player.playerStatsManager.blockingFireAbsorption = player.playerCombatManager.currentWeaponBeingUsed.fireBaseDamageAbsorption;
            player.playerStatsManager.blockingLightningAbsorption = player.playerCombatManager.currentWeaponBeingUsed.lightningBaseDamageAbsorption;
            player.playerStatsManager.blockingHolyAbsorption = player.playerCombatManager.currentWeaponBeingUsed.holyBaseDamageAbsorption;
            player.playerStatsManager.blockingStability = player.playerCombatManager.currentWeaponBeingUsed.stability;
        }
    }
    public void OnIsTwoHandingWeaponChanged(bool oldStatus, bool newStatus)
    {
        if(!isTwoHandingWeapon.Value)
        {
            if(IsOwner)
            {
                isTwoHandingLeftWeapon.Value = false;
                isTwoHandingRightWeapon.Value = false;
            }

            player.playerEquipmentManager.UnTwoHandWeapon();
            player.playerEffectsManager.RemoveStaticEffect(WorldCharacterEffectManager.instance.twoHandingEffect.staticEffectID);
        }
        else
        {
            StaticCharacterEffect twoHandEffect = Instantiate(WorldCharacterEffectManager.instance.twoHandingEffect);
            player.playerEffectsManager.AddStaticEffect(twoHandEffect);
        }
        player.animator.SetBool("IsTwoHandWeapon", isTwoHandingWeapon.Value);
    }

    public void OnIsChuggingChanged(bool oldStatus, bool  newStatus)
    {
        player.animator.SetBool("IsChuggingFlask", isChugging.Value);
    }

    public void OnHeadEquipmentChanged(int oldValue, int newValue)
    {
        if (IsOwner)
            return;

        HeadEquipmentItem equipment = WorldItemDatabase.Instance.GetHeadEquipmentByID(headEquipmentID.Value);

        if(equipment != null)
        {
            player.playerEquipmentManager.LoadHeadEquipment(Instantiate(equipment));    
        }
        else
        {
            player.playerEquipmentManager.LoadHeadEquipment(null);
        }
    }

    public void OnBodyEquipmentChanged(int oldValue, int newValue)
    {
        if (IsOwner)
            return;

        BodyEquipmentItem equipment = WorldItemDatabase.Instance.GetBodyEquipmentByID(bodyEquipmentID.Value);

        if (equipment != null)
        {
            player.playerEquipmentManager.LoadBodyEquipment(Instantiate(equipment));
        }
        else
        {
            player.playerEquipmentManager.LoadBodyEquipment(null);
        }
    }

    public void OnHandEquipmentChanged(int oldValue, int newValue)
    {
        if (IsOwner)
            return;

        HandEquipmentItem equipment = WorldItemDatabase.Instance.GetHandEquipmentByID(handEquipmentID.Value);

        if (equipment != null)
        {
            player.playerEquipmentManager.LoadHandEquipment(Instantiate(equipment));
        }
        else
        {
            player.playerEquipmentManager.LoadHandEquipment(null);
        }
    }
    public void OnLegEquipmentChanged(int oldValue, int newValue)
    {
        if (IsOwner)
            return;

        LegEquipmentItem equipment = WorldItemDatabase.Instance.GetLegEquipmentByID(legEquipmentID.Value);

        if (equipment != null)
        {
            player.playerEquipmentManager.LoadLegEquipment(Instantiate(equipment));
        }
        else
        {
            player.playerEquipmentManager.LoadLegEquipment(null);
        }
    }
    public void OnIsTwoHandingRightWeaponChanged(bool oldStatus, bool newStatus)
    {
        if (!isTwoHandingRightWeapon.Value)
            return;

        if(IsOwner)
        {
            currentWeaponBeingTwoHanded.Value = currentRightHandWeaponID.Value;
            isTwoHandingWeapon.Value = true;
        }
        
        player.playerInventoryManager.currentTwoHandWeapon = player.playerInventoryManager.currentRightHandWeapon;
        player.playerEquipmentManager.TwoHandRightWeapon();
    }
    public void OnIsTwoHandingLeftWeaponChanged(bool oldStatus, bool newStatus)
    {
        if (!isTwoHandingLeftWeapon.Value)
            return;

        if(IsOwner)
        {
            currentWeaponBeingTwoHanded.Value = currentLeftHandWeaponID.Value;
            isTwoHandingWeapon.Value = true;
        }
       
        player.playerInventoryManager.currentTwoHandWeapon = player.playerInventoryManager.currentLeftHandWeapon;
        player.playerEquipmentManager.TwoHandLeftWeapon();
    }

    public void OnCurrentQuickSlotItemIDChange(int oldID,  int newID)
    {
        QuickSlotItem newQuickSlotItem = null;

        if (WorldItemDatabase.Instance.GetQuickSlotItemByID(newID))
            newQuickSlotItem = Instantiate(WorldItemDatabase.Instance.GetQuickSlotItemByID(newID));

        if(newQuickSlotItem != null)
        {
            player.playerInventoryManager.currentQuickSlotItem = newQuickSlotItem;
        }
        else
        {
            player.playerInventoryManager.currentQuickSlotItem = null;
        }

        if (player.IsOwner)
            PlayerUIManager.instance.playerHudManager.SetQuickSlotItemQuickSlotIcon(newID);

    }

    public void OnIsMaleChanged(bool oldStatus, bool newStatus)
    {
        player.playerBodyManager.ToggleBodyType(isMale.Value);
    }

    // item action
    [ServerRpc]
    public void NotifyTheServerOfWeaponActionServerRpc(ulong clientID, int actionID, int weaponID)
    {
        if(IsServer)
        {
            NotifyTheServerOfWeaponActionClientRpc(clientID, actionID, weaponID);
        }
    }
    [ClientRpc]
    public void NotifyTheServerOfWeaponActionClientRpc(ulong clientID, int actionID, int weaponID)
    {
        // i idont want to play action agian for the character who called it, it already played in locally
        if(clientID != NetworkManager.Singleton.LocalClientId)
        {
            PerformWeaponBasedAction(actionID, weaponID);
        }
    }
    private void PerformWeaponBasedAction(int actionID, int weaponID)
    {
        WeaponItemAction weaponAction  =  WorldActionManager.instance.GetWeaponItemActionByID(actionID);

        if(weaponAction != null)
        {
            weaponAction.AttpemtToPerformAction(player, WorldItemDatabase.Instance.GetWeaponByID(actionID));
        }
        else
        {
            Debug.LogError("action null, can not perform");
        }
    }


    [ServerRpc]
    public void HideWeaponServerRpc()
    {
        if (IsServer)
            HideWeaponClientRpc();
    }
    [ClientRpc]
    private void HideWeaponClientRpc()
    {

        if(player.playerEquipmentManager.rightHandWeaponModel != null)
            player.playerEquipmentManager.rightHandWeaponModel.SetActive(false);

        if (player.playerEquipmentManager.leftHandWeaponModel != null)
            player.playerEquipmentManager.leftHandWeaponModel.SetActive(false);
    }

    [ServerRpc]
    public void NotifyServerOfQuickSlotItemActionServerRpc(ulong clientID, int quickSlotItemID)
    {
        NotifyServerOfQuickSlotItemActionClientRpc(clientID, quickSlotItemID);
    }

    [ClientRpc]
    private void NotifyServerOfQuickSlotItemActionClientRpc(ulong clientID, int quickSlotItemID)
    {
        if(clientID != NetworkManager.Singleton.LocalClientId)
        {
            QuickSlotItem item = WorldItemDatabase.Instance.GetQuickSlotItemByID(quickSlotItemID);
            item.AttemptToUseItem(player);
        }
    }
}
