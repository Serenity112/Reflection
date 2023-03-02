using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


namespace Fungus
{
    [CommandInfo("Flow",
                 "FungusSayDialog",
                 "Sets a game object in the scene to be active / inactive.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]

    public class FungusSayDialog : Command
    {
        public string speaker = "nob";

        [TextArea(5, 10)]
        [SerializeField] protected string storyText = "";

        [Tooltip("Type this text in the previous dialog box.")]
        public bool extendPrevious = false;

        [Tooltip("Voiceover audio to play when writing the text")]
        private AudioClip voiceOverClip;

        [Tooltip("Always show this Say text when the command is executed multiple times")]
        private bool showAlways = true;

        [Tooltip("Number of times to show this Say text when the command is executed multiple times")]
        private int showCount = 1;

        [Tooltip("Fade out the dialog box when writing has finished and not waiting for input.")]
        public bool fadeWhenDone = false;

        [Tooltip("Wait for player to click before continuing.")]
        private bool waitForClick = true;

        [Tooltip("Stop playing voiceover when text finishes writing.")]
        private bool stopVoiceover = true;

        [Tooltip("Wait for the Voice Over to complete before continuing")]
        private bool waitForVO1 = false;

        [Tooltip("Sets the active Say dialog with a reference to a Say Dialog object in the scene. All story text will now display using this Say Dialog.")]
        protected SayDialog setSayDialog;

        protected int executionCount;

        #region Public members

        public virtual bool ExtendPrevious { get { return extendPrevious; } }

        public string saveString;

        public bool wasRead;

        GameObject SayDialogCanvas;
        SpriteExpand spriteExpand;

        GameObject scene;

       // GameObject Game;

        void ChangeName(int i)
        {
           // SayDialogCanvas.GetComponent<NameChanger>().SetName(i);
        }

        void Awake()
        {
            //fadeWhenDone = false;


            //if (!wasRead)
            //{
            //    string k = InsertNum.ToString();
            //    wasRead = true;
            //    //Debug.Log(k);

            //    GameObject scene = this.transform.parent.gameObject;
            //    scene.GetComponent<DaysManager>().LoadDay(1);


            //    storyText = scene.GetComponent<DaysManager>().CurrentDay[k][0];

            //    speaker = scene.GetComponent<DaysManager>().CurrentDay[k][1];

            //    switch (scene.GetComponent<DaysManager>().CurrentDay[k][2])
            //    {
            //        case "n":
            //            extendPrevious = false;
            //            break;
            //        case "y":
            //            extendPrevious = true;
            //            break;
            //    }
            //}


        }

        void Start()
        {
            SayDialogCanvas = GameObject.Find("SayDialogCanvas");

            //Game = GameObject.Find("Game");
        }

        public override void OnEnter()
        {
            
            spriteExpand = GameObject.Find("SpriteManager").GetComponent<SpriteExpand>();
            // GameObject CharacterFader = scene.transform.Find("CharacterFader").gameObject;
            // GameObject CharacterAnimator = scene.transform.Find("CharacterAnimator").gameObject;
            //Debug.Log("Player's Parent: " + this.transform.parent.name);
            // string key = storyText;




            //CHAT_LOG_________________

           // GameObject chatlog = SayDialogCanvas.transform.GetChild(0).gameObject; // 0 объект тут это ChatLog

            //Debug.Log("Calling CHAT_LOG for story text " + storyText + " and speaker " + speaker);
            //chatlog.GetComponent<LogManager>().CreateMessage(storyText, speaker);


            //StopPrev нужны, если игрок быстро прокликивает анимации, чтобы старые автоматом заканчивались
            //spriteExpand.StopPrev();
            //spriteExpand.SetExpanding(speaker, 0.2f);
            
            switch (extendPrevious)
            {
                case false:                 
                    break;
                case true:
                    storyText = " " + storyText;
                    break;
            }

            switch (speaker) //Speaker
            {
                case "kat":
                    {
                        ChangeName(0);
                        break;
                    }
                case "nas":
                    {
                        ChangeName(1);
                        break;
                    }
                case "eve":
                    {
                        ChangeName(2);
                        break;
                    }
                case "tan":
                    {
                        ChangeName(3);
                        break;
                    }
                case "pas":
                    {
                        ChangeName(4);
                        break;
                    }
                case "ser":
                    {
                        ChangeName(5);
                        break;
                    }
                case "dev":
                    {
                        ChangeName(6);
                        break;
                    }
                case "nob":
                    {
                        //NameChanger.DelName();
                        break;
                    }
                default:
                    {
                        //NameChanger.DelName();
                        break;
                    }
            }

            if (!extendPrevious)
            {
                Player.IncreaseIndex(1);
            }

            saveString = Player.CurrentBlock + Player.CurrentCommandIndex;

            if (ES3.KeyExists(saveString, "DialogSaves.es3"))
            {
                //wasRead = ES3.Load<bool>(saveString);
                //UnityEngine.Debug.Log("Сейв с именем " + saveString + " существует");
                //SayDialogCanvas.GetComponent<DialogMod>().currentDialogSkip = true;
            }
            else 
            {
                //UnityEngine.Debug.Log("Создаётся новый сейв с именем " + saveString);
                //SayDialogCanvas.GetComponent<DialogMod>().currentDialogSkip = false;
                ES3.Save<bool>(saveString, true, "DialogSaves.es3");
            }
       
            if (!showAlways && executionCount >= showCount)
            {
                Continue();
                return;
            }

            executionCount++;

            if (setSayDialog != null)
            {
                SayDialog.ActiveSayDialog = setSayDialog;
            }

            var sayDialog = SayDialog.GetSayDialog();
            if (sayDialog == null)
            {
                Continue();
                return;
            }

            var flowchart = GetFlowchart();

            sayDialog.SetActive(true);

            string displayText = storyText;

            var activeCustomTags = CustomTag.activeCustomTags;
            for (int i = 0; i < activeCustomTags.Count; i++)
            {
                var ct = activeCustomTags[i];
                displayText = displayText.Replace(ct.TagStartSymbol, ct.ReplaceTagStartWith);
                if (ct.TagEndSymbol != "" && ct.ReplaceTagEndWith != "")
                {
                    displayText = displayText.Replace(ct.TagEndSymbol, ct.ReplaceTagEndWith);
                }
            }

            string subbedText = flowchart.SubstituteVariables(displayText);

            sayDialog.Say(subbedText, !extendPrevious, waitForClick, fadeWhenDone, stopVoiceover, waitForVO1, voiceOverClip, delegate 
            {

                Continue();
            });
        }


        public override string GetName()
        {
            Type t = typeof(FungusSayDialog);
            CommandInfoAttribute MyAttribute =
                (CommandInfoAttribute)Attribute.GetCustomAttribute(t, typeof(CommandInfoAttribute));
            return MyAttribute.CommandName;
        }

        //public override int SetPasteRange()
        //{
        //    string pasterange = storyText;
        //    if (pasterange.Contains("-"))
        //    {
        //        return 0;
        //    } else
        //    {
        //        return 0;
        //    }
        //}

        public override string GetSummary()
        {
            string namePrefix = "";
            if (extendPrevious)
            {
                namePrefix = "EXTEND" + ": ";
            }
            return namePrefix + "\"" + storyText + "\"";
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        public override void OnReset()
        {
            executionCount = 0;
        }

        public override void OnStopExecuting()
        {
            var sayDialog = SayDialog.GetSayDialog();
            if (sayDialog == null)
            {
                return;
            }

            sayDialog.Stop();
        }

        #endregion

        #region ILocalizable implementation

        public virtual string GetStandardText()
        {
            return storyText;
        }

        public virtual void SetStandardText(string standardText)
        {
            storyText = standardText;
        }



        public virtual string GetStringId()
        {
            // String id for Say commands is SAY.<Localization Id>.<Command id>.[Character Name]
            string stringId = "SAY." + GetFlowchartLocalizationId() + "." + itemId + ".";
            return stringId;
        }
        #endregion
    }
}