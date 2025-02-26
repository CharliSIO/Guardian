using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlantSword : BaseMeleeWeapon
{
    private void Awake()
    {
        setBaseWeaponDamage(2);
        MeleeWeaponRangeCollider = GetComponent<Collider2D>();
        MeleeContactFilter.SetLayerMask(AttackingLayer);
    }
}
