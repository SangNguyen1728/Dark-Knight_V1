using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class PickUpItemInteractable : Interactable
{
    public ItemPickUpType pickUpType;

    [Header("Item")]
    [SerializeField] Item item;

    [Header("Creature Loot Pick Up")]
    public NetworkVariable<int> itemID = new NetworkVariable<int>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<Vector3> networkPosition = new NetworkVariable<Vector3>(Vector3.zero, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public NetworkVariable<ulong> droppingCreatureID = new NetworkVariable<ulong>(0, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public bool trackDroppingCreaturesPostion = true;

    [Header("World Spawn Pick Up")]
    [SerializeField] int worldSpawnInteractableID;
    [SerializeField] bool hasBeenLooted = false;

    [Header("Drop SFX")]
    [SerializeField] AudioClip itemDropSFX;
    private AudioSource audioSource;

    protected override void Awake()
    {
        base.Awake();

        audioSource = GetComponent<AudioSource>();
    }
    protected override void Start()
    {
        base.Start();

        //switch(ItemPickUpType)
        //{
        //    case ItemPickUpType.WorlSspawn:
        //        break;
        //    case ItemPickUpType.EnemyDrop:
        //        break;
        //    default:
        //        break;

        //}

        if(pickUpType == ItemPickUpType.WorldSpawn)
            CheckIfWorldItemWasAlreadyLooted();


    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        itemID.OnValueChanged += OnItemIDChanged;
        networkPosition.OnValueChanged += OnNetworkPositionChanged;
        droppingCreatureID.OnValueChanged += OnDroppingCreaturesIDChanged;

        if (pickUpType == ItemPickUpType.CharacterDrop)
            audioSource.PlayOneShot(itemDropSFX);

        if(!IsOwner)
        {
            OnItemIDChanged(0, itemID.Value);
            OnNetworkPositionChanged(Vector3.zero, networkPosition.Value);
            OnDroppingCreaturesIDChanged(0, droppingCreatureID.Value);
        }
    }
    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        itemID.OnValueChanged -= OnItemIDChanged;
        networkPosition.OnValueChanged -= OnNetworkPositionChanged;
        droppingCreatureID.OnValueChanged -= OnDroppingCreaturesIDChanged;
    }
    private void CheckIfWorldItemWasAlreadyLooted()
    {
        //0. Hide item iff the player is not the host
        if(!NetworkManager.Singleton.IsHost)
        {
            gameObject.SetActive(false);
            return;
        }

        // 1. Compare the Data of looted items I.D's with this item's ID
        if(!WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.ContainsKey(worldSpawnInteractableID))
        {
            WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.Add(worldSpawnInteractableID, false);
        }

        hasBeenLooted = WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted[worldSpawnInteractableID];
        // 2. If it has been Looted, hide the gameobject
        if(hasBeenLooted )
        {
            gameObject.SetActive(false);
        }
        //else
        //{
        //    // 3. If it has not been loooted, activated the gameobject
        //    gameObject.SetActive(true);
        //}

    }
    public override void Interact(PlayerManager player)
    {
        if (player.isPerformingAction)
            return;

        player.playerAnimatorManager.PlayTargetActionAnimtion("Pick_Up_Item_01", true);

        base.Interact(player);

        // 1. Play SFX
        player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.pickUpItemSFX);

        // 2. Add Item to inventory
        player.playerInventoryManager.AddItemToInventory(item);

        // 3. Display a UI PopUp showing Item'sName and Picture
        PlayerUIManager.instance.playerUIPopUpManager.SendItemPopUp(item, 1);

        // 4. Save loot status, if its a world sqawn
        if(pickUpType == ItemPickUpType.WorldSpawn)
        {
            if(WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.ContainsKey((int)worldSpawnInteractableID))
            {
                WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.Remove(worldSpawnInteractableID);
            }

            WorldSaveGameManager.instance.currentCharacterData.worldItemsLooted.Add(worldSpawnInteractableID, true);
        }

        // 5. Hide or destroy gameobject
        //if(!IsOwner)
        //{
        //    // Non Owner (Host) cannot Destroy network Objects
        //}
        //else
        //{
        //    Destroy(gameObject);
        //}
        DestroyThisNetworkObjectServerRpc();
    }

    protected void OnItemIDChanged(int oldValue, int newValue)
    {
        if (pickUpType != ItemPickUpType.CharacterDrop)
            return;

        item = WorldItemDatabase.Instance.GetItemByID(itemID.Value);
    }

    protected void OnNetworkPositionChanged(Vector3 oldPostion,  Vector3 newPostion)
    {
        if (pickUpType != ItemPickUpType.CharacterDrop)
            return;

        transform.position = networkPosition.Value;
    }

    protected void OnDroppingCreaturesIDChanged(ulong oldID, ulong newID)
    {
        if (pickUpType != ItemPickUpType.CharacterDrop)
            return;

        if (trackDroppingCreaturesPostion)
            StartCoroutine(TrackDroppingCreaturesPostion());
    }

    protected IEnumerator TrackDroppingCreaturesPostion()
    {
        AICharacterManager droppingCreature = NetworkManager.Singleton.SpawnManager.
            SpawnedObjects[droppingCreatureID.Value].gameObject.GetComponent<AICharacterManager>();

        bool trackCreature = false;

        if (droppingCreature != null)
            trackCreature = true;

        if(trackCreature)
        {
            while(gameObject.activeInHierarchy)
            {
                transform.position = droppingCreature.characterCombatManager.lockOnTransform.position;
                yield return null;
            }
        }

        yield return null;
    }

    [ServerRpc(RequireOwnership = false)]
    protected void DestroyThisNetworkObjectServerRpc()
    {
        if(IsServer)
        {
            GetComponent<NetworkObject>().Despawn();
        }
    }
}
