using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordHandDamageCollider : DamageCollider
{
    [SerializeField] public AICharacterManager undeadCharacter;
    protected override void Awake()
    {
        base.Awake();

        damageCollider = GetComponent<Collider>();
        undeadCharacter = GetComponentInParent<AICharacterManager>();
    }

    protected override void GetBlockingDotValues(CharacterManager damageTarget)
    {
        directionFromAttackToDamageTarget = undeadCharacter.transform.position - damageTarget.transform.position;
        dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
    }
    protected override void DamageTarget(CharacterManager damageTarget)
    {
        // do not want to damage the same target more than once in a single attack
        // add a list that check before applying damage

        if (charactersDamaged.Contains(damageTarget))
            return;

        charactersDamaged.Add(damageTarget);

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.poiseDamage = poiseDamage;
        damageEffect.contactPoint = contactPoint;
        damageEffect.angleHitFrom = Vector3.SignedAngle(undeadCharacter.transform.forward, damageTarget.transform.forward, Vector3.up);

        //damageTarget.characterEffectsManager.ProcessInstantEffects(damageEffect);


        if (damageTarget.IsOwner)
        {
            damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                damageTarget.NetworkObjectId,
                undeadCharacter.NetworkObjectId,
                damageEffect.physicalDamage,
                damageEffect.magicDamage,
                damageEffect.fireDamage,
                damageEffect.holyDamage,
                damageEffect.poiseDamage,
                damageEffect.angleHitFrom,
                damageEffect.contactPoint.x,
                damageEffect.contactPoint.y,
                damageEffect.contactPoint.z);
        }
    }

    protected override void CheckForParry(CharacterManager damageTarget)
    {
        if (charactersDamaged.Contains(damageTarget))
            return;

        if (!undeadCharacter.characterNetworkManager.isParryable.Value)
            return;

        if (!damageTarget.IsOwner)
            return;

        if (damageTarget.characterNetworkManager.isParrying.Value)
        {
            charactersDamaged.Add(damageTarget);
            damageTarget.characterNetworkManager.NotifyServerOfParryServerRpc(undeadCharacter.NetworkObjectId);
            damageTarget.characterAnimatorManager.PlayTargetActionAnimtionInstantly("Parry_Land_01", true);
        }
    }
}
