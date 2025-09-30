using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStompCollider : DamageCollider
{
    [SerializeField] AISlayerCharacterManager slayerCharacterManager;

    protected override void Awake()
    {
        base.Awake();

        slayerCharacterManager = GetComponentInParent<AISlayerCharacterManager>();
    }
    public void StompAttack()
    {
        GameObject slayerVFX = Instantiate(slayerCharacterManager.slayerCombatManager.SlayerImpactVFX, transform);

        Collider[] colliders = Physics.OverlapSphere(transform.position, slayerCharacterManager.slayerCombatManager.stompAttackAOERadius, WorldUtilityManager.Instance.getCharacterLayer());
        List<CharacterManager> charatersDamaged = new List<CharacterManager>();
        foreach (var collider in colliders)
        {
            CharacterManager character = collider.GetComponentInParent<CharacterManager>();

            if (character != null)
            {
                if (charatersDamaged.Contains(character))
                    continue;

                if(character == slayerCharacterManager)
                    continue;

                charatersDamaged.Add(character);

                if (character.IsOwner)
                {
                    // check for blocking

                    TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeDamageEffect);
                    damageEffect.physicalDamage = slayerCharacterManager.slayerCombatManager.stompDamage;
                    damageEffect.poiseDamage = slayerCharacterManager.slayerCombatManager.stompDamage;

                    character.characterEffectsManager.ProcessInstantEffects(damageEffect);
                }
            }
        }
    }
}
