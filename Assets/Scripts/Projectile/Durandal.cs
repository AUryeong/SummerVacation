using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Durandal : Projectile
{
    float downDuration = 0.5f;
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
        return damage * 2;
    }
    public void OnCreate(Vector3 wantPos)
    {
        wantPos = new Vector3(wantPos.x, wantPos.y, wantPos.y - spriteRenderer.size.y);
        isHitable = true;
        spriteRenderer.color = Color.white;
        spriteRenderer.sprite = defaultSprite;
        transform.position = wantPos + increaseHeight;
        transform.DOMove(wantPos, downDuration).SetEase(Ease.InQuint).SetUpdate(true).
        OnComplete(()=> StartCoroutine(OnDownCoroutine()));
    }
    IEnumerator OnDownCoroutine()
    {
        spriteRenderer.sprite = downSprite;
        isHitable = false;
        yield return new WaitForSeconds(3);
        spriteRenderer.DOFade(0, 1).SetEase(Ease.InQuint).SetUpdate(true).
        OnComplete(() => gameObject.SetActive(false));
    }
}
