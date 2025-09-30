using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Effects/ Instant Effects/ Take Blocked Damage")]
public class TakeBlockedDamageEffect : InstantCharacterEffect
{
    [Header("Character Causing Damage")]
    public CharacterManager characterCausingDamage;

    [Header("Damage")]
    public float physicalDamage = 0; // wwill be more detailed in the future
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;

    [Header("Final Damage")]
    private int finalDamageDealt = 0; // the damage character takes after all calculations have been made

    [Header("Poise")]
    public float poiseDamage = 0;
    public bool poiseIsBroken = false; // if character's poise is broken, they will be STUNNED and play damage animation  

    [Header("Stamina")]
    public float staminaDamage = 0;
    public float finalStaminaDamage = 0;
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

        Debug.Log("HIT WAS BLOCKED!");

        //if character dead, no additional damage effect should be process
        if (character.isDead.Value)
            return;

        CalculateDamage(character);
        CalculateStaminaDamage(character); 

        //check which directional damage came from
        //play a damage animation
        PlayDirectionalBasedBlockingDamageAnimation(character);
        // check for build up(poision, bleeding)

        //play damage sound FX
        PlayDamageSFX(character);

        //play damage VFX(blood)
        PlayDamageVFX(character);

        // if character is A.I, check for new target character causing damage is present

        CheckForGuardBreak(character);
    }
    private void CalculateDamage(CharacterManager character)
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

        Debug.Log("Original Physical Damage" + physicalDamage);
        //Debug.Log("Original Magic Damage" + magicDamage);
        //Debug.Log("Original Fire Damage" + fireDamage);
        //Debug.Log("Original Lightning Damage" + lightningDamage);
        //Debug.Log("Original Holy Damage" + holyDamage);

        physicalDamage -= (physicalDamage * (character.characterStatsManager.blockingPhysicalAbsorption / 100));
        magicDamage -= (magicDamage * (character.characterStatsManager.blockingMagicAbsorption / 100));
        fireDamage -= (fireDamage * (character.characterStatsManager.blockingFireAbsorption / 100));
        lightningDamage -= (lightningDamage * (character.characterStatsManager.blockingLightningAbsorption / 100));
        holyDamage -= (holyDamage * (character.characterStatsManager.blockingHolyAbsorption / 100));

        

        finalDamageDealt = Mathf.RoundToInt(physicalDamage + magicDamage + fireDamage + lightningDamage + holyDamage);

        if (finalDamageDealt <= 0)
        {
            finalDamageDealt = 1;
        }

        Debug.Log("Final Damage Given" + finalDamageDealt);
        character.characterNetworkManager.currentHealth.Value -= finalDamageDealt;


        // calculate poise damage to derermine if character will be stunned
    }
    private void CalculateStaminaDamage(CharacterManager character)
    {
        if (!character.IsOwner)
            return;

        finalStaminaDamage = staminaDamage;

        float staminaDamageAbsorption = finalStaminaDamage * (character.characterStatsManager.blockingStability / 100);
        float staminaDamageAfterAbsorption = finalStaminaDamage - staminaDamageAbsorption;

        character.characterNetworkManager.currentStamina.Value -= staminaDamageAfterAbsorption;
    }
    private void CheckForGuardBreak(CharacterManager character)
    {
        if (!character.IsOwner) 
            return;

        if(character.characterNetworkManager.currentStamina.Value <= 0)
        {
            character.characterAnimatorManager.PlayTargetActionAnimtion("Guard_Break_01", true);
            character.characterNetworkManager.isBlocking.Value = false;
        }
    }
    private void PlayDamageVFX(CharacterManager character)
    {
        // fire damage, fire particles
        // lightning damage, lightning particles

        // 1. Get VFX based on blocking weapon
    }
    private void PlayDamageSFX(CharacterManager character)
    {
        //AudioClip physiclaDamageSFX = WorldSoundFXManager.instance.ChooseRandomSFXFromArray(WorldSoundFXManager.instance.physicalDamageSFX);

        //character.characterSoundFXManager.PlaySoundFX(physiclaDamageSFX);
        //character.characterSoundFXManager.PlayDamageGruntsSFX();
        //  fire damage is greater than 0, play burn SFX
        // lightning damage is grater than 0, play zap SFX 

        // 1. Get SFX based on blocking 
        character.characterSoundFXManager.PlayBlockSoundFX();
    }
    private void PlayDirectionalBasedBlockingDamageAnimation(CharacterManager character)
    {
        if (!character.IsOwner)
            return;

        if (character.isDead.Value)
            return;

        // 1. Calculate "INTENSITY" base on poise damage
        DamageIntensity damageIntensity = WorldUtilityManager.Instance.GetDamageIntensityBasedOnPoiseDamage(poiseDamage);

        // 2. Play a proper animation to match the "INTENSITY" on the blow

        switch(damageIntensity)
        {
            case DamageIntensity.Ping:
                damageAnimation = "Block_Ping_01";
                break;
            case DamageIntensity.Light:
                damageAnimation = "Block_Light_01";
                break;
            case DamageIntensity.Medium:
                damageAnimation = "Block_Medium_01";
                break;
            case DamageIntensity.Heavy:
                damageAnimation = "Block_Heavy_01";
                break;
            case DamageIntensity.Colossal:
                damageAnimation = "Block_Colossal_01";
                break;

            default:
                break;
        }

        character.characterAnimatorManager.lastDamageAnimationPlayed = damageAnimation;
        character.characterAnimatorManager.PlayTargetActionAnimtion(damageAnimation, true);
    }
}
