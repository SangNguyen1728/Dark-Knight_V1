using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class UI_Boss_HP_Bar : UI_StatBar
{
    [SerializeField] AIBossCharacterManager bossCharacter;

    public void EnabbleBossHPBar(AIBossCharacterManager boss)
    {
        bossCharacter = boss;
        bossCharacter.aiCharacterNetworkManager.currentHealth.OnValueChanged += OnBossHPChanged;
        SetMaxStat(bossCharacter.aiCharacterNetworkManager.maxhealth.Value);
        SetStat(bossCharacter.aiCharacterNetworkManager.currentHealth.Value);
        GetComponentInChildren<TextMeshProUGUI>().text = bossCharacter.CharacterName; 
    }
    private void OnDestroy()
    {
        bossCharacter.aiCharacterNetworkManager.currentHealth.OnValueChanged -= OnBossHPChanged;
    }
    private void OnBossHPChanged(int oldvalue, int newValue)
    {
        SetStat(newValue);
        if(newValue <= 0)
        {
            RemoveHPBar(2.5f);
        }
    }
    public void RemoveHPBar(float time)
    {
        Destroy(gameObject, time);
    }
}
