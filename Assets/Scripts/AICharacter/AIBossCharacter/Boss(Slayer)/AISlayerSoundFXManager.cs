using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;

public class AISlayerSoundFXManager : CharacterSoundFXManager
{
    [Header("Clup Whooshes")]
    public AudioClip[] clubWhooshes;
    
    [Header("Clup Impacts")]
    public AudioClip[] clubImpacts;
    
    [Header("Stomp Impacts")]
    public AudioClip[] stompImpacts;

    public virtual void PlayClubImpactSoundFX()
    {
        if(clubImpacts.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(clubImpacts));
        }
    }
    public virtual void PlayStompImpactSoundFX()
    {
        if (stompImpacts.Length > 0)
        {
            PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(stompImpacts));
        }
    }
}
