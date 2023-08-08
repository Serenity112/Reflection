using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Fungus
{
    [CommandInfo("Ref",
                   "FStartGuiAnim",
                   "FStartGuiAnim")]
    public class FStartGuiAnim : Command
    {
        [SerializeField]
        private GameObject gameGuiPanel;

        [SerializeField]
        private GameObject gameButtons;

        [SerializeField]
        private GameObject ButtonBlockingOverlay;

        public void Awake()
        {
            
        }

        public override void OnEnter()
        {
            ButtonBlockingOverlay.SetActive(true);
            gameGuiPanel.GetComponent<CanvasGroup>().alpha = 0f;
            gameButtons.GetComponent<CanvasGroup>().alpha = 0f;
            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            yield return new WaitForSeconds(1.5f);

            yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
            {
                FadeManager.FadeObject(gameButtons, true, 4f),
                FadeManager.FadeObject(gameGuiPanel, true, 4f),

            }));

            yield return new WaitForSeconds(0.5f);

            ButtonBlockingOverlay.SetActive(false);

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(255, 102, 102, 255);
        }
    }
}