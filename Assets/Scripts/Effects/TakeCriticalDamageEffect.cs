using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effect/ Instant Effects/ Take Critical Damage Effect")]
public class TakeCriticalDamageEffect : TakeDamageEffect
{
    public override void ProcessEffect(CharacterManager character)
    {
        if (character.characterNetworkManager.isInvulnerable.Value)
            return;

        //base.ProcessEffect(character);

        //if character dead, no additional damage effect should be process
        if (character.isDead.Value)
            return;



        CalculateDamage(character);
        //CalculateStanceDamage(character);

        character.characterCombatManager.pendingCriticalDamage = finalDamageDealt;
    }

    protected override void CalculateDamage(CharacterManager character)
    {
        if (!character.IsOwner)
            return;

        if (characterCausingDamage != null)
        {
            // check for damage motifiers and modify base damage(physic/ elemetalDaamage
        }

        // check character for flat defenses and subtract them from the damage

        // check character for aromor absorptions, and subtract the percentage from damage

        // add all damage types together, and apply final damage
        finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

        if (finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }
        //character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;

        // calculate poise damage to determine if character will be stunned

        // I subject poise damage from the character total
        character.characterStatsManager.totalPoiseDamage -= poiseDamage;

        // I store the previous poise damage taken for other interactions
        character.characterCombatManager.previousPoiseDamageTaken = poiseDamage;

        float remainingPoise = character.characterStatsManager.basePoiseDefense +
            character.characterStatsManager.offensivePoiseBonus +
            character.characterStatsManager.totalPoiseDamage;

        if (remainingPoise <= 0)
            poiseIsBroken = true;

        // When character has been hit, i reset the poise timer
        character.characterStatsManager.poiseResetTimer = character.characterStatsManager.defaultPoiseResetTime;
    }
}
