using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("A.I/States/ Boss Sleep"))]
public class BaseSleepState : AIState
{
    public override AIState Tick(AICharacterManager aiCharacter)
    {
        return base.Tick(aiCharacter);
    }
}
