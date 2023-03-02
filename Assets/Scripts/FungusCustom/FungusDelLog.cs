using UnityEngine;
using System.Collections;

namespace Fungus
{
    [CommandInfo("Ref",
                 "FungusDelLog",
                 "FungusDelLog")]
    public class FungusDelLog : Command
    {
        public override void OnEnter()
        {
            GameObject log = GameObject.Find("ChatLog");
            log.GetComponent<LogManager>().DelLog();
            Continue();
        }

        //private IEnumerator IOnEnter()
        //{
        //    GameObject log = GameObject.Find("ChatLog");
        //    yield return log.GetComponent<LogManager>().DelLog();
        //    Continue();
        //}
        public override Color GetButtonColor()
        {
            return new Color32(200, 130, 200, 255);
        }
    }
}