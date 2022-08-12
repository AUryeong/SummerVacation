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
        if(item.upgrade >= 8)
        isKillingEnemy = true;
    }
    public override float GetDamage(float damage)
    {
        float damageMultipler = 2;
        if (item.upgrade >= 2)
        {
            damageMultipler *= 1.2f;
            if (item.upgrade >= 5)
                damageMultipler *= 1.4f;
        }
        return damage * damageMultipler;
    }
    public void OnCreate(Vector3 wantPos)
    {
        isHitable = true;
        isKillingEnemy = false;

        spriteRenderer.sprite = defaultSprite;
        spriteRenderer.color = Color.white;

        wantPos = new Vector3(wantPos.x, wantPos.y, wantPos.y - spriteRenderer.size.y);
        transform.position = wantPos + increaseHeight;

        Vector3 size = Vector3.one;
        if(item.upgrade >= 4)
        {
            size *= 1.15f;
            if (item.upgrade >= 7)
                size *= 1.3f;
        }
        transform.localScale = size;

        transform.DOMove(wantPos, downDuration).SetEase(Ease.InQuint).
            OnComplete(() => StartCoroutine(OnDownCoroutine()));
    }
    IEnumerator OnDownCoroutine()
    {
        isHitable = false;
        spriteRenderer.sprite = downSprite;

        if (isKillingEnemy)
            (item as Item_Durandal).CreateDurandalEclipse();
        else
            yield return new WaitForSeconds(downWaitDuration);


        spriteRenderer.DOFade(0, downFadeOutDuration).SetEase(Ease.InQuint).
            OnComplete(() => gameObject.SetActive(false));
    }
}
