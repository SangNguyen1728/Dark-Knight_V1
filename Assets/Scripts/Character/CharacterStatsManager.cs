using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CharacterStatsManager : MonoBehaviour
{
    CharacterManager character;
    
    [Header("Stamina Regeneration")]
    [SerializeField] float staminaRegenerationAmount = 2;
    private float staminaRegenerationTimer = 0;
    private float staminaTickTimer = 0;
    [SerializeField] float staminaRegenerationDelay = 2;

    [Header("Block Absorptions")]
    public float blockingPhysicalAbsorption;
    public float blockingMagicAbsorption;
    public float blockingFireAbsorption;
    public float blockingLightningAbsorption;
    public float blockingHolyAbsorption;
    public float blockingStability;

    [Header("Armor Absorption")]
    public float armorPhysicalDamageAbsorption;
    public float armorMagicDamageAbsorption;
    public float armorFireDamageAbsorption;
    public float armorLightningDamageAbsorption;
    public float armorHolyDamageAbsorption;

    [Header("Armor Resistance")]
    public float armorImmunity;    // Resistance to rot and poison
    public float armorRobustness;  // Resistance to bleed and frost
    public float armorFocus;       // Resistance to madness and sleep
    public float armorVitality;    // Resistance to death curse

    [Header("Poise")]
    public float totalPoiseDamage;              // How much poise damage i have taken
    public float offensivePoiseBonus;           // The poise bonus from using weapons(heavy weapons have a much larger bonus)
    public float basePoiseDefense;              // The poise bonus gains from armor/ talismans 
    public float defaultPoiseResetTime = 8;     // The time it takes for poise damage to reset (must not be hit in the time or it will reset)
    public float poiseResetTimer = 0;           // The current timer for poise reset

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }
    protected virtual void Start()
    {

    }
    protected virtual void Update()
    {
        HandlePoiseResetTimer();
    }
    public int CalculateHealthBaseOnVitalityLevel(int vitality)
    {
        float health = 0;

        // create An equation for how i want my stamina to be calculated

        health = vitality * 15;

        return Mathf.RoundToInt(health);
    }
    public int CalculateStaminaBaseOnEnduranceLevel(int endurance)
    {
        float stamina = 0;

        // create An equation for how i want my stamina to be calculated

        stamina = endurance * 10;

        return Mathf.RoundToInt(stamina);
    }
    public virtual void RegenerateStamina()
    {
        // only owner can edit their network variable
        if (!character.IsOwner)
            return;

        // i dont want to regenarateStamina if i'm using it
        if (character.characterNetworkManager.isSprinting.Value)
            return;

        if (character.isPerformingAction)
            return;

        staminaRegenerationTimer += Time.deltaTime;

        if (staminaRegenerationTimer >= staminaRegenerationDelay)
        {
            if (character.characterNetworkManager.currentStamina.Value < character.characterNetworkManager.maxStamina.Value)
            {
                staminaTickTimer += Time.deltaTime;

                if (staminaTickTimer >= 0.1)
                {
                    staminaTickTimer = 0;
                    character.characterNetworkManager.currentStamina.Value += staminaRegenerationAmount;
                }
            }
        }
    }
    public virtual void ResetStaminaRegenTimer(float previousStaminaAmount, float currentStaminaAmount)
    {
        // only want to reset the regeneration if the action used stamina
        // i dont want to reset the regeneration if i am already regeneration stamina
        if(currentStaminaAmount < previousStaminaAmount)
        {
            staminaRegenerationTimer = 0;
        }
    }

    protected virtual void HandlePoiseResetTimer()
    {
        if (poiseResetTimer > 0)
        {
            poiseResetTimer -= Time.deltaTime;
        }
        else
        {
            totalPoiseDamage = 0;
        }
    }
}
