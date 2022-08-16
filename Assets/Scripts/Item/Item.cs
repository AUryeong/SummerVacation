using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item
{

    protected string name;
    protected string[] lore;

    //�� ȹ���� 0, ȹ�� ���� 1 ����
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
    /// <summary>
    /// ������ ������ �ߵ�
    /// </summary>
    /// <param name="name">������ �̸�</param>
    /// <param name="lore">������ �����</param>
    /// <param name="maxUpgrade">�ִ� ���׷��̵� Ƚ��</param>
    /// <param name="icon">������ ������</param>
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

    public virtual void OnKill(Enemy killEnemy)
    {
    }

    public virtual float GetDamage(float damage)
    {
        return damage;
    }

    // true�ϰ�� ���� ����
    public virtual bool OnHit(Enemy enemy)
    {
        return false;
    }
}

//�ڶ�Į
public class Item_Durandal : Item
{
    private float duration;
    readonly private float randomDistance = 4;


    readonly private float defaultDamagePercent = 2;
    private float damagePercent;
    //���׷��̵�
    readonly private float twoDamagePercent = 1.2f;
    readonly private float fiveDamagePercent = 1.4f;


    readonly private float defaultCooltime = 3;
    private float cooltime;
    //���׷��̵�
    readonly private float threeCooltimePercent = 0.8f;
    readonly private float sixCooltimePercent = 0.7f;


    private Vector3 defaultSize = Vector3.one;
    private Vector3 size;
    //���׷��̵�
    readonly private float fourSizePercent = 1.15f;
    readonly private float sevenSizePercent = 1.3f;

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
        // �ְ��̿��� �ι� ��ȯ
        for (int i = 0; i < 2; i++)
        {
            GameObject projectile = ResourcesManager.Instance.GetProjectile(nameof(Durandal_Eclipse));
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

        GameObject projectile = ResourcesManager.Instance.GetProjectile(nameof(Durandal));
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

//���� �����ǳ��� ��ȣ
public class Item_Godbless : Item
{
    float duration;


    float cooltime;
    readonly float defaultCooltime = 60;
    //���׷��̵�
    readonly float upgradeCooltimePercent = 0.9f;


    readonly float upgradeDamageAdder = 5;

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

//���� �극��ũ
public class Item_Daybreak : Item
{

    readonly float critPercentDecrease = 30;
    readonly float damagePercentIncrease = 30;

    //���׷��̵�
    readonly float twoDamagePercentIncrease = 10;
    readonly float threeDamagePercentIncrease = 15;
    readonly float fourDamagePercentIncrease = 20;
    public override bool CanGet()
    {
        return base.CanGet() && Player.Instance is Player_KimMinSu;
    }

    public override void OnEquip()
    {
        base.OnEquip();
        Player.Instance.stat.crit -= critPercentDecrease;
        Player.Instance.stat.damage += damagePercentIncrease;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (upgrade)
        {
            case 2:
                Player.Instance.stat.damage += twoDamagePercentIncrease;
                break;
            case 3:
                Player.Instance.stat.damage += threeDamagePercentIncrease;
                break;
            case 4:
                Player.Instance.stat.damage += fourDamagePercentIncrease;
                break;
        }
    }
}

//�����ϴ� ����
public class Item_KiaraR : Item
{
    float duration = 0;
    float cooltime = 0.3f;

    readonly float defaultDamagePercent = 0.4f;
    float damagePercecnt;
    //���׷��̵�
    readonly float threeDamagePercent = 1.3f;


    readonly Vector3 defaultsize = Vector3.one;
    Vector3 size;
    readonly float overlapMultipler = 1.5125f;
    //���׷��̵�
    readonly float twoSizePercent = 1.15f;
    readonly float fourSizePercent = 1.25f;
    readonly float fiveSizePercent = 1.5f;


    GameObject auraObj;
    public override void OnEquip()
    {
        base.OnEquip();
        auraObj = ResourcesManager.Instance.GetProjectile("KiaraR");
        auraObj.transform.localScale = size;
        auraObj.GetComponent<Projectile>().isHitable = false;
    }

    public override float GetDamage(float damage)
    {
        return damage * damagePercecnt;
    }

    public override void OnReset()
    {
        base.OnReset();
        if(auraObj != null)
        {
            auraObj.gameObject.SetActive(false);
            auraObj = null;
        }
        damagePercecnt = defaultDamagePercent;
        size = defaultsize;
    }

    public override void OnUpgrade()
    {
        base.OnUpgrade();
        switch (upgrade)
        {
            case 2:
                size *= twoSizePercent;
                break;
            case 3:
                damagePercecnt *= threeDamagePercent;
                break;
            case 4:
                size *= fourSizePercent;
                break;
            case 5:
                size *= fiveSizePercent;
                break;
        }
        auraObj.transform.localScale = size;
    }

    public override void OnUpdate(float deltaTime)
    {
        base.OnUpdate(deltaTime);
        if (auraObj != null)
            auraObj.transform.position = Player.Instance.transform.position;

        duration += deltaTime;
        if(duration < cooltime)
            return;
        duration -= cooltime;

        Collider2D[] colliders = Physics2D.OverlapCircleAll(Player.Instance.transform.position, size.x * overlapMultipler, LayerMask.GetMask(nameof(Enemy)));
        if(colliders != null && colliders.Length > 0)
        {
            float damage = GetDamage(Player.Instance.GetDamage());
            foreach (Collider2D collider2D in colliders)
                collider2D.GetComponent<Enemy>().OnHurt(damage);
        }
    }
}