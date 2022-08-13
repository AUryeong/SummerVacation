using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourcesManager : Singleton<ResourcesManager>
{

    private bool isWritingResources = false;
    private Dictionary<string, GameObject> projectiles = new Dictionary<string, GameObject>();
    private Dictionary<string, Sprite> itemIcons = new Dictionary<string, Sprite>();
    public Dictionary<string, Item> items = new Dictionary<string, Item>();

    public override void OnReset()
    {
        if(!isWritingResources)
            ReadResource();
    }

    public void ReadResource()
    {
        foreach (var projectile in Resources.LoadAll<GameObject>("Projectile"))
            projectiles.Add(projectile.name, projectile);

        foreach (var icon in Resources.LoadAll<Sprite>("Item/Icon"))
            itemIcons.Add(icon.name, icon);

        ReadItem();

        isWritingResources = true;
    }

    private void ReadItem()
    {
        foreach (string line in Resources.Load<TextAsset>("Item/ItemList").text.Split('\n'))
        {
            string[] texts = line.Split(',');
            if (texts[0] == "코드 아이템 이름")
                continue;
            Item item = System.Activator.CreateInstance(System.Type.GetType("Item_" + texts[0])) as Item;
            int upgradeMaxCount = int.Parse(texts[2]);
            string[] lore = new string[upgradeMaxCount+1];
            for (int i = 0; i <= upgradeMaxCount; i++)
            {
                if (string.IsNullOrEmpty(texts[i + 3]))
                    break;
                lore[i] = texts[i + 3];
            }
            item.Init(texts[1], lore, upgradeMaxCount, GetItemIcon(texts[0]));
            items.Add(texts[0], item);
        }
    }

    public Sprite GetItemIcon(string spriteName)
    {
        if (!itemIcons.ContainsKey(spriteName))
        {
            Debug.LogAssertion("Sprite not found : " + spriteName);
            return null;
        }
        return itemIcons[spriteName];
    }

    public GameObject GetProjectile(string projectileName)
    {
        if (!projectiles.ContainsKey(projectileName))
        {
            Debug.LogAssertion("Projectile not found : " + projectileName);
            return null;
        }
        return PoolManager.Instance.Init(projectiles[projectileName]);
    }
}
