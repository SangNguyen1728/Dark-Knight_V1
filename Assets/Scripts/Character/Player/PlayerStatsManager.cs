using Unity.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerStatsManager : CharacterStatsManager
{
    PlayerManager player;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();
    }

    protected override void Start()
    {
        base.Start();

        // when i make character creation menu, set stats depending on class, this will be calculated there
        // until then however, stats are never calculated, so i do it here on stat, if a save file exists, they overwrite it
        CalculateHealthBaseOnVitalityLevel(player.playerNetworkManager.vitality.Value);
        CalculateStaminaBaseOnEnduranceLevel(player.playerNetworkManager.endurance.Value);
    }

    public void CalculateTotalArmorAbsorption()
    {
        // Reset all value = 0
        armorPhysicalDamageAbsorption = 0;
        armorMagicDamageAbsorption = 0;
        armorFireDamageAbsorption = 0;
        armorHolyDamageAbsorption = 0;
        armorLightningDamageAbsorption = 0;

        armorImmunity = 0;
        armorRobustness = 0;
        armorFocus = 0;
        armorVitality = 0;

        basePoiseDefense = 0;

        // Head Equipment
        if(player.playerInventoryManager.headEquipment != null)
        {
            // damage Resistance
            armorPhysicalDamageAbsorption += player.playerInventoryManager.headEquipment.physicalDamageAbsorption;
            armorMagicDamageAbsorption += player.playerInventoryManager.headEquipment.magicDamageAbsorption;
            armorFireDamageAbsorption += player.playerInventoryManager.headEquipment.fireDamageAbsorption;
            armorHolyDamageAbsorption += player.playerInventoryManager.headEquipment.holyDamageAbsorption;
            armorLightningDamageAbsorption += player.playerInventoryManager.headEquipment.lightningDamageAbsorption;

            // Status Effect Resistance
            armorImmunity += player.playerInventoryManager.headEquipment.immunity;
            armorRobustness += player.playerInventoryManager.headEquipment.robustness;
            armorFocus += player.playerInventoryManager.headEquipment.focus;
            armorVitality += player.playerInventoryManager.headEquipment.vitality;

            // Poise
            basePoiseDefense += player.playerInventoryManager.headEquipment.poise;
        }
        // Body Equipment
        if (player.playerInventoryManager.bodyEquipment != null)
        {
            // damage Resistance
            armorPhysicalDamageAbsorption += player.playerInventoryManager.bodyEquipment.physicalDamageAbsorption;
            armorMagicDamageAbsorption += player.playerInventoryManager.bodyEquipment.magicDamageAbsorption;
            armorFireDamageAbsorption += player.playerInventoryManager.bodyEquipment.fireDamageAbsorption;
            armorHolyDamageAbsorption += player.playerInventoryManager.bodyEquipment.holyDamageAbsorption;
            armorLightningDamageAbsorption += player.playerInventoryManager.bodyEquipment.lightningDamageAbsorption;

            // Status Effect Resistance
            armorImmunity += player.playerInventoryManager.bodyEquipment.immunity;
            armorRobustness += player.playerInventoryManager.bodyEquipment.focus;
            armorVitality += player.playerInventoryManager.bodyEquipment.vitality;

            // Poise
            basePoiseDefense += player.playerInventoryManager.bodyEquipment.poise;
        }
        // Hand Equipment
        if (player.playerInventoryManager.handEquipment != null)
        {
            // damage Resistance
            armorPhysicalDamageAbsorption += player.playerInventoryManager.handEquipment.physicalDamageAbsorption;
            armorMagicDamageAbsorption += player.playerInventoryManager.handEquipment.magicDamageAbsorption;
            armorFireDamageAbsorption += player.playerInventoryManager.handEquipment.fireDamageAbsorption;
            armorHolyDamageAbsorption += player.playerInventoryManager.handEquipment.holyDamageAbsorption;
            armorLightningDamageAbsorption += player.playerInventoryManager.handEquipment.lightningDamageAbsorption;

            // Status Effect Resistance
            armorImmunity += player.playerInventoryManager.handEquipment.immunity;
            armorRobustness += player.playerInventoryManager.handEquipment.focus;
            armorVitality += player.playerInventoryManager.handEquipment.vitality;

            // Poise
            basePoiseDefense += player.playerInventoryManager.handEquipment.poise;
        }
        // Leg Equipment
        if (player.playerInventoryManager.legEquipment != null)
        {
            // damage Resistance
            armorPhysicalDamageAbsorption += player.playerInventoryManager.legEquipment.physicalDamageAbsorption;
            armorMagicDamageAbsorption += player.playerInventoryManager.legEquipment.magicDamageAbsorption;
            armorFireDamageAbsorption += player.playerInventoryManager.legEquipment.fireDamageAbsorption;
            armorHolyDamageAbsorption += player.playerInventoryManager.legEquipment.holyDamageAbsorption;
            armorLightningDamageAbsorption += player.playerInventoryManager.legEquipment.lightningDamageAbsorption;

            // Status Effect Resistance
            armorImmunity += player.playerInventoryManager.legEquipment.immunity;
            armorRobustness += player.playerInventoryManager.legEquipment.focus;
            armorVitality += player.playerInventoryManager.legEquipment.vitality;

            // Poise
            basePoiseDefense += player.playerInventoryManager.legEquipment.poise;
        }

    }
}
