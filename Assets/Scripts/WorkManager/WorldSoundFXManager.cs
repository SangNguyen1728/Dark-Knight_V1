using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSoundFXManager : MonoBehaviour
{
    public static WorldSoundFXManager instance;

    [Header("Boss Track")]
    [SerializeField] AudioSource bossIntroPlayer;
    [SerializeField] AudioSource bossLoopPlayer;

    [Header("Action Sound")]
    public AudioClip pickUpItemSFX;
    public AudioClip rollSFX;
    public AudioClip stanceBreakSFX;
    public AudioClip criticalStrikeSFX;
    public AudioClip healingSFX;


    [Header("Action Damage")]
    public AudioClip[] physicalDamageSFX;


    private void Awake()
    {
        if(instance == null)
        {
            instance =  this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
  
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    public void PlayBossTrack(AudioClip introTrack, AudioClip LoopTrack)
    {
        bossIntroPlayer.volume = 1;
        bossIntroPlayer.clip = introTrack;
        bossIntroPlayer.loop = false;
        bossIntroPlayer.Play();

        bossLoopPlayer.volume = 1;
        bossLoopPlayer.clip = LoopTrack;
        bossLoopPlayer.loop = true;
        bossLoopPlayer.PlayDelayed(bossIntroPlayer.clip.length);
    }
    public AudioClip ChooseRandomSFXFromArray(AudioClip[] array)
    {
        int index = Random.Range(0, array.Length);

        return array[index];
    }
    /*
    public AudioClip ChooseRandomFootStepSoundBaseOnGround(GameObject steppedOnObject, CharacterManager character)
    {
        if(steppedOnObject.tag == "Dirt")
        {
            return ChooseRandomSFXFromArray(character.characterSoundFXManager.footStepDirt);
        }
        else if (steppedOnObject.tag == "Stone")
        {
            return ChooseRandomSFXFromArray(character.characterSoundFXManager.footStepStone);
        }

        return null;
    }
    */
    public void StopBossMusic()
    {
        StartCoroutine(FadeOutBossMusicThenStop());
    }
    private IEnumerator FadeOutBossMusicThenStop()
    {
        while(bossLoopPlayer.volume > 0)
        {
            bossLoopPlayer.volume -= Time.deltaTime;
            bossIntroPlayer.volume -= Time.deltaTime;

            yield return null;
        }

        bossIntroPlayer.Stop();
        bossLoopPlayer.Stop();
    }
    
}
