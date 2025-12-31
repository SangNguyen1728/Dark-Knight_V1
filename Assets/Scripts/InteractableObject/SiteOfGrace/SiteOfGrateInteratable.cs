using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;



public class SiteOfGrateInteratable : Interactable
{
    [Header("Site of Grace Info")]
    [SerializeField] int siteOfGraceID;
    public NetworkVariable<bool> isActivavted = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    [Header("VFX")]
    [SerializeField] GameObject activavtedParticles;

    [Header("Interaction Text")]
    [SerializeField] string unactivatedInteractionText = "Restore Site of Grace";
    [SerializeField] string activatedInteractionText = "Let Take a Rest";

    protected override void Start()
    {
        base.Start();

        if(IsOwner)
        {
            if (WorldSaveGameManager.instance.currentCharacterData.siteOfGrace.ContainsKey(siteOfGraceID))
            {
                isActivavted.Value = WorldSaveGameManager.instance.currentCharacterData.siteOfGrace[siteOfGraceID];
            }
            else
            {
                isActivavted.Value = false;
            }
        }

        if(isActivavted.Value)
        {
            interactableText = activatedInteractionText;
        }
        else
        {
            interactableText = unactivatedInteractionText;
        }
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        if (!IsOwner)
        {
            OnIsActivavtedChanged(false, isActivavted.Value);
        }

        isActivavted.OnValueChanged += OnIsActivavtedChanged;
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        isActivavted.OnValueChanged -= OnIsActivavtedChanged;
    }
    private void RestoreSiteOfGrace(PlayerManager player)
    {
        isActivavted.Value = true;
        // TODO
        // Add site of grace to activavted sites in save file

        // If my save files contains ìno on this site of grace, remove it
        if(WorldSaveGameManager.instance.currentCharacterData.siteOfGrace.ContainsKey(siteOfGraceID))
        {
            WorldSaveGameManager.instance.currentCharacterData.siteOfGrace.Remove(siteOfGraceID);
        }

        //  then re-add it with value of true(is activacted)
        WorldSaveGameManager.instance.currentCharacterData.siteOfGrace.Add(siteOfGraceID, true);

        // Play anim
        player.playerAnimatorManager.PlayTargetActionAnimtion("Activate_Site_Of_Grace_01", true);

        // Send PopUp
        PlayerUIManager.instance.playerUIPopUpManager.SendGraceRestoredPopUp("SITE OF GRACE RESTORED");
        // Enables/Activaves this site of grace

        WorldSaveGameManager.instance.SaveGame();


        StartCoroutine(WaitForAnimationAndPopUpThenRestoreCollider());
    }
    private void RestAtSiteOfGrace(PlayerManager player)
    {
       
        interactableCollider.enabled = true; // temporarily er-enabling the collider here until i add menu so i can respawn creep indefinitely
        player.playerNetworkManager.currentHealth.Value = player.playerNetworkManager.maxhealth.Value;
        player.playerNetworkManager.currentStamina.Value = player.playerNetworkManager.maxStamina.Value;
        
        //WorldAIManager.instance.SpawnAllCharacters();
        WorldAIManager.instance.ResetAllCharacters();
    }
    private IEnumerator WaitForAnimationAndPopUpThenRestoreCollider()
    {
        yield return new WaitForSeconds(2); // Give enough time for animation to play and pop up begin fading
        interactableCollider.enabled = true;

    }
    private void OnIsActivavtedChanged(bool oldStatus, bool newStatus)
    {
        if (isActivavted.Value)
        {
            activavtedParticles.SetActive(true);

            interactableText = activatedInteractionText;
        }
        else
        {
            interactableText = unactivatedInteractionText;
        }
    }
    public override void Interact(PlayerManager player)
    {
        base.Interact(player);

        if(!isActivavted.Value)
        {
            RestoreSiteOfGrace(player);
        }
        else
        {
            RestAtSiteOfGrace(player);
        }
    }
}
