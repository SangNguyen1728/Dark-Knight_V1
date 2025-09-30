using UnityEngine;

public class SlayerClubDamageCollider : DamageCollider
{
    [SerializeField] public AIBossCharacterManager bossCharacter;
    protected override void Awake()
    {
        base.Awake();

        damageCollider = GetComponent<Collider>();
        bossCharacter = GetComponentInParent<AIBossCharacterManager>();
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
        damageEffect.angleHitFrom = Vector3.SignedAngle(bossCharacter.transform.forward, damageTarget.transform.forward, Vector3.up);

        //damageTarget.characterEffectsManager.ProcessInstantEffects(damageEffect);


        if (damageTarget.IsOwner)
        {
            damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                damageTarget.NetworkObjectId,
                bossCharacter.NetworkObjectId,
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
}
