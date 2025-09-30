using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class CharacterAnimatorManager : MonoBehaviour
{
    CharacterManager character;
    
    int vertical;
    int horizontal;

    [Header("Flags")]
    public bool applyRootMotion = false;

    [Header("Damage Animations")]
    public string lastDamageAnimationPlayed;

    // Ping Hit Reaction
    [SerializeField] string hit_Forward_Ping_01 = "hit_Forward_Ping_01";
    [SerializeField] string hit_Forward_Ping_02 = "hit_Forward_Ping_02";
    [SerializeField] string hit_Backward_Ping_01 = "hit_Backward_Ping_01";
    [SerializeField] string hit_Backward_Ping_02 = "hit_Backward_Ping_02";
    [SerializeField] string hit_Left_Ping_01 = "hit_left_Ping_01";
    [SerializeField] string hit_Left_Ping_02 = "hit_left_Ping_02";
    [SerializeField] string hit_Right_Ping_01 = "hit_Right_Ping_01";
    [SerializeField] string hit_Right_Ping_02 = "hit_Right_Ping_02";

    public List<string> forward_Ping_Damage = new List<string>();
    public List<string> backward_Ping_Damage = new List<string>();
    public List<string> left_Ping_Damage = new List<string>();
    public List<string> right_Ping_Damage = new List<string>();

    // Medium Hit Reaction
    [SerializeField] string hit_Forward_Medium_01 = "hit_Forward_Medium_01";
    [SerializeField] string hit_Forward_Medium_02 = "hit_Forward_Medium_02";
    [SerializeField] string hit_Backward_Medium_01 = "hit_Backward_Medium_01";
    [SerializeField] string hit_Backward_Medium_02 = "hit_Backward_Medium_02";
    [SerializeField] string hit_Left_Medium_01 = "hit_left_Medium_01";
    [SerializeField] string hit_Left_Medium_02 = "hit_left_Medium_02";
    [SerializeField] string hit_Right_Medium_01 = "hit_Right_Medium_01";
    [SerializeField] string hit_Right_Medium_02 = "hit_Right_Medium_02";

    public List<string> forward_Medium_Damage = new List<string>();
    public List<string> backward_Medium_Damage = new List<string>();
    public List<string> left_Medium_Damage = new List<string>();
    public List<string> right_Medium_Damage = new List<string>();

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();

        vertical = Animator.StringToHash("Vertical");
        horizontal = Animator.StringToHash("Horizontal");
    }
    protected virtual void Start()
    {
        forward_Ping_Damage.Add(hit_Forward_Ping_01);
        forward_Ping_Damage.Add(hit_Forward_Ping_02);

        backward_Ping_Damage.Add(hit_Backward_Ping_01);
        backward_Ping_Damage.Add(hit_Backward_Ping_02);

        left_Ping_Damage.Add(hit_Left_Ping_01);
        left_Ping_Damage.Add(hit_Left_Ping_02);

        right_Ping_Damage.Add(hit_Right_Ping_01);
        right_Ping_Damage.Add(hit_Right_Ping_02);


        forward_Medium_Damage.Add(hit_Forward_Medium_01);
        forward_Medium_Damage.Add(hit_Forward_Medium_02);

        backward_Medium_Damage.Add(hit_Backward_Medium_01);
        backward_Medium_Damage.Add(hit_Backward_Medium_02);

        left_Medium_Damage.Add(hit_Left_Medium_01);
        left_Medium_Damage.Add(hit_Left_Medium_02);

        right_Medium_Damage.Add(hit_Right_Medium_01);
        right_Medium_Damage.Add(hit_Right_Medium_02);
    }
    public string GetRandomAnimationFromList(List<string> animationList)
    {
        List<string> finalList = new List<string>();

        foreach(var item in animationList)
        {
            finalList.Add(item);
        }

        // check if we have already played this damage aniamtion so it DOES NOT repeat
        finalList.Remove(lastDamageAnimationPlayed);

        // check the list for null entries, and remove 
        for(int i = finalList.Count - 1; i > -1; i--)
        {
            if(finalList[i] == null)
            {
                finalList.RemoveAt(i);
            }
        }

        int randomValue = Random.Range(0, finalList.Count);

        return finalList[randomValue];
    }
    public void UpdateAnimatorMovementParameters(
        float horizontalValue, 
        float verticalValue, 
        bool isSprinting
        )
    {
        float snappedHorizontal;
        float snappedVertical;

        if(horizontalValue > 0  && horizontalValue <= 0.5f)
        {
            snappedHorizontal = 0.5f;
        }
        else if(horizontalValue > 0.5f && horizontalValue <= 1)
        {
            snappedHorizontal = 1;
        }
        else if(horizontalValue < 0 && horizontalValue >= -0.5f)
        {
            snappedHorizontal = - 0.5f;
        }
        else if(horizontalValue < -0.5f && horizontalValue >= -1)
        {
            snappedHorizontal = -1;
        }
        else
        {
            snappedHorizontal = 0;
        }

        if (verticalValue > 0 && verticalValue <= 0.5f)
        {
            snappedVertical = 0.5f;
        }
        else if(verticalValue > 0.5f && verticalValue <= 1)
        {
            snappedVertical = 1;
        }
        else if(verticalValue < 0 && verticalValue >= -0.5f)
        {
            snappedVertical = -0.5f;
        }
        else if(verticalValue < - 0.5f && verticalValue >= -1)
        {
            snappedVertical= -1;
        }
        else
        {
            snappedVertical = 0;
        }

        if ( isSprinting )
        {
            snappedVertical = 2;
        }

        character.animator.SetFloat(horizontal, snappedHorizontal, 0.1f, Time.deltaTime);
        character.animator.SetFloat(vertical, snappedVertical, 0.1f, Time.deltaTime);
    }
    public virtual void PlayTargetActionAnimtion(
        string targetAnimation, 
        bool isPerformingAction, 
        bool applyRootMotion = true, 
        bool canRotate = false, 
        bool canMove = false,
        bool canRun = true,
        bool canRoll = false)
    {
        // keep track of last attack performed (combo)
        // keep track of current attack type (light, heavy)
        // update animation set to current weapon animations
        // attack can be parried
        // ISATTACK flag is active

        this.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0.2f);
        // can be used to stop character from attemping new action
        // can then check for this before attemping new action
        character.isPerformingAction = isPerformingAction;
        character.characterLocomotionManager.canRotate = canRotate;
        character.characterLocomotionManager.canMove = canMove;
        character.characterLocomotionManager.canRun = canRun;
        character.characterLocomotionManager.canRoll = canRoll;

        // tell the server/host we playered an animation, and to play that animation for everyone else present
        character.characterNetworkManager.NotifyTheServerOfAttackActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }

    public virtual void PlayTargetActionAnimtionInstantly(
        string targetAnimation,
        bool isPerformingAction,
        bool applyRootMotion = true,
        bool canRotate = false,
        bool canMove = false,
        bool canRun = true,
        bool canRoll = false)
    {
        

        this.applyRootMotion = applyRootMotion;
        character.animator.Play(targetAnimation);
        
        // can be used to stop character from attemping new action
        // can then check for this before attemping new action
        character.isPerformingAction = isPerformingAction;
        character.characterLocomotionManager.canRotate = canRotate;
        character.characterLocomotionManager.canMove = canMove;
        character.characterLocomotionManager.canRun = canRun;
        character.characterLocomotionManager.canRoll = canRoll;

        // tell the server/host we playered an animation, and to play that animation for everyone else present
        character.characterNetworkManager.NotifyTheServerOfInstantAttackActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }
    public virtual void PlayTargetAttackActionAnimtion(
        WeaponItem weapon,
        AttackType attackType,
        string targetAnimation, 
        bool isPerformingAction, 
        bool applyRootMotion = true, 
        bool canRotate = false, 
        bool canMove = false,
        bool canRoll = false)
    {
        // keep track of last attack performed (combo)
        // keep track of current attack type (light, heavy)
        // update animation set to current weapon animations
        // attack can be parried
        // ISATTACK flag is active

        character.characterCombatManager.currentAttackTpye = attackType;
        character.characterCombatManager.lastAttackAnimationPerformed = targetAnimation;

        UpdateAnimatorController(weapon.weaponAnimator); 

        this.applyRootMotion = applyRootMotion;
        character.animator.CrossFade(targetAnimation, 0);
        // can be used to stop character from attemping new action
        // can then check for this before attemping new action
        character.isPerformingAction = isPerformingAction;
        character.characterLocomotionManager.canRotate = canRotate;
        character.characterLocomotionManager.canMove = canMove;
        character.characterNetworkManager.isAttacking.Value = true;
        character.characterLocomotionManager.canRoll = canRoll;

        // tell the server/host we playered an animation, and to play that animation for everyone else present
        character.characterNetworkManager.NotifyTheServerOfAttackActionAnimationServerRpc(NetworkManager.Singleton.LocalClientId, targetAnimation, applyRootMotion);
    }
    public void UpdateAnimatorController(AnimatorOverrideController weaponController)
    {
        character.animator.runtimeAnimatorController = weaponController;
    }
}
