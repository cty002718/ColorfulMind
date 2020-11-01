using RemptyTool.ES_MessageSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewDialogueTrigger : MonoBehaviour
{
    public string dialoguePath;

    public void TriggerDialogue()
    {
        TextAsset textAsset = Resources.Load<TextAsset>("Dialogues/" + dialoguePath);
        ES_MessageSystem.instance.BeginTextTask(this, textAsset);
    }
}
