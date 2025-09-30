using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/ Instant Effects/ Take Damage")]
public class TakeDamageEffect : InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager characterCausingDamage;

    [Header("Damage")]
    public float physicalDamage = 0; // will be more detailed in the future
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;

    [Header("Final Damage")]
    protected int finalDamageDealt = 0; // the damage character takes after all calculations have been made

    [Header("Poise")]
    public float poiseDamage = 0;
    public bool poiseIsBroken = false; // if character's poise is broken, they will be STUNNED and play damage animation
    // NEED:
    // TO DO BUILD UPS
    // BUILD UP EFFECT AMOUTS
    [Header("Animation")]
    public bool playDamageAnimation = true;
    public bool manuallySelectedDamageAnimation = false;
    public string damageAnimation;

    [Header("SoundFX")]
    public bool willPlayDamageSFX = true;
    public AudioClip elementalSoundFX; // used on top of regular SFX if there is elemental damage present(magic/fire/lightning/holy)

    [Header("Direction Damage Taken From")]
    public float angleHitFrom; // used to determine what damage animation to play(to left, to right,...)
    public Vector3 contactPoint; // used to determine where blood FX instantite
    public override void ProcessEffect(CharacterManager character)
    {
        if (character.characterNetworkManager.isInvulnerable.Value)
            return;

        base.ProcessEffect(character);

        //if character dead, no additional damage effect should be process
        if (character.isDead.Value)
            return;

        

        CalculateDamage(character);

        //check which directional damage came from
        //play a damage animation
        PlayDirectionalBasedDamageAnimation(character);
        // check for build up(poision, bleeding)

        //play damage sound FX
        PlayDamageSFX(character);

        //play damage VFX(blood)
        PlayDamageVFX(character);


        CalculateStanceDamage(character);
        // if character is A.I, check for new target character causing damage is present
    }
    protected virtual void CalculateDamage(CharacterManager character)
    {
        if(!character.IsOwner)
            return;

        if(characterCausingDamage != null)
        {
            // check for damage motifiers and modify base damage(physic/ elemetalDaamage
        }

        // check character for flat defenses and subtract them from the damage

        // check character for aromor absorptions, and subtract the percentage from damage

        // add all damage types together, and apply final damage
        finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

        if(finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }

        Debug.Log("Final Damage Given" + finalDamageDealt);
        character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;

        // calculate poise damage to determine if character will be stunned

        // I subject poise damage from the character total
        character.characterStatsManager.totalPoiseDamage -= poiseDamage;

        // I store the previous poise damage taken for other interactions
        character.characterCombatManager.previousPoiseDamageTaken = poiseDamage;

        float remainingPoise = character.characterStatsManager.basePoiseDefense +
            character.characterStatsManager.offensivePoiseBonus +
            character.characterStatsManager.totalPoiseDamage;

        if(remainingPoise <= 0)
            poiseIsBroken = true;

        // When character has been hit, i reset the poise timer
        character.characterStatsManager.poiseResetTimer = character.characterStatsManager.defaultPoiseResetTime;
    }

    protected void CalculateStanceDamage(CharacterManager character)
    {
        AICharacterManager aiCharacter = character as AICharacterManager;

        int stanceDamage = Mathf.RoundToInt(poiseDamage);

        if(aiCharacter != null)
        {
            aiCharacter.aiCharacterCombatManager.DamageStance(stanceDamage);
        }
    }

    protected void PlayDamageVFX(CharacterManager character)
    {
        // fire damage, fire particles
        // lightning damage, lightning particles

        character.characterEffectsManager.PlayBloodSplatterVFX(contactPoint);
    }
    protected void PlayDamageSFX(CharacterManager character)
    {
        AudioClip physiclaDamageSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.physicalDamageSFX);

        character.characterSoundFXManager.PlaySoundFX(physiclaDamageSFX);
        character.characterSoundFXManager.PlayDamageGruntsSFX();
        //  fire damage is greater than 0, play burn SFX
        // lightning damage is grater than 0, play zap SFX 
    }
    protected void PlayDirectionalBasedDamageAnimation(CharacterManager character)
    {
        if (!character.IsOwner)
            return;

        if (character.isDead.Value)
            return;

        if(poiseIsBroken)
        {
            if (angleHitFrom >= 145 && angleHitFrom <= 180)
            {
                // play front animation
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Medium_Damage);

            }
            else if (angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                // play front animation
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Medium_Damage);
            }
            else if (angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                // play back animation
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.backward_Medium_Damage);
            }
            else if (angleHitFrom >= -144 && angleHitFrom <= -45)
            {
                // play left animation
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.left_Medium_Damage);
            }
            else if (angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                // play right animation
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.right_Medium_Damage);
            }
        }
        else
        {
            if (angleHitFrom >= 145 && angleHitFrom <= 180)
            {
                // play front animation
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Ping_Damage);

            }
            else if (angleHitFrom <= -145 && angleHitFrom >= -180)
            {
                // play front animation
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.forward_Ping_Damage);
            }
            else if (angleHitFrom >= -45 && angleHitFrom <= 45)
            {
                // play back animation
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.backward_Ping_Damage);
            }
            else if (angleHitFrom >= -144 && angleHitFrom <= -45)
            {
                // play left animation
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.left_Ping_Damage);
            }
            else if (angleHitFrom >= 45 && angleHitFrom <= 144)
            {
                // play right animation
                damageAnimation = character.characterAnimatorManager.GetRandomAnimationFromList(character.characterAnimatorManager.right_Ping_Damage);
            }
        }



        // if poise is broken, play a staggering damage animation
        //if(poiseIsBroken)
        //{
        //    character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
        //    character.characterAnimatorManager.PlayTargetActionAnimtion(damageAnimation, true);
        //}

        character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;

        if(poiseIsBroken)
        {
            // If i am poise broken restrict my movement and actions
            character.characterAnimatorManager.PlayTargetActionAnimtion(damageAnimation, true);
        }
        else
        {
            // If i am NOT poise broken simply play an upperbody animation without rétricting
            character.characterAnimatorManager.PlayTargetActionAnimtion(damageAnimation, false, false, true, true);
        }
           
    }
}
