using DG.Tweening.Core.Easing;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public enum ItemType
{
    Spinner,
    Hook,
    Bumper,
    Tank,
    Delete
}

[System.Serializable]
public class ItemUpgrade
{
    public ItemType itemType;
    public PlacableItem[] itemPrefabs;
    public int level;

    public System.Type GetItemClass()
    {
        switch (itemType)
        {
            case ItemType.Spinner:
                return typeof(SpinnerScript);
            case ItemType.Hook:
                return typeof(HookTrigger);
            case ItemType.Bumper:
                return typeof(BumperScript);
            case ItemType.Tank:
                return typeof(Watertank);
            default:
                return null;
        }
    }
}

public class UpgradeManager : MonoBehaviour
{
    public ItemUpgrade[] upgrades;
    private Dictionary<ItemType, ItemUpgrade> itemUpgrades = new Dictionary<ItemType, ItemUpgrade>();

    private void Awake()
    {
        foreach (ItemUpgrade upgrade in upgrades)
        {
            int level = 0;
            foreach (PlacableItem pi in upgrade.itemPrefabs)
            {
                pi.upgradeLevel = level;
                level++;
            }
            itemUpgrades.Add(upgrade.itemType, upgrade);
        }
    }

    public void UpgradeItem(ItemType itemType)
    {
        ItemUpgrade upgrade = GetItemUpgrade(itemType);
        if (upgrade == null) return;

        if (upgrade.level < upgrade.itemPrefabs.Length)
        {
            upgrade.level++;

            if(upgrade.itemType == ItemType.Tank)
            {
                Watertank nT = Instantiate(upgrade.itemPrefabs[upgrade.level], Vector3.zero, Quaternion.identity).GetComponent<Watertank>();
                GameManager.Instance.currentTank.SwapTanks(nT);
                FindAnyObjectByType<UIManager>().SetNewStoreButton(nT.storeButton);
            }
            else
            {
                System.Type targetType = upgrade.GetItemClass(); // Change this to any class

                foreach (Component t in GameManager.Instance.currentTank.moveableObjectsParent.GetComponentsInChildren(targetType, true))
                {
                    Vector3 pos = t.transform.position;
                    Quaternion rot = t.transform.rotation;
                    Destroy(t.transform.parent.gameObject);
                    PlacableItem nHook = Instantiate(upgrade.itemPrefabs[upgrade.level], pos, rot, GameManager.Instance.currentTank.moveableObjectsParent);

                }
            }

            UpdateStoreWithUpgrades();
        }
    }

    private ItemUpgrade GetItemUpgrade(ItemType itemType)
    {
        itemUpgrades.TryGetValue(itemType, out var upgrade);
        return upgrade;
    }

    private void UpdateStoreWithUpgrades()
    {
        foreach(DraggableUpgrade du in FindObjectsByType<DraggableUpgrade>(FindObjectsSortMode.None))
        {
            if(itemUpgrades.TryGetValue(du.itemType, out var upgrade))
            {
                du.itemPrefab = upgrade.itemPrefabs[upgrade.level];
                du.GetComponent<DraggableUpgrade>().itemPrefab = upgrade.itemPrefabs[upgrade.level];

            }  
        }
    }

    public int GetCurrentLevel(ItemType itemType)
    {
        ItemUpgrade upgrade = GetItemUpgrade(itemType);
        if (upgrade == null)
        {
            Debug.LogError("Looked for item with no upgrades " + itemType);
            return -1;
        }
        return upgrade.level;
    }

    public int GetMaxLevel(ItemType itemType)
    {
        ItemUpgrade upgrade = GetItemUpgrade(itemType);
        if (upgrade == null)
        {
            Debug.LogError("Looked for item with no upgrades " + itemType);
            return -1;
        }
        return upgrade.itemPrefabs.Length-1;
    }

}
