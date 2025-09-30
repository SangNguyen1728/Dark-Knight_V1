using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleScreenLoadMenuInputManager : MonoBehaviour
{ 
    PlayerControls playerControls;

    [Header("Title Screen Inputs")]
    [SerializeField] bool delectCharacterSlot = false;

    private void Update()
    {
        if(delectCharacterSlot)
        {
            delectCharacterSlot = false;
            TileScreenManager.Instance.AttemptToDelectCharacterSlot();
        }
    }
    private void OnEnable()
    {
        if(playerControls == null)
        {
            playerControls = new PlayerControls();
            playerControls.UI.XRightMouse.performed += i => delectCharacterSlot = true;
         
        }

        playerControls.Enable();
    }
    private void OnDisable()
    {
        playerControls.Disable();
    }
}
