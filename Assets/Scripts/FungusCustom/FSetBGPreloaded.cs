using UnityEngine;
using System.Collections;

namespace Fungus
{
    [CommandInfo("Ref",
                 "FSetBGPreloaded",
                 "FSetBGPreloaded")]
    public class FSetBGPreloaded : Command
    {
        public int Bg_num;
        public float BlackScreenSpeed;

        GameObject scene;
        BackgroundManager backgroundManager;

        void Start()
        {
            //if (BlackScreenSpeed == 0)
            //    BlackScreenSpeed = 1.5f;

            scene = this.transform.parent.gameObject;
            backgroundManager = GameObject.Find("BackGroundManager").GetComponent<BackgroundManager>();
        }
        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
           // scene.GetComponent<Player>().IncreaseIndex(1);
           // scene.GetComponent<Player>().CurrentBG = Bg_num;

            yield return StartCoroutine(backgroundManager.IBlackFadeBackground(Bg_num, BlackScreenSpeed));

            Continue();
        }
        public override Color GetButtonColor()
        {
            return new Color32(184, 94, 39, 255);
        }

    }
}