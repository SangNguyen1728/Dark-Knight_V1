using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "A.I/State/Ilde")]
public class IdleState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        if (aiCharacter.characterCombatManager.currentTarget != null)
        {
           return SwitchState(aiCharacter, aiCharacter.pursueTarget);
        }
        else
        {
            // Return this state, continually search for target
            // Keep this state, until a target is found
            aiCharacter.aiCharacterCombatManager.FindATargetViaLineOfSight(aiCharacter);
            Debug.Log("search Target");

            return this;
        }
    }
}
