using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public interface iWeapon
{
    public int iDamage { get; }

    public void Attack();

    public void SetOwningPlayer(GameObject _Player);

    // public void UseWeaponMod1();
    // public void UseWeaponMod2();
}

public abstract class BaseWeapon : MonoBehaviour, iWeapon
{
    protected GameObject OwningPlayer;
    public LayerMask AttackingLayer;
    private int iBaseDamage = 1;

    public int iDamage { get { return iBaseDamage; } }
    protected void setBaseWeaponDamage(int _iDamage) { iBaseDamage = _iDamage; }
    public abstract void Attack();

    public void SetOwningPlayer(GameObject _Player)
    {
        this.OwningPlayer = _Player;
    }
}

public abstract class BaseMeleeWeapon : BaseWeapon
{
    protected Collider2D MeleeWeaponRangeCollider;
    protected ContactFilter2D MeleeContactFilter;

    private void Awake()
    {
        MeleeWeaponRangeCollider = GetComponent<Collider2D>();
        MeleeContactFilter.SetLayerMask(AttackingLayer);
    }

    // Default melee attack function for melee weapon
    public override void Attack()
    {
        Debug.Log("Attacked!");
        RaycastHit2D[] AttackCastHits = new RaycastHit2D[3];
        int iNumberEnemiesHit = MeleeWeaponRangeCollider.Cast(transform.forward, MeleeContactFilter, AttackCastHits, MeleeWeaponRangeCollider.bounds.extents.magnitude, true);
        if (iNumberEnemiesHit > 0)
        {
            foreach (RaycastHit2D ObjectHit in AttackCastHits)
            {
                iAttackableObject AttackedObj = ObjectHit.collider.GetComponentInParent<iAttackableObject>();
                if (AttackedObj != null)
                {
                    AttackedObj.GetAttacked(iDamage);
                }
            }
        }
    }
}
