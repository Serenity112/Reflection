using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Fungus
{
    [CommandInfo("Ref",
                 "ChoiseDialog",
                 "ChoiseDialog")]
    public class ChoiseDialog : Command
    {
        public string ChoiseName;
        public string[] Messages;
        public string[] BlocksRedirection;
        public GameObject ChoiseManager;
        GameObject SayDialogCanvas;

        public override void OnEnter()
        {
            SayDialogCanvas = GameObject.Find("SayDialogCanvas");
            //SayDialogCanvas.GetComponent<DialogMod>().DenySkip();
            ChoiseManager.GetComponent<ChoiseManager>().GenerateChoise(Messages, BlocksRedirection, ChoiseName);

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(210, 170, 130, 255);
        }

    }
}