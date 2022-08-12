using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Durandal_Eclipse : Projectile
{
    float downDuration = 0.25f;
    float downWaitDuration = 1;
    float downFadeOutDuration = 1;
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
    public override float GetDamage(float damage)
    {
        float damageMultipler = 3.36f;
        return damage * damageMultipler;
    }
    public void OnCreate(Vector3 wantPos)
    {
        isHitable = true;
        spriteRenderer.sprite = defaultSprite;
        spriteRenderer.color = Color.white;

        wantPos = new Vector3(wantPos.x, wantPos.y, wantPos.y - spriteRenderer.size.y);
        transform.position = wantPos + increaseHeight;

        transform.DOMove(wantPos, downDuration).SetEase(Ease.InQuint).
            OnComplete(() => StartCoroutine(OnDownCoroutine()));
    }
    IEnumerator OnDownCoroutine()
    {
        isHitable = false;
        spriteRenderer.sprite = downSprite;
        yield return new WaitForSeconds(downWaitDuration);

        spriteRenderer.DOFade(0, downFadeOutDuration).SetEase(Ease.InQuint).
            OnComplete(() => gameObject.SetActive(false));
    }
}
