using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponModelInstantiationSlot : MonoBehaviour
{
    public WeaponModelSlot weaponSlot;

    public GameObject currentWeaponModel;

    public void UnloadWeapon()
    {
        if(currentWeaponModel != null)
        {
            Destroy(currentWeaponModel);
        }
    }
    public void PlaceWeaponIntoSlot(GameObject weaponModel)
    {
        currentWeaponModel = weaponModel;
        weaponModel.transform.parent = transform;

        weaponModel.transform.localPosition = Vector3.zero;
        weaponModel.transform.localRotation = Quaternion.identity;
        weaponModel.transform.localScale = Vector3.one;
    }
    public void PlaceWeaponModelInUnequippedSlot(GameObject weaponModel, WeaponClass weaponClass, PlayerManager player)
    {
        // ToDo: Move weapon on back closer or more outward defending on chest equipment (So it doesn't appear to float)

        currentWeaponModel = weaponModel;
        weaponModel.transform.parent = transform;

        switch(weaponClass)
        {
            case WeaponClass.KatanaBlue:
                weaponModel.transform.localPosition = new Vector3(0.064f, 0f, - 0.06f);
                weaponModel.transform.localRotation = Quaternion.Euler(194,90, - 0.022f);
                break;
            case WeaponClass.LightningTwinBlades:
                weaponModel.transform.localPosition = new Vector3(0.064f, 0f, -0.06f);
                weaponModel.transform.localRotation = Quaternion.Euler(194, 90, -0.022f);
                break;
            case WeaponClass.TwinBlades:
                weaponModel.transform.localPosition = new Vector3(0.064f, 0f, -0.06f);
                weaponModel.transform.localRotation = Quaternion.Euler(194, 90, -0.022f);
                break;
            case WeaponClass.Shield:
                weaponModel.transform.localPosition = new Vector3(0.064f, 0f, -0.06f);
                weaponModel.transform.localRotation = Quaternion.Euler(194, 90, -0.022f);
                break;
            default:
                break;
        }
    }
}
