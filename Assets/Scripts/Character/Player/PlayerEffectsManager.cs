using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffectsManager : CharacterEffectsManager
{
    [Header("test")]
    [SerializeField] InstantCharacterEffect efftectToTest;
    [SerializeField] bool processEffect = false;

    private void Update()
    {
        if(processEffect)
        {
            processEffect = false;

            InstantCharacterEffect effect = Instantiate(efftectToTest);

            ProcessInstantEffects(efftectToTest);  
        }
    }
}
