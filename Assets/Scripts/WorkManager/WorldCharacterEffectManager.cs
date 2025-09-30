using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldCharacterEffectManager : MonoBehaviour
{
    public static WorldCharacterEffectManager instance;

    [Header("VFX")]
    public GameObject bloodSplatterVFX;
    public GameObject criticalBloodSplatterVFX;
    public GameObject healingFlaskVFX;

    [Header("Damage")]
    public TakeDamageEffect takeDamageEffect;
    public TakeBlockedDamageEffect takeBlockedDamageEffect;
    public TakeCriticalDamageEffect takeCriticalDamageEffect;

    [Header("Two Hand")]
    public TwoHandingEffect twoHandingEffect;

    [Header("Instant Effects")]
    [SerializeField] List<InstantCharacterEffect> instanceEffects;

    [Header("Static Effects")]
    [SerializeField] List<StaticCharacterEffect> staticEffects;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        GenerateEffectIDs();
    }
    private void GenerateEffectIDs()
    {
        for(int i = 0; i < instanceEffects.Count; i++)
        {
            instanceEffects[i].instantEffectID = i;
        }

        for(int i =0;  i < staticEffects.Count; i++)
        {
            staticEffects[i].staticEffectID = i;
        }
    }
}
