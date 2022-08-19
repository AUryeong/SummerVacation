using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Durandal : Projectile
{
    float downDuration = 0.5f;
    float downWaitDuration = 3;
    float downFadeOutDuration = 1;

    bool isKillingEnemy = false;

    Vector3 increaseHeight = new Vector3(10, 10, -10);
    SpriteRenderer spriteRenderer;

    [SerializeField]
    Sprite downSprite;
    Sprite defaultSprite;
    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defaultSprite = spriteRenderer.sprite;
    }

    public override void OnKill()
    {
        if (item.upgrade >= 8)
            isKillingEnemy = true;
    }
    public void OnCreate(Vector3 wantPos, Vector3 size)
    {
        isHitable = false;
        isKillingEnemy = false;
        spriteRenderer.sprite = defaultSprite;
        spriteRenderer.color = Color.white;

        wantPos = new Vector3(wantPos.x, wantPos.y, wantPos.y - spriteRenderer.size.y);
        transform.position = wantPos + increaseHeight;

        transform.localScale = size;
        transform.DOMove(wantPos, downDuration).SetEase(Ease.InQuint).
            OnComplete(() => StartCoroutine(OnDownCoroutine()));
    }
    IEnumerator OnDownCoroutine()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, spriteRenderer.bounds.size.x / 2, LayerMask.GetMask(nameof(Enemy)));
        if (enemies != null && enemies.Length > 0)
            foreach (Collider2D collider2D in enemies)
                collider2D.GetComponent<Enemy>().OnHurt(this, true);

        spriteRenderer.sprite = downSprite;

        if (isKillingEnemy)
            (item as Item_Durandal).CreateDurandalEclipse();
        else
            yield return new WaitForSeconds(downWaitDuration);

        spriteRenderer.DOFade(0, downFadeOutDuration).SetEase(Ease.InQuint).
            OnComplete(() => gameObject.SetActive(false));
    }
}
