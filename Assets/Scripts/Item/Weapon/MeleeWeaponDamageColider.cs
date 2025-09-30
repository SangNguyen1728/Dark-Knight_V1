using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeWeaponDamageColider : DamageCollider
{
    [Header("Attacking Character")]
    public CharacterManager characterCausingDamage; // when calculating damage this is used to check for attack damage modifiers, effects 

    [Header("Weapon Attack Modifiers")]
    public float light_Attack_01_Modifier;
    public float light_Attack_02_Modifier;
    public float light_Attack_03_Modifier;
    public float light_Attack_04_Modifier;
    public float heavy_Attack_01_Modifier;
    public float heavy_Attack_02_Modifier;
    public float heavy_Attack_03_Modifier;
    public float charge_Attack_01_Modifier;
    public float charge_Attack_02_Modifier;
    public float charge_Attack_03_Modifier;
    public float charge_Attack_04_Modifier;
    public float running_Attack_01_Modifier;
    public float rolling_Attack_01_Modifier;
    public float backstep_Attack_01_Modifier;



    protected override void Awake()
    {
        base.Awake();

        if(damageCollider == null)
        {
            damageCollider = GetComponent<Collider>();
        }

        damageCollider.enabled = false; // i only want enaable when animation allow
    }

    protected override void OnTriggerEnter(Collider other)
    {
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();


        if (damageTarget != null)
        {
            

            contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            // do not want to damage myself
            if (damageTarget == characterCausingDamage)
                return;

            // check if i can damage this target based on friendly fire
            if (!WorldUtilityManager.Instance.CanIDamageThisTarget(characterCausingDamage.characterGroup, damageTarget.characterGroup))
                return;

            // check if target is parry
            CheckForParry(damageTarget);

            // check if target is blocking
            CheckForBlock(damageTarget);
            // check if target is invulnerable
            

            // Damage
           if(!damageTarget.characterNetworkManager.isInvulnerable.Value)
                DamageTarget(damageTarget);
        }
    }

    protected override void CheckForParry(CharacterManager damageTarget)
    {
        if (charactersDamaged.Contains(damageTarget))
            return;

        if (!characterCausingDamage.characterNetworkManager.isParryable.Value)
            return;

        if(!damageTarget.IsOwner)
            return;

        if(damageTarget.characterNetworkManager.isParrying.Value)
        {
            charactersDamaged.Add(damageTarget);
            damageTarget.characterNetworkManager.NotifyServerOfParryServerRpc(characterCausingDamage.NetworkObjectId);
            damageTarget.characterAnimatorManager.PlayTargetActionAnimtionInstantly("Parry_Land_01", true);
        }
    }
    protected override void GetBlockingDotValues(CharacterManager damageTarget)
    {
        directionFromAttackToDamageTarget = characterCausingDamage.transform.position - damageTarget.transform.position;
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
        damageEffect.angleHitFrom = Vector3.SignedAngle(characterCausingDamage.transform.forward, damageTarget.transform.forward, Vector3.up);
        
        //damageTarget.characterEffectsManager.ProcessInstantEffects(damageEffect);

        //switch(characterCausingDamage.characterCombatManager.currentAttackTpye)
        //{
        //    case AttackType.LightAttack01:
        //        ApplyAttackDamageModifiers(light_Attack_01_Modifier, damageEffect);
        //        break;
        //    case AttackType.LightAttack02:
        //        ApplyAttackDamageModifiers(light_Attack_02_Modifier, damageEffect);
        //        break;
        //    case AttackType.LightAttack03:
        //        ApplyAttackDamageModifiers(light_Attack_03_Modifier, damageEffect);
        //        break;
        //    case AttackType.LightAttack04:
        //        ApplyAttackDamageModifiers(light_Attack_04_Modifier, damageEffect);
        //        break;
        //    case AttackType.HeavyAttack01:
        //        ApplyAttackDamageModifiers(heavy_Attack_01_Modifier, damageEffect);
        //        break;
        //    case AttackType.HeavyAttack02:
        //        ApplyAttackDamageModifiers(heavy_Attack_02_Modifier, damageEffect);
        //        break;
        //    case AttackType.HeavyAttack03:
        //        ApplyAttackDamageModifiers(heavy_Attack_03_Modifier, damageEffect);
        //        break;
        //    case AttackType.ChargedAttack01:
        //        ApplyAttackDamageModifiers(charge_Attack_01_Modifier, damageEffect);
        //        break;
        //    case AttackType.ChargedAttack02:
        //        ApplyAttackDamageModifiers(charge_Attack_02_Modifier, damageEffect);
        //        break;
        //    case AttackType.ChargedAttack03:
        //        ApplyAttackDamageModifiers(charge_Attack_03_Modifier, damageEffect);
        //        break;
        //    //case AttackType.ChargedAttack04:
        //    //    ApplyAttackDamageModifiers(charge_Attack_04_Modifier, damageEffect);
        //    //    break;
        //    default:
        //        break;
        //}

        switch(characterCausingDamage.characterCombatManager.currentAttackTpye)
        {
            case AttackType.LightAttack01:
                ApplyAttackDamageModifiers(light_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.LightAttack02:
                ApplyAttackDamageModifiers(light_Attack_02_Modifier, damageEffect);
                break;
            case AttackType.LightAttack03:
                ApplyAttackDamageModifiers(light_Attack_03_Modifier, damageEffect);
                break;
            case AttackType.LightAttack04:
                ApplyAttackDamageModifiers(light_Attack_04_Modifier, damageEffect);
                break;
            case AttackType.HeavyAttack01:
                ApplyAttackDamageModifiers(heavy_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.HeavyAttack02:
                ApplyAttackDamageModifiers(heavy_Attack_02_Modifier, damageEffect);
                break;
            case AttackType.HeavyAttack03:
                ApplyAttackDamageModifiers(heavy_Attack_03_Modifier, damageEffect);
                break;
            case AttackType.ChargedAttack01:
                ApplyAttackDamageModifiers(charge_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.ChargedAttack02:
                ApplyAttackDamageModifiers(charge_Attack_02_Modifier, damageEffect);
                break;
            case AttackType.ChargedAttack03:
                ApplyAttackDamageModifiers(charge_Attack_03_Modifier, damageEffect);
                break;
            case AttackType.RunningAttack01:
                ApplyAttackDamageModifiers(running_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.RollingAttack01:
                ApplyAttackDamageModifiers(rolling_Attack_01_Modifier, damageEffect);
                break;
            case AttackType.BackstepAttack01:
                ApplyAttackDamageModifiers(backstep_Attack_01_Modifier, damageEffect);
                break;

            default:
                break;



        }

        if(characterCausingDamage.IsOwner)
        {
            damageTarget.characterNetworkManager.NotifyTheServerOfCharacterDamageServerRpc(
                damageTarget.NetworkObjectId,
                characterCausingDamage.NetworkObjectId,
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
    private void ApplyAttackDamageModifiers(float modifer, TakeDamageEffect damage)
    {
        damage.physicalDamage *= modifer;
        damage.magicDamage *= modifer;
        damage.fireDamage *= modifer;
        damage.holyDamage *= modifer;
        damage.poiseDamage *= modifer;

        
    }
}
