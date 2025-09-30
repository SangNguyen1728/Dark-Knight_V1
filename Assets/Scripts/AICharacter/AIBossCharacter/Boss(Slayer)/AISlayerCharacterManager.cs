using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AISlayerCharacterManager : AIBossCharacterManager
{
    [HideInInspector]public AISlayerSoundFXManager slayerSoundFXManager;
    [HideInInspector] public AISlayerCombatManager slayerCombatManager;

    protected override void Awake()
    {
        base.Awake();

        slayerSoundFXManager = GetComponent<AISlayerSoundFXManager>();
        slayerCombatManager = GetComponent<AISlayerCombatManager>();
    }
}
