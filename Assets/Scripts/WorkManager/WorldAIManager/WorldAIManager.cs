using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine.TextCore.Text;

public class WorldAIManager : MonoBehaviour
{
    public static WorldAIManager instance;

    //[Header("Debug")]
    //[SerializeField] bool despawnCharacters = false;
    //[SerializeField] bool respawnCharacters = false;

    [Header("Loading")]
    public bool isPerformingLoadingOperation = false;

    [Header("Characters")]
    [SerializeField] List<AICharacterSpawner> aiCharacterSpawners;
    //[SerializeField] GameObject[] aiCharacters;
    [SerializeField] List<AICharacterManager> spawnedInCharacters;
    private Coroutine spawnAllCharactersCoroutine;
    private Coroutine despawnAllCharactersCoroutine;
    private Coroutine resetAllCharactersCoroutine;

    [Header("Bosses")]
    [SerializeField] List<AIBossCharacterManager> spawnedInBosses;

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
    //private void Start()
    //{
    //    if (NetworkManager.Singleton.IsServer)
    //    {
    //        // Spawn AI in scene
    //        StartCoroutine(WaitForSceneToLoadThenSpawnCharacter());
    //    }
    //}
    //private void Update()
    //{
    //    if(respawnCharacters)
    //    {
    //        respawnCharacters = false;
    //        SqawnAllCharacters();
    //    }

    //    if(despawnCharacters)
    //    {
    //        despawnCharacters = false;
    //        DespawnAllCharacters();
    //    }
    //}
    //private IEnumerator WaitForSceneToLoadThenSpawnCharacter()
    //{
    //    while(!SceneManager.GetActiveScene().isLoaded)
    //    {
    //        yield return null;
    //    }

    //    SqawnAllCharacters();
    //}
    public void SqawnCharacters(AICharacterSpawner aiCharacterSpawner)
    {
        //foreach (var character in aiCharacterSpawners)
        //{
        //    character.AttemptToSpawnCharacter();
        //    GameObject instantiatedCharacter = Instantiate(character);
        //    instantiatedCharacter.GetComponent<NetworkObject>().Spawn();
        //    spawnedInCharacters.Add(instantiatedCharacter);
        //}

        if(NetworkManager.Singleton.IsServer)
        {
            aiCharacterSpawners.Add(aiCharacterSpawner);
            aiCharacterSpawner.AttemptToSpawnCharacter();
        }
    }
    public void AddCharacterToSpawnCharacterList(AICharacterManager character)
    {
        if (spawnedInCharacters.Contains(character))
            return;

        spawnedInCharacters.Add(character);

        AIBossCharacterManager bossCharacter = character as AIBossCharacterManager;

        if(bossCharacter != null )
        {
            if (spawnedInBosses.Contains(bossCharacter))
                return;

            spawnedInBosses.Add(bossCharacter);
        }
    }
    public AIBossCharacterManager GetBossCharacterByID(int ID)
    {
        return spawnedInBosses.FirstOrDefault(boss => boss.bossID == ID);
    }
    public void SpawnAllCharacters()
    {
        isPerformingLoadingOperation = true;

        //DespawnAllCharacters();

       
        if (spawnAllCharactersCoroutine != null)
            StopCoroutine(spawnAllCharactersCoroutine);

        spawnAllCharactersCoroutine = StartCoroutine(SpawnAllCharactersCoroutine());
    }
    private IEnumerator SpawnAllCharactersCoroutine()
    {
        //foreach(var spawner in aiCharacterSpawners)
        //{
        //   spawner.AttemptToSpawnCharacter();
        //}
        for(int i = 0; i < aiCharacterSpawners.Count; i++)
        {
            yield return new WaitForFixedUpdate();

            aiCharacterSpawners[i].AttemptToSpawnCharacter();

            yield return null;
        }

        isPerformingLoadingOperation = false;

        yield return null;
    }

    public void ResetAllCharacters()
    {
        isPerformingLoadingOperation = true;

        //DespawnAllCharacters();


        if (resetAllCharactersCoroutine != null)
            StopCoroutine(resetAllCharactersCoroutine);

        resetAllCharactersCoroutine = StartCoroutine(ResetAllCharactersCoroutine());
    }
    private IEnumerator ResetAllCharactersCoroutine()
    {
        for (int i = 0; i < aiCharacterSpawners.Count; i++)
        {
            yield return new WaitForFixedUpdate();

            aiCharacterSpawners[i].ResetCharacter();

            yield return null;
        }

        isPerformingLoadingOperation = false;

        yield return null;
    }
    private void DespawnAllCharacters()
    {
        isPerformingLoadingOperation = true;

        //DespawnAllCharacters();


        if (despawnAllCharactersCoroutine != null)
            StopCoroutine(despawnAllCharactersCoroutine);

        despawnAllCharactersCoroutine = StartCoroutine(DespawnAllCharactersCoroutine());

        
    }
    private IEnumerator DespawnAllCharactersCoroutine()
    {
        for (int i = 0; i < spawnedInCharacters.Count; i++)
        {
            yield return new WaitForFixedUpdate();

            spawnedInCharacters[i].GetComponent<NetworkObject>().Despawn();

            yield return null;
        }

        spawnedInCharacters.Clear();
        isPerformingLoadingOperation = false;

        yield return null;
    }
    private void DisableAllCharacters()
    {

    }
}
