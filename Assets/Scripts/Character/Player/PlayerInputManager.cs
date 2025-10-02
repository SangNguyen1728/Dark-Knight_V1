using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Netcode;

public class PlayerInputManager : MonoBehaviour
{
    public static PlayerInputManager instance;

    public PlayerManager player;

    PlayerControls playerControls;

    [Header("Player Movement input")]
    [SerializeField] Vector2 movementInput;
    public float verticalInput;
    public float horizontalInput;
    public float moveAmount;

    [Header("Camera Movement input")]
    [SerializeField] Vector2 cameraInput;
    public float cameraVerticalInput;
    public float cameraHorizontalInput;

    [Header("Look On Input")]
    [SerializeField] bool lockOn_Input;
    [SerializeField] bool lockOn_Left_Input;
    [SerializeField] bool lockOn_Right_Input;
    private Coroutine lockOnCoroutine;

    [Header("player action input")]
    [SerializeField] bool dodgeInput = false;
    [SerializeField] bool sprintInput = false;
    [SerializeField] bool jumpInput = false;
    [SerializeField] bool switch_Right_Weapon_Input = false;
    [SerializeField] bool switch_Left_Weapon_Input = false;
    [SerializeField] bool switch_Quick_Slot_Item_Input = false;
    [SerializeField] bool interaction_Input = false;
    [SerializeField] bool use_Item_Input = false;

    [Header("Bumper Inputs")]
    [SerializeField] bool RB_Input = false;
    [SerializeField] bool LB_Input = false;
 

    [Header("Trigger Inputs")]
    [SerializeField] bool RT_Input = false;
    [SerializeField] bool Hold_RT_Input = false;
    [SerializeField] bool LT_Input = false;

    [Header("Two Hand Inputs")]
    [SerializeField] bool two_Hand_Input = false;
    [SerializeField] bool two_Hand_Right_Weapon_Input = false;
    [SerializeField] bool two_Hand_Left_Weapon_Input = false;

    [Header("Qued Inputs")]
    [SerializeField] private bool input_Que_Is_Active = false;
    [SerializeField] float default_Que_Input_Time = 0.35f;
    [SerializeField] float que_Input_Timer = 0;
    [SerializeField] bool que_RB_Input = false;
    [SerializeField] bool que_RT_Input = false;

    [Header("UI Inputs")]
    [SerializeField] bool openCharacterMenuInput = false;
    [SerializeField] bool closeMenuInput = false;
    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);

        // when the scene changes, run this logic
        SceneManager.activeSceneChanged += OnSceneChange;

        instance.enabled = false;
        
        if(playerControls != null)
        {
            playerControls.Disable();
        }
    }
    private void OnSceneChange(Scene oldScene, Scene newScene)
    {
        // if we are loading into our world scene, enable our player controls
        if(newScene.buildIndex == WorldSaveGameManager.instance.GetWorldScenIndex())
        {
            instance.enabled = true;

            if (playerControls != null)
            {
                playerControls.Enable();
            }
        }
        // otherwise we must be at the main menu, disable our player controls
        // this is so my player cânt move around if i enter things like charater creation menu
        else
        {
            instance.enabled = false;

            if (playerControls != null)
            {
                playerControls.Disable();
            }
        }
    }

    private void OnEnable()
    {
        if(playerControls == null)
        {
            playerControls = new PlayerControls();

            playerControls.PlayerMovement.Movement.performed += i => movementInput = i.ReadValue<Vector2>();
            playerControls.PlayerCamera.Movement.performed += i => cameraInput = i.ReadValue<Vector2>();

            // action
            playerControls.PlayerAction.Dodge.performed += i => dodgeInput = true;
            playerControls.PlayerAction.Jump.performed += i => jumpInput = true;
            playerControls.PlayerAction.SwitchRightWeapon.performed += i => switch_Right_Weapon_Input = true;
            playerControls.PlayerAction.SwitchLeftWeapon.performed += i => switch_Left_Weapon_Input = true;
            playerControls.PlayerAction.SwitchQuickSlot.performed += i => switch_Quick_Slot_Item_Input = true;
            playerControls.PlayerAction.Interact.performed += i => interaction_Input = true;
            playerControls.PlayerAction.X.performed += i => use_Item_Input = true;

            // Bumpers
            playerControls.PlayerAction.RB.performed += i => RB_Input = true;
            playerControls.PlayerAction.LB.performed += i => LB_Input = true;
            playerControls.PlayerAction.LB.canceled += i => player.playerNetworkManager.isBlocking.Value = false;
            
            // triggers
            playerControls.PlayerAction.RT.performed += i => RT_Input = true;
            playerControls.PlayerAction.HoldRT.performed += i => Hold_RT_Input = true; 
            playerControls.PlayerAction.HoldRT.canceled += i => Hold_RT_Input = false;
            playerControls.PlayerAction.LT.performed += i => LT_Input = true;

            // Two Hand
            playerControls.PlayerAction.TwoHandWeapon.performed += i => two_Hand_Input = true;
            playerControls.PlayerAction.TwoHandWeapon.canceled += i => two_Hand_Input = false;
            playerControls.PlayerAction.TwoHandRightWeapon.performed += i => two_Hand_Right_Weapon_Input = true;
            playerControls.PlayerAction.TwoHandRightWeapon.canceled += i => two_Hand_Right_Weapon_Input = false;
            playerControls.PlayerAction.TwoHandLeftWeapon.performed += i => two_Hand_Left_Weapon_Input = true;
            playerControls.PlayerAction.TwoHandLeftWeapon.canceled += i => two_Hand_Left_Weapon_Input = false;

            // Look on
            playerControls.PlayerAction.LockOn.performed += i => lockOn_Input = true;
            playerControls.PlayerAction.SeekLeftLockOnTarget.performed += i => lockOn_Left_Input = true;
            playerControls.PlayerAction.SeekRightLockOnTarget.performed += i => lockOn_Right_Input = true;

            // holding the input, set bool to true
            playerControls.PlayerAction.Sprint.performed += i => sprintInput = true;
            // releasing the input, set bool to false
            playerControls.PlayerAction.Sprint.canceled += i => sprintInput = false;

            // Qued Inputs
            playerControls.PlayerAction.QueRB.performed += i => QueInput(ref que_RB_Input);
            playerControls.PlayerAction.QueRT.performed += i => QueInput(ref que_RT_Input);

            // UI Input
            playerControls.PlayerAction.Dodge.performed += i => closeMenuInput = true;
            playerControls.PlayerAction.OpenCharacterMenu.performed += i => openCharacterMenuInput = true;
        }

        playerControls.Enable();
    }
    private void OnDestroy()
    {
        // if we destroy this object, unsubscribe form this event
        SceneManager.activeSceneChanged -= OnSceneChange;
    }
    private void OnApplicationFocus(bool focus)
    {
        if(enabled)
        {
            if(focus)
            {
                playerControls.Enable();
            }
            else
            {
                playerControls.Disable();
            }
        }
    }
    private void Update()
    {
        HandleAllInputs();
    }
    private void HandleAllInputs()
    {
        HandleUseItemInput();
        HandleTwoHandInput();
        HandleLockOnInput();
        HandleLockOnSwitchTargetInput();
        HandlePlayerMovementInput();
        HandleCameraMovementInput();
        HandleDodgeInput();
        HandleSprinting();
        HandleJumpInput();
        HandleRBInput();
        HandleLBInput();
        HandleRTInput();
        HandleChargeRTInput();
        HandleLTInput();
        HandleSwitchRightWeaponInput();
        HandleSwitchLeftWeaponInput();
        HandleSwitcQuickSLotItemInput();
        HandleQuedInputs();
        HandleInteractionInput();
        HandleCloseUIInput();
        HandleOpenCharacterMenuInput();
    }

    // Two Hand
    private void HandleTwoHandInput()
    {
        if(!two_Hand_Input)
            { return; }

        if(two_Hand_Right_Weapon_Input)
        {
            // If I am using the 2 hand input and pressing the right two hand button I want to stop the regular RB Input (or else I would attack)
            RB_Input = false;
            two_Hand_Right_Weapon_Input = false;
            player.playerNetworkManager.isBlocking.Value = false;

            if(player.playerNetworkManager.isTwoHandingWeapon.Value)
            { 
                player.playerNetworkManager.isTwoHandingWeapon.Value = false;
                return;
            }
            else
            {
                player.playerNetworkManager.isTwoHandingRightWeapon.Value = true;
                return;
            }
        }
        else if (two_Hand_Left_Weapon_Input)
        {
            // If I am using the 2 hand input and pressing the right two hand button I want to stop the regular RB Input (or else I would attack)
            LB_Input = false;
            two_Hand_Left_Weapon_Input = false;
            player.playerNetworkManager.isBlocking.Value = false;

            if (player.playerNetworkManager.isTwoHandingWeapon.Value)
            { // If I am two handing weapon already, change the is twohanding bool to false which trigger an  "Onvaluechanged" funtion, which un_twohands current weapon
                // Which un_twohands current weapon
                player.playerNetworkManager.isTwoHandingWeapon.Value = false;
                return;
            }
            else
            {
                // If I am NOT already two handing, change the right hand bool to true,, triggers an onvaluechanged funtion
                // This funtion two hands the right weappon
                player.playerNetworkManager.isTwoHandingLeftWeapon.Value = true;
                return;
            }
        }
    }

    // Use Item
    private void HandleUseItemInput()
    {
        if(use_Item_Input)
        {
            use_Item_Input = false;

            if (PlayerUIManager.instance.menuWindowIsOpen)
            {
               
                return;
            }

            if(player.playerInventoryManager.currentQuickSlotItem != null)
            {
                player.playerInventoryManager.currentQuickSlotItem.AttemptToUseItem(player);

                // Send server PRC so my player Item action on other clients game windows
                player.playerNetworkManager.NotifyServerOfQuickSlotItemActionServerRpc
                    (NetworkManager.Singleton.LocalClientId, player.playerInventoryManager.currentQuickSlotItem.itemID);
            }
        }
    }

    // Lock on
    private void HandleLockOnInput()
    {
        // check for dead target
        if(player.playerNetworkManager.isLockedOn.Value)
        {
            if (player.playerCombatManager.currentTarget == null)
                return;
            if (player.playerCombatManager.currentTarget.isDead.Value)
            {
                player.playerNetworkManager.isLockedOn.Value = false;

                if (lockOnCoroutine != null)
                {
                    StopCoroutine(lockOnCoroutine);
                }

                lockOnCoroutine = StartCoroutine(PlayerCamera.instance.WaitThenFindNewTarget());
            }

            // Find new target 

            // this assures that the coroutine never runs multiple times overplapping itself
            
        }

        if(lockOn_Input && player.playerNetworkManager.isLockedOn.Value)
        {
            lockOn_Input = false;
            PlayerCamera.instance.ClearLockOnTargets();
            player.playerNetworkManager.isLockedOn.Value = false;

            // Disable lock on
            return;
        }

        if (lockOn_Input && !player.playerNetworkManager.isLockedOn.Value)
        {
            lockOn_Input = false;

            PlayerCamera.instance.HandleLocatingLockOnTargets();

            if(PlayerCamera.instance.nearestLockOnTarget !=  null)
            {
                // set target as my current target
                player.playerCombatManager.SetTarget(PlayerCamera.instance.nearestLockOnTarget);
                player.playerNetworkManager.isLockedOn.Value = true;
            }
        }
    }
    private void HandleLockOnSwitchTargetInput()
    {
        if(lockOn_Left_Input)
        {
            lockOn_Left_Input = false;

            if(player.playerNetworkManager.isLockedOn.Value)
            {
                PlayerCamera.instance.HandleLocatingLockOnTargets();    

                if(PlayerCamera.instance.leftLockOnTarget != null)
                {
                    player.playerCombatManager.SetTarget((PlayerCamera.instance.leftLockOnTarget));
                }
            }
        }

        if (lockOn_Right_Input)
        {
            lockOn_Right_Input = false;

            if (player.playerNetworkManager.isLockedOn.Value)
            {
                PlayerCamera.instance.HandleLocatingLockOnTargets();

                if (PlayerCamera.instance.rightLockOnTarget != null)
                {
                    player.playerCombatManager.SetTarget((PlayerCamera.instance.rightLockOnTarget));
                }
            }
        }
    }
    // Movement
    private void HandlePlayerMovementInput()
    {
        verticalInput = movementInput.y;
        horizontalInput = movementInput.x;

        moveAmount = Mathf.Clamp01(Mathf.Abs(verticalInput) + Mathf.Abs(horizontalInput));

        // we clamp the value
        if (moveAmount <= 0.5 && moveAmount > 0)
        {
            moveAmount = 0.5f;
        }
        else if(moveAmount > 0.5 && moveAmount <=1)
        {
            moveAmount = 1;
        }

        if(moveAmount != 0)
        {
            player.playerNetworkManager.isMoving.Value = true;
        }
        else
        {
            player.playerNetworkManager.isMoving.Value = false;
        }

        if (player == null)
            return;

        if(!player.playerNetworkManager.isLockedOn.Value || player.playerNetworkManager.isSprinting.Value)
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(0, moveAmount, player.playerNetworkManager.isSprinting.Value);
        }
        else
        {
            player.playerAnimatorManager.UpdateAnimatorMovementParameters(horizontalInput, verticalInput, player.playerNetworkManager.isSprinting.Value);
        }
        
    }
    private void HandleCameraMovementInput()
    {
        cameraVerticalInput = cameraInput.y;
        cameraHorizontalInput = cameraInput.x;


    }

    // Actions
    private void HandleDodgeInput()
    {
        if(dodgeInput)
        {
            dodgeInput = false;
            // return nothing if menu or ui window is open
            player.playerLocomotionManager.AttemptToPerformDodge();

        }
    }
    private void HandleSprinting()
    {
        if(sprintInput)
        {
            player.playerLocomotionManager.HandleSprinting();
        }
        else
        {
            player.playerNetworkManager.isSprinting.Value = false;
        }
    }
    private void HandleJumpInput()
    {
        if(jumpInput)
        {
            jumpInput = false;

            if(PlayerUIManager.instance.menuWindowIsOpen)
            {
                return;
            }

            player.playerLocomotionManager.AttemptToPerformJump();
        }
    }
    private void HandleRBInput()
    {
        if (two_Hand_Input)
            return; 

        if(RB_Input)
        {
            RB_Input = false;

            player.playerNetworkManager.SetCharacterActionHand(true);

            player.playerCombatManager.PerformingWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.oh_RB_Action, player.playerInventoryManager.currentRightHandWeapon);
        }

        
    }
    private void HandleLBInput()
    {

        if (two_Hand_Input)
            return;

        if (LB_Input)
        {
            LB_Input = false;

            player.playerNetworkManager.SetCharacterActionHand(false);

            player.playerCombatManager.PerformingWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.oh_LB_Action, player.playerInventoryManager.currentRightHandWeapon); 
            //player.playerCombatManager.PerformingWeaponBasedAction(player.playerInventoryManager.currentLeftHandWeapon.oh_LB_Action, player.playerInventoryManager.currentLeftHandWeapon);
        }


    }
    private void HandleRTInput()
    {
        if (RT_Input)
        {
            RT_Input = false;

            player.playerNetworkManager.SetCharacterActionHand(true);

            player.playerCombatManager.PerformingWeaponBasedAction(player.playerInventoryManager.currentRightHandWeapon.oh_RT_Action, player.playerInventoryManager.currentRightHandWeapon);
        }
    }
    private void HandleChargeRTInput()
    {
        // Only want to check for a charge if i am in action that request it
        if(player.isPerformingAction)
        {
            if(player.playerNetworkManager.isUsingRightHand.Value)
            {
                player.playerNetworkManager.isChargingAttack.Value = Hold_RT_Input;
            }
        }
    }

    private void HandleLTInput()
    {
        if (LT_Input)
        {
            LT_Input = false;

            WeaponItem weaponPerformingAshOfWar = player.playerCombatManager.SelectWeaponToPerformAshOfWar();

            weaponPerformingAshOfWar.ashOfWarAction.AttempToPerformAction(player);
        }
    }
    private void HandleSwitchRightWeaponInput()
    {
        if(switch_Right_Weapon_Input)
        {
            switch_Right_Weapon_Input = false;

            if(PlayerUIManager.instance.menuWindowIsOpen)
            {
                return;
            }

            player.playerEquipmentManager.SwitchRightWeapon();
        }
    }
    private void HandleSwitchLeftWeaponInput()
    {
        if (switch_Left_Weapon_Input)
        {
            switch_Left_Weapon_Input = false;

            if (PlayerUIManager.instance.menuWindowIsOpen)
            {
                return;
            }

            player.playerEquipmentManager.SwitchLeftWeapon();
        }
    }

    private void HandleSwitcQuickSLotItemInput()
    {
        if (switch_Quick_Slot_Item_Input)
        {
            switch_Quick_Slot_Item_Input = false;

            if (PlayerUIManager.instance.menuWindowIsOpen)
            {
                return;
            }
            if (player.isPerformingAction)
                return;
            if (player.playerCombatManager.isUsingItem)
                return;

            player.playerEquipmentManager.SwitchQuickSlotItem();
        }
    }

    private void HandleInteractionInput()
    {
        if(interaction_Input)
        {
            interaction_Input = false;

            // Close Pop Up
            player.playerInteractableManager.Interact();
        }
    }
    private void QueInput(ref bool quedInput) // passing a reference means i pass a specific bool, and not the value of that bool(true or false)
    {
        // Reset all Qued Inputs so only one can Que at a time
        que_RB_Input = false;
        que_RT_Input = false;
        //que_LB_Input = false;
        //que_LT_Input = false;

        // Check for UI window being open, if it is open return
        if(player.isPerformingAction || player.playerNetworkManager.isJumping.Value)
        {
            quedInput = true;
            // Attempt this new input for [ amount of time
            que_Input_Timer =  default_Que_Input_Time;
            input_Que_Is_Active = true;
        }
    }

    private void ProcessQuedInput()
    {
        if (player.isDead.Value)
            return;

        if (que_RB_Input)
            RB_Input = true;
        
        if (que_RT_Input)
            RT_Input = true;
    }

    private void HandleQuedInputs()
    {
        if(input_Que_Is_Active)
        {
            // While the timer is above 0, keep attempting to press the input
            if(que_Input_Timer > 0)
            {
                que_Input_Timer -= Time.deltaTime;
                ProcessQuedInput();
            }
            else
            {
                // Reset all Qued Input
                que_RB_Input = false;
                que_RT_Input = false;

                input_Que_Is_Active = false;
                que_Input_Timer = 0;
            }
        }
    }

    private void HandleOpenCharacterMenuInput()
    {
        if(openCharacterMenuInput)
        {
            openCharacterMenuInput = false;

            PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();
            PlayerUIManager.instance.CloseAllMenuWindow();
            PlayerUIManager.instance.playerUICharacterMenuManager.OpenCharacterMenu();
        }
    }
    private void HandleCloseUIInput()
    {
        if(closeMenuInput)
        {
            closeMenuInput = false;

            if(PlayerUIManager.instance.menuWindowIsOpen)
            {
                PlayerUIManager.instance.CloseAllMenuWindow();
            }
        }
    }
}
