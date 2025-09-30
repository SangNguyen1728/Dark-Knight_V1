using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterSoundFXManager : MonoBehaviour
{
    private AudioSource audioSource;

    [Header("Damage Grunts")]
    [SerializeField] protected AudioClip[] damageGrunts;

    [Header("Attack Grunts")]
    [SerializeField] protected AudioClip[] attackGrunts;

    [Header("FootStep")]
    [SerializeField] protected AudioClip[] footStep;
    //public AudioClip[] footStepDirt;
    //public AudioClip[] footStepStone;
    protected virtual void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }
    public void PlaySoundFX(AudioClip soundFX, float volume = 1, bool randomizePitch = true, float pitchRandom = 0.1f)
    {
        audioSource.PlayOneShot(soundFX, volume);
        // reset pitch
        audioSource.pitch = 1;
        if(randomizePitch)
        {
            audioSource.pitch += Random.Range(pitchRandom, pitchRandom);
        }
    }
    public void PlayRollSoundFX()
    {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.rollSFX);
    }
    public virtual void PlayDamageGruntsSFX()
    {
        if(damageGrunts.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(damageGrunts));
        }
    }
    public virtual void PlayAttackGruntSFX()
    {
       if(attackGrunts.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(attackGrunts));
        }
    }
    public virtual void PlayFootStepSFX()
    {
        if (footStep.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(footStep));
        }
    }

    public virtual void PlayStanceBreakSoundFX()
    {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.stanceBreakSFX);
    }
    public virtual void PlayCriticalStrikeSoundFX()
    {
        audioSource.PlayOneShot(WorldSoundFXManager.instance.criticalStrikeSFX);
    }

    public virtual void PlayBlockSoundFX()
    {

    }
}

