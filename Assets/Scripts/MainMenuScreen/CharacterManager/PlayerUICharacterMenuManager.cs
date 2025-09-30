using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUICharacterManager : MonoBehaviour
{
    [Header("Menu")]
    [SerializeField] GameObject menu;

    public void OpenCharacterMenu()
    {
        PlayerUIManager.instance.menuWindowIsOpen = true;
        menu.SetActive(true);
    }
    public void CLoseCharacterMenu()
    {
        PlayerUIManager.instance.menuWindowIsOpen = false;
        menu.SetActive(false);
    }
    public void CloseCharacterMenuAfterFixedFrame()
    {
        StartCoroutine(WaitThenCLoseMenu());
    }
    private IEnumerator WaitThenCLoseMenu()
    {
        yield return new WaitForFixedUpdate();

        PlayerUIManager.instance.menuWindowIsOpen = false;
        menu.SetActive(false);
    }
}
