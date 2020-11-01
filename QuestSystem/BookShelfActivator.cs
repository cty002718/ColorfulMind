using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BookShelfActivator : QuestObjectActivator
{


	public override void OnComplete() {
		gameObject.GetComponent<NewDialogueTrigger>().dialoguePath = "Level1/bookShelf2";
		HeroController.instance.inventory.AddItem(new Item { itemType = Item.ItemType.Heart, amount = 1 });
		HeroController.instance.inventory.RemoveItem(new Item {itemType = Item.ItemType.Book, amount = 1});
	}
}
