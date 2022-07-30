using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    public int upgrade;
    public virtual int MaxUpgrade
    {
        get
        {
            return 1;
        }
    }

    public virtual void OnEquip()
    {
    }

    public virtual void OnUpdate(float deltaTime)
    {
    }

    public virtual void OnKill(Projectile lastAttack)
    {
    }

    // true일경우 공격 무시
    public virtual bool OnHit(Enemy enemy)
    {
        return false;
    }
}
