using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class AICharacterInventoryManager : CharacterInventoryManager
{
    AICharacterManager aiCharacter;

    [Header("Loot Chance")]
    [SerializeField] Item[] droppableItems;
    public int dropItemChance = 10;

    protected override void Awake()
    {
        base.Awake();

        aiCharacter = GetComponent<AICharacterManager>();
    }
    public void DropItem()
    {
        if (!aiCharacter.IsOwner)
            return;

        // Status of if this character will drop item
        bool willDropItem = false;

        // Random number rolled from 0 - 100 
        int itemChanceRoll = Random.Range(0, 100);

        // If the number is equal to or lower than the item drop chance
        if(itemChanceRoll <= dropItemChance)
            willDropItem = true;

        if (!willDropItem)
            return;

        Item generatedItem = droppableItems[Random.Range(0, droppableItems.Length)];

        if (generatedItem == null)
            return;

        GameObject itemPickUpInteractetableGameObject = Instantiate(WorldItemDatabase.Instance.pickUpItemPrefab);
        PickUpItemInteractable pickUpInteractable = itemPickUpInteractetableGameObject.GetComponent<PickUpItemInteractable>();

        itemPickUpInteractetableGameObject.GetComponent<NetworkObject>().Spawn();


        pickUpInteractable.itemID.Value = generatedItem.itemID;

        pickUpInteractable.networkPosition.Value = transform.position;

        pickUpInteractable.droppingCreatureID.Value = aiCharacter.NetworkObjectId;
    }
}
