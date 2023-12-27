using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRemover : MonoBehaviour
{
    public static SpriteRemover instance = null;

    private void Awake()
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

    public IEnumerator RemoveSprite(Character character, float speed, bool skip, bool release)
    {
        SpriteFade.instance.StopSpritesFading();
        SpriteMove.instance.StopSpriteMoving();
        SpriteController.instance.LoadSpritesDataInfo();

        GameSpriteObject? sprite_obj = SpriteController.instance.GetSpriteByName(character);

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

        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            sprite.ReleaseHandler(SpritePart.Body),
            sprite.ReleaseHandler(SpritePart.Face1),
        }));

        if (release)
        {
            SpriteController.instance.ClearSpriteData(sprite.num);
            PackageConntector.instance.DisconnectPackageGroup(character);
            SpriteController.instance.SaveSpriteDataPreloaded(sprite.num, false);
        }
        else
        {
            SpriteController.instance.SaveSpriteData(sprite.num, 0f);
        }

        yield return null;
    }
}
