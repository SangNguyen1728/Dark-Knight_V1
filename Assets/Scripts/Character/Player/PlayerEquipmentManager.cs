using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class PlayerEquipmentManager : CharacterEquipmentManager
{
    PlayerManager player;

    [Header("Weapon Model Instantiation Slots")]
    [HideInInspector] public WeaponModelInstantiationSlot rightHandWeaponSlot;
    [HideInInspector] public WeaponModelInstantiationSlot leftHandWeaponSlot;
    [HideInInspector] public WeaponModelInstantiationSlot leftHandShieldSlot;
    [HideInInspector] public WeaponModelInstantiationSlot backSlot;

    [Header("Weapon Managers")]
    public WeaponManager rightWeaponManager;
    public WeaponManager leftWeaponManager;

    [Header("Weapon Models")]
    [HideInInspector] public GameObject rightHandWeaponModel;
    [HideInInspector] public GameObject leftHandWeaponModel;

    //[Header("Debug, Delete later")]
    //[SerializeField] bool equipmentNewItem = false;

    [Header("General Equipment Models")]
    public GameObject hatsObject;
    [HideInInspector] public GameObject[] hats;
    public GameObject hoodsObject;
    [HideInInspector] public GameObject[] hoods;
    public GameObject faceCoversObject;
    [HideInInspector] public GameObject[] faceCovers;
    public GameObject helmetAccessoriesObject;
    [HideInInspector] public GameObject[] helmetAccessories;
    public GameObject backAccessoriesObject;
    [HideInInspector] public GameObject[] backAccessories;
    public GameObject hipAccessoriesObject;
    [HideInInspector] public GameObject[] hipAccessories;
    public GameObject rightShoulderObject;
    [HideInInspector] public GameObject[] rightShoulder;
    public GameObject rightElbowObject;
    [HideInInspector] public GameObject[] rightElbow;
    public GameObject rightKneeObject;
    [HideInInspector] public GameObject[] rightKnee;
    public GameObject leftShoulderObject;
    [HideInInspector] public GameObject[] leftShoulder;
    public GameObject leftElbowObject;
    [HideInInspector] public GameObject[] leftElbow;
    public GameObject leftKneeObject;
    [HideInInspector] public GameObject[] leftKnee;

    [Header("Male Equipment Models")]
    public GameObject maleFullHelmetObject;
    [HideInInspector] public GameObject[] maleHeadFullHelmets;
    public GameObject maleFullBodyObject;
    [HideInInspector] public GameObject[] maleBodies;
    public GameObject maleRightUpperArmObject;
    [HideInInspector] public GameObject[] maleRightUpperArms;
    public GameObject maleLRightLowerArmObject;
    [HideInInspector] public GameObject[] maleRightLowerArms;
    public GameObject maleRightHandObject;
    [HideInInspector] public GameObject[] maleRightHands;
    public GameObject maleLeftUpperArmObject;
    [HideInInspector] public GameObject[] maleLeftUpperArms;
    public GameObject maleLeftLowerArmObject;
    [HideInInspector] public GameObject[] maleLeftLowerArms;
    public GameObject maleLeftHandObject;
    [HideInInspector] public GameObject[] maleLeftHands;
    public GameObject maleHipsObject;
    [HideInInspector] public GameObject[] maleHips;
    public GameObject maleRightLegObject;
    [HideInInspector] public GameObject[] maleRightLeg;
    public GameObject maleLeftLegObject;
    [HideInInspector] public GameObject[] maleLeftLeg;

    [Header("Female Equipment Models")]
    public GameObject femaleFullHelmetObject;
    [HideInInspector] public GameObject[] femaleHeadFullHelmets;
    public GameObject femaleFullBodyObject;
    [HideInInspector] public GameObject[] femaleBodies;
    public GameObject femaleRightUpperArmObject;
    [HideInInspector] public GameObject[] femaleRightUpperArms;
    public GameObject femaleLRightLowerArmObject;
    [HideInInspector] public GameObject[] femaleRightLowerArms;
    public GameObject femaleRightHandObject;
    [HideInInspector] public GameObject[] femaleRightHands;
    public GameObject femaleLeftUpperArmObject;
    [HideInInspector] public GameObject[] femaleLeftUpperArms;
    public GameObject femaleLeftLowerArmObject;
    [HideInInspector] public GameObject[] femaleLeftLowerArms;
    public GameObject femaleLeftHandObject;
    [HideInInspector] public GameObject[] femaleLeftHands;
    public GameObject femaleHipsObject;
    [HideInInspector] public GameObject[] femaleHips;
    public GameObject femaleRightLegObject;
    [HideInInspector] public GameObject[] femaleRightLeg;
    public GameObject femaleLeftLegObject;
    [HideInInspector] public GameObject[] femaleLeftLeg;

    protected override void Awake()
    {
        base.Awake();

        player = GetComponent<PlayerManager>();

        InitializeWeaponSlots();
        InitializeArmorModels();


    }

    protected override void Start()
    {
        base.Start();

        LoadWeaponOnBotHands();
    }
    //private void Update()
    //{
    //    if(equipmentNewItem)
    //    {
    //        equipmentNewItem = false;
    //        EquipArmor();
    //    }
    //}
    public void EquipArmor()
    {
        LoadHeadEquipment(player.playerInventoryManager.headEquipment);
        LoadBodyEquipment(player.playerInventoryManager.bodyEquipment);
        LoadHandEquipment(player.playerInventoryManager.handEquipment);
        LoadLegEquipment(player.playerInventoryManager.legEquipment);
    }
    // Quick Slot
    public void SwitchQuickSlotItem()
    {
        if (!player.IsOwner)
            return;

        QuickSlotItem selectedItem = null;
        
        // add 1 to our index to switch to the next potential weapon
        player.playerInventoryManager.quickSlotItemIndex += 1;

        // if my index is out of bonds, reset  it to position 1
        if (player.playerInventoryManager.quickSlotItemIndex < 0 || player.playerInventoryManager.quickSlotItemIndex > 4)
        {
            player.playerInventoryManager.quickSlotItemIndex = 0;

            // check if i am hoding more than 1 weapon
            float itemCount = 0;
            QuickSlotItem firstItem = null;
            int firstItemPosition = 0;

            for (int i = 0; i < player.playerInventoryManager.quickSlotItemInQuickSlots.Length; i++)
            {
                if (player.playerInventoryManager.quickSlotItemInQuickSlots[i] != null)
                {
                    itemCount += 1;

                    if (firstItem == null)
                    {
                        firstItem = player.playerInventoryManager.quickSlotItemInQuickSlots[i];
                        firstItemPosition = i;
                    }
                }
            }

            if (itemCount <= 1)
            {
                player.playerInventoryManager.quickSlotItemIndex = -1;
                selectedItem = null;
                player.playerNetworkManager.currentQuickSlotItemID.Value = -1;
            }
            else
            {
                player.playerInventoryManager.quickSlotItemIndex = firstItemPosition;
                player.playerNetworkManager.currentQuickSlotItemID.Value = firstItem.itemID;
            }

            return;
        }

        //foreach (QuickSlotItem quickSlotItem in player.playerInventoryManager.quickSlotItemInQuickSlots)
        //{
        //    // if the next potential weapon does not equal the unarmed weapon
        //    if (player.playerInventoryManager.quickSlotItemInQuickSlots[player.playerInventoryManager.quickSlotItemIndex] != null) 
        //    {
        //        selectedItem = player.playerInventoryManager.quickSlotItemInQuickSlots[player.playerInventoryManager.quickSlotItemIndex];

        //        player.playerNetworkManager.currentQuickSlotItemID.Value =
        //            player.playerInventoryManager.quickSlotItemInQuickSlots[player.playerInventoryManager.quickSlotItemIndex].itemID;

        //        return;
        //    }
        //}
        if (player.playerInventoryManager.quickSlotItemInQuickSlots[player.playerInventoryManager.quickSlotItemIndex] != null)
        {
            selectedItem = player.playerInventoryManager.quickSlotItemInQuickSlots[player.playerInventoryManager.quickSlotItemIndex];

            player.playerNetworkManager.currentQuickSlotItemID.Value =
                player.playerInventoryManager.quickSlotItemInQuickSlots[player.playerInventoryManager.quickSlotItemIndex].itemID;

        }
        else
        {
            player.playerNetworkManager.currentQuickSlotItemID.Value = -1;
        }

        if (selectedItem == null && player.playerInventoryManager.quickSlotItemIndex <= 4)
        {
            SwitchQuickSlotItem();
        }
    }

    // Equipment

    private void InitializeArmorModels()
    {
        // UNISEX EQUIPMENT
        // Hat 
        List<GameObject> hatList = new List<GameObject>();

        foreach (Transform child in hatsObject.transform)
        {
            hatList.Add(child.gameObject);
        }
        hats = hatList.ToArray();

        // Hood
        List<GameObject> hoodList = new List<GameObject>();

        foreach (Transform child in hoodsObject.transform)
        {
            hoodList.Add(child.gameObject);
        }
        hoods = hoodList.ToArray();

        // FaceCover
        List<GameObject> faceCoverList = new List<GameObject>();

        foreach (Transform child in faceCoversObject.transform)
        {
            faceCoverList.Add(child.gameObject);
        }
        faceCovers = faceCoverList.ToArray();

        // Helmet Accessories
        List<GameObject> helmetAccessoriesList = new List<GameObject>();

        foreach (Transform child in helmetAccessoriesObject.transform)
        {
            helmetAccessoriesList.Add(child.gameObject);
        }
        helmetAccessories = helmetAccessoriesList.ToArray();

        // Back Accessories
        List<GameObject> backAccessoriesList = new List<GameObject>();

        foreach (Transform child in backAccessoriesObject.transform)
        {
            backAccessoriesList.Add(child.gameObject);
        }
        backAccessories = backAccessoriesList.ToArray();

        // Hip Accessories
        List<GameObject> hipAccessoriesList = new List<GameObject>();

        foreach (Transform child in hipAccessoriesObject.transform)
        {
            hipAccessoriesList.Add(child.gameObject);
        }
        hipAccessories = hipAccessoriesList.ToArray();

        // Right Shoulder
        List<GameObject> rightShoulderList = new List<GameObject>();

        foreach (Transform child in rightShoulderObject.transform)
        {
            rightShoulderList.Add(child.gameObject);
        }
        rightShoulder = rightShoulderList.ToArray();

        // Right Elbow
        List<GameObject> rightElbowList = new List<GameObject>();

        foreach (Transform child in rightElbowObject.transform)
        {
            rightElbowList.Add(child.gameObject);
        }
        rightElbow = rightElbowList.ToArray();

        // Right Knee
        List<GameObject> rightKneeList = new List<GameObject>();

        foreach (Transform child in rightKneeObject.transform)
        {
            rightKneeList.Add(child.gameObject);
        }
        rightKnee = rightKneeList.ToArray();

        // Left Shoulder
        List<GameObject> leftShoulderList = new List<GameObject>();

        foreach (Transform child in leftShoulderObject.transform)
        {
            leftShoulderList.Add(child.gameObject);
        }
        leftShoulder = leftShoulderList.ToArray();

        // Left Elbow
        List<GameObject> leftElbowList = new List<GameObject>();

        foreach (Transform child in leftElbowObject.transform)
        {
            leftElbowList.Add(child.gameObject);
        }
        leftElbow = leftElbowList.ToArray();

        // Left Knee
        List<GameObject> leftKneeList = new List<GameObject>();

        foreach (Transform child in leftKneeObject.transform)
        {
            leftKneeList.Add(child.gameObject);
        }
        leftKnee = leftKneeList.ToArray();



        // MALE EQUIPMENT
        // Male Full Helmet
        List<GameObject> maleFullHelmetsList = new List<GameObject>();

        foreach (Transform child in maleFullHelmetObject.transform)
        {
            maleFullHelmetsList.Add(child.gameObject);
        }
        maleHeadFullHelmets = maleFullHelmetsList.ToArray();

        // Male Body
        List<GameObject> maleBodiesList = new List<GameObject>();

        foreach (Transform child in maleFullBodyObject.transform)
        {
            maleBodiesList.Add(child.gameObject);
        }
        maleBodies = maleBodiesList.ToArray();

        // Male Right Upper Arms
        List<GameObject> maleRightUpperArmsList = new List<GameObject>();

        foreach (Transform child in maleRightUpperArmObject.transform)
        {
            maleRightUpperArmsList.Add(child.gameObject);
        }
        maleRightUpperArms = maleRightUpperArmsList.ToArray();

        // Male Right Lower Arms
        List<GameObject> maleRightLowerArmsList = new List<GameObject>();

        foreach (Transform child in maleLRightLowerArmObject.transform)
        {
            maleRightLowerArmsList.Add(child.gameObject);
        }
        maleRightLowerArms = maleRightLowerArmsList.ToArray();

        // Male Right Hand
        List<GameObject> maleRighthandList = new List<GameObject>();

        foreach (Transform child in maleRightHandObject.transform)
        {
            maleRighthandList.Add(child.gameObject);
        }
        maleRightHands = maleRighthandList.ToArray();

        // Male Left Upper Arms
        List<GameObject> maleLeftUpperArmsList = new List<GameObject>();

        foreach (Transform child in maleLeftUpperArmObject.transform)
        {
            maleLeftUpperArmsList.Add(child.gameObject);
        }
        maleLeftUpperArms = maleLeftUpperArmsList.ToArray();

        // Male Left Lower Arms
        List<GameObject> maleLeftLowerArmsList = new List<GameObject>();

        foreach (Transform child in maleLRightLowerArmObject.transform)
        {
            maleLeftLowerArmsList.Add(child.gameObject);
        }
        maleLeftLowerArms = maleLeftLowerArmsList.ToArray();

        // Male Left Hand
        List<GameObject> maleLefthandList = new List<GameObject>();

        foreach (Transform child in maleLeftHandObject.transform)
        {
            maleLefthandList.Add(child.gameObject);
        }
        maleLeftHands = maleLefthandList.ToArray();

        // Male Hips
        List<GameObject> maleHipsList = new List<GameObject>();

        foreach (Transform child in maleHipsObject.transform)
        {
            maleHipsList.Add(child.gameObject);
        }
        maleHips = maleHipsList.ToArray();

        // Male Right Leg
        List<GameObject> maleRightLegList = new List<GameObject>();

        foreach (Transform child in maleRightLegObject.transform)
        {

            maleRightLegList.Add(child.gameObject);
        }
        maleRightLeg = maleRightLegList.ToArray();

        // Male Left Leg
        List<GameObject> maleLeftLegList = new List<GameObject>();

        foreach (Transform child in maleLeftLegObject.transform)
        {
            maleLeftLegList.Add(child.gameObject);
        }
        maleLeftLeg = maleLeftLegList.ToArray();

        // FEMALE EQUIPMENT
        // Female Full Helmet
        List<GameObject> femaleFullHelmetsList = new List<GameObject>();

        foreach (Transform child in femaleFullHelmetObject.transform)
        {
            femaleFullHelmetsList.Add(child.gameObject);
        }
        femaleHeadFullHelmets = femaleFullHelmetsList.ToArray();

        // Female Body
        List<GameObject> femaleBodiesList = new List<GameObject>();

        foreach (Transform child in femaleFullBodyObject.transform)
        {
            femaleBodiesList.Add(child.gameObject);
        }
        femaleBodies = femaleBodiesList.ToArray();

        // Female Right Upper Arms
        List<GameObject> femaleRightUpperArmsList = new List<GameObject>();

        foreach (Transform child in femaleRightUpperArmObject.transform)
        {
            femaleRightUpperArmsList.Add(child.gameObject);
        }
        femaleRightUpperArms = femaleRightUpperArmsList.ToArray();

        // Female Right Lower Arms
        List<GameObject> femaleRightLowerArmsList = new List<GameObject>();

        foreach (Transform child in femaleLRightLowerArmObject.transform)
        {
            femaleRightLowerArmsList.Add(child.gameObject);
        }
        femaleRightLowerArms = femaleRightLowerArmsList.ToArray();

        // Female Right Hand
        List<GameObject> femaleRighthandList = new List<GameObject>();

        foreach (Transform child in femaleRightHandObject.transform)
        {
            femaleRighthandList.Add(child.gameObject);
        }
        femaleRightHands = femaleRighthandList.ToArray();

        // Female Left Upper Arms
        List<GameObject> femaleLeftUpperArmsList = new List<GameObject>();

        foreach (Transform child in femaleLeftUpperArmObject.transform)
        {
            femaleLeftUpperArmsList.Add(child.gameObject);
        }
        femaleLeftUpperArms = femaleLeftUpperArmsList.ToArray();

        // Female Left Lower Arms
        List<GameObject> femaleLeftLowerArmsList = new List<GameObject>();

        foreach (Transform child in femaleLeftLowerArmObject.transform)
        {
            femaleLeftLowerArmsList.Add(child.gameObject);
        }
        femaleLeftLowerArms = femaleLeftLowerArmsList.ToArray();

        // Female Left Hand
        List<GameObject> femaleLefthandList = new List<GameObject>();

        foreach (Transform child in femaleLeftHandObject.transform)
        {
            femaleLefthandList.Add(child.gameObject);
        }
        femaleLeftHands = femaleLefthandList.ToArray();

        // Female Hips
        List<GameObject> femaleHipsList = new List<GameObject>();

        foreach (Transform child in femaleHipsObject.transform)
        {
            femaleHipsList.Add(child.gameObject);
        }
        femaleHips = femaleHipsList.ToArray();

        // Female Right Leg
        List<GameObject> femaleRightLegList = new List<GameObject>();

        foreach (Transform child in femaleRightLegObject.transform)
        {
            femaleRightLegList.Add(child.gameObject);
        }
        femaleRightLeg = femaleRightLegList.ToArray();

        // Female Left Leg
        List<GameObject> femaleLeftLegList = new List<GameObject>();

        foreach (Transform child in femaleLeftLegObject.transform)
        {
            femaleLeftLegList.Add(child.gameObject);
        }
        femaleLeftLeg = femaleLeftLegList.ToArray();
    }
    public void LoadHeadEquipment(HeadEquipmentItem equipment)
    {
        Debug.Log("Loading head equipment...");
        // ToDo:
        // Unload old HeadEquipment models(any)
        UnloadHeadEquipmentModels();
        // if null, set equipment in inventory to null and return
        if(equipment == null)
        {
            if (player.IsOwner)
            {
                player.playerNetworkManager.headEquipmentID.Value = -1; // -1 for an null. always never item ID
            }

            player.playerInventoryManager.headEquipment = null;
            return;
        }
        // if have an On Item Equipment call on your equipment, run it now
        // set current head equipment in player inventory to equipment that is passed this function
       player.playerInventoryManager.headEquipment =  equipment;
        // Check head equipment type to disable certain body features
        switch(equipment.headEquipmentType)
        {
            case HeadEquipmentType.FullHelmet:
                player.playerBodyManager.DisableHair();
                player.playerBodyManager.DisableHead();
                break;
            case HeadEquipmentType.Hat:
                break;
            case HeadEquipmentType.Hood:
                player.playerBodyManager.DisableHair();
                break;
            case HeadEquipmentType.FaceCover:
                player.playerBodyManager.DisableFacialHair();
                break;
            default:
                break;
        }

        // load head equipment models
       
        foreach (var model in equipment.equipmentModels)
        { 
            model.LoadModel(player, player.playerNetworkManager.isMale.Value);
        }
        // calculate total equipment load (total weight, total worn equipment)
        // calculate total armor aborsption
        player.playerStatsManager.CalculateTotalArmorAbsorption();

        if (player.IsOwner)
            player.playerNetworkManager.headEquipmentID.Value = equipment.itemID;
    }
    private void UnloadHeadEquipmentModels()
    {
        foreach(var model in maleHeadFullHelmets)
        {
            model.SetActive(false);
        }

        foreach (var model in femaleHeadFullHelmets)
        {
            model.SetActive(false);
        }

        foreach (var model in hats)
        {
            model.SetActive(false);
        }

        foreach (var model in faceCovers)
        {
            model.SetActive(false);
        }

        foreach (var model in hoods)
        {
            model.SetActive(false);
        }

        foreach (var model in helmetAccessories)
        {
            model.SetActive(false);
        }

        player.playerBodyManager.EnableHead();
        player.playerBodyManager.EnableHair();
    }
    public void LoadBodyEquipment(BodyEquipmentItem equipment)
    {
        // ToDo:
        // Unload old HeadEquipment models(any)
        UnloadBodyEquipment();
        if (equipment == null)
        {
            if (player.IsOwner)
                player.playerNetworkManager.bodyEquipmentID.Value = -1; // -1 for an null. always never item ID

            player.playerInventoryManager.bodyEquipment = null;
            return;
        }
        // if have an On Item Equipment call on your equipment, run it now
        // set current head equipment in player inventory to equipment that is passed this function
        player.playerInventoryManager.bodyEquipment = equipment;

        // Check head equipment type to disable certain body features
        player.playerBodyManager.DisableBody();

        // load head equipment models
        foreach (var model in equipment.equipmentModels)
        {
            model.LoadModel(player, player.playerNetworkManager.isMale.Value);
        }

        // calculate total equipment load (total weight, total worn equipment)
        // calculate total armor aborsption
        player.playerStatsManager.CalculateTotalArmorAbsorption();

        if (player.IsOwner)
            player.playerNetworkManager.bodyEquipmentID.Value = equipment.itemID;
    }

    private void UnloadBodyEquipment()
    {
        foreach(var model in rightShoulder)
        {
            model.SetActive(false);
        }

        foreach (var model in rightElbow)
        {
            model.SetActive(false);
        }

        foreach (var model in leftShoulder)
        {
            model.SetActive(false);
        }

        foreach (var model in leftElbow)
        {
            model.SetActive(false);
        }

        foreach (var model in backAccessories)
        {
            model.SetActive(false);
        }

        // Male
        foreach (var model in maleBodies)
        {
            model.SetActive(false);
        }

        foreach (var model in maleRightUpperArms)
        {
            model.SetActive(false);
        }

        foreach (var model in maleLeftUpperArms)
        {
            model.SetActive(false);
        }

        // Female
        foreach (var model in femaleBodies)
        {
            model.SetActive(false);
        }

        foreach (var model in femaleRightUpperArms)
        {
            model.SetActive(false);
        }

        foreach (var model in femaleLeftUpperArms)
        {
            model.SetActive(false);
        }

        player.playerBodyManager.EnableBody();
    }
    public void LoadHandEquipment(HandEquipmentItem equipment)
    {
        UnloadHandEquipment();
        if (equipment == null)
        {
            if (player.IsOwner)
                player.playerNetworkManager.handEquipmentID.Value = -1; // -1 for an null. always never item ID

            player.playerInventoryManager.handEquipment = null;
            return;
        }
        // if have an On Item Equipment call on your equipment, run it now
        // set current head equipment in player inventory to equipment that is passed this function
        player.playerInventoryManager.handEquipment = equipment;

        // Check head equipment type to disable certain body features
        player.playerBodyManager.DisableArms();

        // load head equipment models
        foreach (var model in equipment.equipmentModels)
        {
            model.LoadModel(player, player.playerNetworkManager.isMale.Value);
        }

        // calculate total equipment load (total weight, total worn equipment)
        // calculate total armor aborsption
        player.playerStatsManager.CalculateTotalArmorAbsorption();

        if (player.IsOwner)
            player.playerNetworkManager.handEquipmentID.Value = equipment.itemID;
    }
    private void UnloadHandEquipment()
    {
        foreach (var model in maleLeftLowerArms)
        {
            model.SetActive(false);
        }
        foreach (var model in maleRightLowerArms)
        {
            model.SetActive(false);
        }
        foreach (var model in maleLeftHands)
        {
            model.SetActive(false);
        }
        foreach (var model in maleRightHands)
        {
            model.SetActive(false);
        }

        foreach (var model in femaleLeftLowerArms)
        {
            model.SetActive(false);
        }
        foreach (var model in femaleRightLowerArms)
        {
            model.SetActive(false);
        }
        foreach (var model in femaleLeftHands)
        {
            model.SetActive(false);
        }
        foreach (var model in femaleRightHands)
        {
            model.SetActive(false);
        }

        player.playerBodyManager.EnableArms();
    }
    public void LoadLegEquipment(LegEquipmentItem equipment)
    {
     
        UnloadLegEquipment();
        if (equipment == null)
        {
            if (player.IsOwner)
                player.playerNetworkManager.legEquipmentID.Value = -1; // -1 for an null. always never item ID

            player.playerInventoryManager.legEquipment = null;
            return;
        }
        // if have an On Item Equipment call on your equipment, run it now
        // set current head equipment in player inventory to equipment that is passed this function
        player.playerInventoryManager.legEquipment = equipment;

        // Check head equipment type to disable certain body features
        player.playerBodyManager.DisableLowerBody();


        // load head equipment models
        foreach (var model in equipment.equipmentModels)
        {
            Debug.Log("load leg item" + model.name);
            model.LoadModel(player, player.playerNetworkManager.isMale.Value);
        }

        // calculate total equipment load (total weight, total worn equipment)
        // calculate total armor aborsption
        player.playerStatsManager.CalculateTotalArmorAbsorption();

        if (player.IsOwner)
            player.playerNetworkManager.legEquipmentID.Value = equipment.itemID;
    }
    private void UnloadLegEquipment()
    {
        foreach (var model in maleHips)
        {
            model.SetActive(false);
        }
        foreach (var model in femaleHips)
        {
            model.SetActive(false);
        }

        foreach (var model in leftKnee)
        {
            model.SetActive(false);
        }
        foreach (var model in rightKnee)
        {
            model.SetActive(false);
        }

        foreach (var model in maleLeftLeg)
        {
            model.SetActive(false);
        }
        foreach (var model in maleRightLeg)
        {
            model.SetActive(false);
        }

        foreach (var model in femaleLeftLeg)
        {
            model.SetActive(false);
        }
        foreach (var model in femaleRightLeg)
        {
            model.SetActive(false);
        }

        player.playerBodyManager.EnableLowerBody();
    }

    private void InitializeWeaponSlots()
    {
        WeaponModelInstantiationSlot[] weaponSlots = GetComponentsInChildren<WeaponModelInstantiationSlot>();

        foreach(var weaponSlot in weaponSlots)
        {
            if(weaponSlot.weaponSlot == WeaponModelSlot.RightHand)
            {
                rightHandWeaponSlot = weaponSlot;
            }
            else if(weaponSlot.weaponSlot == WeaponModelSlot.LeftHandWeaponSlot)
            {
                leftHandWeaponSlot = weaponSlot;
            }
            else if (weaponSlot.weaponSlot == WeaponModelSlot.LeftHandShieldSlot)
            {
                leftHandShieldSlot = weaponSlot;
            }
            else if (weaponSlot.weaponSlot == WeaponModelSlot.BackSlot)
            {
                backSlot = weaponSlot;
            }
        }
    }
    public void LoadWeaponOnBotHands()
    {
        LoadRightHandWeapon();
        LoadLeftHandWeapon();
    }
    // right hand/ right weapon
    public void SwitchRightWeapon()
    {
        if (!player.IsOwner)
            return;
        player.playerAnimatorManager.PlayTargetActionAnimtion("Swap_Right_Weapon_01", false,false, true, true );

        // DO IN FUTURE
        // check if we have another weapon besiders my main weapon, rotate between weapon 1 and 2
        // if not, that make unreal, then SKIP the other empty slot and swap back. Not process both emty slots before returning to main weapon

        WeaponItem selectedWeapon = null;
        // disable two hand if i am 2 handing

        // add 1 to our index to switch to the next potential weapon
        player.playerInventoryManager.rightHandWeaponIndex += 1;

        // if my index is out of bonds, reset  it to position 1
        if(player.playerInventoryManager.rightHandWeaponIndex < 0 || player.playerInventoryManager.rightHandWeaponIndex >11)
        {
            player.playerInventoryManager.rightHandWeaponIndex = 0;

            // check if i am hoding more than 1 weapon
            float weaponCount = 0;
            WeaponItem firstWeapon = null;
            int firstWeaponPosion = 0;

            for (int i = 0; i < player.playerInventoryManager.weaponInRightHandSlot.Length; i++)
            {
                if (player.playerInventoryManager.weaponInRightHandSlot[i].itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
                {
                    weaponCount += 1;

                    if (firstWeapon == null)
                    {
                        firstWeapon = player.playerInventoryManager.weaponInRightHandSlot[i];
                        firstWeaponPosion = i;
                    }
                }
            }

            if (weaponCount <= 1)
            {
                player.playerInventoryManager.rightHandWeaponIndex = -1;
                selectedWeapon = WorldItemDatabase.Instance.unarmedWeapon;
                player.playerNetworkManager.currentRightHandWeaponID.Value = selectedWeapon.itemID;
            }
            else
            {
                player.playerInventoryManager.rightHandWeaponIndex = firstWeaponPosion;
                player.playerNetworkManager.currentRightHandWeaponID.Value = firstWeapon.itemID;
            }

            return;
        }

        foreach(WeaponItem weapon in player.playerInventoryManager.weaponInRightHandSlot)
        {
            // if the next potential weapon does not equal the unarmed weapon
            if (player.playerInventoryManager.weaponInRightHandSlot[player.playerInventoryManager.rightHandWeaponIndex].itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
            {
                selectedWeapon = player.playerInventoryManager.weaponInRightHandSlot[player.playerInventoryManager.rightHandWeaponIndex];

                player.playerNetworkManager.currentRightHandWeaponID.Value = player.playerInventoryManager.weaponInRightHandSlot[player.playerInventoryManager.rightHandWeaponIndex].itemID;

                //player.playerNetworkManager.currentRightHandWeaponID.Value = selectedWeapon.itemID;
                return;
            }
        }

        if(selectedWeapon == null && player.playerInventoryManager.rightHandWeaponIndex <= 2)
        {
            SwitchRightWeapon();
        }
        
    }
    public void LoadRightHandWeapon()
    {
        if(player.playerInventoryManager.currentRightHandWeapon != null)
        {
            // remove old weapon
            rightHandWeaponSlot.UnloadWeapon();

            // bring in new weapon 
            rightHandWeaponModel = Instantiate(player.playerInventoryManager.currentRightHandWeapon.weaponModel);
            rightHandWeaponSlot.PlaceWeaponIntoSlot(rightHandWeaponModel);
            rightWeaponManager = rightHandWeaponModel.GetComponent<WeaponManager>();
            rightWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightHandWeapon);
            // assign weapon damage, to its collider

            player.playerAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentRightHandWeapon.weaponAnimator);
        }
    }
    // left hand, left weapon
    public void SwitchLeftWeapon()
    {
        // DO TEST LATER

        if (!player.IsOwner)
            return;
        player.playerAnimatorManager.PlayTargetActionAnimtion("Swap_Left_Weapon_01", false, false, true, true);

        // DO IN FUTURE
        // check if we have another weapon besiders my main weapon, rotate between weapon 1 and 2
        // if not, that make unreal, then SKIP the other empty slot and swap back. Not process both emty slots before returning to main weapon

        WeaponItem selectedWeapon = null;
        // disable two hand if i am 2 handing

        // add 1 to our index to switch to the next potential weapon
        player.playerInventoryManager.leftHandWeaponIndex += 1;

        // if my index is out of bonds, reset  it to position 1
        if (player.playerInventoryManager.leftHandWeaponIndex < 0 || player.playerInventoryManager.leftHandWeaponIndex > 11)
        {
            player.playerInventoryManager.leftHandWeaponIndex = 0;

            // check if i am hoding more than 1 weapon
            float weaponCount = 0;
            WeaponItem firstWeapon = null;
            int firstWeaponPosion = 0;

            for (int i = 0; i < player.playerInventoryManager.weaponInLeftHandSlot.Length; i++)
            {
                if (player.playerInventoryManager.weaponInLeftHandSlot[i].itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
                {
                    weaponCount += 1;

                    if (firstWeapon == null)
                    {
                        firstWeapon = player.playerInventoryManager.weaponInLeftHandSlot[i];
                        firstWeaponPosion = i;
                    }
                }
            }

            if (weaponCount <= 1)
            {
                player.playerInventoryManager.leftHandWeaponIndex = -1;
                selectedWeapon = WorldItemDatabase.Instance.unarmedWeapon;
                player.playerNetworkManager.currentLeftHandWeaponID.Value = selectedWeapon.itemID;
            }
            else
            {
                player.playerInventoryManager.leftHandWeaponIndex = firstWeaponPosion;
                player.playerNetworkManager.currentLeftHandWeaponID.Value = firstWeapon.itemID;
            }

            return;
        }

        foreach (WeaponItem weapon in player.playerInventoryManager.weaponInLeftHandSlot)
        {
            // if the next potential weapon does not equal the unarmed weapon
            if (player.playerInventoryManager.weaponInLeftHandSlot[player.playerInventoryManager.leftHandWeaponIndex].itemID != WorldItemDatabase.Instance.unarmedWeapon.itemID)
            {
                selectedWeapon = player.playerInventoryManager.weaponInLeftHandSlot[player.playerInventoryManager.leftHandWeaponIndex];

                player.playerNetworkManager.currentLeftHandWeaponID.Value = player.playerInventoryManager.weaponInLeftHandSlot[player.playerInventoryManager.leftHandWeaponIndex].itemID;

                //player.playerInventoryManager.currentLeftHandWeapon = selectedWeapon;
                return;
            }
        }

        if (selectedWeapon == null && player.playerInventoryManager.leftHandWeaponIndex <= 2)
        {
            SwitchLeftWeapon();
        }
    }
    public void LoadLeftHandWeapon()
    {
        if (player.playerInventoryManager.currentLeftHandWeapon != null)
        {
            //remove old weapon
            if(leftHandWeaponSlot.currentWeaponModel != null)
                leftHandWeaponSlot.UnloadWeapon();

            if(leftHandShieldSlot.currentWeaponModel != null)
                leftHandShieldSlot.UnloadWeapon();

            // bring in new weapon
            leftHandWeaponModel = Instantiate(player.playerInventoryManager.currentLeftHandWeapon.weaponModel);

            switch (player.playerInventoryManager.currentLeftHandWeapon.weaponModelType)
            {
                case WeaponModelType.Weapon:
                    leftHandWeaponSlot.PlaceWeaponIntoSlot(leftHandWeaponModel);
                    break;
                case WeaponModelType.Shield:
                    leftHandShieldSlot.PlaceWeaponIntoSlot(leftHandWeaponModel);
                    break;
                default:
                    break;
            }


            
            leftWeaponManager = leftHandWeaponModel.GetComponent<WeaponManager>();
            leftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon);
        }
    }

    

    // Two Hand
    public void UnTwoHandWeapon()
    {
        // Update animator controller to current main hand weapon
        player.playerAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentRightHandWeapon.weaponAnimator);
        // Two hand weapon makes my strength level (strength + (strength + 0.5))
        // Un-two hand the model and move the model that is NOT being two handed back to its hand (if there is any)
       
        // Left Hand
        if(player.playerInventoryManager.currentLeftHandWeapon.weaponModelType == WeaponModelType.Weapon)
        {
            leftHandWeaponSlot.PlaceWeaponIntoSlot(leftHandWeaponModel);
        }
        else if(player.playerInventoryManager.currentLeftHandWeapon.weaponModelType == WeaponModelType.Shield)
        {
            leftHandShieldSlot.PlaceWeaponIntoSlot(leftHandWeaponModel);
        }
        
        // Right Hand
        rightHandWeaponSlot.PlaceWeaponIntoSlot(rightHandWeaponModel);

        // Refresh the damage collider calculations (strength scaling would be effected since the strength bonus was removed)
        rightWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightHandWeapon);
        leftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon);
    }
    public void TwoHandRightWeapon()
    {
        // 1. Check for un-twohandable item
        if(player.playerInventoryManager.currentRightHandWeapon == WorldItemDatabase.Instance.unarmedWeapon)
        {
            // 2. If I am returning and NOT two handing the weapon, reset bool status's
            if (player.IsOwner)
            {
                player.playerNetworkManager.isTwoHandingRightWeapon.Value = false;
                player.playerNetworkManager.isTwoHandingWeapon.Value = false;
            }

            return;
        }

        // Update animator
        player.playerAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentRightHandWeapon.weaponAnimator);

        // 3. Place the non-two handed weapon model in the back slot or hip slot
        backSlot.PlaceWeaponModelInUnequippedSlot(leftHandWeaponModel, player.playerInventoryManager.currentLeftHandWeapon.weaponClass, player);

        // Add hand strength bonus

        // 4. Place the two handed weapon model in the right hand
        rightHandWeaponSlot.PlaceWeaponIntoSlot(rightHandWeaponModel);

        rightWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightHandWeapon);
        leftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon);
    }
    public void TwoHandLeftWeapon()
    {
        // 1. Check for un-twohandable item
        if (player.playerInventoryManager.currentLeftHandWeapon == WorldItemDatabase.Instance.unarmedWeapon)
        {
            // 2. If I am returning and NOT two handing the weapon, reset bool status's
            if (player.IsOwner)
            {
                player.playerNetworkManager.isTwoHandingLeftWeapon.Value = false;
                player.playerNetworkManager.isTwoHandingWeapon.Value = false;
            }

            return;
        }

        
        // Update animator
        player.playerAnimatorManager.UpdateAnimatorController(player.playerInventoryManager.currentLeftHandWeapon.weaponAnimator);

        // 3. Place the non-two handed weapon model in the back slot or hip slot
        backSlot.PlaceWeaponModelInUnequippedSlot(rightHandWeaponModel, player.playerInventoryManager.currentRightHandWeapon.weaponClass, player);

        // Add hand strength bonus

        // 4. Place the two handed weapon model in the right hand
        rightHandWeaponSlot.PlaceWeaponIntoSlot(leftHandWeaponModel);

        rightWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentRightHandWeapon);
        leftWeaponManager.SetWeaponDamage(player, player.playerInventoryManager.currentLeftHandWeapon);
    }
    
    public void OnSwapWeapon()
    {
        LoadRightHandWeapon();
    }
    // damage colliders
    public void OpenDamageCollider()
    {
        // open right weapon damage collider
        if(player.playerNetworkManager.isUsingRightHand.Value)
        {
            rightWeaponManager.meleeDamageCollider.EnableDamageCollider();
            player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(player.playerInventoryManager.currentRightHandWeapon.whooshes));
        }
        // open left weapon damage collider
        if(player.playerNetworkManager.isUsingLeftHand.Value)
        {
            leftWeaponManager.meleeDamageCollider.EnableDamageCollider();
            player.characterSoundFXManager.PlaySoundFX(WorldSoundFXManager.instance.ChooseRandomSFXFromArray(player.playerInventoryManager.currentLeftHandWeapon.whooshes));
        }

        // play whoosh sfx
    }
    public void CloseDamageCollider()
    {
        // open right weapon damage collider
        if (player.playerNetworkManager.isUsingRightHand.Value)
        {
            rightWeaponManager.meleeDamageCollider.DisableDamageCollider();
        }
        // open left weapon damage collider
        if (player.playerNetworkManager.isUsingLeftHand.Value)
        {
            leftWeaponManager.meleeDamageCollider.DisableDamageCollider();
        }
    }

    // Unhide Weapon
    public void UnHideWeapon()
    {
        if (player.playerEquipmentManager.rightHandWeaponModel != null)
            player.playerEquipmentManager.rightHandWeaponModel.SetActive(true);

        if (player.playerEquipmentManager.leftHandWeaponModel != null)
            player.playerEquipmentManager.leftHandWeaponModel.SetActive(true);
    }
}
