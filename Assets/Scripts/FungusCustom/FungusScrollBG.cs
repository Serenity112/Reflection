using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System;
namespace Fungus
{
    [CommandInfo("Ref",
                 "FungusScrollBG",
                 "FungusScrollBG")]
    public class FungusScrollBG : Command
    {
        public int Bg_layer;
        public float ScrollSpeed;
        public float Ycoord;
        GameObject scene;
        GameObject BackGroundManager;
        GameObject bg_current;
        RectTransform rt;
        private void Start()
        {
           // scene = this.transform.parent.gameObject;
            //BackGroundManager = scene.transform.Find("BackGroundManager").gameObject;
           // bg_current = BackGroundManager.GetComponent<ChangeBG>().GetByLayer(Bg_layer);
            //rt = bg_current.GetComponent<RectTransform>();
        }
        public override void OnEnter()
        {
            StartCoroutine(IOnEnter());
        }

        IEnumerator IOnEnter()
        {     
            yield return StartCoroutine(MoveSprite(new Vector3(rt.localPosition.x, rt.localPosition.y - 539, rt.localPosition.z), ScrollSpeed, 1f, false));
            Continue();
            
        }
        IEnumerator MoveSprite(Vector3 vect, float smoothTime, float vExtend, bool skip)
        {
            Vector3 velocity = Vector3.zero;
            Vector3 Evect = new Vector3(vect.x, vExtend*vect.y, vect.z);

            while (bg_current.transform.localPosition != Evect)
            {
                Debug.Log("Scroll");
                if ((Math.Abs(Math.Abs(bg_current.transform.localPosition.y) - Math.Abs(vect.y)) < 0.001) || skip)
                {
                    Debug.Log("Break!");
                    bg_current.transform.localPosition = vect;
                    yield break;
                }

                bg_current.transform.localPosition = Vector3.SmoothDamp(bg_current.transform.localPosition, Evect, ref velocity, smoothTime);
                yield return null;
            }
            yield break;
        }

        public override Color GetButtonColor()
        {
            return new Color32(200, 130, 40, 255);
        }

    }
}