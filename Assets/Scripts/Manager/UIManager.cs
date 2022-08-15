using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

public class UIManager : Singleton<UIManager>
{

    #region ���� �κ��丮 ����
    [SerializeField] GridLayoutGroup inventoryLayoutGroup;
    protected RectTransform inventoryRectTransform;

    [SerializeField] ItemInven itemInventoryImage;
    protected List<ItemInven> itemInventories = new List<ItemInven>();

    private float itemInventoryFadeInPosX = -160f;
    private float itemInventoryFadeOutPosX = -56f;
    private float itemInventoryFadeDuration = 0.3f;

    private bool inventoryOpen = true;
    #endregion

    #region ���� ȹ�� ����
    [SerializeField] GameObject levelUpUI;
    [SerializeField] TMP_Text levelUpText;

    [SerializeField] RectTransform itemSlotsParent;
    [SerializeField] ItemSlot[] itemSlots;

    [SerializeField] ParticleSystem itemSlotParticle;

    public Sprite deselectSlotSprite;
    public Sprite selectSlotSprite;

    private Vector3 itemSlotsParentPos = new Vector3(1280, 0, 0);
    private float levelUpUIAppearDuration = 0.5f;

    private int itemSlotChooseIdx = 0;
    private bool itemSlotActiviting = false;

    private float deselctItemSlotMovePosX = 1500;
    private float selctItemSlotMovePosX = -2000;
    private float ItemSlotMoveDuration = 1;

    private float levelUpTextSinPower = 15;
    private float levelUpTextSinAdder = 0.5f;
    #endregion

    #region Ÿ�̸� ����
    [SerializeField] TextMeshProUGUI timerText;
    private float timer;
    #endregion

    #region ���� ����
    [SerializeField] private Image lvBarImage;
    [SerializeField] private TextMeshProUGUI lvBarText;
    #endregion

    public override void OnReset()
    {
        levelUpUI.gameObject.SetActive(false);
        itemSlotActiviting = false;

        inventoryOpen = true;
        itemInventories.Clear();
        inventoryRectTransform = inventoryLayoutGroup.GetComponent<RectTransform>();

        inventoryRectTransform.DOAnchorPosX(itemInventoryFadeOutPosX, itemInventoryFadeDuration);
        inventoryRectTransform.DOScaleX(1, itemInventoryFadeDuration);

        timer = 0;

        UpdateLevel();
    }

    private void Update()
    {
        if (!GameManager.Instance.isGaming)
            return;
        CheckInventoryUI();
        if (levelUpUI.gameObject.activeSelf)
            UpdateLevelUpUI();
        UpdateTimer();
    }

    public bool IsActable()
    {
        return !levelUpUI.gameObject.activeSelf && Time.timeScale != 0 && GameManager.Instance.isGaming;
    }

    #region ���� �κ��丮 �Լ�
    private void CheckInventoryUI()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            if (inventoryOpen)
            {
                inventoryRectTransform.DOAnchorPosX(itemInventoryFadeInPosX, itemInventoryFadeDuration).SetUpdate(true);
                inventoryRectTransform.DOScaleX(0, itemInventoryFadeDuration).SetUpdate(true);
            }
            else
            {
                inventoryRectTransform.DOAnchorPosX(itemInventoryFadeOutPosX, itemInventoryFadeDuration).SetUpdate(true);
                inventoryRectTransform.DOScaleX(1, itemInventoryFadeDuration).SetUpdate(true);
            }
            inventoryOpen = !inventoryOpen;
        }
    }

    public void UpdateItemInven(Item item)
    {
        ItemInven itemInventory = itemInventories.Find((ItemInven x) => x.gameObject.activeSelf && x.item == item);
        if (itemInventory == null)
        {
            itemInventory = PoolManager.Instance.Init(itemInventoryImage.gameObject).GetComponent<ItemInven>();
            itemInventory.rectTransform.SetParent(itemInventoryImage.rectTransform.parent);
            itemInventory.rectTransform.anchoredPosition3D = Vector3.zero;
            itemInventory.rectTransform.localScale = Vector3.one;
            itemInventories.Add(itemInventory);
        }
        int itemInventoryCount = itemInventories.FindAll((ItemInven x) => x.gameObject.activeSelf).Count;
        if (itemInventoryCount <= 3)
        {
            inventoryLayoutGroup.constraintCount = itemInventoryCount;
        }
        itemInventory.item = item;
    }
    #endregion

    #region ���� ȹ�� �Լ�

    private void UpdateLevelUpUI()
    {
        if (itemSlotActiviting)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                MoveUpChooseSlot();
            else if (Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                MoveDownChooseSlot();
            else if (Input.GetKeyDown(KeyCode.Z) || Input.GetKeyDown(KeyCode.Return))
                EndChooseItem(itemSlots[itemSlotChooseIdx]);
        }
        itemSlotParticle.transform.position = itemSlots[itemSlotChooseIdx].rectTransform.position;
        TMPUpdate();
    }

    private void TMPUpdate()
    {
        levelUpText.ForceMeshUpdate();
        TMP_TextInfo textInfo = levelUpText.textInfo;
        for (int i = 0; i < textInfo.characterCount; ++i)
        {
            TMP_CharacterInfo charInfo = textInfo.characterInfo[i];

            if (!charInfo.isVisible)
                continue;

            Vector3[] vectors = textInfo.meshInfo[charInfo.materialReferenceIndex].vertices;
            for (int j = 0; j < 4; ++j)
            {
                var vector = vectors[charInfo.vertexIndex + j];
                vectors[charInfo.vertexIndex + j] = vector + new Vector3(0, Mathf.Sin(Time.unscaledTime + i * levelUpTextSinAdder) * levelUpTextSinPower, 0);
            }
        }
        for (int i = 0; i < textInfo.meshInfo.Length; ++i)
        {
            var meshinfo = textInfo.meshInfo[i];
            meshinfo.mesh.vertices = meshinfo.vertices;
            levelUpText.UpdateGeometry(meshinfo.mesh, i);
        }
    }

    public void StartChooseItem(List<Item> items)
    {
        itemSlotActiviting = true;
        levelUpUI.gameObject.SetActive(true);
        Time.timeScale = 0;

        for (int i = 0; i < items.Count; i++)
            itemSlots[i].item = items[i];
        itemSlotChooseIdx = 0;
        UpdateChooseSlot();

        foreach (ItemSlot itemSlot in itemSlots)
            itemSlot.rectTransform.anchoredPosition = Vector2.zero;
        itemSlotsParent.anchoredPosition = itemSlotsParentPos;

        itemSlotsParent.DOAnchorPosX(0, levelUpUIAppearDuration).SetEase(Ease.OutBack).SetUpdate(true);
    }

    private void MoveUpChooseSlot()
    {
        itemSlotChooseIdx--;
        if (itemSlotChooseIdx < 0)
            itemSlotChooseIdx += itemSlots.Length;
        UpdateChooseSlot();
    }

    private void MoveDownChooseSlot()
    {
        itemSlotChooseIdx++;
        if (itemSlotChooseIdx >= itemSlots.Length)
            itemSlotChooseIdx -= itemSlots.Length;
        UpdateChooseSlot();
    }

    public void UpdateChooseSlot()
    {
        for (int i = 0; i < itemSlots.Length; i++)
            if (i == itemSlotChooseIdx)
                itemSlots[i].SlotSelect();
            else
                itemSlots[i].SlotDeselect();
    }

    public void EndChooseItem(ItemSlot chooseItemSlot)
    {
        itemSlotActiviting = false;

        foreach (ItemSlot itemSlot in itemSlots)
            if (chooseItemSlot != itemSlot)
                itemSlot.rectTransform.DOAnchorPosX(deselctItemSlotMovePosX, ItemSlotMoveDuration).SetEase(Ease.OutSine).SetUpdate(true);
        chooseItemSlot.rectTransform.DOAnchorPosX(selctItemSlotMovePosX, ItemSlotMoveDuration).SetEase(Ease.InBack).SetUpdate(true).
            OnComplete(() =>
            {
                Player.Instance.AddItem(chooseItemSlot.item);

                levelUpUI.gameObject.SetActive(false);
                Time.timeScale = 1;
            });
    }
    #endregion

    #region Ÿ�̸� �Լ�
    private void UpdateTimer()
    {
        timer += Time.deltaTime;
        int timerInt = (int)timer;
        timerText.text = (timerInt / 60).ToString("D2") + " : " + (timerInt % 60).ToString("D2");
    }
    #endregion

    #region ���� �Լ�
    public void UpdateLevel()
    {
        lvBarImage.DOFillAmount(Player.Instance.Exp / Player.Instance.maxExp, 0.3f).SetUpdate(true);
        lvBarText.text = "LV: " + Player.Instance.lv;
    }
    #endregion
}
