using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterFootStepSoundFXMaker : MonoBehaviour
{
    // Using for boss who have atk with foot(to do later)

    CharacterManager character;

    AudioSource audioSource;
    GameObject steppedOnObject;

    private bool hasTouchedGround = false;
    private bool hasPlayedFootStepSFX = false;
    [SerializeField] float distanceToGround = 0.05f;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        character = GetComponentInParent<CharacterManager>();
    }
    private void FixedUpdate()
    {
        CheckForFootSteeps();
    }

    private void CheckForFootSteeps()
    {
        if(character == null)
            return;

        if(!character.characterNetworkManager.isMoving.Value)
            return;

        RaycastHit hit;

        if(Physics.Raycast( transform.position, character.transform.TransformDirection(Vector3.down), out hit, distanceToGround, WorldUtilityManager.Instance.getEnviroLayer()))
        {
            hasTouchedGround = true;

            if(!hasPlayedFootStepSFX)
                steppedOnObject = hit.transform.gameObject;
        }
        else
        {
            hasTouchedGround = false;
            hasPlayedFootStepSFX = false;
            steppedOnObject = null;
        }

        if(hasTouchedGround && !hasPlayedFootStepSFX)
        {
            hasPlayedFootStepSFX = true;
            PlayFootStepSoundFX();
        }
    }
    private void PlayFootStepSoundFX()
    {
        // Play different SFX dependping on the layer of the ground(snow, wood,...)
        //audioSource.PlayOneShot(WorldSoundFXManager.instance.ChooseRandomFootStepSoundBaseOnGround(steppedOnObject, character));

        character.characterSoundFXManager.PlayFootStepSFX();
    }
}
