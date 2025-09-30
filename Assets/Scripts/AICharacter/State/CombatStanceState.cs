using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(menuName = "A.I/State/Combat Stance")]
public class CombatStanceState : AIState
{
    [Header("Attacks")]
    public List<AICharacterAttackAtion> aiCharacterAttacks;
    protected List<AICharacterAttackAtion> potentialAttacks;
    private AICharacterAttackAtion choosenAttack;
    private AICharacterAttackAtion previousAttack;
    protected bool hasAttack = false;   

    [Header("Combo")]
    [SerializeField] protected bool canPerformCombo = false; // If the character can perform a combo attack, after the initial attack
    [SerializeField] protected int chanceToPerformCombo = 25; // Percent of the character to perform a combo on the next attack
    protected bool hasRolledForComboChance = false; // is i have already for chance during this state
   
    //[Header("Pivot")]
    //[SerializeField] protected bool enablePivot;
    
    [Header("Engagement Distance")]
    [SerializeField] public float maximumEngagementDistance = 5; // Distance i have to be away from target before character enter the pursue target state

    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.isPerformingAction)
            return this;

       
        if (!aiCharacter.navMeshAgent.enabled)
            aiCharacter.navMeshAgent.enabled = true;
        

        // Want the a.i character to face and turn toward its target when its outside it's fov include this
        if(aiCharacter.aiCharacterCombatManager.enablePivot)
        {
            if (!aiCharacter.aiCharacterNetworkManager.isMoving.Value)
            {
                if (aiCharacter.aiCharacterCombatManager.viewableAngle <= -30 ||
                    aiCharacter.aiCharacterCombatManager.viewableAngle > 30)
                {
                    aiCharacter.aiCharacterCombatManager.PivotTowardsTarget(aiCharacter);
                }
            }
        }


        // Rotate to face target
        aiCharacter.aiCharacterCombatManager.RotateTowardsAgent(aiCharacter);

        // If my target is no longer present,switch back to idle
        if (aiCharacter.aiCharacterCombatManager.currentTarget == null)
            return SwitchState(aiCharacter, aiCharacter.idle);

        // If i do not have an attack, get one
        if(!hasAttack)
        {
            
            GetNewAttack(aiCharacter);
        }
        else
        {
           
            aiCharacter.attack.currentAttack = choosenAttack;
            // roll for combo chance
            return SwitchState(aiCharacter, aiCharacter.attack);
        }

        // Out of the Combat engament distance, switch to pursue target state
        if(aiCharacter.aiCharacterCombatManager.distanceFromTarget > maximumEngagementDistance)
            return SwitchState(aiCharacter, aiCharacter.pursueTarget);

        NavMeshPath path = new NavMeshPath();
        aiCharacter.navMeshAgent.CalculatePath(aiCharacter.aiCharacterCombatManager.currentTarget.transform.position, path);
        aiCharacter.navMeshAgent.SetPath(path);

        return this;
    }

    protected virtual void GetNewAttack(AICharacterManager aiCharacter)
    {
        potentialAttacks = new List<AICharacterAttackAtion>();

        foreach(var potentialAttack in aiCharacterAttacks)
        {
            Debug.Log("0");
            // If too close, check the next
            if(potentialAttack.minimumAttackDistance > aiCharacter.aiCharacterCombatManager.distanceFromTarget)
            {
                continue;
                
            }
           
            // If too far, check the next
            if (potentialAttack.maximumAttackDistance < aiCharacter.aiCharacterCombatManager.distanceFromTarget)
                continue;
            
            // If target is outside minimum field of view, check the next
            if (potentialAttack.minimumAttackDistance > aiCharacter.aiCharacterCombatManager.viewableAngle)
                continue;
           
            // If target is outside maximum field of view, check the next
            if (potentialAttack.maximumAttackDistance > aiCharacter.aiCharacterCombatManager.viewableAngle)
                continue;

           
            potentialAttacks.Add(potentialAttack);
        }

        if(potentialAttacks.Count <= 0)
                return;

        var totalWeight = 0;

        foreach(var attack in potentialAttacks)
        {
            totalWeight += attack.attackWeight;
        }

        var randomWeightValue = Random.Range(1, totalWeight + 1);
        var processedWeidght = 0;

        foreach(var attack in potentialAttacks)
        {
            processedWeidght += attack.attackWeight;

            if(randomWeightValue <= processedWeidght)
            {
                // My attack
                choosenAttack = attack;
                previousAttack = choosenAttack;
                hasAttack = true;
                return;
            }
        }
    }
    protected virtual bool RollForOutComeChance(int outcomeChance)
    {
        bool outcomeWillBePerformed = false;

        int randomPercenttage = Random.Range(0, 100);

        if(randomPercenttage < outcomeChance)
            outcomeWillBePerformed = true;

        return outcomeWillBePerformed;
    }
    protected override void ResetStateFlags(AICharacterManager aiCharacter)
    {
        base.ResetStateFlags(aiCharacter);

        hasAttack = false;
        hasRolledForComboChance = false;
    }
}
