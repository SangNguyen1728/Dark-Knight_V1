using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class WorldObjectManager : MonoBehaviour
{
   
    public static WorldObjectManager instance;

    [Header("Network Objects")]
    [SerializeField] List<NetworkObjectSpawner> networkObjectSpawner;
    [SerializeField] List<GameObject> spawnedInObjects;

    [Header("Fog Walls")]
    public List<FogWallInteractable> fogWalls;

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SqawnObject(NetworkObjectSpawner networkObjectSpawner)
    {
        if (NetworkManager.Singleton.IsServer)
        {
            this.networkObjectSpawner.Add(networkObjectSpawner);
            networkObjectSpawner.AttemptToSpawnCharacter();
        }
    }
    public void AddFogWallTolist(FogWallInteractable fogWall)
    {
        if(!fogWalls.Contains(fogWall))
        {
            fogWalls.Add(fogWall);
        }
    }
    public void RemoveFogWallFromList(FogWallInteractable fogWall)
    {
        if (!fogWalls.Contains(fogWall))
        {
            fogWalls.Remove(fogWall);
        }
    }
}
