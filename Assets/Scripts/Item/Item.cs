using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{
    // 비 획득은 0, 획득 부터 1 시작
    public void Init(string name, string[] lore, int maxUpgrade, Sprite icon)
    {
        this.name = name;
        this.lore = lore;
        this.maxUpgrade = maxUpgrade;
        this.icon = icon;
        OnReset();
    }

    public virtual void OnReset()
    {
        upgrade = 0;
    }

    protected string name;
    protected string[] lore;
    public int upgrade
    {
        get;
        private set;
    }
    protected int maxUpgrade;

    public Sprite icon
    {
        get;
        protected set;
    }

    public string GetName()
    {
        if (upgrade < 1)
            return name;
        else
            return name + " LV." + upgrade.ToString();
    }

    public string GetLore()
    {
        return lore[upgrade];
    }
    public virtual bool CanGet()
    {
        return upgrade <= maxUpgrade;
    }
    public virtual void OnEquip()
    {
        upgrade = 1;
    }

    public virtual void OnUpdate(float deltaTime)
    {
    }

    public virtual void OnUpgrade()
    {
        upgrade++;
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

//뒤랑칼
public class Item_Durandal : Item
{
    private float defaultCooltime = 3;
    private float cooltime;

    private float defaultDuration = 0;
    private float duration;

    private float randomDistance = 4;

    public override void OnReset()
    {
        duration = defaultDuration;
        cooltime = defaultCooltime;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        if (upgrade == 3)
            cooltime *= 0.8f;
        else if (upgrade == 6)
            cooltime *= 0.6f;
    }
    public override bool CanGet()
    {
        return base.CanGet() && Player.Instance is Player_KimMinSu;
    }

    public void CreateDurandalEclipse()
    {
        for(int i = 0; i< 2; i++)
        {
            GameObject projectile = ResourcesManager.Instance.GetProjectile("Durandal_Eclipse");
            Durandal_Eclipse durandal = projectile.GetComponent<Durandal_Eclipse>();

            durandal.item = this;
            if (GameManager.Instance.inCameraEnemies.Count > 0)
                durandal.OnCreate(RandomManager.SelectOne(GameManager.Instance.inCameraEnemies).transform.position + Random.onUnitSphere);
            else
                durandal.OnCreate(Player.Instance.transform.position + Random.onUnitSphere * randomDistance);

            projectile.gameObject.SetActive(true);
        }
    }

    public override void OnUpdate(float detlaTime)
    {
        duration += detlaTime;
        if (duration < cooltime)
        {
            return;
        }
        duration -= cooltime;

        GameObject projectile = ResourcesManager.Instance.GetProjectile("Durandal");
        Durandal durandal = projectile.GetComponent<Durandal>();

        durandal.item = this;
        if (GameManager.Instance.inCameraEnemies.Count > 0)
            durandal.OnCreate(RandomManager.SelectOne(GameManager.Instance.inCameraEnemies).transform.position + Random.onUnitSphere);
        else
            durandal.OnCreate(Player.Instance.transform.position + Random.onUnitSphere * randomDistance);

        projectile.gameObject.SetActive(true);
    }
}

//여신 세라피네의 가호
public class Item_Godbless : Item
{
    public override bool CanGet()
    {
        return base.CanGet() && Player.Instance is Player_KimMinSu;
    }
}

//데이 브레이크
public class Item_Daybreak : Item
{
    public override bool CanGet()
    {
        return base.CanGet() && Player.Instance is Player_KimMinSu;
    }
}

//폭주하는 날개
public class Item_KiaraR : Item
{
}