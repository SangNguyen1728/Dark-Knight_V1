using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Character Effects/ Instant Effects/ Take Stamina Damage")]
public class TakeStaminaDamageEffect : InstantCharacterEffect
{
    public float staminaDamage;
    public override void ProcessEffect(CharacterManager character)
    {
        CalculateStaminaDamage(character);
    }
    private void CalculateStaminaDamage(CharacterManager character)
    {
        if(character.IsOwner)
        {
            Debug.Log("character is taking: " + staminaDamage + "Stamina damage");
            character.characterNetworkManager.currentStamina.Value -= staminaDamage;
        }
    }
}
