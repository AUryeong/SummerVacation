using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.SceneManagement;

public class Player : Unit
{
    public static Player Instance;

    //캐릭터 기본 스텟, 일반 stat은 퍼센트로 표현
    public Stat defaultStat;
    private List<Item> items = new List<Item>();

    #region 레벨 변수
    public float xpAdd = 100f;
    public int lv = 0;
    protected float exp = 0;
    public float maxExp = 100f;
    public float Exp
    {
        get
        {
            return exp;
        }
        set
        {
            exp = value * xpAdd / 100;
            if (exp >= maxExp)
            {
                exp -= maxExp;
                lv++;
                maxExp = Mathf.Pow(lv, 1.3f) * 100;
                GameManager.Instance.AddWeapon();
            }
            UIManager.Instance.UpdateLevel();
        }
    }
    #endregion

    #region 화살표 변수
    [SerializeField]
    GameObject arrowObj;
    [SerializeField]
    SpriteRenderer arrowSprite;
    Color arrowPressColor = new Color(0.7f, 0.7f, 0.7f);
    Color arrowDefaultColor = Color.white;
    float arrowFadeTime = 0.1f;
    float arrowTurnSpeed = 20;
    #endregion

    #region 몹 충돌 변수

    protected bool inv = false;
    protected bool hurtInv = false;

    Color hitTextColor = new Color(255 / 255f, 66 / 255f, 66 / 255f);
    Color healTextColor = new Color(101 / 255f, 255 / 255f, 76 / 255f);
    float hitFadeInAlpha = 0.8f;
    float hitFadeOutAlpha = 1f;
    float hitFadeTime = 0.1f;

    float damageRandomPercent = 10;

    [SerializeField]
    SpriteRenderer hpBarSprite;
    [HideInInspector]
    public List<Enemy> collisionEnemyList = new List<Enemy>();
    #endregion

    #region 애니메이션 변수
    SpriteRenderer spriteRenderer;
    Animator animator;
    float animatorScaleSpeed = 0.2f;
    #endregion


    protected virtual void Awake()
    {
        Instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    protected virtual void Start()
    {
    }

    protected virtual void Update()
    {
        if (UIManager.Instance.IsActable())
        {
            float deltaTime = Time.deltaTime;
            Move(deltaTime);
            HitCheck();
            ArrowUpdate();
            foreach (Item item in items)
                item.OnUpdate(deltaTime);
        }
    }


    public void OnKill(Enemy enemy)
    {
        foreach (Item item in items)
            item.OnKill(enemy);
    }

    #region 인벤 함수
    public List<Item> GetInven()
    {
        return new List<Item>(items);
    }

    public void AddItem(Item addItem)
    {
        Item item = items.Find((Item x) => x == addItem);
        if (item == null)
        {
            item = addItem;
            items.Add(item);
            item.OnEquip();
        }
        else
            item.OnUpgrade();
        UIManager.Instance.UpdateItemInven(item);
    }
    #endregion

    #region 화살표 함수
    public Quaternion GetArrowRotation()
    {
        return arrowObj.transform.rotation;
    }

    protected void ArrowUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            arrowSprite.DOColor(arrowPressColor, arrowFadeTime);
        else if (Input.GetKeyUp(KeyCode.Z))
            arrowSprite.DOColor(arrowDefaultColor, arrowFadeTime);
    }
    #endregion

    #region 몹 충돌 함수
    protected void Die()
    {
        GameManager.Instance.GameOver();
    }
    public override float GetDamage()
    {
        float damage = stat.damage / 100 * defaultStat.damage;
        damage += Random.Range(damage / -damageRandomPercent, damage / damageRandomPercent);
        if (Random.Range(0f, 100f) <= stat.crit)
        {
            damage *= stat.critDmg / 100;
        }
        return damage;
    }

    public void TakeHeal(float healAmount, bool isSkipText = false)
    {
        stat.hp += healAmount;
        if (stat.hp > stat.maxHp)
        {
            stat.hp = stat.maxHp;
        }
        hpBarSprite.size = new Vector2(stat.hp / (stat.maxHp / 100 * defaultStat.maxHp), 1);
        if (!isSkipText)
            GameManager.Instance.ShowInt((int)healAmount, transform.position, healTextColor);
    }
    public void TakeDamage(float damage, bool invAttack = false, bool isSkipText = false)
    {
        if (invAttack || inv)
        {
            if (!isSkipText)
                GameManager.Instance.ShowText("MISS", transform.position, Color.white);
            return;
        }
        SoundManager.Instance.PlaySound("hurt", SoundType.SE);
        stat.hp -= damage;
        if (stat.hp <= 0)
        {
            stat.hp = 0;
            Die();
        }
        hpBarSprite.size = new Vector2(stat.hp / (stat.maxHp / 100 * defaultStat.maxHp), 1);
        if (!isSkipText)
            GameManager.Instance.ShowInt((int)damage, transform.position, hitTextColor);
    }

    protected override void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D == null)
            return;

        if (collider2D.CompareTag("Enemy"))
            collisionEnemyList.Add(collider2D.GetComponent<Enemy>());
        if (collider2D.CompareTag("Exp"))
            collider2D.GetComponent<Exp>().OnGet();
    }

    protected override void OnTriggerExit2D(Collider2D collider2D)
    {
        if (collider2D == null)
            return;

        if (collider2D.CompareTag("Enemy"))
            collisionEnemyList.Remove(collider2D.GetComponent<Enemy>());
    }

    protected void HitCheck()
    {
        if (hurtInv || stat.hp <= 0) return;

        foreach (Enemy enemy in collisionEnemyList.ToArray())
        {
            if (enemy == null || !enemy.gameObject.activeSelf || enemy.dying) return;

            if (Random.Range(0f, 100f) <= stat.evade)
            {
                TakeDamage(0, true);
                return;
            }
            hurtInv = true;

            bool invAttack = false;
            foreach (Item item in items)
                if (item.OnHit(enemy))
                    invAttack = true;
            TakeDamage(enemy.GetDamage(), invAttack);

            spriteRenderer.DOFade(hitFadeInAlpha, hitFadeTime).
                OnComplete(() => spriteRenderer.DOFade(hitFadeOutAlpha, hitFadeTime).
                OnComplete(() => hurtInv = false));
        }
    }

    #endregion

    #region 이동 함수
    protected void Move(float deltaTime)
    {
        float speedX = 0;
        float speedY = 0;

        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            speedX = stat.speed / 100 * defaultStat.speed;
            spriteRenderer.flipX = true;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            speedX -= stat.speed / 100 * defaultStat.speed;
            spriteRenderer.flipX = false;
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            speedY = stat.speed / 100 * defaultStat.speed;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            speedY -= stat.speed / 100 * defaultStat.speed;
        }
        if (speedX == 0 && speedY == 0)
        {
            animator.SetBool("isWalking", false);
            animator.speed = 1;
        }
        else
        {
            animator.SetBool("isWalking", true);
            animator.speed = stat.speed / 100 * defaultStat.speed * animatorScaleSpeed;
            if (!Input.GetKey(KeyCode.Z))
            {
                float rotationZ;
                if (speedX > 0)
                    if (speedY > 0) rotationZ = 225;
                    else if (speedY == 0) rotationZ = 180;
                    else rotationZ = 135;
                else if (speedX == 0)
                    if (speedY > 0) rotationZ = 270;
                    else rotationZ = 90;
                else
                    if (speedY > 0) rotationZ = 315;
                else if (speedY == 0) rotationZ = 0;
                else rotationZ = 45;
                arrowObj.transform.rotation = Quaternion.Lerp(arrowObj.transform.rotation, Quaternion.Euler(0, 0, rotationZ), Time.deltaTime * arrowTurnSpeed);
            }
        }
        transform.Translate(speedX * deltaTime, speedY * deltaTime, speedY * deltaTime);
    }
    #endregion
}
