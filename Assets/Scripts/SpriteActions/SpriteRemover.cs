using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

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

    public IEnumerator RemoveSprite(string characterName, float speed, bool skip)
    {
        SpriteController.instance.printData();

        SpriteFade.instance.StopSpritesFading();
        SpriteMove.instance.StopSpriteMoving();
        SpriteController.instance.SkipSpriteActions();

        GameSpriteObject? sprite_obj = SpriteController.instance.GetSpriteNumByName(characterName);

        if (sprite_obj == null)
        {
            yield break;
        }

        GameSpriteObject sprite = (GameSpriteObject)sprite_obj;

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            SpriteFade.instance.ISetFadingSprite(sprite.ByPart(SpritePart.Body), false, speed, skip),
            SpriteFade.instance.ISetFadingSprite(sprite.ByPart(SpritePart.Face1), false, speed, skip)
        }));

        Addressables.Release(SpriteController.instance.GameSprites[sprite.num].Handlers[0]);
        Addressables.Release(SpriteController.instance.GameSprites[sprite.num].Handlers[1]);

        SpriteController.instance.DelActivity(sprite.num);
        PackageConntector.instance.DisconnectPackage(characterName);
    }
}
