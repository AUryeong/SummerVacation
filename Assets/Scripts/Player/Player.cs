using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Player : Unit
{
    public static Player Instance;
    public List<Item> items = new List<Item>();

    #region 饭骇
    public float xpAdd = 100f;
    public int lv = 0;
    public float exp = 0;
    public float maxExp = 100f;
    #endregion

    [SerializeField]
    GameObject arrowObj;
    [SerializeField]
    SpriteRenderer arrowSprite;

    #region 各 面倒 包府
    protected bool inv = false;
    protected bool hurtInv = false;
    [SerializeField]
    SpriteRenderer hpBarSprite;
    Color hitTextColor = new Color(255 / 255f, 66 / 255f, 66 / 255f);
    public List<Enemy> collisionEnemyList = new List<Enemy>();
    #endregion

    #region 局聪皋捞记 包府
    SpriteRenderer spriteRenderer;
    Animator animator;
    #endregion

    public Quaternion GetArrowRotation()
    {
        return arrowObj.transform.rotation;
    }
    protected virtual void Start()
    {
        Instance = this;
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    protected virtual void Update()
    {
        float deltaTime = Time.deltaTime;
        Move(deltaTime);
        HitCheck();
        ArrowUpdate();
        foreach (Item item in items)
            item.OnUpdate(deltaTime);
    }


    public void OnKill(Projectile projectile)
    {
        foreach (Item item in items)
            item.OnKill(projectile);
    }

    void ArrowUpdate()
    {
        if (Input.GetKeyDown(KeyCode.Z))
            arrowSprite.DOColor(new Color(0.7f, 0.7f, 0.7f), 0.1f);
        else if (Input.GetKeyUp(KeyCode.Z))
            arrowSprite.DOColor(Color.white, 0.1f);
    }

    #region 各 面倒
    void Die()
    {

    }
    public void TakeDamage(float damage, bool invAttack = false, bool isSkipText = false)
    {
        if (invAttack || inv)
        {
            if (!isSkipText)
                GameManager.Instance.ShowText("MISS", transform.position, Color.white);
            return;
        }
        stat.hp -= damage;
        if(stat.hp <= 0)
        {
            stat.hp = 0;
            Die();
        }
        hpBarSprite.size = new Vector2(stat.hp / stat.maxHp, 1);
        if (!isSkipText)
            GameManager.Instance.ShowDamage((int)damage, transform.position, hitTextColor);
    }

    protected override void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (collider2D != null && collider2D.CompareTag("Enemy"))
            collisionEnemyList.Add(collider2D.GetComponent<Enemy>());
    }

    protected override void OnTriggerExit2D(Collider2D collider2D)
    {
        if (collider2D != null && collider2D.CompareTag("Enemy"))
            collisionEnemyList.Remove(collider2D.GetComponent<Enemy>());
    }

    void HitCheck()
    {
        if (hurtInv)
            return;
        foreach(Enemy enemy in collisionEnemyList)
        {
            if (enemy.dying) return;

            if (Random.Range(0f, 100f) <= stat.evade)
            {
                TakeDamage(0, true);
                return;
            }

            bool invAttack = false;
            foreach (Item item in items)
                if (item.OnHit(enemy))
                    invAttack = true;
            TakeDamage(enemy.GetDamage(), invAttack);
            hurtInv = true;
            spriteRenderer.DOFade(0.8f, 0.1f).SetUpdate(true).OnComplete(() =>
            spriteRenderer.DOFade(1, 0.1f).SetUpdate(true).OnComplete(() =>
            hurtInv = false));
        }
    }

    #endregion

    #region 捞悼
    float speedX;
    float speedY;
    void Move(float deltaTime)
    {
        speedX = 0;
        speedY = 0;
        if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.D))
        {
            speedX = stat.speed;
            spriteRenderer.flipX = true;
        }
        if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.A))
        {
            speedX -= stat.speed;
            spriteRenderer.flipX = false;
        }
        if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W))
        {
            speedY = stat.speed;
        }
        if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S))
        {
            speedY -= stat.speed;
        }
        if (speedX == 0 && speedY == 0)
        {
            animator.SetBool("isWalking", false);
            animator.speed = 1;
        }
        else
        {
            animator.SetBool("isWalking", true);
            animator.speed = stat.speed / 5;
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
                arrowObj.transform.rotation = Quaternion.Lerp(arrowObj.transform.rotation, Quaternion.Euler(0, 0, rotationZ), Time.deltaTime * 20);
            }
        }
        transform.Translate(speedX * deltaTime, speedY * deltaTime, speedY * deltaTime);
    }
    #endregion
}
