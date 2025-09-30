using UnityEngine;
using Unity.Netcode;
public class PlayerUIManager : MonoBehaviour
{
    public static PlayerUIManager instance;

    [Header("NETWORK JOINT")]
    [SerializeField] bool startGameAsClient;

    [HideInInspector] public PlayerHudManager playerHudManager;
    [HideInInspector] public PlayerUIPopUpManager playerUIPopUpManager;
    [HideInInspector] public PlayerUICharacterMenuManager playerUICharacterMenuManager;
    [HideInInspector] public PlayerUIEquipmentManager playerUIEquipmentManager;
    [HideInInspector] public PlayerLoadingScreenManager playerLoadingScreenManager;

    [Header("UI Flags")]
    public bool menuWindowIsOpen = false; // inventory sceen, equiment menu, ...
    public bool popUpWindowIsOpen = false; // item pick up, (dialogue pop up)

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

        playerHudManager = GetComponentInChildren<PlayerHudManager>();
        playerUIPopUpManager = GetComponentInChildren<PlayerUIPopUpManager>();
        playerUICharacterMenuManager = GetComponentInChildren<PlayerUICharacterMenuManager>();
        playerUIEquipmentManager = GetComponentInChildren<PlayerUIEquipmentManager>();
        playerLoadingScreenManager = GetComponentInChildren<PlayerLoadingScreenManager>();
    }
    private void Start()
    {
        DontDestroyOnLoad(gameObject);
    }
    private void Update()
    {
        if(startGameAsClient)
        {
            startGameAsClient = false;
            // we must first shut down, because we have started as a host during the tile screen    
            NetworkManager.Singleton.Shutdown();
            // we must first shut down, because we have started as a host during the tile screen    
            NetworkManager.Singleton.StartClient();
        }
    }

    
    public void CloseAllMenuWindow()
    {
        playerUICharacterMenuManager.CloseCharaterMenu();
        playerUIEquipmentManager.CloseEquipmentManagerMenu();
    }
    
}
