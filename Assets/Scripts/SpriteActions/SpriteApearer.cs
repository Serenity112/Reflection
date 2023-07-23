using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteApearer : MonoBehaviour
{
    public static SpriteApearer instance = null;

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

    public IEnumerator SpriteAppear(string characterName, int pose, int emotion, Vector3 position, float speed, bool skip)
    {
        int newSpriteNum = SpriteController.instance.GetAvaliableSpriteNum(characterName);

        if (newSpriteNum != -1)
        {
            SpriteMove.instance.StopSpriteMoving();
            SpriteFade.instance.StopSpritesFading();
            SpriteController.instance.SkipSpriteActions();

            SpriteController.instance.SaveSpriteData(newSpriteNum, characterName, pose, emotion, position, 1f, false);

            GameObject Sprite = SpriteController.instance.GetSprite(newSpriteNum);
            GameObject Face1 = Sprite.transform.GetChild(0).gameObject;
            GameObject Face2 = Sprite.transform.GetChild(1).gameObject;

            Sprite.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
            Face1.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
            Face2.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);

            SpriteController.instance.SetDefaultScale(Sprite, characterName);

            Sprite.transform.localPosition = position;

            yield return StartCoroutine(SpriteController.instance.LoadSpriteByParts(Sprite, newSpriteNum, characterName, pose, emotion));

            // Можно добавить опционпльность синк/асинк появления, но вроде синк лучше всегда

            if (skip)
            {
                Sprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                Face1.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
                yield return null;
            }
            else
            {
                yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
                {
                    FadeManager.ColorAlphaFadeObject(Sprite, true, speed),
                    FadeManager.ColorAlphaFadeObject(Face1, true, speed)
                }));
            }
        }
        else
        {
            yield return null;
        }
    }
}
