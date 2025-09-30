using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.Rendering;
using Unity.VisualScripting;
public class TileScreenManager : MonoBehaviour
{
    public static TileScreenManager Instance;

    [Header("Menu")]
    [SerializeField] GameObject titleScreenMainMenu;
    [SerializeField] GameObject tileScreenLoadMenu;

    [Header("Button")]
    [SerializeField] Button loadMenuReturnButton;
    [SerializeField] Button mainMenuLoadGameButton;
    [SerializeField] Button mainMenuNewGameButton;
    [SerializeField] Button delectCharacterPopUpConfirmButton;

    [Header("Pop Ups")]
    [SerializeField] GameObject noCharacterSlotsPopUp;
    [SerializeField] Button noCharacterSlotsOkayButton;
    [SerializeField] GameObject delecteCharacterSlotPopUp;

    [Header("Character Slots")]
    public CharacterSlot currentSelectedSlot = CharacterSlot.NO_SLOT;
    

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void StartNetWorkAsHost()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void StartNewGame()
    {
        WorldSaveGameManager.instance.AttempToCreateNewGame();
    }

    public void OpenLoadGameMenu()
    {
        // close main menu
        titleScreenMainMenu.SetActive(false);

        // open load menu
        tileScreenLoadMenu.SetActive(true);

        // select the return button first
        loadMenuReturnButton.Select();
    }
    public void CloseLoadGameMenu()
    {
        // open main menu
        titleScreenMainMenu.SetActive(true);

        // close load menu
        tileScreenLoadMenu.SetActive(false);

        // select the load button 
        mainMenuLoadGameButton.Select();
    }
    public void DisplayNoFreeCharacterSlotsPopUp()
    {
        noCharacterSlotsPopUp.SetActive(true);
        noCharacterSlotsOkayButton.Select();    
    }
    public void CloseNoFreeCharaterSlotsPopUp()
    {
        noCharacterSlotsPopUp.SetActive(false);
        mainMenuNewGameButton.Select();
    }
    public void SelectCharacterSlot(CharacterSlot characterSlot)
    {
        currentSelectedSlot = characterSlot;
    }
    public void SelectNoSlot()
    {
        currentSelectedSlot = CharacterSlot.NO_SLOT;
    }
    public void AttemptToDelectCharacterSlot()
    {
        if(currentSelectedSlot != CharacterSlot.NO_SLOT)
        {
            delecteCharacterSlotPopUp.SetActive(true);
            delectCharacterPopUpConfirmButton.Select();
        }
    }
    public void DelectCharacterSlot()
    {
        delecteCharacterSlotPopUp.SetActive(false);
        WorldSaveGameManager.instance.DelectGame(currentSelectedSlot);

        // we disable and enable load menu sceen for refeshing slots(a delected slot will no become inactive)
        tileScreenLoadMenu.SetActive(false);
        tileScreenLoadMenu.SetActive(true);

        loadMenuReturnButton.Select();
        
    }
    public void CloseDelectCharacterPopUp()
    {
        delecteCharacterSlotPopUp.SetActive(false);
        loadMenuReturnButton.Select();
    }
}
