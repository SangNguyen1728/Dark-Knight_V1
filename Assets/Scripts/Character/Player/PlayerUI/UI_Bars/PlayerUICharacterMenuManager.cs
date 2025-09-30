using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUICharacterMenuManager : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] GameObject menu;

    public void OpenCharacterMenu()
    {
        PlayerUIManager.instance.menuWindowIsOpen = true;
        menu.SetActive(true);
    }
    public void CloseCharaterMenu() 
    {

        PlayerUIManager.instance.menuWindowIsOpen = false;
        menu.SetActive(false);
    }

    public void CloseCharacterMenuAfterFixedFrame()
    {
        StartCoroutine(WaitThenCloseMenu());
    }

    private IEnumerator WaitThenCloseMenu()
    {
        yield return new WaitForFixedUpdate();

        PlayerUIManager.instance.menuWindowIsOpen = false;
        menu.SetActive(false);
    }
}
