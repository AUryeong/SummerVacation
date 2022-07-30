using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item_Argitoxose : Item
{
    float duration;
    float shootDuration;
    int shootCount;

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        duration += Time.deltaTime;
        if (duration > 3)
        {
            duration -= 3;
            ArrowCreate();
            shootCount = 2;
        }
        if (shootCount > 0)
        {
            shootDuration += Time.deltaTime;
            if (shootDuration >= 0.3f)
            {
                shootDuration -= 0.3f;
                shootCount--;
                ArrowCreate();
            }
        }
    }
    void ArrowCreate()
    {
        GameObject arrowObj = GameManager.Instance.GetProjectile("Arrow");
        arrowObj.transform.position = Player.Instance.transform.position;
        arrowObj.transform.rotation = Player.Instance.GetArrowRotation();
    }
}
