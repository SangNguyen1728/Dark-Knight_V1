using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIToggleHUD : MonoBehaviour
{
    private void OnEnable()
    {
        // Hide the HUD
        PlayerUIManager.instance.playerHudManager.ToggleHUD(false);
    }

    private void OnDisable()
    {
        // Bring the HUD back
        PlayerUIManager.instance.playerHudManager.ToggleHUD(true);
    }
}
