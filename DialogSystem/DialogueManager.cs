using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LitJson;

public class DialogueManager : MonoBehaviour
{
	public static DialogueManager instance;
    JsonData dialogue;
    JsonData currentLayer;
    int index;
    string speaker;
	public Text sentence;
	public Animator animator;
	bool inDialogue;
	string currentSentence;

	public GameObject[] buttons;

	void Start() {
		deactivateButtons();
		instance = this;
	}

    public bool loadDialogue(string path) {
    	if(!inDialogue) {
	    	index = 0;
	        var jsonTextFile = Resources.Load<TextAsset>("Dialogues/JSON/" + path);
	        dialogue = JsonMapper.ToObject(jsonTextFile.text);
	        currentLayer = dialogue;
	        inDialogue = true;
	        return true;
    	}
    	return false;
        
    }

    public bool printLine(DialogueTrigger trigger) {
    	if(inDialogue) {

    		animator.SetBool("isOpen", true);
	    	JsonData line = currentLayer[index];
	    	speaker = "";
	    	foreach(JsonData key in line.Keys) {
	    		speaker = key.ToString();
	    	}

	    	StopAllCoroutines();
	    	if(speaker == "!") {
	    		speaker = "";
	    		ItemWorld itemWorld = trigger.GetComponent<ItemWorld>();
	    		itemWorld.canTake = true;
	    	}
	    	if(speaker == "EOD"){  		
	    		sentence.text = "";
	    		animator.SetBool("isOpen", false);
    			HeroController.instance.dialogue_open = false;
	    		inDialogue = false;
	    		if(trigger.triggerType == DialogueTrigger.TriggerType.item) {
	    			ItemWorld itemWorld = trigger.GetComponent<ItemWorld>();
	    			if(itemWorld.canTake == true) {
	    				Inventory.instance.AddItem(itemWorld.GetItem());
	    				itemWorld.DestroySelf();
	    			}
	    		}
	    		return false;
	    	} else if(speaker == "?") {
	    		JsonData options = line[0];
	    		for(int optionsNumber = options.Count - 1; optionsNumber >= 0; optionsNumber--) {
	    			activateButton(buttons[optionsNumber], options[optionsNumber]);
	    			buttons[optionsNumber].GetComponent<Button>().Select();
	    			buttons[optionsNumber].GetComponent<Button>().OnSelect(null);
	    		}
	    	} else {
	    		string printString = speaker == "" ? line[0].ToString() : speaker + "\n" + line[0].ToString();
    			StartCoroutine(TypeSentence(printString));
	        	index++;
	    	}
    	}

    	return true;
    	
    }

    IEnumerator TypeSentence(string sentenceText) {
    	sentence.text = "";
    	foreach(char letter in sentenceText.ToCharArray()) {
    		sentence.text += letter;
    		yield return null;
    	}
    }

    void deactivateButtons(){
    	foreach(GameObject button in buttons) {
    		button.SetActive(false);
    		button.GetComponentInChildren<Text>().text = "";
    		button.GetComponent<Button>().onClick.RemoveAllListeners();
    	}
    }

    void activateButton(GameObject button, JsonData choice) {
    	button.GetComponent<Button>().Select();
	    button.GetComponent<Button>().OnSelect(null);
    	button.SetActive(true);
    	button.GetComponentInChildren<Text>().text = choice[0][0].ToString();
    	button.GetComponent<Button>().onClick.AddListener(delegate { toDoOnClick(choice);});
    }

    void toDoOnClick(JsonData choice) {
    	currentLayer = choice[0];
    	index = 1;
    	//printLine();
    	deactivateButtons();
    }



}
