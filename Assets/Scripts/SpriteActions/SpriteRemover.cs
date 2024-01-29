using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteRemover : MonoBehaviour
{
    public static SpriteRemover instance = null;

    private void Awake()
    {
        instance = this;
    }

    public IEnumerator RemoveSprite(Character character, float time, bool skip, bool release, bool waitForFinished)
    {
        GameSpriteObject? sprite_obj = SpriteController.instance.GetSpriteByCharacter(character);
        if (sprite_obj == null)
        {
            yield break;
        }
        GameSpriteObject sprite = (GameSpriteObject)sprite_obj;

        sprite.CompletePostAction();
        SpriteController.instance.CompleteAnimations(sprite.Number);
        SpriteController.instance.LoadSpriteActualData(character);

        List<IEnumerator> fade_enumerators = new List<IEnumerator>
        {
            SpriteFade.instance.IFadeSprite(sprite.ByPart(SpritePart.Body), time, 0f, skip),
            SpriteFade.instance.IFadeSprite(sprite.ByPart(SpritePart.Face1), time, 0f, skip)
        };
        SpriteController.instance.AddAnimation(sprite.Number, fade_enumerators);

        if (waitForFinished || skip)
        {
            yield return StartCoroutine(IFade());
        }
        else
        {
            StartCoroutine(IFade());
        }

        IEnumerator IFade()
        {
            yield return StartCoroutine(WaitForAll(fade_enumerators));

            yield return StartCoroutine(WaitForAll(new List<IEnumerator>()
            {
                sprite.ReleaseHandler(SpritePart.Body),
                sprite.ReleaseHandler(SpritePart.Face1),
            }));

            // Remove
            if (release)
            {
                SpriteController.instance.ClearSpriteData(character);
                SpriteController.instance.ClearSpriteData(sprite.Number);
                PackageConntector.instance.DisconnectPackageGroup(character);
            }
            // Hide
            else
            {
                SpriteController.instance.SaveSpriteAlpha(character, 0f);
            }
        }

        yield return null;
    }

    private IEnumerator WaitForAll(List<IEnumerator> coroutines)
    {
        int tally = 0;

        foreach (IEnumerator c in coroutines)
        {
            StartCoroutine(RunCoroutine(c));
        }

        while (tally > 0)
        {
            yield return null;
        }

        IEnumerator RunCoroutine(IEnumerator c)
        {
            tally++;
            yield return StartCoroutine(c);
            tally--;
        }
    }
}
