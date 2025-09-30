using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class PlayerBodyManager : MonoBehaviour
{
    PlayerManager player;

    [Header("Hair")]
    [SerializeField] public GameObject hair;
    [SerializeField] public GameObject facialHair;


    [Header("Male")]
    [SerializeField] public GameObject maleObject;      // Master Male gameobject part
    [SerializeField] public GameObject maleHead;        // Default head model when unequipping armor    
    [SerializeField] public GameObject[] maleBody;      // Default upper body model when unequipping armor(chest, upper right arms/ left arm)
    [SerializeField] public GameObject[] maleArms;      // Default upper body model when unequipping armor(lower right arms/ left arm)
    [SerializeField] public GameObject[] maleLegs;      // Default lower body model when unequipping armor(right/ left leg, hips )
    [SerializeField] public GameObject maleEyebrows;    // Facial feature
    [SerializeField] public GameObject maleFacialHair;  // Facial feature

    [Header("Female")]
    [SerializeField] public GameObject femaleObject;
    [SerializeField] public GameObject femaleHead;
    [SerializeField] public GameObject[] femaleBody;
    [SerializeField] public GameObject[] femaleArms;
    [SerializeField] public GameObject[] femaleLegs;
    [SerializeField] public GameObject femaleEyebrows;
    [SerializeField] public GameObject femaleFacialHair;


    private void Awake()
    {
        player = GetComponent<PlayerManager>();
    }
    // Enable Body Feature
    public void EnableHead()
    {
        // Enable Head Object
        maleHead.SetActive(true);
        femaleHead.SetActive(true);

        // Enable facial object(Eyes, noise, lips,...)
        maleEyebrows.SetActive(true);
        femaleEyebrows.SetActive(true);
    }

    public void DisableHead()
    {
        // Disable Head Object
        maleHead.SetActive(false);
        femaleHead.SetActive(false);

        // Disable facial object(Eyes, noise, lips,...)
        maleEyebrows.SetActive(false);
        femaleEyebrows.SetActive(false);
    }

    public void EnableHair()
    {
        hair.SetActive(true);
    }

    public void DisableHair()
    {
        hair.SetActive(false);
    }
   

    public void EnableFacialHair()
    {
        facialHair.SetActive(true);
    }

    public void DisableFacialHair()
    {
        facialHair.SetActive(false);
    }

    public void EnableBody()
    {
        foreach(var model in maleBody)
        {
            model.SetActive(true);
        }

        foreach (var model in femaleBody)
        {
            model.SetActive(true);
        }
    }
    
    public void EnableLowerBody()
    {
        foreach( var model in maleLegs)
        {
            model.SetActive(true);
        }

        foreach (var model in femaleLegs)
        {
            model.SetActive(true);
        }
    }

    public void EnableArms()
    {
        foreach (var model in maleArms)
        {
            model.SetActive(true);
        }

        foreach (var model in femaleArms)
        {
            model.SetActive(true);
        }
    }

    public void DisableBody()
    {
        foreach (var model in maleBody)
        {
            model.SetActive(false);
        }

        foreach (var model in femaleBody)
        {
            model.SetActive(false);
        }
    }

    public void DisableLowerBody()
    {
        foreach (var model in maleLegs)
        {
            model.SetActive(false);
        }

        foreach (var model in femaleLegs)
        {
            model.SetActive(false);
        }
    }

    public void DisableArms()
    {
        foreach (var model in maleArms)
        {
            model.SetActive(false);
        }

        foreach (var model in femaleArms)
        {
            model.SetActive(false);
        }
    }

    public void ToggleBodyType(bool isMale)
    {
        if(isMale)
        {
            maleObject.SetActive(true);
            femaleObject.SetActive(false);
        }
        else
        {
            maleObject.SetActive(false);
            femaleObject.SetActive(true);
        }

        player.playerEquipmentManager.EquipArmor();
    }
}
