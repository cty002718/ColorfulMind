using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestManager : MonoBehaviour
{
	public string[] questNames;
	public bool[] questComplete;

	public static QuestManager instance;

	// Start is called before the first frame update
    void Start()
    {
    	instance = this;
    	questComplete = new bool[questNames.Length];    
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateQuestObjects();
    }

    public int GetQuestNumber(string questToFind) {
    	for(int i=0;i<questNames.Length;i++) {
    		if(questNames[i] == questToFind) {
    			return i;
    		}
    	}
    	return 0;
    }

    public bool CheckIfComplete(string questToCheck) {
    	if(GetQuestNumber(questToCheck) != 0) {
    		return questComplete[GetQuestNumber(questToCheck)];
    	}
    	return false;
    }

    public void MarkQuestComplete(string questToMark) {
    	questComplete[GetQuestNumber(questToMark)] = true;
    	UpdateQuestObjects();
    }

    public void MarkQuestInComplete(string questToMark) {
    	questComplete[GetQuestNumber(questToMark)] = false;
    	UpdateQuestObjects();
    }

    public void UpdateQuestObjects() {
    	QuestObjectActivator[] questObjects = FindObjectsOfType<QuestObjectActivator>();

    	for(int i=0;i<questObjects.Length;i++) {
    		if(!questObjects[i].isActive && questObjects[i].CheckActivation()) {
    			questObjects[i].OnComplete();
                questObjects[i].isActive = true;
    		}
    	}
    }
}
