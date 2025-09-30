using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class FogWallInteractable : Interactable
{
    [Header("Fog")]
    [SerializeField] GameObject[] fogGameObject;

    [Header("Collision")]
    [SerializeField] Collider fogWallCollider;

    [Header("I.D")]
    public int fogWallID;

    [Header("Active")]
    public NetworkVariable<bool> isActive = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone,NetworkVariableWritePermission.Owner);

    [Header("SoundFX")]
    public AudioSource fogWallAudioSource;
    [SerializeField] AudioClip fogWallSFX;

    //private void OnEnable()
    //{
    //    WorldObjectManager.instance.AddFogWallTolist(this);
    //}
    //private void Awake()
    //{
    //    fogWallAudioSource = gameObject.GetComponent<AudioSource>();
    //}
    public override void Interact(PlayerManager player)
    {
        base.Interact(player);

        //Vector3 wallRight = transform.right;
        //wallRight.y = 0;
        //wallRight.Normalize();
        //Quaternion targetRotation = Quaternion.LookRotation(wallRight);
        //player.transform.rotation = targetRotation;

        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward);
        player.transform.rotation = targetRotation;

        AllowPlayerThroughFogWallCollidersClientRpc(player.NetworkObjectId);
        player.playerAnimatorManager.PlayTargetActionAnimtion("Pass_Through_Fog_01", true);
    }
    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();

        OnIsActiveChange(false, isActive.Value);
        isActive.OnValueChanged += OnIsActiveChange;
        WorldObjectManager.instance.AddFogWallTolist(this);
    }

    public override void OnNetworkDespawn()
    {
        base.OnNetworkDespawn();

        isActive.OnValueChanged -= OnIsActiveChange;
        WorldObjectManager.instance.RemoveFogWallFromList(this);
    }

    private void OnIsActiveChange(bool oldStatus, bool newStatus)
    {
        if (isActive.Value)
        {
            foreach (var fogObject in fogGameObject)
            {
                fogObject.SetActive(true);
            }
        }
        else
        {
            foreach (var fogObject in fogGameObject)
            {
                fogObject.SetActive(false);
            }
        }
    }

    // when server do not require ownership, a non owner can active the funtion 
    [ServerRpc(RequireOwnership = false)]
    private void AllowPlayerThroughFogWallCollidersServerRpc(ulong playerObjectID)
    {
        if (IsServer)
            AllowPlayerThroughFogWallCollidersClientRpc(playerObjectID);
    }

    [ClientRpc]
    private void AllowPlayerThroughFogWallCollidersClientRpc(ulong playerObjectID)
    {
        PlayerManager player = NetworkManager.Singleton.SpawnManager.SpawnedObjects[playerObjectID].GetComponent<PlayerManager>();

        fogWallAudioSource.PlayOneShot(fogWallSFX);

        if (player != null)
            StartCoroutine(DisableFogWallCollider(player));

    }
    private IEnumerator DisableFogWallCollider(PlayerManager player)
    {
        // Make this funtion when walk through FogWall with animation lenght
        Physics.IgnoreCollision(player.characterController, fogWallCollider, true); // Physics.IgnoreCollision(collider A, collider B, true/false) => if you want to ignore collider => true
        yield return new WaitForSeconds(3);
        Physics.IgnoreCollision(player.characterController, fogWallCollider, false); // => if not ignore => false
    }
}
