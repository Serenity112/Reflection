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

            yield return StartCoroutine(Typewriter.Instance.ISayExtend(prevText, extendedText, character));

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
