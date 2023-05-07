using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fungus;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
public class SpriteRemover : MonoBehaviour
{
    public static SpriteRemover instance = null;
    private void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }
    }
   
    public IEnumerator RemoveSprite(string characterName, float speed, bool extend, bool skip)
    {
        SpriteFade.instance.StopSpritesFading();
        SpriteMove.instance.StopSpriteMoving();
        SpriteController.instance.SkipSpriteActions();

        int spriteNum = SpriteController.instance.GetSpriteByName(characterName);

        GameObject Current = SpriteController.instance.GetSprite(spriteNum);
        GameObject Face1 = Current.transform.GetChild(0).gameObject;

        SpriteController.instance.GameSpriteData[spriteNum].name = null;

        Addressables.Release(SpriteController.instance.GameSpriteData[spriteNum].handles[0]);

        StartCoroutine(SpriteFade.instance.ISetFadingSprite(Current, false, speed, skip));
        yield return StartCoroutine(SpriteFade.instance.ISetFadingSprite(Face1, false, speed, skip));

        Addressables.Release(SpriteController.instance.GameSpriteData[spriteNum].handles[1]);
       
        PackageConntector.instance.DisconnectPackage(characterName);
    }
}
