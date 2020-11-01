using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoActivator2 : QuestObjectActivator
{
	public override void OnComplete() {
		gameObject.GetComponent<NewDialogueTrigger>().dialoguePath = "Level1/piano3";
		HeroController.instance.inventory.AddItem(new Item { itemType = Item.ItemType.Heart, amount = 1 });
		// play the music	
	}
}

