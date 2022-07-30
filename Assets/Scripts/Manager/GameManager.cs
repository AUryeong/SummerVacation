using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance
    {
        get;
        private set;
    }
    public List<Enemy> inCameraEnemies = new List<Enemy>();
    [SerializeField]
    GameObject damageText;
    void Start()
    {
        Instance = this;
    }

    void Update()
    {
        Camera.main.transform.position = Player.Instance.transform.position - new Vector3(0, 0, 100);
    }

    public GameObject GetProjectile(string projectileName)
    {
        return PoolManager.Instance.Init(Resources.Load<GameObject>("Projectile/" + projectileName));
    }

    public void ShowText(string text, Vector3 pos, Color color)
    {
        GameObject damageTextObj = PoolManager.Instance.Init(damageText);
        TextMeshPro textMesh = damageTextObj.GetComponent<TextMeshPro>();
        textMesh.text = text;
        textMesh.color = new Color(color.r, color.g, color.b, 0);
        textMesh.DOFade(1, 0.2f).SetEase(Ease.InBack).
        OnComplete(() => textMesh.DOFade(0, 0.5f).SetEase(Ease.InBack));
        damageTextObj.transform.position = pos;
        damageTextObj.transform.DOMoveX(damageTextObj.transform.position.x + 0.7f, 0.7f);
        damageTextObj.transform.DOMoveY(damageTextObj.transform.position.y + 1f, 0.7f).SetEase(Ease.OutBack).
        OnComplete(() => damageTextObj.SetActive(false));
    }
    public void ShowDamage(int damage, Vector3 pos, Color color)
    {
        ShowText(damage.ToString(), pos, color);
    }
}
