using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class AIBossCharacterManager : AICharacterManager
{

    // Give AI Boss a unique ID
    public int bossID = 0;

    [Header("Music")]
    [SerializeField] AudioClip bossIntroClip;
    [SerializeField] AudioClip bossBattleLoopClip;

    [Header("Status")]
    public NetworkVariable<bool> bossFightIsActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> hasBeenAwakened = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<bool> hasBeenDefeated = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    [SerializeField] List<FogWallInteractable> fogWalls;
    [SerializeField] string sleepAnimation; 
    [SerializeField] string awakenAnimation;

    [Header("Phase Shift")]
    public float minimumHealthPercentageToShift = 50;
    [SerializeField] string phaseShiftAnimation = " Phase_Change_01";
    [SerializeField] CombatStanceState phase02CombatStanceState;

    [Header("States")]
    [SerializeField] BaseSleepState sleepState;

    [Header("Character Name")]
    [SerializeField] public string CharacterName;
    // When this AI is spawned, check save file (use dictionary)
    // If save file does not contain a Boss with ID add it
    // if it is present, check if the Boss/Bosses have been defeated
    // If Bosses have been defeated, disable this gameObj
    // If not, allow this object to continue to be active

    
    protected override void Awake()
    {
        base.Awake();

    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        bossFightIsActive.OnValueChanged += OnBossFightIsActiveChanged;
        OnBossFightIsActiveChanged(false, bossFightIsActive.Value);

        

        if (IsOwner)
        {
            sleepState = Instantiate(sleepState);
            currentState = sleepState;
        }

        if (IsServer)
        {
            // if our save date does not contain information on this Boss, add it now
            if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, false);
                WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, false);
            }
            // Otherwise, load the data that already exists on this boss
            else
            {
                hasBeenDefeated.Value = WorldSaveGameManager.instance.currentCharacterData.bossesDefeated[bossID];
                hasBeenAwakened.Value = WorldSaveGameManager.instance.currentCharacterData.bossesAwakened[bossID];
               
            }

            // Locate Fog Wall
            StartCoroutine(GetFogWallsFromWorldObjectManager());

            // Boss Awake, enable fog walls
            if (hasBeenAwakened.Value) 
            {
                for(int i = 0; i < fogWalls.Count; i++)
                {
                    fogWalls[i].isActive.Value = true;
                }
            }

            // Boss Defeatead, disable fog walls
            if (hasBeenDefeated.Value) 
            {
                for (int i = 0; i < fogWalls.Count; i++)
                {
                    fogWalls[i].isActive.Value = false;
                }
                aiCharacterNetworkManager.isActive.Value = false;
            }
        }

        if(!hasBeenAwakened.Value)
        {
            animator.Play(sleepAnimation);
            //characterAnimatorManager.PlayTargetActionAnimtion(sleepAnimation, true);
            //currentState = sleepState;
        }
       //else
       // {
       //     currentState = idle;
       // }
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        bossFightIsActive.OnValueChanged -= OnBossFightIsActiveChanged;
    }
    private IEnumerator GetFogWallsFromWorldObjectManager()
    {
        while (WorldObjectManager.instance.fogWalls.Count == 0)
            yield return new WaitForEndOfFrame();

        fogWalls = new List<FogWallInteractable>();

        foreach (var fogWall in WorldObjectManager.instance.fogWalls)
        {
            if (fogWall.fogWallID == bossID)
            {
                fogWalls.Add(fogWall);
            }
        }
    }
    public override IEnumerator ProcessDeathEvent(bool manuallySelectDeathAnimation = false)
    {

        PlayerUIManager.instance.playerUIPopUpManager.SendBossDefeatedPopUp("Graet Foe Fella");

        if (IsOwner)
        {
            characterNetworkManager.currentHealth.Value = 0;
            isDead.Value = true;
            bossFightIsActive.Value = false;
           
            foreach(var fogWall in fogWalls)
            {
                fogWall.isActive.Value = false;
            }
            
            if (!manuallySelectDeathAnimation)
            {
                characterAnimatorManager.PlayTargetActionAnimtion("Dead_01", true);

                hasBeenDefeated.Value = true;

                if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
                {
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                    WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
                }
                // Otherwise, load the data that already exists on this boss
                else
                {
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Remove(bossID);
                    WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Remove(bossID);
                    WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
                    WorldSaveGameManager.instance.currentCharacterData.bossesDefeated.Add(bossID, true);
                }

                WorldSaveGameManager.instance.SaveGame();
            }

        }
        yield return new WaitForSeconds(5);
    }
    public void WakeBoss()
    {
        if(IsOwner)
        {
            if (!hasBeenAwakened.Value)
            {
                characterAnimatorManager.PlayTargetActionAnimtion(awakenAnimation, true);
            }

            bossFightIsActive.Value = true;
            hasBeenAwakened.Value = true;
            currentState = idle;

            if (!WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.ContainsKey(bossID))
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);

            }
            else
            {
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Remove(bossID);
                WorldSaveGameManager.instance.currentCharacterData.bossesAwakened.Add(bossID, true);
            }

            for (int i = 0; i < fogWalls.Count; i++)
            {
                fogWalls[i].isActive.Value = true;
            }

        }

        
    }
    private void OnBossFightIsActiveChanged(bool oldStatus, bool newStatus)
    {
        if(bossFightIsActive.Value)
        {
            WorldSoundFXManager.instance.PlayBossTrack(bossIntroClip, bossBattleLoopClip);

            GameObject bossHealthBar = Instantiate(PlayerUIManager.instance.playerHudManager.bossHealthBarObject, PlayerUIManager.instance.playerHudManager.bossHealthBarInParent);

            UI_Boss_HP_Bar bossHpBar = bossHealthBar.GetComponentInChildren<UI_Boss_HP_Bar>();
            bossHpBar.EnabbleBossHPBar(this);
        }
        else
        {
            WorldSoundFXManager.instance.StopBossMusic();
        }
    }
    public void PhaseShift()
    {
        characterAnimatorManager.PlayTargetActionAnimtion(phaseShiftAnimation, true);
        combatStance = Instantiate(phase02CombatStanceState);
        currentState = combatStance;
    }
}
