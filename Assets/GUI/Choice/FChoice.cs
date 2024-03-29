using System.Collections;
using UnityEngine;
using static ChoiceManager;

namespace Fungus
{
    [CommandInfo("Ref",
                 "FChoice",
                 "FChoice")]
    public class FChoice : Command
    {
        [SerializeField]
        private string ChoiceName;

        [SerializeField]
        private ChoiceArr[] Choices;

        public override void OnEnter()
        {
            

            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            yield return ChoiceManager.instance.CreateChoice(Choices, ChoiceName);
        }

        public override Color GetButtonColor()
        {
            return new Color32(222, 110, 200, 255);
        }
    }
}
