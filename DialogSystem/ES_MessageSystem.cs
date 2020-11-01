using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using System.Linq;

namespace RemptyTool.ES_MessageSystem
{
    /// <summary>The messageSystem is made by Rempty EmptyStudio. 
    /// UserFunction
    ///     SetText(string) -> Make the system to print or execute the commands.
    ///     Next()          -> If the system is WaitingForNext, then it will continue the remaining contents.
    ///     AddSpecialCharToFuncMap(string _str, Action _act)   -> You can add your customized special-characters into the function map.
    /// Parameters
    ///     IsCompleted     -> Is the input text parsing completely by the system.
    ///     text            -> The result, witch you can show on your interface as a dialog.
    ///     IsWaitingForNext-> Waiting for user input -> The Next() function.
    ///     textSpeed       -> Setting the updating period of text.
    /// </summary> 
    public class ES_MessageSystem : MonoBehaviour
    {
        public static ES_MessageSystem instance; 

        #region Original param
        public bool IsCompleted { get { return IsMsgCompleted; } }
        public string text { get { return msgText; } }
        public bool IsWaitingForNext { get { return IsWaitingForNextToGo; } }
        public float textSpeed = 0.01f; //Updating period of text. The actual period may not less than deltaTime.

        private const char SPECIAL_CHAR_STAR = '[';
        private const char SPECIAL_CHAR_END = ']';
        private enum SpecialCharType { StartChar, CmdChar, EndChar, NormalChar }
        private bool IsMsgCompleted = true;
        private bool IsOnSpecialChar = false;
        private bool IsWaitingForNextToGo = false;
        private bool IsOnCmdEvent = false;
        private string specialCmd = "";
        private string msgText;
        private char lastChar = ' ';
        private Dictionary<string, Action> specialCharFuncMap = new Dictionary<string, Action>();

        #endregion

        #region Modified param
        private List<string> textList = new List<string>();     //訊息（以行為單位）
        private int textIndex = 0;                              //第幾行
        private string sCurLine;                                //目前行
        private Coroutine curLineTask;                          //目前行的coroutine
        private bool isDoingTextTask = false;
        public bool IsDoingTextTask { get { return isDoingTextTask; } }

        [SerializeField]
        protected Text textMsgBox;                                  //訊息框
        [SerializeField]
        protected Text textNameBox;                                 //名字對話框
        [SerializeField]
        protected Image imgSpeaker;                                 //說話者頭像框
        [SerializeField]
        protected Image imgShow;                                    //要顯示在畫面中央的圖
        [SerializeField]
        protected Transform trDialogueBox;                         //底圖
        private int index;                                      //目前到第幾個字
        
        [SerializeField]
        protected BtnSetting btnSetting;
        private Stack<Tuple<int, int, bool>> stackChoices = new Stack<Tuple<int, int, bool>>();      //用來紀錄選擇處理到的部份
                                                                                                     //可有多層巢狀選項
        private List<Coroutine> lChoicesRoutines = new List<Coroutine>();                            //與上面的stack配合使用的list
        private bool isChoosed = false;                     //是否做出選擇
        private int iChoosed;                               //選中的id

        private AudioSource audiosource;

        [SerializeField]
        protected string sPicturePath;
        [SerializeField]
        protected string sMusicPath;
        

        #endregion

        NewDialogueTrigger trigger = null;

        #region Game loop
        void Start()
        {
            //Register the Keywords Function.
            specialCharFuncMap.Add("w", () => StartCoroutine(CmdFun_w_Task()));
            specialCharFuncMap.Add("r", () => StartCoroutine(CmdFun_r_Task()));
            specialCharFuncMap.Add("l", () => StartCoroutine(CmdFun_l_Task()));
            specialCharFuncMap.Add("lr", () => StartCoroutine(CmdFun_lr_Task()));
            specialCharFuncMap.Add("Music", PlayMusic);
            specialCharFuncMap.Add("/Music", EndMusic);
            specialCharFuncMap.Add("Picture", ShowPicture);
            specialCharFuncMap.Add("/Picture", DisablePicture);
            specialCharFuncMap.Add("Item", GiveItem);
            specialCharFuncMap.Add("QuestComplete", QuestComplete);
            specialCharFuncMap.Add("Say", SetName);
            specialCharFuncMap.Add("Choose", () => StartCoroutine(Choose()));
            specialCharFuncMap.Add("End", EndDialogue);

            if (!trDialogueBox)
            {
                Debug.LogError("Dialog box not set.");
            }
            trDialogueBox.gameObject.SetActive(false);
        }

        private void Awake()
        {
            instance = this;
            //audiosource = this.GetComponent<AudioSource>();
        }

        private void Update()
        {
            UpdateText();
        }

        #endregion

        #region Public Function
        public void AddSpecialCharToFuncMap(string _str, Action _act)
        {
            specialCharFuncMap.Add(_str, _act);
        }
        #endregion

        #region User Function
        //Begin whole task
        public void BeginTextTask(NewDialogueTrigger _trigger, TextAsset _text)
        {
            if (IsDoingTextTask) return;
            ReadTextDataFromAsset(_text);
            trigger = _trigger;
            if (textList.Count != 0)
            {
                //Open dialogue box, check objects
                trDialogueBox.gameObject.SetActive(true);
                if (textMsgBox == null)
                {
                    Debug.LogError("UI text Component not assign.");
                }

                //Set states
                textMsgBox.text = "";
                if (textNameBox) textNameBox.text = "";
                if (imgSpeaker) imgSpeaker.gameObject.SetActive(false);
                if (imgShow) imgShow.gameObject.SetActive(false);
                isDoingTextTask = true;
                IsMsgCompleted = true;
                IsOnSpecialChar = false;
                IsWaitingForNextToGo = false;
                IsOnCmdEvent = false;
                specialCmd = "";
            }
        }
        
        #endregion

        #region Keywords Function
        private IEnumerator CmdFun_l_Task()
        {
            IsOnCmdEvent = true;
            IsWaitingForNextToGo = true;
            yield return new WaitUntil(() => IsWaitingForNextToGo == false);
            IsOnCmdEvent = false;
            yield return null;
        }
        private IEnumerator CmdFun_r_Task()
        {
            IsOnCmdEvent = true;
            msgText += '\n';
            IsOnCmdEvent = false;
            yield return null;
        }
        private IEnumerator CmdFun_w_Task()
        {
            IsOnCmdEvent = true;
            IsWaitingForNextToGo = true;
            yield return new WaitUntil(() => IsWaitingForNextToGo == false);
            msgText = "";   //Erase the messages.
            IsOnCmdEvent = false;
            yield return null;
        }
        private IEnumerator CmdFun_lr_Task()
        {
            IsOnCmdEvent = true;
            IsWaitingForNextToGo = true;
            yield return new WaitUntil(() => IsWaitingForNextToGo == false);
            msgText += '\n';
            IsOnCmdEvent = false;
            yield return null;
        }
        private void PlayMusic()
        {
            string sFileName = GetExtendedCmd();
            //Debug.Log(sMusicPath + sFileName);
            //play music
            if (audiosource) {
                AudioClip audio = Resources.Load<AudioClip>(sMusicPath + sFileName);
                if (audio)
                {
                    audiosource.clip = audio;
                    audiosource.Play();
                }
            }
        }
        private void EndMusic()
        {
            if (audiosource)
            {
                audiosource.Stop();
                audiosource.clip = null;
            }
        }
        private void ShowPicture()
        {
            //name,pos.x,pos.y,size.x,size.y
            string[] sCmd = GetExtendedCmd().Split(',');

            //show picture
            Sprite s = Resources.Load<Sprite>(sPicturePath + sCmd[0]);
            if (s && imgShow)
            {
                imgShow.gameObject.SetActive(true);
                imgShow.sprite = s;
                imgShow.rectTransform.anchoredPosition = new Vector2(int.Parse(sCmd[1]), int.Parse(sCmd[2]));
                imgShow.rectTransform.sizeDelta = new Vector2(int.Parse(sCmd[3]), int.Parse(sCmd[4]));
            }
        }
        private void DisablePicture()
        {
            if (imgShow) imgShow.gameObject.SetActive(false);
        }
        private void GiveItem()
        {
            //string[] sItemInfo = GetExtendedCmd().Split(',');

            //give item
            ItemWorld itemWorld = trigger.GetComponent<ItemWorld>();
            Inventory.instance.AddItem(itemWorld.GetItem());
            itemWorld.DestroySelf();
        }

        // [QuestComplete] quest1, quest2...;
        private void QuestComplete() {
            string[] quests = GetExtendedCmd().Split(',');
            foreach(string quest in quests) {
                QuestManager.instance.MarkQuestComplete(quest);
            }
        }

        //[Say] cmd 設定名字
        private void SetName()
        {
            string sName = GetExtendedCmd();

            //SetName
            if (textNameBox.text != sName)
            {
                textNameBox.text = sName;
                if (imgSpeaker)
                {
                    if (sName != "")
                    {
                        Sprite s = Resources.Load<Sprite>(sPicturePath + sName);
                        if (s) { imgSpeaker.gameObject.SetActive(true); imgSpeaker.sprite = s; }
                        else imgSpeaker.gameObject.SetActive(false);
                    }
                    else imgSpeaker.gameObject.SetActive(false);
                }
            }
        }
        //[Choose] cmd 觸發選擇模式
        private IEnumerator Choose()
        {
            IsOnCmdEvent = true;
            isChoosed = false;

            string[] sChoices = GetExtendedCmd().Split(',');
            Button[] btnChoices = SetChoices(sChoices);
            yield return new WaitUntil(() => isChoosed == true);
            //Debug.Log(iChoosed.ToString());

            EndChoose(btnChoices);
            IsOnCmdEvent = false;
            yield return null;
        }
        //[End] cmd, 結束整個對話
        private void EndDialogue()
        {
            textList.Clear();
            textIndex = 0;
            isDoingTextTask = false;
            if (curLineTask != null) StopCoroutine(curLineTask);
            foreach (Coroutine c in lChoicesRoutines)
            {
                if (c != null) StopCoroutine(c);
            }
            lChoicesRoutines.Clear();
            curLineTask = null;
            stackChoices.Clear();
            //Close Dialogue box
            trDialogueBox.gameObject.SetActive(false);

            if (audiosource)
            {
                audiosource.Stop();
                audiosource.clip = null;
            }
        }

        #endregion

        #region Messages Core
        private void AddChar(char _char)
        {
            msgText += _char;
            lastChar = _char;
        }
        private SpecialCharType CheckSpecialChar(char _char)
        {
            if (_char == SPECIAL_CHAR_STAR)
            {
                if (lastChar == SPECIAL_CHAR_STAR)
                {
                    specialCmd = "";
                    IsOnSpecialChar = false;
                    return SpecialCharType.NormalChar;
                }
                IsOnSpecialChar = true;
                return SpecialCharType.CmdChar;
            }
            else if (_char == SPECIAL_CHAR_END && IsOnSpecialChar)
            {
                //exe cmd!
                if (specialCharFuncMap.ContainsKey(specialCmd))
                {
                    specialCharFuncMap[specialCmd]();
                    //Debug.Log("The keyword : [" + specialCmd + "] execute!");
                }
                else
                    Debug.LogError("The keyword : [" + specialCmd + "] is not exist!");
                specialCmd = "";
                IsOnSpecialChar = false;
                return SpecialCharType.EndChar;
            }
            else if (IsOnSpecialChar)
            {
                specialCmd += _char;
                return SpecialCharType.CmdChar;
            }
            return SpecialCharType.NormalChar;
        }
        //Show line routine
        private IEnumerator SetTextTask()
        {
            IsOnSpecialChar = false;
            IsMsgCompleted = false;
            specialCmd = "";
            for (index = 0; index < sCurLine.Length; index++)
            {
                switch (CheckSpecialChar(sCurLine[index]))
                {
                    case SpecialCharType.NormalChar:
                        AddChar(sCurLine[index]);
                        lastChar = sCurLine[index];
                        yield return new WaitForSeconds(textSpeed);
                        break;
                }
                lastChar = sCurLine[index];
                yield return new WaitUntil(() => IsOnCmdEvent == false);
            }
            IsMsgCompleted = true;
            curLineTask = null;
            yield return null;
        }

        #region update functions
        //Equal to update
        private void UpdateText()
        {
            if (IsDoingTextTask)
            {
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    //Continue the messages, stoping by [w] or [lr] keywords.
                    this.Next();
                }

                //If the message is complete, stop updating text.
                if (this.IsCompleted == false)
                {
                    textMsgBox.text = this.text;
                }

                //Auto update from textList.
                if (this.IsCompleted == true && textIndex < textList.Count)
                {
                    //Debug.Log(stackChoices.Count);
                    if (stackChoices.Count != 0 && lChoicesRoutines[lChoicesRoutines.Count - 1] == null)
                    {
                        lChoicesRoutines[lChoicesRoutines.Count - 1] = StartCoroutine(ExecuteChooseMode());
                    }
                    else if (stackChoices.Count != 0 && lChoicesRoutines[lChoicesRoutines.Count - 1] != null)
                    {

                    }
                    else
                    {
                        this.SetText(textList[textIndex]);
                        textIndex++;
                    }
                }
                else if (this.IsCompleted == true && textIndex >= textList.Count)
                {
                    EndDialogue();
                }
            }
        }
        //[l],[lr],[w]
        private void Next()
        {
            IsWaitingForNextToGo = false;
        }
        //Start a line
        private void SetText(string _text)
        {;
            sCurLine = _text;
            curLineTask = StartCoroutine(SetTextTask());
        }

        #endregion

        //Load given text
        private void ReadTextDataFromAsset(TextAsset _textAsset)
        {
            textList.Clear();
            textList = new List<string>();
            textIndex = 0;
            var lineTextData = _textAsset.text.Split('\n');
            //Debug.Log(lineTextData.Count<string>());
            foreach (string line in lineTextData)
            {
                textList.Add(line);
            }
        }
        //Get extended cmd after ']' and ended with ';'
        private string GetExtendedCmd()
        {
            index++;
            string sExtString = "";
            while (sCurLine[index] != ';')
            {
                sExtString += sCurLine[index];
                if (index >= sCurLine.Length - 1) break;
                index++;
            }
            return sExtString;
        }


        #region Core functions about choices
        private Button[] SetChoices(string[] sChoices)
        {
            Button[] btnChoices = new Button[sChoices.Length];
            for (int i = 0; i < btnChoices.Length; i++)
            {
                btnChoices[i] = Instantiate(btnSetting.btnPrefab, trDialogueBox).GetComponent<Button>();
                btnChoices[i].name = ((stackChoices.Count * 10) + i + 1).ToString();
                btnChoices[i].transform.GetChild(0).GetComponent<Text>().text = sChoices[i];
                btnChoices[i].transform.GetChild(0).GetComponent<Text>().fontSize = btnSetting.iBtnFontSize;
                btnChoices[i].GetComponent<RectTransform>().anchoredPosition = new Vector2(btnSetting.v2BtnSize.x / 2 + btnSetting.v2BtnOffset.x, (btnChoices.Length - i) * (btnSetting.v2BtnSize.y + btnSetting.v2BtnOffset.y));
                btnChoices[i].GetComponent<RectTransform>().sizeDelta = btnSetting.v2BtnSize;
                Button b = btnChoices[i];
                btnChoices[i].onClick.AddListener(() => { BtnCallback(b); });
            }
            btnChoices[0].Select();
            return btnChoices;
        }
        private void EndChoose(Button[] btnChoices)
        {
            stackChoices.Push(new Tuple<int, int, bool>(iChoosed, btnChoices.Length, false));
            Coroutine c = null;
            lChoicesRoutines.Add(c);
            foreach (Button b in btnChoices)
            {
                GameObject.Destroy(b.gameObject);
            }
            msgText = "";
        }
        private void BtnCallback(Button btn)
        {
            iChoosed = int.Parse(btn.name);
            isChoosed = true;
        }
        
        private IEnumerator ExecuteChooseMode()
        {
            int layer = stackChoices.Count;
            int target = stackChoices.Peek().Item1;
            int total = stackChoices.Peek().Item2 + (layer - 1) * 10;
            if (stackChoices.Peek().Item3 == false)
            {
                while(!textList[textIndex].Contains("[" + target.ToString() + "]") && textIndex < textList.Count)
                {
                    textIndex++;
                }
            }
            while (textIndex < textList.Count)
            {
                if (textList[textIndex].Contains("[" + target.ToString() + "]"))
                {
                    string sAlt = textList[textIndex].Substring(textList[textIndex].IndexOf("[" + target.ToString() + "]") + 3);
                    if (sAlt != "") this.SetText(sAlt);
                    yield return new WaitUntil(() => curLineTask == null);
                }
                else if (textList[textIndex].Contains("[/" + target.ToString() + "]"))
                {
                    string sAlt = textList[textIndex].Substring(0, textList[textIndex].IndexOf("[/" + target.ToString() + "]"));
                    if (sAlt != "")  this.SetText(sAlt);
                    yield return new WaitUntil(() => curLineTask == null);
                    break;
                }
                else if (textList[textIndex].Contains("[Choose]"))
                {
                    //Debug.Log("do");
                    this.SetText(textList[textIndex]);
                    yield return new WaitUntil(() => curLineTask == null);
                    yield return new WaitUntil(() => layer == lChoicesRoutines.Count);
                    //Debug.Log("do2");
                    continue;
                }
                else
                {
                    this.SetText(textList[textIndex]);
                    yield return new WaitUntil(() => curLineTask == null);
                }

                textIndex++;
            }
            stackChoices.Pop();
            stackChoices.Push(new Tuple<int, int, bool>(target, total, true));


            if (stackChoices.Peek().Item3 == true && target != total && textIndex < textList.Count)
            {
                while (!textList[textIndex].Contains("[/" + total.ToString() + "]"))
                {
                    textIndex++;
                }
                if (textIndex < textList.Count)
                {
                    textIndex++;
                }
            }
            else if (stackChoices.Peek().Item3 == true && target == total && textIndex < textList.Count) 
            {
                textIndex++;
            }
            lChoicesRoutines[lChoicesRoutines.Count - 1] = null;
            lChoicesRoutines.RemoveAt(lChoicesRoutines.Count - 1);
            stackChoices.Pop();
            //Debug.Log("I am layer " + layer + ", mewwage: " + textList[textIndex]);
            yield return null;
        }
        
        #endregion

        #endregion
    }

    [Serializable]
    public struct BtnSetting
    {
        public GameObject btnPrefab;        //按鈕
        public Vector2 v2BtnSize;           //大小
        public Vector2 v2BtnOffset;         //x的位置和y的間隔
        public int iBtnFontSize;            //字體大小
    }
}
