using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "A.I/Action/Attack")]
public class AICharacterAttackAtion : ScriptableObject
{
    [Header("Attack")]
    [SerializeField] private string attackAnimation;
    [SerializeField] bool isParryable = true;

    [Header("Combo Action")]
    //public bool actionHasComboAction = false; // if this action has a combo action
    public AICharacterAttackAtion comboAction; // Combo Action of this attack action

    [Header("Action Values")]
    [SerializeField] AttackType attackType;
    public int attackWeight = 50;
    // attack can be repeated
    public float actionRecoveryTime = 1.5f; // the time before characater can make another afer performing this one
    public float minimumAttackAngle = -35;
    public float maximumAttackAngle = 35;
    public float minimumAttackDistance = 0;
    public float maximumAttackDistance = 2;

    public void AttempToPerformAction(AICharacterManager aiCharacter)
    {
        aiCharacter.characterAnimatorManager.PlayTargetActionAnimtion(attackAnimation, true);
        aiCharacter.characterNetworkManager.isParryable.Value = isParryable;
    }
}
