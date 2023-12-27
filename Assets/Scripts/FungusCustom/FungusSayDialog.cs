using System.Collections;
using UnityEngine;
using System;

namespace Fungus
{
    [CommandInfo("Ref",
                 "FungusSayDialog",
                 "FungusSayDialog")]
    [AddComponentMenu("")]
    [ExecuteInEditMode]

    public class FungusSayDialog : Command
    {
        [SerializeField]
        private string storyText = string.Empty;

        [SerializeField]
        private string speaker = string.Empty;

        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        IEnumerator IOnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            var parsed = Enum.TryParse<global::Character>(speaker, true, out global::Character character);
            var character_input = parsed ? character : global::Character.None;

            yield return StartCoroutine(Typewriter.Instance.ISayDialog(storyText, character_input));

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
            return storyText;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }
    }
}
