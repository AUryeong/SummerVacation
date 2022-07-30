using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : Projectile
{
    public float speed;
    Coroutine coroutine;

    void Start()
    {
        coroutine = StartCoroutine(DestroyArrow());
    }
    void OnEnable()
    {
        isHitable = true;
    }

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);
        transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.y);
    }

    IEnumerator DestroyArrow()
    {
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
    }
    public override void OnHit(Enemy enemy)
    {
        gameObject.SetActive(false);
        if (coroutine == null)
            StopCoroutine(coroutine);
    }
    public override float GetDamage(float damage)
    {
        return damage * 1.2f;
    }
}
