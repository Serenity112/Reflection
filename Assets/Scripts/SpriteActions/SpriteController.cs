using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

// Представляет собой игровой объект и его части для упрощения доступа к ним
public struct GameSpriteObject
{
    public GameSpriteObject(int num, GameObject spriteObject)
    {
        this.num = num;

        Parts = new Dictionary<SpritePart, GameObject>()
        {
            { SpritePart.Body, spriteObject },
            { SpritePart.Face1, spriteObject.transform.GetChild(0).gameObject },
            { SpritePart.Face2, spriteObject.transform.GetChild(1).gameObject }
        };

        Handlers = new AsyncOperationHandle<Sprite>[2];
    }

    public int num;
    public AsyncOperationHandle<Sprite>[] Handlers;
    private Dictionary<SpritePart, GameObject> Parts;

    #region alpha

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

    #region rect

    public void SetScale(Vector3 scale)
    {
        Parts[SpritePart.Body].GetComponent<RectTransform>().localScale = scale;
    }

    public Vector3 GetScale()
    {
        return Parts[SpritePart.Body].GetComponent<RectTransform>().localScale;
    }

    public void SetPosition(Vector3 scale)
    {
        Parts[SpritePart.Body].GetComponent<RectTransform>().localPosition = scale;
    }

    public Vector3 GetPosition()
    {
        return Parts[SpritePart.Body].GetComponent<RectTransform>().localPosition;
    }

    public GameObject ByPart(SpritePart part)
    {
        return Parts[part];
    }

    public void SetImage(SpritePart part, Sprite sprite)
    {
        Parts[part].GetComponent<SpriteRenderer>().sprite = sprite;
    }

    public Sprite GetImage(SpritePart part)
    {
        return Parts[part].GetComponent<SpriteRenderer>().sprite;
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
public struct SpriteData
{
    public SpriteData(int SaveNum)
    {
        name = null;
        pose = 0;
        emotion = 0;
        postion = Vector3.zero;
        alpha = 0;
        expanded = false;
        prevSprite = -1;
    }

    public string name;
    public int pose;
    public int emotion;
    public Vector3 postion;
    public float alpha;
    public bool expanded;
    public int prevSprite;
}

public class SpriteController : MonoBehaviour
{
    public static SpriteController instance = null;

    private const int maxSpritesOnScreen = 4;

    [HideInInspector] public SpriteData[] GameSpriteData;

    [HideInInspector] public Dictionary<string, Vector3> CharactersScales;

    public GameObject Sprites;

    [HideInInspector] public GameSpriteObject[] GameSprites;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance == this)
        {
            Destroy(gameObject);
        }

        GameSpriteData = new SpriteData[maxSpritesOnScreen];
        for (int i = 0; i < maxSpritesOnScreen; i++)
        {
            GameSpriteData[i] = new SpriteData(i);
        }

        GameSprites = new GameSpriteObject[maxSpritesOnScreen];
        for (int i = 0; i < maxSpritesOnScreen; i++)
        {
            GameSprites[i] = new GameSpriteObject(i, Sprites.transform.GetChild(i).gameObject);
        }

        InitScales();
    }

    private void InitScales()
    {
        CharactersScales = new Dictionary<string, Vector3>
        {
            { "Pasha", new Vector3(36f, 36f, 0f) },
            { "Nastya", new Vector3(45f, 45f, 0f) },
            { "Evelina", new Vector3(40f, 40f, 0f) },
            { "Tanya", new Vector3(41f, 41f, 0f) },
            { "Katya", new Vector3(37f, 37f, 0f) },
            { "Raketnikov", new Vector3(35f, 35f, 0f) },
            { "Tumanov", new Vector3(33f, 33f, 0f) }
        };
    }

    private void InitAssetByName(string assetName, int len)
    {
        Type type = GetType();
        for (int i = 1; i <= len; i++)
        {
            FieldInfo fieldInfo = type.GetField(assetName + i, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                //characterAssets.Add((assetName, i), (List<AssetReference>)fieldInfo.GetValue(this));
            }
        }
    }

    public void printData()
    {
        StringBuilder builder = new StringBuilder();

        for (int i = 0; i < 4; i++)
        {
            if (GameSpriteData[i].name == null)
            {
                builder.AppendLine($"Sprite {i}: null, pose: {GameSpriteData[i].pose}, emo: {GameSpriteData[i].emotion} alpha: {GameSpriteData[i].alpha}\n");
            }
            else
            {
                builder.AppendLine($"Sprite {i}: {GameSpriteData[i].name}, pose: {GameSpriteData[i].pose}, emo: {GameSpriteData[i].emotion} alpha: {GameSpriteData[i].alpha}\n");
            }
        }

        Debug.Log(builder.ToString());
    }

    // SaveSpriteData
    public void SaveSpriteData(int spriteNum, string name, int pose, int emotion, Vector3 position, float alpha, bool expanded) // Full
    {
        GameSpriteData[spriteNum].name = name;
        GameSpriteData[spriteNum].pose = pose;
        GameSpriteData[spriteNum].emotion = emotion;
        GameSpriteData[spriteNum].postion = position;
        GameSpriteData[spriteNum].alpha = alpha;
        GameSpriteData[spriteNum].expanded = expanded;
    }
    public void SaveSpriteData(int Sprite, Vector3 newVector) //save Position
    {
        GameSpriteData[Sprite].postion = newVector;
    }

    public void SaveSpriteData(int Sprite, float alpha) //save Alpha
    {
        GameSpriteData[Sprite].alpha = alpha;
    }

    public void SaveSpriteData(int Sprite, string name, int pose, int emotion) //save Name+pos+emo
    {
        GameSpriteData[Sprite].name = name;
        GameSpriteData[Sprite].pose = pose;
        GameSpriteData[Sprite].emotion = emotion;
    }

    public void SaveSpriteData(int Sprite, bool expanded) //save Position
    {
        GameSpriteData[Sprite].expanded = expanded;
    }

    // Scale
    public void SetScaleByName(GameSpriteObject sprite, string name)
    {
        SetScaleByName(sprite, name, 1f);
    }

    public void SetScaleByName(GameSpriteObject sprite, string name, float coefficient)
    {
        sprite.SetScale(CharactersScales[name] * coefficient);
    }

    // Activity
    public GameSpriteObject? GetSpriteNumByName(string name)
    {
        for (int i = 0; i < maxSpritesOnScreen; i++)
        {
            if (GameSpriteData[i].name == name)
            {
                return GameSprites[i];
            }
        }

        return null;
    }

    public GameSpriteObject? GetAvaliableSprite(string name)
    {
        for (int i = 0; i < maxSpritesOnScreen; i++)
        {
            if (GameSpriteData[i].name == null)
            {
                GameSpriteData[i].name = name;
                return GameSprites[i];
            }
        }

        return null;
    }

    public void DelActivity(int num)
    {
        GameSpriteData[num] = new SpriteData(num);
        return;
    }

    public void SkipSpritesExpanding()
    {
        for (int i = 0; i < maxSpritesOnScreen; i++)
        {
            GameSpriteObject sprite = GameSprites[i];

            SpriteData data = GameSpriteData[i];

            if (data.name != null)
            {
                Vector3 scale = CharactersScales[data.name];

                if (data.expanded)
                {
                    scale *= SpriteExpand.instance.expand_coefficient;
                }

                sprite.SetScale(scale);
            }
        }
    }

    // Доводит состояния спрайтов до актуального состояния
    public void SkipSpriteActions()
    {
        for (int i = 0; i < maxSpritesOnScreen; i++)
        {
            GameSpriteObject sprite = GameSprites[i];

            SpriteData data = GameSpriteData[i];

            if (data.name != null)
            {
                sprite.SetPosition(data.postion);

                Vector3 scale = CharactersScales[data.name];

                if (data.expanded)
                {
                    scale *= SpriteExpand.instance.expand_coefficient;
                }

                sprite.SetScale(scale);

                sprite.SetAlpha(data.alpha, data.alpha, 0);

            }
            else
            {
                if (data.alpha == 0f)
                {
                    sprite.SetAlpha(0);
                }
            }
        }
    }

    // Загрузка спрайтов для сейв системы
    public IEnumerator LoadSprites(SpriteData[] data)
    {
        GameSpriteData = data;

        List<IEnumerator> list = new List<IEnumerator>();

        for (int i = 0; i < maxSpritesOnScreen; i++)
        {
            SpriteData i_data = GameSpriteData[i];

            GameSpriteObject sprite = GameSprites[i];

            if (i_data.name != null)
            {
                sprite.SetPosition(i_data.postion);

                sprite.SetAlpha(i_data.alpha, i_data.alpha, 0);

                Vector3 scale = CharactersScales[i_data.name];
                if (i_data.expanded)
                {
                    scale *= SpriteExpand.instance.expand_coefficient;
                }
                sprite.SetScale(scale);

                StartCoroutine(PackageConntector.instance.IConnectPackage(i_data.name));
                list.Add(LoadSpriteByParts(sprite, i_data.name, i_data.pose, i_data.emotion));
            }

            yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(list));
        }
    }

    public IEnumerator IUnloadSprites()
    {
        for (int i = 0; i < maxSpritesOnScreen; i++)
        {
            if (GameSpriteData[i].name != null)
            {
                GameSpriteObject sprite = GameSprites[i];

                if (sprite.Handlers[0].Status == AsyncOperationStatus.Succeeded)
                {
                    Addressables.Release(sprite.Handlers[0]);
                }

                if (sprite.Handlers[1].Status == AsyncOperationStatus.Succeeded)
                {
                    Addressables.Release(sprite.Handlers[1]);
                }

                yield return null;

                sprite.SetAlpha(0f);
            }
        }
    }

    public IEnumerator LoadSpriteByParts(GameSpriteObject sprite, string character, int pose, int emotion)
    {
        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            ILoadSpriteOfSpecificObject(sprite, SpritePart.Body, character, pose, 0),
            ILoadSpriteOfSpecificObject(sprite, SpritePart.Face1, character, pose, emotion)
        }));
    }

    public IEnumerator ILoadSpriteOfSpecificObject(GameSpriteObject sprite, SpritePart part, string name, int pose, int emotion)
    {
        AsyncOperationHandle<Sprite> newHandle = Addressables.LoadAssetAsync<Sprite>($"{name}{pose}_p{emotion}");
        yield return newHandle;

        switch (part)
        {
            case SpritePart.Body:
                sprite.Handlers[0] = newHandle;
                break;
            case SpritePart.Face1:
                sprite.Handlers[1] = newHandle;
                break;
        }

        sprite.SetImage(part, newHandle.Result);
    }
}
