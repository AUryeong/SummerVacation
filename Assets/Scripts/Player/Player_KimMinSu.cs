using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_KimMinSu : Player
{
    protected override void Start()
    {
        base.Start();
        items.Add(new Item_Argitoxose());
    }
}
