using System.Collections;
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
            ButtonBlockingOverlay.SetActive(true);
            gameGuiPanel.GetComponent<CanvasGroup>().alpha = 0f;
            gameButtons.GetComponent<CanvasGroup>().alpha = 0f;
        }
        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        private IEnumerator IOnEnter()
        {
            UserData.instance.CurrentCommandIndex += 1;

            yield return new WaitForSeconds(0.5f);

            StartCoroutine(FadeManager.FadeObject(gameButtons, true, 4f));
            yield return StartCoroutine(FadeManager.FadeObject(gameGuiPanel, true, 4f));

            yield return new WaitForSeconds(0.33f);

            ButtonBlockingOverlay.SetActive(false);

            Continue();
        }

        public override Color GetButtonColor()
        {
            return new Color32(255, 102, 102, 255);
        }
    }
}