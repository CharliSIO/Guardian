using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSwordPickup : ItemPickup
{
    private iWeapon AttachedWeapon;
    private GameObject AttachedWeaponObject;
    public GameObject WeaponPrefab;


    public override void Interact(PlayerMovement _InteractingCharacter)
    {
        if (AttachedWeapon == null)
        {
            AttachedWeaponObject = Instantiate(WeaponPrefab, _InteractingCharacter.gameObject.transform);
            AttachedWeapon = AttachedWeaponObject.GetComponent<PlantSword>();
            AttachedWeapon.SetOwningPlayer(_InteractingCharacter.gameObject);
        }

        _InteractingCharacter.CurrentWeapon = AttachedWeapon;
    }
}
