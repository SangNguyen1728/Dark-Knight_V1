using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterLocomotionManager : MonoBehaviour
{
    CharacterManager character;

    [Header("Ground Check & Jumping")]
    [SerializeField] protected float gravityForce = -5.55f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] float groundCheckSphereRadius = 1;
    [SerializeField] protected Vector3 yVelocity; // force jumping or falling
    [SerializeField] protected float groundedYVelocity = -20; // force at which my character is sticking to ground whilst they are ground
    [SerializeField] protected float fallStartYVelocity = -5; // the force at which my character begin to fall(when character ungrounded)
    protected bool fallingVelocityHasBeenSet = false;
    protected float inAirTimer = 0;

    [Header("Flag")]
    public bool isRolling = false;
    public bool canRotate = true;
    public bool canMove = true;
    public bool canRun = true;
    public bool canRoll = true;
    public bool isGrounded = true;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }
    protected virtual void Update()
    {
        HandleGroundCheck();

        if(character.characterLocomotionManager.isGrounded)
        {

            //if i am not attempting to jump or move upward
            if(yVelocity.y <0)
            {
                inAirTimer = 0;
                fallingVelocityHasBeenSet = false;
                yVelocity.y = groundedYVelocity;
            }
        }
        else
        {
            // if not jumping,and our falling velocity has not been set
            if (!character.characterNetworkManager.isJumping.Value && !fallingVelocityHasBeenSet)
            {
                fallingVelocityHasBeenSet = true;
                yVelocity.y = fallStartYVelocity;
            }

            inAirTimer +=  Time.deltaTime;
            //character.animator.SetFloat("InAirTimer", inAirTimer);

            yVelocity.y += gravityForce * Time.deltaTime;

            
        }
        character.characterController.Move(yVelocity * Time.deltaTime);
    }

    protected void HandleGroundCheck()
    {
        isGrounded = Physics.CheckSphere(character.transform.position , groundCheckSphereRadius, groundLayer);
        
    }
    // testing

    //protected void OnDrawGizmosSelected()
    //{
    //    Gizmos.DrawSphere(character.transform.position, groundCheckSphereRadius);
    //}
    public void EnableCanRotate()
    {
        canRotate = true;
    } 
    public void DisableCanRotate()
    {
        canRotate = false;
    }
}
