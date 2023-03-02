using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Fungus
{
    [CommandInfo("Ref",
                 "LoadSprite",
                 "LoadSprite")]
    public class LoadSprite : Command
    {
        public string NewCharacterName;
        public int Pose;
        public int Emotion;
        public int HandlerNum;
        public bool Face1;
        public Vector3 Position;

        GameObject scene;
        GameObject CharacterFader;
        SpriteController spritecontroller;

        void Start()
        {
            scene = this.transform.parent.gameObject;
            CharacterFader = scene.transform.Find("CharacterFader").gameObject;
            spritecontroller = scene.transform.Find("SpriteManager").gameObject.GetComponent<SpriteController>();
        }
        public override void OnEnter()
        {
         StartCoroutine(IOnEnter());   
        }
        private IEnumerator IOnEnter()
        {
            int Sprite = 1;//spritecontroller.GetNewActivity();
            GameObject Current = spritecontroller.GetSprite(Sprite);
            if (Face1)
            {
                Current = Current.transform.GetChild(0).gameObject;
            }
            Current.transform.localPosition = Position;
            Current.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);

            string AssetName = NewCharacterName + "_" + Pose + "_" + Emotion;


            spritecontroller.GameSpriteData[spritecontroller.GetActivityByName(NewCharacterName) - 1].handles[HandlerNum] = Addressables.LoadAssetAsync<Sprite>(AssetName);

            yield return spritecontroller.GameSpriteData[spritecontroller.GetActivityByName(NewCharacterName) - 1].handles[HandlerNum];

            if (spritecontroller.GameSpriteData[spritecontroller.GetActivityByName(NewCharacterName) - 1].handles[HandlerNum].Status == AsyncOperationStatus.Succeeded)

            {
                Current.GetComponent<SpriteRenderer>().sprite = spritecontroller.GameSpriteData[spritecontroller.GetActivityByName(NewCharacterName) - 1].handles[HandlerNum].Result;
            }
            else
            {
                Debug.Log("Error loading asset with adress" + AssetName);
            }

            Continue();
        }
    }

}
