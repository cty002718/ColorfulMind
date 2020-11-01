using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PianoActivator1 : QuestObjectActivator
{
	public override void OnComplete() {
		gameObject.GetComponent<NewDialogueTrigger>().dialoguePath = "Level1/piano2";
	}
}
