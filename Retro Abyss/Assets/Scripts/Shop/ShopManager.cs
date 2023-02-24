using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ShopManager : SingletonDontDestroy<ShopManager>
{
    [SerializeField]
    private List<GameObject> itemSlotPrefabs = new List<GameObject>();
    [SerializeField]
    private GameObject availabilityIconPrefab;

    [SerializeField]
    private List<Transform> itemLists = new List<Transform>();

    private List<List<ItemSlot>> allShopItems = new List<List<ItemSlot>>();

    public static event Action<ShopItem> PurchaseDecriptionHandler;
    public static event Action<ShopItem> OnPurchaseHandler;
    // Start is called before the first frame update
    void Start()
    {
        List<ShopItem[]> itemsData = new List<ShopItem[]>();

        var upgradeItemsData = Resources.LoadAll<ShopItem>("ShopItems/Upgrades");
        var powerupItemsData = Resources.LoadAll<ShopItem>("ShopItems/Powerups");
        var shopItemsData = Resources.LoadAll<ShopItem>("ShopItems/Shop");

        itemsData.Add(upgradeItemsData);
        itemsData.Add(powerupItemsData);
        itemsData.Add(shopItemsData);

        for (int i = 0; i < itemsData.Count; i++)
        {
            List<ItemSlot> newItemSlots = new List<ItemSlot>();
            for (int j = 0; j < itemsData[i].Length; j++)
            {
                var item = itemsData[i][j];

                GameObject itemSlotObj = Instantiate(itemSlotPrefabs[i], itemLists[i]);
                ItemSlot itemSlotScript = itemSlotObj.GetComponent<ItemSlot>();

                PurchaseDecriptionHandler?.Invoke(item);

                UpdateShopUI(itemSlotScript, item);

                newItemSlots.Add(itemSlotScript);
            }
            allShopItems.Add(newItemSlots);
        }

        AssignShopButtons();
        UpdateShopButtons();

    }
    private void AssignShopButtons()
    {
        foreach (var itemtype in allShopItems)
        {
            foreach (var itemSlot in itemtype)
            {
                itemSlot.itemButton.onClick.AddListener(() => PurchaseButton(itemSlot));
            }
        }
    }
    public void UpdateShopButtons()
    {
        for (int i = 0; i < allShopItems.Count; i++)
        {
            for (int j = 0; j < allShopItems[i].Count; j++)
            {
                CheckPurcahseButtonUsability(instance.allShopItems[i][j]);
            }
        }
    }
    public void CheckPurcahseButtonUsability(ItemSlot itemSlot)
    {
        if (GameManager.instance.GetCurrency() < itemSlot.itemData.cost || itemSlot.itemData.level >= itemSlot.itemData.maxLevel)
        {
            itemSlot.itemButton.interactable = false;
            return;
        }
        else
            itemSlot.itemButton.interactable = true;
    }
    private void PurchaseButton(ItemSlot itemSlot)
    {
        // Update cost
        itemSlot.itemData.level++;
        // reduce our currency
        GameManager.instance.UseCurrency(itemSlot.itemData.cost);

        OnPurchaseHandler?.Invoke(itemSlot.itemData);
        PurchaseDecriptionHandler?.Invoke(itemSlot.itemData);
        // Update UI
        UpdateShopUI(itemSlot, itemSlot.itemData);
    }
    private void UpdateShopUI(ItemSlot itemSlot, ShopItem itemData)
    {
        itemSlot.itemNameText.text = itemData._name;
        itemSlot.itemDescriptionText.text = itemData.description + " " + itemData.purchaseDescription;

        if(!itemData._name.Contains("Bundle"))
            itemData.cost = Mathf.CeilToInt(itemData.priceAtLevel.Evaluate(itemData.level));
        
        itemSlot.costText.text = itemData.cost.ToString("N0");
        if (itemData.level >= itemData.maxLevel && !itemData._name.Contains("Bundle"))
            itemSlot.costText.text = "Sold Out!";

        if (itemData.level <= 0)
            itemData.isUnlocked = false;
        else
            itemData.isUnlocked = true;

        if (itemSlot.itemImage != null)
            itemSlot.itemImage.sprite = itemData.image;

        if (itemSlot.availabilityImages.Count <= 0)
        {
            for (int x = 0; x < itemData.maxLevel; x++)
            {
                var availabilityIconGO = Instantiate(availabilityIconPrefab, itemSlot.availabilityImagesParent);
                itemSlot.availabilityImages.Add(availabilityIconGO.transform.Find("Image").GetComponent<Image>());
            }
        }

        if (itemSlot.availabilityImages.Count > 0)
        {
            for (int x = 0; x < itemSlot.availabilityImages.Count; x++)
                itemSlot.availabilityImages[x].enabled = false;
            for (int x = 0; x < itemData.level; x++)
                itemSlot.availabilityImages[x].enabled = true;
        }
        itemSlot.itemData = itemData;
    }
    private void UpgradeShopItem()
    {
        
    }
    public bool CheckShopItemWithNameExists(string itemName)
    {
        for (int i = 0; i < allShopItems.Count; i++)
        {
            for (int j = 0; j < allShopItems[i].Count; j++)
            {
                var itemSlot = allShopItems[i][j];
                if (itemSlot.itemData._name == itemName)
                    return true;
            }
        }
        return false;
    }
    public ShopItem GetShopItemWithName(string itemName)
    {
        for (int i = 0; i < allShopItems.Count; i++)
        {
            for (int j = 0; j < allShopItems[i].Count; j++)
            {
                var itemSlot = allShopItems[i][j];
                if (itemSlot.itemData._name == itemName)
                    return itemSlot.itemData;
            }
        }
        return null;
    }
}
