using UnityEngine;
using System.Collections;

namespace Fungus
{
    [CommandInfo("Ref",
                 "FLightningFlash",
                 "FLightningFlash")]
    public class FLightningFlash : Command
    {
        public LightningManager lightningmanager;
        public float disappearSpeed;

        private void Start()
        {
            lightningmanager = GameObject.Find("LightningManager").GetComponent<LightningManager>();
        }

        public override void OnEnter()
        {
           StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            yield return StartCoroutine(lightningmanager.ISummonScreenLightning(disappearSpeed));
            Continue();
        }         
        public override Color GetButtonColor()
        {
            return new Color32(100, 170, 50, 255);
        }

    }
}