using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldUtilityManager : MonoBehaviour
{
    public static WorldUtilityManager Instance;

    [Header("Layers")]
    [SerializeField] LayerMask characterLayers;
    [SerializeField] LayerMask enviroLayers;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    public LayerMask getCharacterLayer()
    {
        return characterLayers;
    }
    public LayerMask getEnviroLayer()
    {
        return enviroLayers;
    }
    public bool CanIDamageThisTarget(CharacterGroup attackingCharacter, CharacterGroup targetCharacter)
    {
        if(attackingCharacter == CharacterGroup.Team01)
        {
            switch (targetCharacter)
            {
                case CharacterGroup.Team01: return false;

                case CharacterGroup.Team02: return true;

                //case CharacterGroup.Team03: return true;
             
                //case CharacterGroup.Team04: return true;
         
                default:
                    break;
            }
        }
        else if(attackingCharacter == CharacterGroup.Team02)
        {
            switch (targetCharacter)
            {
                case CharacterGroup.Team01: return true;
                   
                case CharacterGroup.Team02: return false;
                    
                //case CharacterGroup.Team03: return true;
                    
                //case CharacterGroup.Team04: return true;
                    
                default:
                    break;
            }
        }
        //else if (attackingCharacter == CharacterGroup.Team03)
        //{
        //    switch (targetCharacter)
        //    {
        //        case CharacterGroup.Team01: return true;
                   
        //        case CharacterGroup.Team02: return true;
                    
        //        case CharacterGroup.Team03: return false;
                    
        //        case CharacterGroup.Team04: return false;
                    
        //        default:
        //            break;
        //    }
        //}
        //else if( attackingCharacter == CharacterGroup.Team04)
        //{
        //    switch (targetCharacter)
        //    {
        //        case CharacterGroup.Team01: return true;
                    
        //        case CharacterGroup.Team02: return true;
                    
        //        case CharacterGroup.Team03: return true;
                   
        //        case CharacterGroup.Team04: return false;
                    
        //        default:
        //            break;
        //    }
        //}
        return false;
    }
    public float GetAngleOfTarget(Transform characterTransform, Vector3 targetsDirection)
    {
        targetsDirection.y = 0;
        float viewableAngle = Vector3.Angle(characterTransform.forward, targetsDirection);
        Vector3 cross = Vector3.Cross(characterTransform.forward, targetsDirection);

        if(cross.y <0) 
            viewableAngle = -viewableAngle;

        return viewableAngle;
    }
    public DamageIntensity GetDamageIntensityBasedOnPoiseDamage(float poiseDamage)
    {
        // Throwing dagger
        DamageIntensity damageIntensity = DamageIntensity.Ping;

        // Dagger / Light Atk
        if (poiseDamage >= 10)
            damageIntensity = DamageIntensity.Light;

        // Standard weapons / Medium Atk
        if (poiseDamage >= 30)
            damageIntensity = DamageIntensity.Medium;

        // Great weapons / Heavy Atk
        if (poiseDamage >= 70)
            damageIntensity = DamageIntensity.Heavy;

        // Ultra weapons / Colossal Atk
        if (poiseDamage >= 120)
            damageIntensity = DamageIntensity.Colossal;

        return damageIntensity;
    }

    public Vector3 GetRipostingPositionBasedOnWeaponClass(WeaponClass weaponClass)
    {
        Vector3 position = new Vector3(0.11f, 0, 0.7f);

        switch(weaponClass)
        {
            case WeaponClass.KatanaBlue: // Change Position here if you desire
                break;
            case WeaponClass.TwinBlades:
                break;
            case WeaponClass.LightningTwinBlades:
                break;
            case WeaponClass.Shield:
                break;
            case WeaponClass.Fist:
                break;

            default:
                break;
        }

        return position;
    }
    public Vector3 GetBackstabbPositionBasedOnWeaponClass(WeaponClass weaponClass)
    {
        Vector3 position = new Vector3(0.12f, 0, 0.74f);

        switch(weaponClass)
        {
            case WeaponClass.KatanaBlue: // Change Position here if you desire
                break;
            case WeaponClass.TwinBlades:
                break;
            case WeaponClass.LightningTwinBlades:
                break;
            case WeaponClass.Shield:
                break;
            case WeaponClass.Fist:
                break;

            default:
                break;
        }

        return position;
    }
}
