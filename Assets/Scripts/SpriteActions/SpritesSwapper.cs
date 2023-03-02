using System.Collections;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Fungus;
public class SpritesSwapper : MonoBehaviour
{
    public static SpritesSwapper instance = null;

    void Start()
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

    public IEnumerator SwapSprites(string spriteName, int Pose, int Emotion, Vector3 newPosition, float disappearSpeed, float appearSpeed, float moveSpeed, bool skip)
    {
        SpriteController.instance.printData();
        SpriteMove.instance.StopSpriteMoving();
        SpriteFade.instance.StopSpritesFading();
        //StopAllCoroutines();
        SpriteController.instance.SkipSpriteActions();

        int sprite1 = SpriteController.instance.GetActivityByName(spriteName);

        bool ChangePose = SpriteController.instance.GameSpriteData[sprite1].pose != Pose;

        if (ChangePose)
        {
            int sprite2 = SpriteController.instance.GetNewActivity(spriteName);

            // При вызове LoadSpriteByParts ищется спрайт по имени чтобы занести данные о нем в массив. Но надо находить уже новый спрайт.
            // Поэтому 1й делается "невидимым", ставя имя null. А потом уже чистится
            SpriteController.instance.GameSpriteData[sprite1].name = null;
            SpriteController.instance.GameSpriteData[sprite2].prevSprite = sprite1;

            SpriteController.instance.SaveSpriteData(sprite1, 0f);
            SpriteController.instance.SaveSpriteData(sprite2, spriteName, Pose, Emotion);
            SpriteController.instance.SaveSpriteData(sprite2, 1f);

            GameObject Current1 = SpriteController.instance.GetSprite(sprite1);
            GameObject Current2 = SpriteController.instance.GetSprite(sprite2);

            GameObject Face1_1 = Current1.transform.GetChild(0).gameObject;
            GameObject Face1_2 = Current2.transform.GetChild(0).gameObject;

            if (SpriteController.instance.GameSpriteData[sprite1].expanded)
            {
                SpriteController.instance.SaveSpriteData(sprite2, true);
                SpriteController.instance.SetNewScale(Current2, spriteName, SpriteExpand.instance.expand_coefficient);
            }
            else
            {
                SpriteController.instance.SetDefaultScale(Current2, spriteName);
            }

            Current2.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
            Face1_2.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
            Current2.transform.localPosition = Current1.transform.localPosition;

            yield return StartCoroutine(SpriteController.instance.LoadSpriteByParts(Current2, spriteName, Pose, Emotion));

            if (newPosition == Vector3.zero) //if new pos = 0 0 0 => sprite stand still
            {
                SpriteController.instance.SaveSpriteData(sprite2, Current1.transform.localPosition);
            }
            else
            {
                SpriteController.instance.SaveSpriteData(sprite2, newPosition);

                SpriteMove.instance.SetMovementSprites(sprite1, newPosition, moveSpeed, 1, skip);
                SpriteMove.instance.SetMovementSprites(sprite2, newPosition, moveSpeed, 1, skip);
            }

            StartCoroutine(SpriteFade.instance.ISetFadingSprite(Current1, false, disappearSpeed, skip));
            StartCoroutine(SpriteFade.instance.ISetFadingSprite(Face1_1, false, disappearSpeed, skip));

            StartCoroutine(SpriteFade.instance.ISetFadingSprite(Current2, true, appearSpeed, skip));
            yield return StartCoroutine(SpriteFade.instance.ISetFadingSprite(Face1_2, true, appearSpeed, skip));

           /* SpriteFade.instance.SetFadingSprite(Current1, false, disappearSpeed, skip);
            SpriteFade.instance.SetFadingSprite(Face1_1, false, disappearSpeed, skip);

            SpriteFade.instance.SetFadingSprite(Current2, true, appearSpeed, skip);
            SpriteFade.instance.SetFadingSprite(Face1_2, true, appearSpeed, skip);*/

            Addressables.Release(SpriteController.instance.GameSpriteData[sprite1].handles[0]);
            Addressables.Release(SpriteController.instance.GameSpriteData[sprite1].handles[1]);
            Resources.UnloadUnusedAssets();

            SpriteController.instance.GameSpriteData[sprite2].prevSprite = -1;
            SpriteController.instance.DelActivity(sprite1);

            DialogMod.denyNextDialog = false;
        } else
        {
            SpriteController.instance.SaveSpriteData(sprite1, spriteName, Pose, Emotion);

            GameObject Current = SpriteController.instance.GetSprite(sprite1);
            GameObject Face1 = Current.transform.GetChild(0).gameObject;
            GameObject Face2 = Current.transform.GetChild(1).gameObject;
            //spritecontroller.SetScale(Current, NewSpriteName);

            //Используем 2й handle как буффер
            SpriteController.instance.GameSpriteData[sprite1].handles[2] = SpriteController.instance.GameSpriteData[sprite1].handles[1];

            Face2.GetComponent<SpriteRenderer>().sprite = Face1.GetComponent<SpriteRenderer>().sprite;
            Face2.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            Face1.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);

            yield return StartCoroutine(SpriteController.instance.ILoadSpriteOfSpecificObject(Face1, spriteName, Pose, Emotion, SpritePart.FACE1));

            if (newPosition != Vector3.zero) //if new pos = 0 0 0 => sprite stand still
            {
                SpriteController.instance.SaveSpriteData(sprite1, newPosition);
                SpriteMove.instance.SetMovementSprites(sprite1, newPosition, moveSpeed, 1, skip);
            }

            Addressables.Release(SpriteController.instance.GameSpriteData[sprite1].handles[2]);

            SpriteFade.instance.SetFadingSprite(Face2, false, appearSpeed, skip);
            SpriteFade.instance.SetFadingSprite(Face1, true, disappearSpeed, skip);

            DialogMod.denyNextDialog = false;
        }
    }
}