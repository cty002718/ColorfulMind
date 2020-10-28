using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuestObjectActivator : MonoBehaviour
{
	//public GameObject objectToActivate;

	public string[] questToCheck;
	bool initCheck = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!initCheck) {
        	initCheck = true;
        	if(CheckActivation()) {
        		doSomething();
        	}
        }
    }

    public bool CheckActivation() {
    	bool activation = true;
    	for(int i=0;i<questToCheck.Length;i++)
	    	if(!QuestManager.instance.CheckIfComplete(questToCheck[i]))
	    		activation = false;

	    return activation;
    }

    public void doSomething() {
    	
    }
}
