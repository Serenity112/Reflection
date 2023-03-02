using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Fungus
{
    [CommandInfo("Ref",
                 "BlackScreen",
                 "BlackScreen")]
    public class BlackScreen : Command
    {
        public bool FadeIn;
        public float Speed;
        public GameObject BlackPanel;
        GameObject scene;

        public override void OnEnter()
        {
            if(Speed == 0)
                Speed = 1.5f;
            scene = this.transform.parent.gameObject;
            Player.IncreaseIndex(1);
            StartCoroutine(Fade(FadeIn, Speed));   
        }

        IEnumerator Fade(bool fadein, float speed)
        {
            BlackPanel.SetActive(true);

            if (fadein)
            {
                BlackPanel.GetComponent<CanvasGroup>().alpha = 0;
                int color = 1;
                for (float i = 0; i <= color + 0.05; i += speed * Time.deltaTime)
                {
                    BlackPanel.GetComponent<CanvasGroup>().alpha = i;
                    yield return null;
                }
                Continue();
            }
            else
            {
                BlackPanel.GetComponent<CanvasGroup>().alpha = 1;
                Continue();
                int color = 0;
                for (float i = 1; i >= color - 0.05; i -= speed * Time.deltaTime)
                {
                    BlackPanel.GetComponent<CanvasGroup>().alpha = i;
                    yield return null;
                }
                BlackPanel.SetActive(false);           
            }
            
        }

        public override Color GetButtonColor()
        {
            return new Color32(200, 200, 40, 255);
        }

    }
}