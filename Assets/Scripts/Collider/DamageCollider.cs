using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageCollider : MonoBehaviour
{
    [Header("Collider")]
    [SerializeField] protected Collider damageCollider;

    [Header("Damage")]
    public float physicalDamage = 0; // wwill be more detailed in the future
    public float magicDamage = 0;
    public float fireDamage = 0;
    public float lightningDamage = 0;
    public float holyDamage = 0;

    [Header("Poise")]
    public float poiseDamage = 0;

    [Header("Contact Point")]
    protected Vector3 contactPoint;

    [Header("Characters Damage")]
    protected List<CharacterManager> charactersDamaged = new List<CharacterManager>();

    [Header("Block")]
    protected Vector3 directionFromAttackToDamageTarget;
    protected float dotValueFromAttackToDamageTarget;


    protected virtual void Awake()
    {
        damageCollider = GetComponent<Collider>();
    }
    protected virtual void OnTriggerEnter(Collider other)
    {
        CharacterManager damageTarget = other.GetComponentInParent<CharacterManager>();
        

        if(damageTarget != null )
        {
            contactPoint = other.gameObject.GetComponent<Collider>().ClosestPointOnBounds(transform.position);

            // check if i can damage this target based on friendly fire

            // check if target is blocking
            CheckForBlock(damageTarget);

            // check if target is parrying
            CheckForParry(damageTarget);

            // check if target is invulnerable


            // Damage
            if (!damageTarget.characterNetworkManager.isInvulnerable.Value)
                DamageTarget(damageTarget);
        }
    }
    protected virtual void CheckForBlock(CharacterManager damageTarget)
    {
        // If this chacater has already been damaged, do not proceceed
        if (charactersDamaged.Contains(damageTarget))
            return;

        GetBlockingDotValues(damageTarget);

       
  
        // 1. Check if the character being damaged is blocking
        if(damageTarget.characterNetworkManager.isBlocking.Value && dotValueFromAttackToDamageTarget > 0.3f)
        {
            // 2. If character is blocking, check if they are facing in the correct direction to block sucessfully
            charactersDamaged.Add(damageTarget);

            TakeBlockedDamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeBlockedDamageEffect);

            damageEffect.physicalDamage = physicalDamage;
            damageEffect.magicDamage = magicDamage;
            damageEffect.fireDamage = fireDamage;
            damageEffect.holyDamage = holyDamage;
            damageEffect.poiseDamage = poiseDamage;
            damageEffect.staminaDamage = poiseDamage;
            damageEffect.contactPoint = contactPoint;

            // 3. Apply blocked character damage to target
            damageTarget.characterEffectsManager.ProcessInstantEffects(damageEffect);
        }
    }
    protected virtual void CheckForParry(CharacterManager damageTarget)
    {

    }
    protected virtual void GetBlockingDotValues(CharacterManager damageTarget)
    {
        directionFromAttackToDamageTarget = transform.position - damageTarget.transform.position;
        dotValueFromAttackToDamageTarget = Vector3.Dot(directionFromAttackToDamageTarget, damageTarget.transform.forward);
    }
    protected virtual void DamageTarget(CharacterManager damageTarget)
    {
        // do not want to damage the same target more than once in a single attack
        // add a list that check before applying damage

        if(charactersDamaged.Contains(damageTarget))
            return;

        charactersDamaged.Add(damageTarget);

        TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeDamageEffect);
        damageEffect.physicalDamage = physicalDamage;
        damageEffect.magicDamage = magicDamage;
        damageEffect.fireDamage = fireDamage;
        damageEffect.holyDamage = holyDamage;
        damageEffect.poiseDamage = poiseDamage;
        damageEffect.contactPoint = contactPoint;
        damageTarget.characterEffectsManager.ProcessInstantEffects(damageEffect);
    }
    public virtual void EnableDamageCollider()
    {
        damageCollider.enabled = true;
    }
    public virtual void DisableDamageCollider()
    {
        damageCollider.enabled = false;
        charactersDamaged.Clear(); // reset character that have been hit when i reset the collider, so they may be hit again
    }

}
