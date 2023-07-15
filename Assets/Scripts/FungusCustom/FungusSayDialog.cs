using System.Collections;
using UnityEngine;
using System;
using ChristinaCreatesGames.Typography.Typewriter;

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
                Typewriter.Instance.wasCurrentDialogRead = true;
            }
            else
            {
                Typewriter.Instance.wasCurrentDialogRead = true;

                ES3.Save<bool>(saveString, true, "DialogSaves.es3");
            }

            yield return StartCoroutine(Typewriter.Instance.Say(storyText, extendPrevious));
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
    }
}
