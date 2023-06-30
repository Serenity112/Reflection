using System.Collections;
using UnityEngine;
using System;

namespace Fungus
{
    [CommandInfo("Ref",
                 "FungusSayDialog",
                 "Sets a game object in the scene to be active / inactive.")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]

    public class FungusSayDialog : Command
    {
        [SerializeField]
        private string storyText = "";

        [SerializeField]
        private string speaker = null;

        [SerializeField]
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

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        IEnumerator IOnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            SpriteExpand.instance.StopPrev();
            SpriteExpand.instance.SetExpanding(speaker, DialogMod.skipping);
            
            LogManager.instance.NewMessage(storyText, speaker);

            NameChanger.instance.SetName(speaker);

            saveString = UserData.instance.CurrentBlock + "_" + UserData.instance.CurrentCommandIndex;

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

        public override void OnStopExecuting()
        {
            var sayDialog = SayDialog.GetSayDialog();
            if (sayDialog == null)
            {
                return;
            }

            sayDialog.Stop();
        }
    }
}
