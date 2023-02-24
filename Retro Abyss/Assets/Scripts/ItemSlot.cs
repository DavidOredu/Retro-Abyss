using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ItemSlot : MonoBehaviour
{
    public TextMeshProUGUI itemNameText;
    public TextMeshProUGUI itemDescriptionText;
    public Image itemImage;
    public Transform availabilityImagesParent;
    public List<Image> availabilityImages = new List<Image>();
    public TextMeshProUGUI costText;
    public Button itemButton;
    public ShopItem itemData;
}
