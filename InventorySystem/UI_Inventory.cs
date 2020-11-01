using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UI_Inventory : MonoBehaviour
{
    public static UI_Inventory instance;
    Inventory inventory;
    Transform itemSlotContainer;
    Transform itemSlotTemplate;
    Transform descriptionContainer;
    Button firstButton;

    void Awake()
    {
        instance = this;
        itemSlotContainer = transform.Find("itemSlotContainer");
        descriptionContainer = transform.Find("description");
        itemSlotTemplate = itemSlotContainer.Find("itemSlotTemplate");
        gameObject.SetActive(false);
    }
    

    public void SetInventory(Inventory inventory)
    {
        this.inventory = inventory;

        inventory.OnItemListChanged += Inventory_OnItemListChanged;
        RefreshInventoryItems();
    }

    public void SelectFirstButton() {
        if (inventory.GetItemList().Count != 0)
        {
            firstButton.Select();
            firstButton.OnSelect(null);
        }
    }

    private void Inventory_OnItemListChanged(object sender, System.EventArgs e)
    {
        RefreshInventoryItems();
    }

    void RefreshInventoryItems()
    {
        foreach(Transform child in itemSlotContainer)
        {
            if (child == itemSlotTemplate) continue;
            Destroy(child.gameObject);
        }
        int x = 0;
        int y = 0;
        float itemSlotCellSize = 90f;
        bool first = true;
        foreach(Item item in inventory.GetItemList())
        {
            RectTransform itemSlotRectTransform = Instantiate(itemSlotTemplate, itemSlotContainer).GetComponent<RectTransform>();
            Button button = itemSlotRectTransform.GetComponent<Button>();
            if(item.itemController != null) {
                button.onClick.AddListener(delegate {item.itemController.UseItem();});
            }
            itemSlotRectTransform.gameObject.SetActive(true);
            itemSlotRectTransform.anchoredPosition = new Vector2(x * itemSlotCellSize, -y * itemSlotCellSize);
            Image image = itemSlotRectTransform.Find("Image").GetComponent<Image>();
            image.sprite = item.GetSprite();
            TextMeshProUGUI text = itemSlotRectTransform.Find("text").GetComponent<TextMeshProUGUI>();
            if (item.amount > 1)
                text.SetText(item.amount.ToString());
            else
                text.SetText("");
            x++;
            if(x > 3)
            {
                x = 0;
                y++;
            }
 
            if(first) {
                firstButton = button;
                first = false;
            }

        }
    }
     
}
