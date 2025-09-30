using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AICharacterCombatManager : CharacterCombatManager
{
    protected AICharacterManager aiCharacter;

    [Header("Action Recovery")]
    public float actionRecoveryTimer = 0;

    [Header("Pivot")]
    public bool enablePivot = true;

    [Header("Target Infor")]
    public float distanceFromTarget;
    public float viewableAngle;
    public Vector3 targetsDirection;

    [Header("Detection")]
    [SerializeField] float decectionRadius = 15;
    public float minimumFOV = -35;
    public float maximumFOV = 35;

    //private float rotationSpeed = 1f;

    [Header("Attack Rotation Speed")]
    public float attackRotationSpeed = 25;

    [Header("Stance Setting")]
    public float maxStance = 150;
    public float currentStance;
    [SerializeField] float stanceRegeneratedPersecond = 15;
    [SerializeField] bool ignoreStanceBreak = false;

    [Header("Stance Timer")]
    [SerializeField] float stanceRegenerationTimer = 0;
    private float stanceTickTimer = 0;
    [SerializeField] float defaultTimerUntilStanceRegenerationBegins = 15;

    protected override void Awake()
    {
        base.Awake();

        aiCharacter = GetComponent<AICharacterManager>();
        lockOnTransform = GetComponentInChildren<LockOnTransform>().transform;
    }

    private void FixedUpdate()
    {
        HandleStanceBreaking();
    }

    private void HandleStanceBreaking()
    {
        if (!aiCharacter.IsOwner)
            return;

        if (aiCharacter.isDead.Value)
            return;

        if(stanceRegenerationTimer > 0)
        {
            stanceRegenerationTimer -= Time.deltaTime;
        }
        else
        {
            stanceRegenerationTimer = 0;

            if (currentStance < maxStance)
            {
                stanceTickTimer += Time.deltaTime;

                if(stanceTickTimer >= 1)
                {
                    stanceTickTimer = 0;
                    currentStance = stanceRegeneratedPersecond;
                }
            }
            else
            {
                currentStance = maxStance;
            }
        }

        if(currentStance <= 0)
        {
            // For would feel les impactful in gameplay
            DamageIntensity previousDamageIntensity = WorldUtilityManager.Instance.GetDamageIntensityBasedOnPoiseDamage(previousPoiseDamageTaken);

            if(previousDamageIntensity == DamageIntensity.Colossal)
            {
                currentStance = 1;
                return;
            }

            // ToDo: Backstabbed/ ripoised

            currentStance = maxStance;

            if (ignoreStanceBreak)
                return;

            aiCharacter.characterAnimatorManager.PlayTargetActionAnimtionInstantly("Stance_Break_01", true);
        }

    }

    public void DamageStance(int stanceDamage)
    {
        // When stance is damage, the timer is reset
        stanceRegenerationTimer = defaultTimerUntilStanceRegenerationBegins;

        currentStance -= stanceDamage;
    }
    public void FindATargetViaLineOfSight(AICharacterManager aiCharacter)
    {
        if (currentTarget != null)
            return;

        Collider[] colliders = Physics.OverlapSphere(aiCharacter.transform.position, decectionRadius, WorldUtilityManager.Instance.getCharacterLayer());

        for(int i = 0; i < colliders.Length; i++)
        {
            CharacterManager targetCharacter = colliders[i].transform.GetComponent<CharacterManager>();

            if(targetCharacter == null)
                continue;

            if(targetCharacter == aiCharacter) 
                continue;

            if(targetCharacter.isDead.Value)
                continue;

            if(WorldUtilityManager.Instance.CanIDamageThisTarget(aiCharacter.characterGroup,targetCharacter.characterGroup))
            {
                Vector3 targetsDirection = targetCharacter.transform.position - aiCharacter.transform.position;
                float AngleOfPotentialTarget = Vector3.Angle(targetsDirection,aiCharacter.transform.forward);

                if(AngleOfPotentialTarget > minimumFOV && AngleOfPotentialTarget < maximumFOV)
                {
                    if(Physics.Linecast(aiCharacter.characterCombatManager.lockOnTransform.position, 
                        targetCharacter.characterCombatManager.lockOnTransform.position, 
                        WorldUtilityManager.Instance.getEnviroLayer()))
                    {
                        Debug.DrawLine(aiCharacter.characterCombatManager.lockOnTransform.position, targetCharacter.characterCombatManager.lockOnTransform.position);
                        Debug.Log("blocked");
                    }
                    else
                    {
                        targetsDirection = targetCharacter.transform.position - transform.position;
                        viewableAngle = WorldUtilityManager.Instance.GetAngleOfTarget(transform, targetsDirection);
                        aiCharacter.characterCombatManager.SetTarget(targetCharacter);

                        if(enablePivot)
                        {
                            PivotTowardsTarget(aiCharacter);
                        }
                        
                    }
                }
            }
        }
    }
    public virtual void PivotTowardsTarget(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction)
            return;

        if (viewableAngle >= 60 && viewableAngle <= 140)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimtion("Turn_Right_90", true);
        }
        else if (viewableAngle <= -60 && viewableAngle >= -140)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimtion("Turn_Left_90", true);
        }
        else if (viewableAngle >= 140 && viewableAngle <= 180)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimtion("Turn_Right_180", true);
        }
        else if (viewableAngle <= -140 && viewableAngle >= -180)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimtion("Turn_Left_180", true);
        }
        else if(viewableAngle >-60 &&  viewableAngle < 60)
        {
            Quaternion targetRotation = Quaternion.LookRotation(targetsDirection);
            aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, Time.deltaTime* attackRotationSpeed);
        }
    }
    public void RotateTowardsAgent(AICharacterManager aiCharacter)
    {
        if(aiCharacter.aiCharacterNetworkManager.isMoving.Value)
        {
            aiCharacter.transform.rotation = aiCharacter.navMeshAgent.transform.rotation;
        }
    }
    public void RotateTowardsTargetWhilstAttacking(AICharacterManager aiCharacter)
    {
        if (currentTarget == null)
            return;

        if (!aiCharacter.characterLocomotionManager.canRotate)
            return;
        if (!aiCharacter.isPerformingAction)
            return;

        Vector3 Targetdirection = currentTarget.transform.position - aiCharacter.transform.position;
        Targetdirection.y = 0;

        if(Targetdirection == Vector3.zero)
            Targetdirection = aiCharacter.transform.forward;

        Quaternion targetRotation = Quaternion.LookRotation(Targetdirection);

        aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation,attackRotationSpeed * Time.deltaTime);
    }
    public void HandleActionRecovery(AICharacterManager aiCharacter)
    {
        if(actionRecoveryTimer > 0)
        {
            if(!aiCharacter.isPerformingAction)
            {
                actionRecoveryTimer -= Time.deltaTime;
            }
        }
    }
}
