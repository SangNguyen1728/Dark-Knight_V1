using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIUndeadComBatManager : AICharacterCombatManager
{
    [Header("Damage Collider")]
    [SerializeField] UndeadHandDamageCollider rightHandDamageCollider;
    [SerializeField] UndeadHandDamageCollider leftHandDamageCollider;

    [Header("Damage")]
    [SerializeField] int baseDamage = 25;
    [SerializeField] int basePoiseDamage = 25;
    [SerializeField] float attack01DamageModifer = 1.0f;
    [SerializeField] float attack02DamageModifier = 1.4f;

    public void SetAttack01Damage()
    {
        rightHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifer;
        leftHandDamageCollider.physicalDamage = baseDamage * attack01DamageModifer;

        rightHandDamageCollider.poiseDamage = basePoiseDamage * attack01DamageModifer;
        leftHandDamageCollider.poiseDamage = basePoiseDamage * attack01DamageModifer;
    }
    public void SetAttack02Damage()
    {
        rightHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
        leftHandDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
    }
    public void OpenRightDamageCollider()
    {
        rightHandDamageCollider.EnableDamageCollider();
    }
    public void DisableRightDamageCollider()
    {
        rightHandDamageCollider.DisableDamageCollider();
    }
    public void OpenLeftDamageCollider()
    {
        leftHandDamageCollider.EnableDamageCollider();
    }
    public void DisableLefttDamageCollider()
    {
        leftHandDamageCollider.DisableDamageCollider();
    }

    public override void CloseAllDamageColliders()
    {
        base.CloseAllDamageColliders();

        rightHandDamageCollider.DisableDamageCollider();
        leftHandDamageCollider.DisableDamageCollider();
    }
}
