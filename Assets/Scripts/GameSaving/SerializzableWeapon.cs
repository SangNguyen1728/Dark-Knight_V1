using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SerializzableWeapon : ISerializationCallbackReceiver
{
    [SerializeField] public int itemID;
    [SerializeField] public int ashOfWarID;


    public WeaponItem GetWeapon()
    {
        WeaponItem weapon = WorldItemDatabase.Instance.GetWeaponFromSerializedData(this);

        return weapon;
    }

    public void OnAfterDeserialize()
    {
        
    }

    public void OnBeforeSerialize()
    {
        
    }
}
