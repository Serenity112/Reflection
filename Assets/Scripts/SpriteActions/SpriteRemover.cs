using System.Collections;
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

        int spriteNum = SpriteController.instance.GetSpriteByName(characterName);

        GameObject Current = SpriteController.instance.GetSprite(spriteNum);
        GameObject Face1 = Current.transform.GetChild(0).gameObject;

        StartCoroutine(SpriteFade.instance.ISetFadingSprite(Current, false, speed, skip));
        yield return StartCoroutine(SpriteFade.instance.ISetFadingSprite(Face1, false, speed, skip));

        Addressables.ReleaseInstance(SpriteController.instance.GameSpriteData[spriteNum].handles[0]);
        Addressables.ReleaseInstance(SpriteController.instance.GameSpriteData[spriteNum].handles[1]);

        SpriteController.instance.DelActivity(spriteNum);
        PackageConntector.instance.DisconnectPackage(characterName);
    }
}
