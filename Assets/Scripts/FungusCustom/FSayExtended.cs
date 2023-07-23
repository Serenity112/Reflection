using System.Collections;
using UnityEngine;
using System;

namespace Fungus
{
    [CommandInfo("Ref",
                 "FSayExtended",
                 "FSayExtended")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]

    public class FSayExtended : Command
    {
        [SerializeField]
        private string prevText = string.Empty;

        [SerializeField]
        private string extendedText = string.Empty;

        [SerializeField]
        private string speaker = null;

        public string saveString;

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        IEnumerator IOnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            SpriteExpand.instance.StopPrev();
            SpriteExpand.instance.SetExpanding(speaker, Typewriter.Instance.skipping);

             LogManager.instance.NewMessageExtended(extendedText, speaker);

            NameChanger.instance.SetName(speaker);

            saveString = UserData.instance.CurrentBlock + "_" + UserData.instance.CurrentCommandIndex;

            if (ES3.KeyExists(saveString, "DialogSaves.es3"))
            {
                Typewriter.Instance.wasCurrentDialogRead = true;
            }
            else
            {
                Typewriter.Instance.wasCurrentDialogRead = false;

                ES3.Save<bool>(saveString, true, "DialogSaves.es3");
            }

            yield return StartCoroutine(Typewriter.Instance.SayExtend(extendedText, prevText));

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
            return extendedText;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }
    }
}
