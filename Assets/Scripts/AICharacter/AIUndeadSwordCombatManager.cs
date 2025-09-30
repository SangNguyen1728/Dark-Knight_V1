using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUndeadSwordCombatManager :AICharacterCombatManager
{
    [Header("Damage Collider")]
    [SerializeField] SwordHandDamageCollider rightHandDamageCollider;

    [Header("Damage")]
    [SerializeField] int baseDamage = 25;
    [SerializeField] float attack01DamageModifer = 1.0f;
    [SerializeField] float attack02DamageModifier = 1.4f;

    public void SetAttack01Damage()
    {
        rightHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifer;
       
    }
    public void SetAttack02Damage()
    {
        rightHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
       
    }
    public void OpenRightDamageCollider()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGruntSFX();
        rightHandDamageCollider.EnableDamageCollider();
    }
    public void DisableRightDamageCollider()
    {
        rightHandDamageCollider.DisableDamageCollider();
    }
    
}
