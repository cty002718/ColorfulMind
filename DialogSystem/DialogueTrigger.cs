using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
	public enum TriggerType 
    {
        NPC,
        item,
    }

    public TriggerType triggerType;
	public string dialoguePath;
	bool dialogueLoaded = false;

    public void RunDialogue() {
    	if(!dialogueLoaded) {
    		DialogueManager.instance.loadDialogue(dialoguePath);
    		dialogueLoaded = true;
    	}

    	dialogueLoaded = DialogueManager.instance.printLine(this);
    }
}
