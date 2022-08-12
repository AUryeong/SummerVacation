using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Enemy : Unit
{
    public bool dying
    {
        get;
        private set;
    }
    protected Direction direction;
    protected SpriteRenderer spriteRenderer;
    private Coroutine enemyFlashWhiteCo;

    protected virtual void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    protected virtual void OnEnable()
    {
        stat.hp = stat.maxHp;
        spriteRenderer.color = Color.white;
        dying = false;
    }

    protected virtual void Update()
    {
        float deltaTime = Time.deltaTime;
        if (!dying)
        {
            Move(deltaTime);
        }
    }

    void Move(float deltaTime)
    {
        transform.Translate(stat.speed * deltaTime * (Player.Instance.transform.position - transform.position).normalized);
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);

        if (Player.Instance.transform.position.x - transform.position.x < 0)
        {
            direction = Direction.Left;
            spriteRenderer.flipX = false;
        }
        else
        {
            direction = Direction.Right;
            spriteRenderer.flipX = true;
        }
    }
    protected override void OnTriggerExit2D(Collider2D collider2D)
    {
        if (dying) return;
        if (collider2D == null) return;

        if (collider2D.CompareTag("Camera"))
            GameManager.Instance.inCameraEnemies.Remove(this);
    }

    protected virtual void OnHurt(Projectile projectile)
    {
        if (!projectile.isHitable)
            return;
        projectile.OnHit(this);

        if (Random.Range(0f, 100f) <= stat.evade)
        {
            GameManager.Instance.ShowText("MISS", transform.position, Color.white);
            return;
        }

        float damage = projectile.GetDamage(Player.Instance.GetDamage());
        stat.hp -= damage;
        GameManager.Instance.ShowDamage((int)damage, transform.position, Color.white);
        enemyFlashWhiteCo = StartCoroutine(HitFlashWhite());
        if (stat.hp > 0)
        {
            return;
        }

        dying = true;
        projectile.OnKill();
        Player.Instance.OnKill(projectile);
        GameManager.Instance.OnKill(this);
        transform.DOMoveX(transform.position.x + ((direction == Direction.Left) ? 1 : -1), 0.5f);
        spriteRenderer.DOFade(0, 0.5f).
            OnComplete(() => gameObject.SetActive(false));
        spriteRenderer.DOColor(new Color(1, 0.7f, 0.7f), 0.1f).SetEase(Ease.InQuart);
    }

    protected override void OnTriggerEnter2D(Collider2D collider2D)
    {
        if (dying) return;
        if (collider2D == null) return;

        if (collider2D.CompareTag("Camera"))
            GameManager.Instance.inCameraEnemies.Add(this);

        else if (collider2D.CompareTag("Projectile"))
            OnHurt(collider2D.GetComponent<Projectile>());
    }

    protected IEnumerator HitFlashWhite()
    {
        Material original = spriteRenderer.material;
        spriteRenderer.material = GameManager.Instance.flashWhiteMaterial;
        yield return new WaitForSeconds(0.2f);
        spriteRenderer.material = original;
    }
}
