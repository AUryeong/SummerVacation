using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{

    protected string name;
    protected string[] lore;

    //비 획득은 0, 획득 부터 1 시작
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

    public virtual float GetDamage(float damage)
    {
        return damage;
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
    private float duration;
    private float randomDistance = 4;


    private float defaultDamagePercent = 2;
    private float damagePercent;
    //업그레이드
    private float twoDamagePercent = 1.2f;
    private float fiveDamagePercent = 1.4f;


    private float defaultCooltime = 3;
    private float cooltime;
    //업그레이드
    private float threeCooltimePercent = 0.8f;
    private float sixCooltimePercent = 0.7f;


    private Vector3 defaultSize = Vector3.one;
    private Vector3 size;
    //업그레이드
    private float fourSizePercent = 1.15f;
    private float sevenSizePercent = 1.3f;

    public override void OnReset()
    {
        duration = 0;
        cooltime = defaultCooltime;
        damagePercent = defaultDamagePercent;
        size = defaultSize;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (upgrade)
        {
            case 2:
                damagePercent *= twoDamagePercent;
                break;
            case 3:
                cooltime *= threeCooltimePercent;
                break;
            case 4:
                size *= fourSizePercent;
                break;
            case 5:
                damagePercent *= fiveDamagePercent;
                break;
            case 6:
                cooltime *= sixCooltimePercent;
                break;
            case 7:
                size *= sevenSizePercent;
                break;
        }
    }

    public override float GetDamage(float damage)
    {
        return damage * damagePercent;
    }

    public override bool CanGet()
    {
        return base.CanGet() && Player.Instance is Player_KimMinSu;
    }

    public void CreateDurandalEclipse()
    {
        // 쌍검이여서 두번 소환
        for (int i = 0; i < 2; i++)
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

        Vector3 pos;
        if (GameManager.Instance.inCameraEnemies.Count > 0)
            pos = RandomManager.SelectOne(GameManager.Instance.inCameraEnemies).transform.position + Random.onUnitSphere;
        else
            pos = Player.Instance.transform.position + Random.onUnitSphere * randomDistance;

        durandal.OnCreate(pos, size);
    }
}

//여신 세라피네의 가호
public class Item_Godbless : Item
{
    float duration;


    float cooltime;
    float defaultCooltime = 60;
    //업그레이드
    float upgradeCooltimePercent = 0.9f;


    float upgradeDamageAdder = 5;

    public override bool CanGet()
    {
        return base.CanGet() && Player.Instance is Player_KimMinSu;
    }
    public override void OnReset()
    {
        base.OnReset();
        cooltime = defaultCooltime;
    }

    public override void OnEquip()
    {
        base.OnEquip();
        Player.Instance.stat.damage += upgradeDamageAdder;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (upgrade)
        {
            case 2:
                cooltime *= upgradeCooltimePercent;
                break;
            case 3:
                cooltime *= upgradeCooltimePercent;
                Player.Instance.stat.damage += upgradeDamageAdder;
                break;
        }
    }

    public override void OnUpdate(float deltaTime)
    {
        duration += deltaTime;
        if (duration >= cooltime)
        {
            duration = 0;
            Player.Instance.TakeHeal(Player.Instance.stat.maxHp - Player.Instance.stat.hp);
        }
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