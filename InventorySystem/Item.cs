using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Item
{
    public enum ItemType 
    {
        Book,
        Heart,
    }

    public ItemType itemType;
    public int amount;
    public string description;
    public ItemController itemController;

    public void SetController() {
        switch(itemType) {
            case ItemType.Book: itemController = new BookController(); break;
            default: itemController = null; break;
        }
    }

    public string GetDialoguePath() {
        switch(itemType) {
            default:
            case ItemType.Book: return ItemAssets.Instance.bookDialoguePath;

        }
    }
    public Sprite GetSprite() {
        switch(itemType)
        {
            default:
            case ItemType.Book: return ItemAssets.Instance.bookSprite;   
            case ItemType.Heart: return ItemAssets.Instance.heartSprite;    
        }
    }

    public bool IsStackable()
    {
        switch(itemType)
        {
            default:
                return true;
        }
    }
}
