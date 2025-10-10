using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using Unity.Netcode;

public class PlayerManager : CharacterManager
{
    //[Header("Debug")]
    //[SerializeField] bool respawnCharacter = false;
    //[SerializeField] bool switchRightWeapon = false;

    [HideInInspector] public PlayerAnimatorManager playerAnimatorManager;
    [HideInInspector] public PlayerLocomotionManager playerLocomotionManager;
    [HideInInspector] public PlayerNetworkManager playerNetworkManager;
    [HideInInspector] public PlayerStatsManager playerStatsManager;
    [HideInInspector] public PlayerInventoryManager playerInventoryManager;
    [HideInInspector] public PlayerEquipmentManager playerEquipmentManager;
    [HideInInspector] public PlayerCombatManager playerCombatManager;
    [HideInInspector] public PlayerInteractableManager playerInteractableManager;
    [HideInInspector] public PlayerEffectsManager playerEffectsManager;
    [HideInInspector] public PlayerBodyManager playerBodyManager;
    protected override void Awake()
    {
        base.Awake();

        // only for the player

        playerLocomotionManager = GetComponent<PlayerLocomotionManager>();
        playerAnimatorManager = GetComponent<PlayerAnimatorManager>();
        playerNetworkManager = GetComponent<PlayerNetworkManager>();
        playerStatsManager = GetComponent<PlayerStatsManager>();
        playerInventoryManager = GetComponent<PlayerInventoryManager>();
        playerEquipmentManager = GetComponent<PlayerEquipmentManager>();
        playerCombatManager = GetComponent<PlayerCombatManager>();
        playerInteractableManager = GetComponent<PlayerInteractableManager>();
        playerEffectsManager = GetComponent<PlayerEffectsManager>();
        playerBodyManager = GetComponent<PlayerBodyManager>();
    }
    protected override void Update()
    {
        base.Update();

        // if do not own this gameobject, can not control or edit it
        if (!IsOwner)
            return;

        //Handle Movement
        playerLocomotionManager.HandleAllMovement();

        // regen stamina
        playerStatsManager.RegenerateStamina();

        //DebugMenu();
    }
    protected override void LateUpdate()
    {
        if (!IsOwner) 
            return;

        base.LateUpdate();

        PlayerCamera.instance.HandleAllCameraActions();
    }
    protected override void OnEnable()
    {
        base.OnEnable();

       
    }
    protected override void OnDisable()
    {
        base.OnDisable();

        
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallBack;
        // if this is the player object owned by this client
        if (IsOwner)
        {
            PlayerCamera.instance.player = this;
            PlayerInputManager.instance.player = this;
            WorldSaveGameManager.instance.player = this;

            // update the total amount of health or stamina when stat limited to either changes
            playerNetworkManager.vitality.OnValueChanged += playerNetworkManager.SetNewMaxHealthValue;
            playerNetworkManager.endurance.OnValueChanged += playerNetworkManager.SetNewMaxStaminaValue;

            //updates UI stat bars when stat changes (health or stamina)
            //playerNetWorkManager.currentHealth.OnValueChanged += playerNetWorkManager.CheckHP;
            playerNetworkManager.currentHealth.OnValueChanged += PlayerUIManager.instance.playerHudManager.SetNewHealthValue;

            playerNetworkManager.currentStamina.OnValueChanged += PlayerUIManager.instance.playerHudManager.SetNewStaminaValue;
            playerNetworkManager.currentStamina.OnValueChanged += playerStatsManager.ResetStaminaRegenTimer;
        }
        // stats
        playerNetworkManager.currentHealth.OnValueChanged += playerNetworkManager.CheckHP;

        // Body Type
        playerNetworkManager.isMale.OnValueChanged += playerNetworkManager.OnIsMaleChanged;

        // Only update floating HPBar if this character is not the local player. character
        if (!IsOwner)
        {
            characterNetworkManager.currentHealth.OnValueChanged += characterUIManager.OnHPCharged;
        }

        // Lock on
        playerNetworkManager.isLockedOn.OnValueChanged += playerNetworkManager.OnIsLockedOnChanged;
        playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged += playerNetworkManager.OnLockOnTargetIDChange;

        // Equipment
        playerNetworkManager.currentRightHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentRightHandWeaponIDChange;
        playerNetworkManager.currentLeftHandWeaponID.OnValueChanged += playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
        playerNetworkManager.currentWeaponBeingUsed.OnValueChanged += playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
        playerNetworkManager.isBlocking.OnValueChanged += playerNetworkManager.OnIsBlockingChanged;
        playerNetworkManager.currentQuickSlotItemID.OnValueChanged += playerNetworkManager.OnCurrentQuickSlotItemIDChange;
        playerNetworkManager.isChugging.OnValueChanged += playerNetworkManager.OnIsChuggingChanged;

        // Armor
        playerNetworkManager.headEquipmentID.OnValueChanged += playerNetworkManager.OnHeadEquipmentChanged;
        playerNetworkManager.bodyEquipmentID.OnValueChanged += playerNetworkManager.OnBodyEquipmentChanged;
        playerNetworkManager.handEquipmentID.OnValueChanged += playerNetworkManager.OnHandEquipmentChanged;
        playerNetworkManager.legEquipmentID.OnValueChanged += playerNetworkManager.OnLegEquipmentChanged;

        // Two Hand
        playerNetworkManager.isTwoHandingWeapon.OnValueChanged += playerNetworkManager.OnIsTwoHandingWeaponChanged;
        playerNetworkManager.isTwoHandingRightWeapon.OnValueChanged += playerNetworkManager.OnIsTwoHandingRightWeaponChanged;
        playerNetworkManager.isTwoHandingLeftWeapon.OnValueChanged += playerNetworkManager.OnIsTwoHandingLeftWeaponChanged;

        // Flag
        playerNetworkManager.isChargingAttack.OnValueChanged += playerNetworkManager.OnIsChargingAttackCharged;

        // if i am the ower but not the server, reload my character data to this newly instantiated character
        if(IsOwner && !IsServer)
        {
            LoadGameFromCurrentCharacterData(ref WorldSaveGameManager.instance.currentCharacterData);
        }
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnectedCallBack;
        // if this is the player object owned by this client
        if (IsOwner)
        {
            // update the total amount of health or stamina when stat limited to either changes
            playerNetworkManager.vitality.OnValueChanged -= playerNetworkManager.SetNewMaxHealthValue;
            playerNetworkManager.endurance.OnValueChanged -= playerNetworkManager.SetNewMaxStaminaValue;

            //updates UI stat bars when stat changes (health or stamina)
            //playerNetWorkManager.currentHealth.OnValueChanged -= playerNetWorkManager.CheckHP;
            playerNetworkManager.currentHealth.OnValueChanged -= PlayerUIManager.instance.playerHudManager.SetNewHealthValue;

            playerNetworkManager.currentStamina.OnValueChanged -= PlayerUIManager.instance.playerHudManager.SetNewStaminaValue;
            playerNetworkManager.currentStamina.OnValueChanged -= playerStatsManager.ResetStaminaRegenTimer;
        }
        // stats
        playerNetworkManager.currentHealth.OnValueChanged -= playerNetworkManager.CheckHP;

        // Body Type
        playerNetworkManager.isMale.OnValueChanged -= playerNetworkManager.OnIsMaleChanged;

        if (!IsOwner)
        {
            characterNetworkManager.currentHealth.OnValueChanged -= characterUIManager.OnHPCharged;
        }

        // Lock on
        playerNetworkManager.isLockedOn.OnValueChanged -= playerNetworkManager.OnIsLockedOnChanged;
        playerNetworkManager.currentTargetNetworkObjectID.OnValueChanged -= playerNetworkManager.OnLockOnTargetIDChange;

        // equipment
        playerNetworkManager.currentRightHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentRightHandWeaponIDChange;
        playerNetworkManager.currentLeftHandWeaponID.OnValueChanged -= playerNetworkManager.OnCurrentLeftHandWeaponIDChange;
        playerNetworkManager.currentWeaponBeingUsed.OnValueChanged -= playerNetworkManager.OnCurrentWeaponBeingUsedIDChange;
        playerNetworkManager.currentQuickSlotItemID.OnValueChanged -= playerNetworkManager.OnCurrentQuickSlotItemIDChange;
        playerNetworkManager.isChugging.OnValueChanged -= playerNetworkManager.OnIsChuggingChanged;


        // Armor
        playerNetworkManager.headEquipmentID.OnValueChanged -= playerNetworkManager.OnHeadEquipmentChanged;
        playerNetworkManager.bodyEquipmentID.OnValueChanged -= playerNetworkManager.OnBodyEquipmentChanged;
        playerNetworkManager.handEquipmentID.OnValueChanged -= playerNetworkManager.OnHandEquipmentChanged;
        playerNetworkManager.legEquipmentID.OnValueChanged -= playerNetworkManager.OnLegEquipmentChanged;

        // Two Hand
        playerNetworkManager.isTwoHandingWeapon.OnValueChanged -= playerNetworkManager.OnIsTwoHandingWeaponChanged;
        playerNetworkManager.isTwoHandingRightWeapon.OnValueChanged -= playerNetworkManager.OnIsTwoHandingRightWeaponChanged;
        playerNetworkManager.isTwoHandingLeftWeapon.OnValueChanged -= playerNetworkManager.OnIsTwoHandingLeftWeaponChanged;

        // Flag
        playerNetworkManager.isChargingAttack.OnValueChanged -= playerNetworkManager.OnIsChargingAttackCharged;

    }
    private void OnClientConnectedCallBack(ulong clientID)
    {
        // keep list of active players in game
        WorldGameSessionManager.instance.AddPlayerToActivePlayersList(this);

        // if i am in server, i am the host, so i do not need to load player to sync them
        // just need to load other players gear to sync it if i join a game that already been active without i am present
        if(!IsServer && IsOwner)
        {
            foreach(var player in WorldGameSessionManager.instance.players)
            {
               if(player != this)
                {
                    player.LoadOtherPlayerCharacterWhenJoiningServer();
                }
            }
        }
    }
    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        if(IsOwner)
        {
            PlayerUIManager.instance.playerUIPopUpManager.SendYouDiedPopUp();
        }

        return base.ProcessDeathEvent(manuallySelectDeathAnimation);
    }
    public override void ReviveCharacter()
    {
        base.ReviveCharacter();

        isDead.Value = false;

        if (IsOwner)
        {
            isDead.Value = false;

            playerNetworkManager.currentHealth.Value = playerNetworkManager.maxhealth.Value;
            playerNetworkManager.currentStamina.Value = playerNetworkManager.maxStamina.Value;

            // play rebirth effect
            playerAnimatorManager.PlayTargetActionAnimtion("Empty", false);
        }
    }
    public void SaveGameDataToCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
       currentCharacterData.SenceIndex = SceneManager.GetActiveScene().buildIndex;

        currentCharacterData.CharacterName = playerNetworkManager.characterName.Value.ToString();
        currentCharacterData.isMale = playerNetworkManager.isMale.Value;
        currentCharacterData.xPosition = transform.position.x;
        currentCharacterData.yPosition = transform.position.y;
        currentCharacterData.zPosition = transform.position.z;

        currentCharacterData.currentHealth = playerNetworkManager.currentHealth.Value;
        currentCharacterData.currentStamina = playerNetworkManager.currentStamina.Value;

        currentCharacterData.vitality = playerNetworkManager.vitality.Value;
        currentCharacterData.endurance = playerNetworkManager.endurance.Value;

        currentCharacterData.currentHealthFlaskRemaining = playerNetworkManager.remainingHealthFlasks.Value;
        //currentCharacterData.currentForcusPointFlaskRemaining = playerNetworkManager.remainingForcusPointFlasks.Value;

        // Equipment
        currentCharacterData.headEquipment = playerNetworkManager.headEquipmentID.Value;
        currentCharacterData.bodyEquipment = playerNetworkManager.bodyEquipmentID.Value;
        currentCharacterData.handEquipment = playerNetworkManager.handEquipmentID.Value;
        currentCharacterData.legEquipment = playerNetworkManager.legEquipmentID.Value;

        currentCharacterData.rightWeaponIndex = playerInventoryManager.rightHandWeaponIndex;
        currentCharacterData.rightWeapon01 = WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(playerInventoryManager.weaponInRightHandSlot[0]);
        currentCharacterData.rightWeapon02 = WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(playerInventoryManager.weaponInRightHandSlot[1]);
        currentCharacterData.rightWeapon03 = WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(playerInventoryManager.weaponInRightHandSlot[2]);

        currentCharacterData.leftWeaponIndex = playerInventoryManager.leftHandWeaponIndex;
        currentCharacterData.leftWeapon01 = WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(playerInventoryManager.weaponInLeftHandSlot[0]);
        currentCharacterData.leftWeapon02 = WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(playerInventoryManager.weaponInLeftHandSlot[1]);
        currentCharacterData.leftWeapon03 = WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(playerInventoryManager.weaponInLeftHandSlot[2]);

        currentCharacterData.quickSlotIndex = playerInventoryManager.quickSlotItemIndex;
        currentCharacterData.quickSlotItem01 = WorldSaveGameManager.instance.GetSerializableQuickSlotItemFromQuickSlotItem(playerInventoryManager.quickSlotItemInQuickSlots[0]);
        currentCharacterData.quickSlotItem02 = WorldSaveGameManager.instance.GetSerializableQuickSlotItemFromQuickSlotItem(playerInventoryManager.quickSlotItemInQuickSlots[1]);
        currentCharacterData.quickSlotItem03 = WorldSaveGameManager.instance.GetSerializableQuickSlotItemFromQuickSlotItem(playerInventoryManager.quickSlotItemInQuickSlots[2]);

        // Clear list before save
        currentCharacterData.weaponsInInventory = new List<SerializzableWeapon>();
        currentCharacterData.quickSlotItemInInventory = new List<SerializableQuickSlotItem>();
        currentCharacterData.headEquipmentInInventory = new List<int>();
        currentCharacterData.bodyEquipmentInInventory = new List<int>();
        currentCharacterData.handEquipmentInInventory = new List<int>();
        currentCharacterData.legEquipmentInInventory = new List<int>();


        for(int i = 0; i < playerInventoryManager.itemsInInventory.Count; i++)
        {
            if(playerInventoryManager.itemsInInventory[i] == null) 
                continue;

            WeaponItem weaponInInventory = playerInventoryManager.itemsInInventory[i] as WeaponItem;
            HeadEquipmentItem headEquipmentInInventory = playerInventoryManager.itemsInInventory[i] as HeadEquipmentItem;
            BodyEquipmentItem bodyEquipmentInInventory = playerInventoryManager.itemsInInventory[i] as BodyEquipmentItem;
            HandEquipmentItem handEquipmentInInventory = playerInventoryManager.itemsInInventory[i] as HandEquipmentItem;
            LegEquipmentItem legEquipmentInInventory = playerInventoryManager.itemsInInventory[i] as LegEquipmentItem;
            QuickSlotItem quickSlotItemInInventory = playerInventoryManager.itemsInInventory[i] as QuickSlotItem;

            if (weaponInInventory != null)
                currentCharacterData.weaponsInInventory.Add(WorldSaveGameManager.instance.GetSerializableWeaponFromWeaponItem(weaponInInventory));

            if(headEquipmentInInventory != null)
                currentCharacterData.headEquipmentInInventory.Add(headEquipmentInInventory.itemID);

            if (bodyEquipmentInInventory != null)
                currentCharacterData.bodyEquipmentInInventory.Add(bodyEquipmentInInventory.itemID);

            if (handEquipmentInInventory != null)
                currentCharacterData.handEquipmentInInventory.Add(handEquipmentInInventory.itemID);

            if (legEquipmentInInventory != null)
                currentCharacterData.legEquipmentInInventory.Add(legEquipmentInInventory.itemID);

            if (quickSlotItemInInventory != null)
                currentCharacterData.quickSlotItemInInventory.Add(WorldSaveGameManager.instance.GetSerializableQuickSlotItemFromQuickSlotItem(quickSlotItemInInventory));
        }
    }
    public void LoadGameFromCurrentCharacterData(ref CharacterSaveData currentCharacterData)
    {
        playerNetworkManager.characterName.Value = currentCharacterData.CharacterName;
        playerNetworkManager.isMale.Value = currentCharacterData.isMale;
        playerBodyManager.ToggleBodyType(currentCharacterData.isMale);
        Vector3 myPosition = new Vector3(currentCharacterData.xPosition, currentCharacterData.yPosition, currentCharacterData.zPosition);
        transform.position = myPosition;

         playerNetworkManager.vitality.Value = currentCharacterData.vitality;
         playerNetworkManager.endurance.Value = currentCharacterData.endurance;

        playerNetworkManager.remainingHealthFlasks.Value = currentCharacterData.currentHealthFlaskRemaining;
        //playerNetworkManager.remainingForcusPointFlasks.Value = currentCharacterData.currentForcusPointFlaskRemaining; // For ForcusPoint (Todo in future)

        // this willl be moved when saving and loading is added
        playerNetworkManager.maxhealth.Value = playerStatsManager.CalculateHealthBaseOnVitalityLevel(playerNetworkManager.vitality.Value);
        playerNetworkManager.maxStamina.Value = playerStatsManager.CalculateStaminaBaseOnEnduranceLevel(playerNetworkManager.endurance.Value);

        playerNetworkManager.currentHealth.Value = currentCharacterData.currentHealth;
        playerNetworkManager.currentStamina.Value = currentCharacterData.currentStamina;

        PlayerUIManager.instance.playerHudManager.SetMaxStaminaValue(playerNetworkManager.maxStamina.Value);

        // Equipment

        if(WorldItemDatabase.Instance.GetHeadEquipmentByID(currentCharacterData.headEquipment))
        {
            HeadEquipmentItem headEquipment = Instantiate(WorldItemDatabase.Instance.GetHeadEquipmentByID(currentCharacterData.headEquipment));
            playerInventoryManager.headEquipment = headEquipment;
        }
        else
        {
            playerInventoryManager.headEquipment = null;
        }

        if(WorldItemDatabase.Instance.GetBodyEquipmentByID(currentCharacterData.bodyEquipment))
        {
            BodyEquipmentItem bodyEquipment = Instantiate(WorldItemDatabase.Instance.GetBodyEquipmentByID(currentCharacterData.bodyEquipment));
            playerInventoryManager.bodyEquipment = bodyEquipment;
        }
        else
        {
            playerInventoryManager.bodyEquipment = null;
        }

        if(WorldItemDatabase.Instance.GetHandEquipmentByID(currentCharacterData.handEquipment))
        {
            HandEquipmentItem handEquipment = Instantiate(WorldItemDatabase.Instance.GetHandEquipmentByID(currentCharacterData.handEquipment));
            playerInventoryManager.handEquipment = handEquipment;
        }
        else
        {
            playerInventoryManager.handEquipment = null;
        }

        if(WorldItemDatabase.Instance.GetLegEquipmentByID(currentCharacterData.legEquipment))
        {
            LegEquipmentItem legEquipment = Instantiate(WorldItemDatabase.Instance.GetLegEquipmentByID(currentCharacterData.legEquipment));
            playerInventoryManager.legEquipment = legEquipment;
        }
        else
        {
            playerInventoryManager.legEquipment = null;
        }

        playerInventoryManager.rightHandWeaponIndex = currentCharacterData.rightWeaponIndex;
        playerInventoryManager.weaponInRightHandSlot[0] = currentCharacterData.rightWeapon01.GetWeapon();
        playerInventoryManager.weaponInRightHandSlot[1] = currentCharacterData.rightWeapon02.GetWeapon();
        playerInventoryManager.weaponInRightHandSlot[2] = currentCharacterData.rightWeapon03.GetWeapon();

        //if ((WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.rightWeapon01)))
        //{
        //    WeaponItem rightWeapon01 = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.rightWeapon01));
        //    playerInventoryManager.weaponInRightHandSlot[0] = rightWeapon01;
        //}
        //else
        //{
        //    playerInventoryManager.weaponInRightHandSlot[0] = null;
        //}

        //if ((WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.rightWeapon02)))
        //{
        //    WeaponItem rightWeapon02 = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.rightWeapon02));
        //    playerInventoryManager.weaponInRightHandSlot[1] = rightWeapon02;
        //}
        //else
        //{
        //    playerInventoryManager.weaponInRightHandSlot[1] = null;
        //}

        //if ((WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.rightWeapon03)))
        //{
        //    WeaponItem rightWeapon03 = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.rightWeapon03));
        //    playerInventoryManager.weaponInRightHandSlot[2] = rightWeapon03;
        //}
        //else
        //{
        //    playerInventoryManager.weaponInRightHandSlot[2] = null;
        //}

        playerInventoryManager.leftHandWeaponIndex = currentCharacterData.leftWeaponIndex;
        playerInventoryManager.weaponInLeftHandSlot[0] = currentCharacterData.leftWeapon01.GetWeapon();
        playerInventoryManager.weaponInLeftHandSlot[1] = currentCharacterData.leftWeapon02.GetWeapon();
        playerInventoryManager.weaponInLeftHandSlot[2] = currentCharacterData.leftWeapon03.GetWeapon();

        //if ((WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.leftWeapon01)))
        //{
        //    WeaponItem leftWeapon01 = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.leftWeapon01));
        //    playerInventoryManager.weaponInLeftHandSlot[0] = leftWeapon01;
        //}
        //else
        //{
        //    playerInventoryManager.weaponInLeftHandSlot[0] = null;
        //}

        //if ((WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.leftWeapon02)))
        //{
        //    WeaponItem leftWeapon02 = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.leftWeapon02));
        //    playerInventoryManager.weaponInLeftHandSlot[1] = leftWeapon02;
        //}
        //else
        //{
        //    playerInventoryManager.weaponInLeftHandSlot[1] = null;
        //}

        //if ((WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.leftWeapon03)))
        //{
        //    WeaponItem leftWeapon03 = Instantiate(WorldItemDatabase.Instance.GetWeaponByID(currentCharacterData.leftWeapon03));
        //    playerInventoryManager.weaponInLeftHandSlot[2] = leftWeapon03;
        //}
        //else
        //{
        //    playerInventoryManager.weaponInLeftHandSlot[2] = null;
        //}

        // QuickSlot
        playerInventoryManager.quickSlotItemIndex = currentCharacterData.quickSlotIndex;
        playerInventoryManager.quickSlotItemInQuickSlots[0] = currentCharacterData.quickSlotItem01.GetQuickSlotItem();
        playerInventoryManager.quickSlotItemInQuickSlots[1] = currentCharacterData.quickSlotItem02.GetQuickSlotItem();
        playerInventoryManager.quickSlotItemInQuickSlots[2] = currentCharacterData.quickSlotItem03.GetQuickSlotItem();
        playerEquipmentManager.LoadQuickSlotEquipment(playerInventoryManager.quickSlotItemInQuickSlots[playerInventoryManager.quickSlotItemIndex]); //  will refesh HUD

        playerInventoryManager.rightHandWeaponIndex = currentCharacterData.rightWeaponIndex;

        if (currentCharacterData.rightWeaponIndex >= 0)
        {
            playerInventoryManager.currentRightHandWeapon = playerInventoryManager.weaponInRightHandSlot[currentCharacterData.rightWeaponIndex];
            playerNetworkManager.currentRightHandWeaponID.Value = playerInventoryManager.weaponInRightHandSlot[currentCharacterData.rightWeaponIndex].itemID;
        }
        else
        {
            playerNetworkManager.currentRightHandWeaponID.Value = WorldItemDatabase.Instance.unarmedWeapon.itemID;
        }

        playerInventoryManager.leftHandWeaponIndex = currentCharacterData.leftWeaponIndex;

        if (currentCharacterData.leftWeaponIndex >= 0)
        {
            playerInventoryManager.currentLeftHandWeapon = playerInventoryManager.weaponInLeftHandSlot[currentCharacterData.leftWeaponIndex];
            playerNetworkManager.currentLeftHandWeaponID.Value = playerInventoryManager.weaponInLeftHandSlot[currentCharacterData.leftWeaponIndex].itemID;
        }
        else
        {
            playerNetworkManager.currentLeftHandWeaponID.Value = WorldItemDatabase.Instance.unarmedWeapon.itemID;
        }

        for(int i = 0; i < currentCharacterData.weaponsInInventory.Count; i++)
        {
            WeaponItem weapon = currentCharacterData.weaponsInInventory[i].GetWeapon();
            playerInventoryManager.AddItemToInventory(weapon);
        }

        for(int  i = 0; i < currentCharacterData.headEquipmentInInventory.Count; i++)
        {
            EquipmentItem equipment = WorldItemDatabase.Instance.GetHeadEquipmentByID(currentCharacterData.headEquipmentInInventory[i]);
            playerInventoryManager.AddItemToInventory(equipment);
        }

        for (int i = 0; i < currentCharacterData.bodyEquipmentInInventory.Count; i++)
        {
            EquipmentItem equipment = WorldItemDatabase.Instance.GetBodyEquipmentByID(currentCharacterData.bodyEquipmentInInventory[i]);
            playerInventoryManager.AddItemToInventory(equipment);
        }

        for (int i = 0; i < currentCharacterData.handEquipmentInInventory.Count; i++)
        {
            EquipmentItem equipment = WorldItemDatabase.Instance.GetHandEquipmentByID(currentCharacterData.handEquipmentInInventory[i]);
            playerInventoryManager.AddItemToInventory(equipment);
        }

        for (int i = 0; i < currentCharacterData.legEquipmentInInventory.Count; i++)
        {
            EquipmentItem equipment = WorldItemDatabase.Instance.GetLegEquipmentByID(currentCharacterData.legEquipmentInInventory[i]);
            playerInventoryManager.AddItemToInventory(equipment);
        }

        for (int i = 0; i < currentCharacterData.quickSlotItemInInventory.Count; i++)
        {
            QuickSlotItem quickSlotItem = currentCharacterData.quickSlotItemInInventory[i].GetQuickSlotItem();
            playerInventoryManager.AddItemToInventory(quickSlotItem);
        }

        playerEquipmentManager.EquipArmor();

    }

    public void LoadOtherPlayerCharacterWhenJoiningServer()
    {
        // Body Type
        playerNetworkManager.OnIsMaleChanged(false, playerNetworkManager.isMale.Value);

        // Weapon
        playerNetworkManager.OnCurrentRightHandWeaponIDChange(0, playerNetworkManager.currentRightHandWeaponID.Value);
        playerNetworkManager.OnCurrentLeftHandWeaponIDChange(0, playerNetworkManager.currentLeftHandWeaponID.Value);

        // Sync Armor
        playerNetworkManager.OnHeadEquipmentChanged(0, playerNetworkManager.headEquipmentID.Value);
        playerNetworkManager.OnBodyEquipmentChanged(0, playerNetworkManager.bodyEquipmentID.Value);
        playerNetworkManager.OnHandEquipmentChanged(0, playerNetworkManager.handEquipmentID.Value);
        playerNetworkManager.OnLegEquipmentChanged(0, playerNetworkManager.legEquipmentID.Value);

        // Sync Two Hand status
        playerNetworkManager.OnIsTwoHandingRightWeaponChanged(false, playerNetworkManager.isTwoHandingRightWeapon.Value);
        playerNetworkManager.OnIsTwoHandingLeftWeaponChanged(false, playerNetworkManager.isTwoHandingLeftWeapon.Value);

        // Sync Block Status
        playerNetworkManager.OnIsBlockingChanged(false, playerNetworkManager.isBlocking.Value);

        // Amor

        // Lock On
        if(playerNetworkManager.isLockedOn.Value)
        {
            playerNetworkManager.OnLockOnTargetIDChange(0, playerNetworkManager.currentTargetNetworkObjectID.Value);
        }
    }

    //private void DebugMenu()
    //{
    //    if(respawnCharacter)
    //    {
    //        respawnCharacter = false;
    //        ReviveCharacter();
    //    }

    //    if(switchRightWeapon)
    //    {
    //        switchRightWeapon = false;
    //        playerEquipmentManager.SwitchRightWeapon();
    //    }
    //}
}
