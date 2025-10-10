using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class SerializableQuickSlotItem : ISerializationCallbackReceiver
{
    [SerializeField] public int itemID;
    [SerializeField] public int itemAmount;
    //[SerializeField] public int maxFlaskCharges;
    //[SerializeField] public int flaskHealAmount;


    public QuickSlotItem GetQuickSlotItem()
    {
        QuickSlotItem flask = WorldItemDatabase.Instance.GetQuickSlotItemFromSerializedData(this);

        return flask;
    }

    public void OnAfterDeserialize()
    {

    }

    public void OnBeforeSerialize()
    {

    }
}
