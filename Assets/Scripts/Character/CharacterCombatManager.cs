using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using System.Threading;

public class CharacterCombatManager : NetworkBehaviour
{
    protected CharacterManager character;

    [Header("Last Attack Animation Performed")]
    public string lastAttackAnimationPerformed;

    [Header("Previous Poise Damage Taken")]
    public float previousPoiseDamageTaken;

    [Header("Attack Target")]
    public CharacterManager currentTarget;

    [Header("Attack Type")]
    public AttackType currentAttackTpye;

    [Header("lock On Transform")]
    public Transform lockOnTransform;

    [Header("Action Flags")]
    public bool canPerformRollingAttack = false; 
    public bool canPerformBackStepAttack = false;
    public bool canBlock = true;
    public bool canBeBackStabbed = true;

    [Header("Critical Attack")]
    private Transform riposteReceiverTransform;
    private Transform backstabReceiverTransform;
    [SerializeField] float criticalAttackDistanceCheck = 0.7f;
    public int pendingCriticalDamage;

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();

        if (lockOnTransform == null)
            lockOnTransform = transform;
    }
    public virtual void SetTarget(CharacterManager newTarget)
    {
        if(character.IsOwner)
        {
            if(newTarget != null)
            {
                currentTarget = newTarget;
                character.characterNetworkManager.currentTargetNetworkObjectID.Value = newTarget.GetComponent<NetworkObject>().NetworkObjectId;
            }
            else
            {
                currentTarget = null;
            }
        }
    }

    // For Backstab/Riposte
    public virtual void AttemptCriticalAttack()
    {
        // Can not perform critical strike if performing another
        if (character.isPerformingAction)
            return;

        // Can not perform critical strike if out of stamina
        if (character.characterNetworkManager.currentStamina.Value <= 0)
            return;

        // Aim a raycast infont of me and check for any potential target to critical attack
        RaycastHit[] hits = Physics.RaycastAll(character.characterCombatManager.lockOnTransform.position,
            character.transform.TransformDirection(Vector3.forward), criticalAttackDistanceCheck, WorldUtilityManager.Instance.getCharacterLayer());

        for(int i = 0; i < hits.Length; i++)
        {
            // Check each of the HITS 1 by 1, giving them their own variable
            RaycastHit hit = hits[i];

            CharacterManager targetCharacter = hit.transform.GetComponent<CharacterManager>();

            if(targetCharacter != null)
            {
                // if the character is the one attemping the critical strike, go to next hit in the array of total hits
                if (targetCharacter == character)
                    continue;

                // If i can not damage the character that is targeted continue to check the next hit in the array of HITS
                if (!WorldUtilityManager.Instance.CanIDamageThisTarget(character.characterGroup, targetCharacter.characterGroup))
                    continue;

                // This gets my position and angle in respect to my current critical damage target
                Vector3 directionFromCharacterToTarget = character.transform.position - targetCharacter.transform.position;
                float targetViewableAngle = Vector3.SignedAngle(directionFromCharacterToTarget, targetCharacter.transform.forward,Vector3.up);

                if(targetCharacter.characterNetworkManager.isRipostable.Value)
                {
                    if(targetViewableAngle >= -60 && targetViewableAngle <= 60)
                    {
                        AttemptRiposte(hit);
                        return;
                    }
                }

                // ToDo: BackStab
                if(targetCharacter.characterCombatManager.canBeBackStabbed)
                {
                    if (targetViewableAngle <= 180 && targetViewableAngle >= 145)
                    {
                        AttemptBackStab(hit);
                        return;
                    }
                    if (targetViewableAngle >= -180 && targetViewableAngle <= -145)
                    {
                        AttemptBackStab(hit);
                        return;
                    }
                }
            }
        }
    }
    public virtual void AttemptRiposte(RaycastHit hit)
    {
        Debug.Log("Riposting Target");

        
    }
    public virtual void AttemptBackStab(RaycastHit hit)
    {
        Debug.Log("BackStab Target");


    }
    public virtual void ApplyCriticalDamage()
    {
        character.characterEffectsManager.PlayCriticalBloodSplatterVFX(character.characterCombatManager.lockOnTransform.position);
        character.characterSoundFXManager.PlayCriticalStrikeSoundFX();

        if(character.IsOwner)
        {
            character.characterNetworkManager.currentHealth.Value -= pendingCriticalDamage;
        }
    }
    public IEnumerator ForceMoveEnemyCharacterToRipostePosition(CharacterManager enemyCharacter, Vector3 ripostePosition)
    {
        float timer = 0;

        while(timer < 0.2f)
        {
            timer += Time.deltaTime;

            if(riposteReceiverTransform == null)
            {
                GameObject riposteTransformObject = new GameObject("Riposte Transform");
                riposteTransformObject.transform.parent = transform;
                riposteTransformObject.transform.position = Vector3.zero;
                riposteReceiverTransform = riposteTransformObject.transform;
            }

            riposteReceiverTransform.localPosition = ripostePosition;
            enemyCharacter.transform.position = riposteReceiverTransform.position;
            transform.rotation = Quaternion.LookRotation(-enemyCharacter.transform.forward);

            yield return null;
        }
    }

    public IEnumerator ForceMoveEnemyCharacterToBackstabPosition(CharacterManager enemyCharacter, Vector3 backstabPosition)
    {
        float timer = 0;

        while (timer < 0.2f)
        {
            timer += Time.deltaTime;

            if (riposteReceiverTransform == null)
            {
                GameObject backstabTransformObject = new GameObject("Backstab Transform");
                backstabTransformObject.transform.parent = transform;
                backstabTransformObject.transform.position = Vector3.zero;
                backstabReceiverTransform = backstabTransformObject.transform;
            }

            backstabReceiverTransform.localPosition = backstabPosition;
            enemyCharacter.transform.position = backstabReceiverTransform.position;
            transform.rotation = Quaternion.LookRotation(enemyCharacter.transform.forward);
            yield return null;
        }
    }
    public void EnableIsInvulnerable()
    {
        if(character.IsOwner)
        {
            character.characterNetworkManager.isInvulnerable.Value = true;
        }
    }
    public void DisableIsInvulnerable()
    {
        if(character.IsOwner)
        {
            character.characterNetworkManager.isInvulnerable.Value = false;
        }
    }

    public void EnableIsParring()
    {
        if (character.IsOwner)
        {
            character.characterNetworkManager.isParrying.Value = true;
        }
    }
    public void DisableIsParrying()
    {
        if (character.IsOwner)
        {
            character.characterNetworkManager.isParrying.Value = false;
        }
    }

    public void EnableRipostable()
    {
        if( character.IsOwner) 
            character.characterNetworkManager.isRipostable.Value = true;
    }
    public void EnableCanDoRollingAttack()
    {
        canPerformRollingAttack = true;
    }
    public void DisableCanDoRollingAttack()
    {
        canPerformRollingAttack = false;
    }
    public void EnableCanDoBackStepAttack()
    {
        canPerformBackStepAttack = true;
    }
    public void DisableCanDoBackStepAttack()
    {
        canPerformBackStepAttack = false;
    }
    public virtual void EnableCanDoCombo()
    {
    }
    public virtual void DisableCanDoCombo()
    {

    }

    public virtual void CloseAllDamageColliders()
    {

    }
}
