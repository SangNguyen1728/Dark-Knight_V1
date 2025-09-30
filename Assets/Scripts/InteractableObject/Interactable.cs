using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class Interactable : NetworkBehaviour
{
    public string interactableText; // Text prompt when entering the interactable collider (Pick up items, ...)
    [SerializeField] protected Collider interactableCollider; // that checks for player interaction
    [SerializeField] protected bool hostOnlyInteractable = true;

    protected virtual void Awake()
    {
        // Check if its null, in some case i may want to manually assign a collider á a child object
        if(interactableCollider == null)
            interactableCollider = GetComponent<Collider>();
    }
    protected virtual void Start()
    {
        
    }
    public virtual void Interact(PlayerManager player)
    {
        Debug.Log("Có Sự Tư Tác");

        if (!player.IsOwner)
            return;

        interactableCollider.enabled = false;
        player.playerInteractableManager.RemoveInteractionFromList(this);
        PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();

        
    }
    public virtual void OnTriggerEnter(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();

        if(player != null)
        {
            if (!player.playerNetworkManager.IsHost && hostOnlyInteractable)
                return;

            if (!player.IsOwner)
                return;

            // Pass the interactable to player
            player.playerInteractableManager.AddInteractionToList(this);
        }
    }
    public virtual void OnTriggerExit(Collider other)
    {
        PlayerManager player = other.GetComponent<PlayerManager>();

        if (player != null)
        {
            if (!player.playerNetworkManager.IsHost && hostOnlyInteractable)
                return;
            if (!player.IsOwner)
                return;

            // Rmove the interactable from player
            player.playerInteractableManager.RemoveInteractionFromList(this);
            PlayerUIManager.instance.playerUIPopUpManager.CloseAllPopUpWindows();
        }
    }
}
