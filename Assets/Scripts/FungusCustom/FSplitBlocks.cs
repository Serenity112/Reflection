using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

namespace Fungus
{
    [CommandInfo("Ref",
                 "FSplitBlocks",
                 "FSplitBlocks")]
    public class FSplitBlocks : Command
    {
        public string ChoiseName;
        //0 - 0oprion
        //1 - 1option
        public string[] BlocksRedirection;

        public GameObject ChoiseManager;
        GameObject SayDialogCanvas;

        public override void OnEnter()
        {
            SayDialogCanvas = GameObject.Find("SayDialogCanvas");
            ChoiseManager.GetComponent<ChoiseManager>().SplitBlocks(ChoiseName, BlocksRedirection);
            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(210, 170, 130, 255);
        }

    }
}