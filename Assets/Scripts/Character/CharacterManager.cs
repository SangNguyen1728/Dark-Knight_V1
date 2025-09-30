using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.TextCore.Text;
public class CharacterManager : NetworkBehaviour
{

    [Header("Status")]
    public NetworkVariable<bool> isDead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [HideInInspector] public CharacterController characterController;
    [HideInInspector] public Animator animator;

    [HideInInspector] public CharacterNetworkManager characterNetworkManager;
    [HideInInspector] public CharacterEffectsManager characterEffectsManager;
    [HideInInspector] public CharacterAnimatorManager characterAnimatorManager;
    [HideInInspector] public CharacterCombatManager characterCombatManager;
    [HideInInspector] public CharacterSoundFXManager characterSoundFXManager;
    [HideInInspector] public CharacterLocomotionManager characterLocomotionManager;
    [HideInInspector] public CharacterUIManager characterUIManager;
    [HideInInspector] public CharacterStatsManager characterStatsManager;

    [Header("Character Group")]
    public CharacterGroup characterGroup;

    [Header("Flags")] // there are many animation exceptions, so it is necessary to use flags to make the animation transition smoothly
    public bool isPerformingAction = false;
    //public bool isJumping = false;
    //public bool isGrounded = true;
    //public bool applyRootMotion = false;
    //public bool canRotate = true;
    //public bool canMove = true;

    

    protected virtual void Awake()
    {
        DontDestroyOnLoad(this);

        characterController = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        characterNetworkManager = GetComponent<CharacterNetworkManager>();
        characterEffectsManager = GetComponent<CharacterEffectsManager>();
        characterAnimatorManager = GetComponent<CharacterAnimatorManager>();
        characterCombatManager = GetComponent<CharacterCombatManager>();
        characterSoundFXManager = GetComponent<CharacterSoundFXManager>();
        characterLocomotionManager = GetComponent<CharacterLocomotionManager>();
        characterUIManager = GetComponent<CharacterUIManager>();
        characterStatsManager = GetComponent<CharacterStatsManager>();
    }
    protected virtual void Start()
    {
        IgnoreMyOwnColliders();
    }
    protected virtual void Update()
    {
        animator.SetBool("IsGrounded", characterLocomotionManager.isGrounded);
        // if this character is being controlled form my side, then assign its network position to the position of our transform
        if (IsOwner)
        {
            characterNetworkManager.networkPosition.Value = transform.position;
            characterNetworkManager.networkRotaion.Value = transform.rotation;
        }
        // if this character is being controlled form else where,then assign its position here locally by the position of its network transform
        else
        {
            //position
            transform.position = Vector3.SmoothDamp
                (transform.position,
                characterNetworkManager.networkPosition.Value,
                ref characterNetworkManager.networkPositionVelocity,
                characterNetworkManager.networkPositionSmoothTime);

            //rotation
            transform.rotation = Quaternion.Slerp
                (transform.rotation,
                characterNetworkManager.networkRotaion.Value,
                characterNetworkManager.networkRotationSmoothTime);
        }
    }
    protected virtual void FixedUpdate()
    {

    }
    protected virtual void LateUpdate()
    {

    }
    protected virtual void OnEnable()
    {

    }
    protected virtual void OnDisable()
    {

    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        animator.SetBool("isMoving", characterNetworkManager.isMoving.Value);
        characterNetworkManager.OnIsActiveChanged(false, characterNetworkManager.isActive.Value);

        isDead.OnValueChanged += characterNetworkManager.OnIsDeadChanged;
        characterNetworkManager.isMoving.OnValueChanged += characterNetworkManager.OnIsMovingChanged;
        characterNetworkManager.isActive.OnValueChanged += characterNetworkManager.OnIsActiveChanged;
       
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        isDead.OnValueChanged -= characterNetworkManager.OnIsDeadChanged;
        characterNetworkManager.isMoving.OnValueChanged -= characterNetworkManager.OnIsMovingChanged;
        characterNetworkManager.isActive.OnValueChanged -= characterNetworkManager.OnIsActiveChanged;
    }
   
    public virtual IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {
        if(IsOwner)
        {
            characterNetworkManager.currentHealth.Value = 0;
            isDead.Value = true;

            // reset any flags here that need reset

            // if not ground, set aerial death anim
            
            if(!manuallySelectDeathAnimation)
            {
                characterAnimatorManager.PlayTargetActionAnimtion("Dead_01", true);
            }

        }
        yield return new WaitForSeconds(5);
    }
    public virtual void ReviveCharacter()
    {

    }
    protected virtual void IgnoreMyOwnColliders()
    {
        Collider characterControllerCollider = GetComponent<Collider>();
        Collider[] damageableCharacterColliders = GetComponentsInChildren<Collider>();

        List<Collider> ignoreColliders = new List<Collider>();

        // add all of our damageable character colliders. to the list that will be used to ignore collitions
        foreach(var collider in  damageableCharacterColliders)
        {
            ignoreColliders.Add(collider);
        }

        // adds my character controller colliders to the list that will be used to ignore collising
        ignoreColliders.Add(characterControllerCollider);

        // goese through collider on the list, and ignore collision with each other
        foreach(var collider in ignoreColliders)
        {
            foreach(var othercollider in ignoreColliders)
            {
                Physics.IgnoreCollision(collider, othercollider, true);
            }
        }
    }
}
