using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Durandal : Item
{
    float cooltime = 3;
    float duration = 0;

    public override void OnUpdate(float detlaTime)
    {
        duration += detlaTime;
        if(duration >= cooltime)
        {
            duration -= cooltime;
            GameObject projectile = GameManager.Instance.GetProjectile("Durandal");
            Durandal durandal = projectile.GetComponent<Durandal>();
            durandal.item = this;
            if(GameManager.Instance.inCameraEnemies.Count > 0)
                durandal.OnCreate(RandomManager.SelectOne(GameManager.Instance.inCameraEnemies).transform.position + Random.onUnitSphere);
            else
                durandal.OnCreate(Player.Instance.transform.position + Random.onUnitSphere * 4);
            projectile.gameObject.SetActive(true);
        }
    }
}
