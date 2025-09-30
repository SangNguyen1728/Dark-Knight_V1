using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class CharacterEffectsManager : MonoBehaviour
{
    // process instant effects: take damage, heal

    // process timed effects: postion build up

    // static effects

    CharacterManager character;

    [Header("VFX")]
    [SerializeField] GameObject bloodSplatterVFX;
    [SerializeField] GameObject criticalBloodSplatterVFX;

    [Header("Current Active FX")]
    public GameObject activeQuickSlotItemFX;

    [Header("Static Effect")]
    public List<StaticCharacterEffect> staticEffects = new List<StaticCharacterEffect>();

    protected virtual void Awake()
    {
        character = GetComponent<CharacterManager>();
    }
    public virtual void ProcessInstantEffects(InstantCharacterEffect effect)
    {
        effect.ProcessEffect(character);
    }
    public void PlayBloodSplatterVFX(Vector3 contactPoint)
    {

        // if i manually have placed a blood splatter vfx on model, play its
        //if (bloodSplatterVFX != null)
        //{
        //    GameObject bloodSplatter = Instantiate(bloodSplatterVFX,contactPoint,Quaternion.identity);
        //}
        //// play default wherever else
        //else
        //{
        //    GameObject bloodSplatter = Instantiate(WorldCharacterEffectManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
        //}
        if (bloodSplatterVFX != null)
        {
            Debug.Log(contactPoint);
            GameObject bloodSplatter = Instantiate(bloodSplatterVFX, contactPoint, Quaternion.identity);
        }
        else
        {
            Debug.Log(contactPoint);
            GameObject bloodSplatter = Instantiate(WorldCharacterEffectManager.instance.bloodSplatterVFX, contactPoint, Quaternion.identity);
            
        }
    }

    public void PlayCriticalBloodSplatterVFX(Vector3 contactPoint)
    {

        // if i manually have placed a blood splatter vfx on model, play its
        
        if (bloodSplatterVFX != null)
        {
            Debug.Log(contactPoint);
            GameObject bloodSplatter = Instantiate(criticalBloodSplatterVFX, contactPoint, Quaternion.identity);
        }
        else
        {
            Debug.Log(contactPoint);
            GameObject bloodSplatter = Instantiate(WorldCharacterEffectManager.instance.criticalBloodSplatterVFX, contactPoint, Quaternion.identity);

        }
    }
    public void AddStaticEffect(StaticCharacterEffect effect)
    {
        // Add static effect to character
        staticEffects.Add(effect);

        // Process its effect
        effect.ProcessStaticEffect(character);

        // Check null and Remove effect 
        for(int i = staticEffects.Count -1; i > -1; i--)
        {
            if(staticEffects[i] == null)
                staticEffects.RemoveAt(i);
        }
    }

    public void RemoveStaticEffect(int effectID)
    {
        StaticCharacterEffect effect;

        for(int i = 0; i < staticEffects.Count; i++)
        {
            if (staticEffects[i] != null)
            {
                if (staticEffects[i].staticEffectID == effectID)
                {
                    effect = staticEffects[i];

                    // Remove static effect from character
                    effect.RemoveStaticEffect(character);
                    // Remove static effet from list
                    staticEffects.Remove(effect);
                }
            }
        }

        // Check null and Remove effect 
        for (int i = staticEffects.Count - 1; i > -1; i--)
        {
            if (staticEffects[i] == null)
                staticEffects.RemoveAt(i);
        }
    }
}
