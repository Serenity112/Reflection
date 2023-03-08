using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


namespace Fungus
{
    [CommandInfo("Ref",
                 "FDialogSequence",
                 "Set dialog sequence")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]

    public class FDialogSequence : Command
    {
        [SerializeField]
        private int beg = 0;

        [SerializeField]
        private int end = 0;

        private string speaker = null;

        public string storyText = "";

        private bool extendPrevious = false;

        public string saveString;

        public bool wasRead;

        [Tooltip("Voiceover audio to play when writing the text")]
        private AudioClip voiceOverClip;

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

        public virtual bool ExtendPrevious { get { return extendPrevious; } }

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        IEnumerator IOnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            if (UserData.instance.indexToLoad != -1)
            {
                beg = UserData.instance.indexToLoad;

                if(DaysManager.instance.daySentences[beg].extend)
                {
                    beg--;
                }

                UserData.instance.indexToLoad = -1;
            }

            for (int currentDialog = beg; currentDialog <= end; currentDialog++)
            {
                SentenceData sentenceData = DaysManager.instance.daySentences[currentDialog];

                storyText = sentenceData.phrase;
                storyText = extendPrevious ? " " + storyText : storyText;
                speaker = sentenceData.speaker;
                extendPrevious = sentenceData.extend;

                UserData.instance.CurrentDialogIndex = currentDialog;

                SpriteExpand.instance.StopPrev();
                SpriteExpand.instance.SetExpanding(speaker, DialogMod.skipping);

                //LogManager.instance.NewMessage(storyText, speaker);

                //nameChanger.SetName(speaker);

                saveString = UserData.instance.CurrentBlock + "_" + UserData.instance.CurrentCommandIndex + "_" + currentDialog;

                if (ES3.KeyExists(saveString, "DialogSaves.es3"))
                {
                    DialogMod.wasCurrentDialogRead = true;
                }
                else
                {
                    DialogMod.wasCurrentDialogRead = false;

                    ES3.Save<bool>(saveString, true, "DialogSaves.es3");
                }


                var sayDialog = SayDialog.GetSayDialog();
                sayDialog.SetActive(true);

                var flowchart = GetFlowchart();

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

                yield return StartCoroutine(sayDialog.DoSay(subbedText, !extendPrevious, waitForClick, fadeWhenDone, stopVoiceover, waitForVO1, voiceOverClip, delegate { }));
            }

            Continue();
        }

        public override string GetName()
        {
            Type t = typeof(FungusSayDialog);
            CommandInfoAttribute MyAttribute =
                (CommandInfoAttribute)Attribute.GetCustomAttribute(t, typeof(CommandInfoAttribute));
            return MyAttribute.CommandName;
        }

        public override string GetSummary()
        {
            if (storyText == "")
                return beg + "-" + end + "\"";
            else
                return storyText;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        public override void OnStopExecuting()
        {
            StopAllCoroutines();

            var sayDialog = SayDialog.GetSayDialog();
            if (sayDialog == null)
            {
                return;
            }

            sayDialog.Stop();
        }
    }
}