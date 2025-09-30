using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISlayerCombatManager : AICharacterCombatManager
{
    AISlayerCharacterManager slayerManager;

    [Header("Damage Collider")]
    [SerializeField] SlayerClubDamageCollider clubDamageCollider;
    [SerializeField] BossStompCollider stompCollider;
    //[SerializeField] Transform bossStompingFoot;
    public float stompAttackAOERadius = 1.5f;

    [Header("Damage")]
    [SerializeField] int baseDamage = 25;
    [SerializeField] int basePoiseDamage = 25;
    [SerializeField] float attack01DamageModifer = 1.0f;
    [SerializeField] float attack02DamageModifier = 1.4f;
    [SerializeField] float attack03DamageModifier = 1.6f;
    public float stompDamage = 25;

    [Header("VFX")]
    [SerializeField]public GameObject SlayerImpactVFX;

    protected override void Awake()
    {
        base.Awake();

        slayerManager = GetComponent<AISlayerCharacterManager>();
    }
    public void SetAttack01Damage()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGruntSFX();
        clubDamageCollider.physicalDamage = baseDamage * attack01DamageModifer;
        clubDamageCollider.poiseDamage = basePoiseDamage * attack01DamageModifer;
    }
    public void SetAttack02Damage()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGruntSFX();
        clubDamageCollider.physicalDamage = baseDamage * attack02DamageModifier;
        clubDamageCollider.poiseDamage = basePoiseDamage * attack02DamageModifier;
    }
    public void SetAttack03Damage()
    {
        aiCharacter.characterSoundFXManager.PlayAttackGruntSFX();
        clubDamageCollider.physicalDamage = baseDamage * attack03DamageModifier;
        clubDamageCollider.poiseDamage = basePoiseDamage * attack03DamageModifier;
    }
    public void OpenClubCollider()
    {
        //aiCharacter.characterSoundFXManager.PlayAttackGrunt();
        clubDamageCollider.EnableDamageCollider();
        slayerManager.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(slayerManager.slayerSoundFXManager.clubWhooshes));
    }
    public void DisableClubCollider()
    {
        clubDamageCollider.DisableDamageCollider();
    }
    //public void ActiveSlayerstomp()
    //{
        

    //    stompCollider.StompAttack();

    //    //Collider[] colliders = Physics.OverlapSphere(bossStompingFoot.position, stompAttackAOERadius,WorldUtilityManager.Instance.getCharacterLayer());
    //    //List<CharacterManager> charatersDamaged = new List<CharacterManager>();
    //    //foreach(var collider in colliders)
    //    //{
    //    //    CharacterManager character = collider.GetComponentInParent<CharacterManager>();

    //    //    if(character != null)
    //    //    {
    //    //        if (charatersDamaged.Contains(character))
    //    //            continue;

    //    //        charatersDamaged.Add(character);

    //    //        if (character.IsOwner)
    //    //        {
    //    //            // check for blocking

    //    //            TakeDamageEffect damageEffect = Instantiate(WorldCharacterEffectManager.instance.takeDamageEffect);
    //    //            damageEffect.physicalDamage = stompDamage;
    //    //            damageEffect.poiseDamage = stompDamage;

    //    //            character.characterEffectsManager.ProcessInstantEffects(damageEffect);
    //    //        }
    //    //    }
    //    //}
    //}


    public override void PivotTowardsTarget(AICharacterManager aiCharacter)
    {
        base.PivotTowardsTarget(aiCharacter);

        if (aiCharacter.isPerformingAction)
            return;

        if (viewableAngle >= 61 && viewableAngle <= 110)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimtion("Turn_Right_90", true);
        }
        else if (viewableAngle <= -61 && viewableAngle >= -110)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimtion("Turn_Left_90", true);
        }
        else if (viewableAngle >= 146 && viewableAngle <= 180)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimtion("Turn_Right_180", true);
        }
        else if (viewableAngle <= -146 && viewableAngle >= -180)
        {
            aiCharacter.characterAnimatorManager.PlayTargetActionAnimtion("Turn_Left_180", true);
        }
        //else if (viewableAngle > -60 && viewableAngle < 60)
        //{
        //    Quaternion targetRotation = Quaternion.LookRotation(targetsDirection);
        //    aiCharacter.transform.rotation = Quaternion.Slerp(aiCharacter.transform.rotation, targetRotation, Time.deltaTime * attackRotationSpeed);
        //}
    }
}
