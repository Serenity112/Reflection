using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// Представляет собой игровой объект и его части для упрощения доступа к ним
public struct GameSpriteObject
{
    public int Number;
    private AsyncOperationHandle<Sprite>[] Handlers;
    private Dictionary<SpritePart, GameObject> Parts;
    private Action PostAction;

    public GameSpriteObject(int num, GameObject spriteObject)
    {
        this.Number = num;

        Parts = new Dictionary<SpritePart, GameObject>()
        {
            { SpritePart.Body, spriteObject },
            { SpritePart.Face1, spriteObject.transform.GetChild(0).gameObject },
            { SpritePart.Face2, spriteObject.transform.GetChild(1).gameObject }
        };

        Handlers = new AsyncOperationHandle<Sprite>[2];
        PostAction = delegate { };
    }

    #region PostAction

    public void AddPostAction(Action action)
    {
        PostAction += action;
    }

    public void CompletePostAction()
    {
        PostAction.Invoke();
        PostAction = delegate { };
    }

    public void ClearPostAction()
    {
        PostAction = delegate { };
    }
    #endregion

    #region Alpha

    public void SetAlpha(SpritePart part, float alpha)
    {
        Parts[part].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
    }

    public void SetAlpha(float alpha)
    {
        foreach (var part in Parts)
        {
            part.Value.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
        }
    }

    public void SetAlpha(float alpha1, float alpha2, float alpha3)
    {
        Parts[SpritePart.Body].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha1);
        Parts[SpritePart.Face1].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha2);
        Parts[SpritePart.Face2].GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha3);
    }

    #endregion

    #region Rect

    public void SetScale(Vector3 scale)
    {
        Parts[SpritePart.Body].GetComponent<RectTransform>().localScale = scale;
    }

    public Vector3 GetScale()
    {
        return Parts[SpritePart.Body].GetComponent<RectTransform>().localScale;
    }

    public void SetPosition(Vector3 position)
    {
        Parts[SpritePart.Body].GetComponent<RectTransform>().localPosition = position;
    }

    public Vector3 GetPosition()
    {
        return Parts[SpritePart.Body].GetComponent<RectTransform>().localPosition;
    }

    public GameObject ByPart(SpritePart part)
    {
        return Parts[part];
    }

    public void ClearSprite()
    {
        Parts[SpritePart.Body].GetComponent<SpriteRenderer>().sprite = null;
        Parts[SpritePart.Face1].GetComponent<SpriteRenderer>().sprite = null;
        Parts[SpritePart.Face2].GetComponent<SpriteRenderer>().sprite = null;
    }

    public void SetSprite(SpritePart part, Sprite sprite)
    {
        Parts[part].GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public Sprite GetSprite(SpritePart part)
    {
        return Parts[part].GetComponent<SpriteRenderer>().sprite;
    }

    #endregion

    #region Handlers

    public void AddHandler(SpritePart part, AsyncOperationHandle<Sprite> handler)
    {
        switch (part)
        {
            case SpritePart.Body:
                Handlers[0] = handler;
                break;
            case SpritePart.Face1:
                Handlers[1] = handler;
                break;
        }
    }

    public IEnumerator ReleaseHandler(SpritePart part)
    {
        switch (part)
        {
            case SpritePart.Body:
                if (Handlers[0].IsValid())
                {
                    yield return Addressables.ReleaseInstance(Handlers[0]);
                }
                break;
            case SpritePart.Face1:
                if (Handlers[1].IsValid())
                {
                    yield return Addressables.ReleaseInstance(Handlers[1]);
                }
                break;
        }
    }

    #endregion
}

public enum SpritePart
{
    Body,
    Face1,
    Face2,
}

// Представляет данные о спрайте, используется в системе сохранений
public class SpriteData
{
    public SpriteData()
    {
        Pose = 0;
        Emotion = 0;
        Postion = Vector3.zero;
        Alpha = 0;
        Expanded = false;
        Preloaded = false;
    }

    public int Pose;
    public int Emotion;
    public Vector3 Postion;
    public float Alpha;
    public bool Expanded;
    public bool Preloaded;

    public override string ToString()
    {
        return $"[{Pose} {Emotion} {Preloaded}]";
    }
}


public enum Character
{
    None,

    Sergey,
    Pasha,

    Katya,
    Nastya,
    Evelina,
    Tanya,

    Tumanov,
    Raketnikov,

    Speakers, // Говорящие в актовом зале
    Stranger, // Незнакомка (Настя, Таня - первое знакомство)
    Student, // Случайный студент
    Zriteli, // Зрители
}

public class SpriteController : MonoBehaviour
{
    public static SpriteController instance = null;

    public const int MaxSpritesOnScreen = 6;

    public GameObject SpritesParent;

    // Массив игровых объектов
    private GameSpriteObject[] _spriteObjects = new GameSpriteObject[MaxSpritesOnScreen];

    // Данные для сейв системы
    public Dictionary<Character, SpriteData> CharacterSpriteData = new();

    // Игровой объект персонажа
    public Dictionary<Character, GameSpriteObject> CharacterSpriteObject = new();

    // Доступные игровые объекты
    public Dictionary<int, bool> AvaliableSpriteObjects = new();

    private List<IEnumerator>[] Animations = new List<IEnumerator>[MaxSpritesOnScreen];

    private void Awake()
    {
        instance = this;

        for (int i = 0; i < MaxSpritesOnScreen; i++)
        {
            _spriteObjects[i] = new GameSpriteObject(i, SpritesParent.transform.GetChild(i).gameObject);
            AvaliableSpriteObjects.Add(i, true);
            Animations[i] = new List<IEnumerator>();
        }
    }

    public void AddAnimation(int num, IEnumerator anim)
    {
        Animations[num].Add(anim);
    }

    public void AddAnimation(int num, List<IEnumerator> anim)
    {
        Animations[num].AddRange(anim);
    }

    public void CompleteAnimations(int num)
    {
        foreach (var item in Animations[num])
        {
            if (item != null)
            {
                StopCoroutine(item);
            }
        }
        Animations[num].Clear();
    }

    #region SaveSpriteData

    // Полное сохранение
    public void SaveSpriteData(Character character, int pose, int emotion, Vector3 position, float alpha, bool expanded)
    {
        if (CharacterSpriteData.ContainsKey(character))
        {
            var data = CharacterSpriteData[character];
            data.Pose = pose;
            data.Emotion = emotion;
            data.Postion = position;
            data.Alpha = alpha;
            data.Expanded = expanded;
        }
    }

    // Сохранение позиции
    public void SaveSpritePosition(Character character, Vector3 newPosition)
    {
        if (CharacterSpriteData.ContainsKey(character))
        {
            var data = CharacterSpriteData[character];
            data.Postion = newPosition;
        }
    }

    // Сохранение альфы
    public void SaveSpriteAlpha(Character character, float newAlpha)
    {
        if (CharacterSpriteData.ContainsKey(character))
        {
            var data = CharacterSpriteData[character];
            data.Alpha = newAlpha;
        }
    }

    public void SaveSpriteData(Character character, int pose, int emotion) //save Name+pos+emo
    {
        if (CharacterSpriteData.ContainsKey(character))
        {
            var data = CharacterSpriteData[character];
            data.Pose = pose;
            data.Emotion = emotion;
        }
    }

    public void SaveSpriteDataExpanded(Character character, bool expanded)
    {
        if (CharacterSpriteData.ContainsKey(character))
        {
            var data = CharacterSpriteData[character];
            data.Expanded = expanded;
        }
    }

    public void SaveSpriteDataPreloaded(Character character, bool preloaded)
    {
        if (CharacterSpriteData.ContainsKey(character))
        {
            var data = CharacterSpriteData[character];
            data.Preloaded = preloaded;
        }
    }

    #endregion

    // Activity 
    public GameSpriteObject? GetAvaliableSprite(Character character)
    {
        for (int i = 0; i < MaxSpritesOnScreen; i++)
        {
            if (AvaliableSpriteObjects[i])
            {
                AvaliableSpriteObjects[i] = false;
                CharacterSpriteData.Add(character, new SpriteData());
                CharacterSpriteObject.Add(character, _spriteObjects[i]);
                return _spriteObjects[i];
            }
        }
        return null;
    }

    public GameSpriteObject? GetSpriteByCharacter(Character character)
    {
        if (CharacterSpriteData.ContainsKey(character) && CharacterSpriteObject.ContainsKey(character))
        {
            return CharacterSpriteObject[character];
        }
        return null;
    }

    // Удаляет спрайт из активного пула, но ещё не освобождает объект под него
    public void ClearSpriteData(Character character)
    {
        if (CharacterSpriteData.ContainsKey(character))
        {
            CharacterSpriteData.Remove(character);
            CharacterSpriteObject.Remove(character);
        }
    }

    // Освобождает объект под спрайт
    public void ClearSpriteData(int num)
    {
        AvaliableSpriteObjects[num] = true;
    }

    // Метод для настроек. Сжимает все спрайты до исходных, если увеличения выключаются
    public void UnExpandAllSprites()
    {
        foreach (var characterData in CharacterSpriteData)
        {
            Character character = characterData.Key;
            SpriteData data = characterData.Value;
            if (!data.Preloaded && CharacterSpriteObject.ContainsKey(character))
            {
                float scale = SpriteScalesBase.GetCharacterScale(character);
                CharacterSpriteObject[character].SetScale(new Vector3(scale, scale, scale));
            }
        }
    }

    // Увеличивает спрайты, если они должны быть таковыми
    public void LoadSpritesExpandingInfo(bool animated = false)
    {
        foreach (var characterData in CharacterSpriteData)
        {
            Character character = characterData.Key;
            SpriteData data = characterData.Value;
            GameSpriteObject gameSpriteObject = CharacterSpriteObject[character];

            if (!data.Preloaded && CharacterSpriteObject.ContainsKey(character))
            {
                float scale = SpriteScalesBase.GetCharacterScale(character);
                if (data.Expanded)
                {
                    scale *= SpriteExpand.ExpandCoefficient;
                }
                Vector3 vScale = new Vector3(scale, scale, scale);

                SpriteExpand.instance.StartCoroutine(SpriteExpand.instance.Expand(gameSpriteObject, vScale, 0.05f, !animated));
            }
        }
    }

    public void LoadSpriteActualData(Character character)
    {
        SpriteData data = CharacterSpriteData[character];
        GameSpriteObject gameSpriteObject = CharacterSpriteObject[character];

        if (data.Preloaded)
        {
            gameSpriteObject.SetAlpha(0);
        }
        else
        {
            gameSpriteObject.SetPosition(data.Postion);
            gameSpriteObject.SetAlpha(data.Alpha, data.Alpha, 0);
        }
    }

    // Загрузка спрайтов для сейв системы
    public IEnumerator IUploadSprites(Dictionary<Character, SpriteData> characterData)
    {
        CharacterSpriteData = characterData;

        List<IEnumerator> list = new List<IEnumerator>();

        for (int i = 0; i < characterData.Count; i++)
        {
            var element = CharacterSpriteData.ElementAt(i);
            Character character = element.Key;
            SpriteData spriteData = element.Value;
            GameSpriteObject gameSpriteObject = _spriteObjects[i];

            AvaliableSpriteObjects[i] = false;
            CharacterSpriteObject.Add(character, gameSpriteObject);

            list.Add(PackageConntector.instance.IConnectPackageGroup(character));

            if (!spriteData.Preloaded)
            {
                gameSpriteObject.SetPosition(spriteData.Postion);
                gameSpriteObject.SetAlpha(spriteData.Alpha, spriteData.Alpha, 0);

                float scale = SpriteScalesBase.GetCharacterScale(character);
                if (spriteData.Expanded && SettingsConfig.IfAllowExpandings())
                {
                    scale *= SpriteExpand.ExpandCoefficient;
                }
                gameSpriteObject.SetScale(new Vector3(scale, scale, scale));

                list.Add(LoadSpriteByParts(gameSpriteObject, character, spriteData.Pose, spriteData.Emotion));
            }
        }

        yield return StartCoroutine(CoroutineUtils.WaitForAll(list));
    }

    // Отгрузка спрайтов для сейв системы
    public IEnumerator IUnloadSprites()
    {
        List<IEnumerator> list = new List<IEnumerator>();

        foreach (var characterData in CharacterSpriteData)
        {
            Character character = characterData.Key;
            SpriteData data = characterData.Value;
            GameSpriteObject gameSpriteObject = CharacterSpriteObject[character];

            if (!data.Preloaded)
            {
                list.Add(gameSpriteObject.ReleaseHandler(SpritePart.Body));
                list.Add(gameSpriteObject.ReleaseHandler(SpritePart.Face1));
                gameSpriteObject.SetAlpha(0f);
                yield return null;
            }
        }

        yield return StartCoroutine(CoroutineUtils.WaitForAll(list));

        CharacterSpriteData.Clear();
        CharacterSpriteObject.Clear();
        for (int i = 0; i < AvaliableSpriteObjects.Count; i++)
        {
            AvaliableSpriteObjects[i] = true;
            Animations[i].Clear();
            _spriteObjects[i].ClearPostAction();
        }
    }

    public IEnumerator LoadSpriteByParts(GameSpriteObject sprite, Character character, int pose, int emotion)
    {
        yield return StartCoroutine(CoroutineUtils.WaitForAll(new List<IEnumerator>
        {
            ILoadSpriteOfSpecificObject(sprite, SpritePart.Body, character, pose, 0),
            ILoadSpriteOfSpecificObject(sprite, SpritePart.Face1, character, pose, emotion)
        }));
    }

    public IEnumerator ILoadSpriteOfSpecificObject(GameSpriteObject sprite, SpritePart part, Character character, int pose, int emotion)
    {
        AsyncOperationHandle<Sprite> newHandle = Addressables.LoadAssetAsync<Sprite>($"{character}{pose}_p{emotion}");
        yield return newHandle;

        sprite.AddHandler(part, newHandle);
        sprite.SetSprite(part, newHandle.Result);
    }
}
