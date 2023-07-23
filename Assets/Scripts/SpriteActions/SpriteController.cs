using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Reflection;
using System.Text;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public enum SpritePart
{
    BASE,
    FACE1,
    FACE2,
}

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

        handles = new AsyncOperationHandle<Sprite>[3];
    }

    public string name;
    public int pose;
    public int emotion;
    public Vector3 postion;
    public float alpha;
    public bool expanded;
    public int prevSprite;

    public AsyncOperationHandle<Sprite>[] handles;
}


public class SpriteController : MonoBehaviour
{
    public static SpriteController instance = null;

    [SerializeField] private Dictionary<(string name, int pose_num), List<AssetReference>> characterAssets = new Dictionary<(string, int), List<AssetReference>>();

    [SerializeField] private List<AssetReference> Pasha1;//Clothes pose1
    [SerializeField] private List<AssetReference> Pasha2;//Clothes pose2
    [SerializeField] private List<AssetReference> Pasha3;//Clothes pose3
    [SerializeField] private List<AssetReference> Pasha4;//Pose1 dry
    [SerializeField] private List<AssetReference> Pasha5;//Pose1 wet
    [SerializeField] private List<AssetReference> Pasha6;//Pose2 dry
    [SerializeField] private List<AssetReference> Pasha7;//Pose2 wet
    [SerializeField] private List<AssetReference> Pasha8;//Pose3 dry
    [SerializeField] private List<AssetReference> Pasha9;//Pose3 wet

    [SerializeField] private List<AssetReference> Katya1;
    [SerializeField] private List<AssetReference> Katya2;
    [SerializeField] private List<AssetReference> Katya3;
    [SerializeField] private List<AssetReference> Katya4;
    [SerializeField] private List<AssetReference> Katya5;
    [SerializeField] private List<AssetReference> Katya6;
    [SerializeField] private List<AssetReference> Katya7;
    [SerializeField] private List<AssetReference> Katya8;
    [SerializeField] private List<AssetReference> Katya9;

    [SerializeField] private List<AssetReference> Nastya1;
    [SerializeField] private List<AssetReference> Nastya2;
    [SerializeField] private List<AssetReference> Nastya3;

    [SerializeField] private List<AssetReference> Evelina1;
    [SerializeField] private List<AssetReference> Evelina2;
    [SerializeField] private List<AssetReference> Evelina3;

    [SerializeField] private List<AssetReference> Tanya1;
    [SerializeField] private List<AssetReference> Tanya2;
    [SerializeField] private List<AssetReference> Tanya3;

    [SerializeField] private List<AssetReference> Raketnikov1;

    [SerializeField] private List<AssetReference> Tumanov1; //pose 1 glasses
    [SerializeField] private List<AssetReference> Tumanov2; //pose 1 no glasses
    [SerializeField] private List<AssetReference> Tumanov3; //pose 2 glasses
    [SerializeField] private List<AssetReference> Tumanov4; //pose 2 no glasses

    [HideInInspector] public GameObject Sprites;

    private const int maxSpritesOnScreen = 4;

    public SpriteData[] GameSpriteData;

    [HideInInspector] public AsyncOperationHandle<Sprite> _SpriteOperationHandler;

    [HideInInspector] public Dictionary<string, AsyncOperationHandle<Sprite>[]> handles = new Dictionary<string, AsyncOperationHandle<Sprite>[]>();

    [HideInInspector] public Dictionary<string, Vector3> CharactersScales = new Dictionary<string, Vector3>();

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

        CharactersScales.Add("Pasha", new Vector3(36f, 36f, 0f));
        CharactersScales.Add("Nastya", new Vector3(45f, 45f, 0f));
        CharactersScales.Add("Evelina", new Vector3(40f, 40f, 0f));
        CharactersScales.Add("Tanya", new Vector3(41f, 41f, 0f));
        CharactersScales.Add("Katya", new Vector3(37f, 37f, 0f));
        CharactersScales.Add("Raketnikov", new Vector3(35f, 35f, 0f));
        CharactersScales.Add("Tumanov", new Vector3(33f, 33f, 0f));

        // Assets       
        InitAssetByName("Pasha", 9);
        InitAssetByName("Nastya", 3);
        InitAssetByName("Katya", 3);
        InitAssetByName("Evelina", 3);
        InitAssetByName("Tanya", 3);
        InitAssetByName("Raketnikov", 1);
        InitAssetByName("Tumanov", 4);
    }

    private void InitAssetByName(string assetName, int len)
    {
        Type type = GetType();
        for (int i = 1; i <= len; i++)
        {
            FieldInfo fieldInfo = type.GetField(assetName + i, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
            if (fieldInfo != null)
            {
                characterAssets.Add((assetName, i), (List<AssetReference>)fieldInfo.GetValue(this));
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

        //Debug.Log(builder.ToString());
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
    public void SetDefaultScale(GameObject obj, string Name)
    {
        SetNewScale(obj, Name, 1f);
    }

    public void SetNewScale(GameObject obj, string Name, float coefficient)
    {
        obj.GetComponent<RectTransform>().localScale = CharactersScales[Name] * coefficient;
    }

    // Activity
    public int GetSpriteByName(string name)
    {
        for (int i = 0; i < maxSpritesOnScreen; i++)
        {
            if (GameSpriteData[i].name == name)
            {
                return i;
            }
        }

        return -1;
    }

    public int GetAvaliableSpriteNum(string name)
    {
        for (int i = 0; i < maxSpritesOnScreen; i++)
        {
            if (GameSpriteData[i].name == null)
            {
                GameSpriteData[i].name = name;
                return i;
            }
        }

        return -1;
    }

    public void DelActivity(int i)
    {
        GameSpriteData[i] = new SpriteData(i);
        return;
    }

    public GameObject GetSprite(int num)
    {
        return Sprites.transform.GetChild(num).gameObject;
    }

    public void LoadSpritesExpandings()
    {
        for (int i = 0; i < maxSpritesOnScreen; i++)
        {
            GameObject sprite = GetSprite(i);

            SpriteData data = GameSpriteData[i];

            if (data.name != null)
            {
                Vector3 scale = CharactersScales[data.name];

                if (data.expanded)
                {
                    scale *= SpriteExpand.instance.expand_coefficient;
                }

                sprite.transform.localScale = scale;
            }
        }
    }

    // Доводит состояния спрайтов до актуального состояния
    public void SkipSpriteActions()
    {
        for (int i = 0; i < maxSpritesOnScreen; i++)
        {
            GameObject sprite = GetSprite(i);

            SpriteData data = GameSpriteData[i];

            if (data.name != null)
            {
                sprite.transform.localPosition = data.postion;

                Vector3 scale = CharactersScales[data.name];

                if (data.expanded)
                {
                    scale *= SpriteExpand.instance.expand_coefficient;
                }

                sprite.transform.localScale = scale;
                sprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, data.alpha);

                sprite.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, data.alpha);
                sprite.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

            }
            else
            {
                if (data.alpha == 0f)
                {
                    sprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0f);
                    sprite.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0f);
                    sprite.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0f);
                }
            }
        }
    }

    // Загрузка спрайтов для сейв системы
    public IEnumerator LoadSprites()
    {
        for (int i = 0; i < maxSpritesOnScreen; i++)
        {
            GameObject sprite = GetSprite(i);

            SpriteData data = GameSpriteData[i];

            if (data.name != null)
            {
                // Position
                sprite.transform.localPosition = data.postion;

                // Color
                sprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, data.alpha);
                sprite.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, data.alpha);
                sprite.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

                // Scale
                Vector3 scale = CharactersScales[data.name];
                if (data.expanded)
                {
                    scale *= SpriteExpand.instance.expand_coefficient;
                }
                sprite.transform.localScale = scale;

                // Loading
                yield return LoadCurrSprite(sprite, i, data.name, data.pose, data.emotion);
            }
        }
    }

    public IEnumerator AutoConnectData(SpriteData[] data)
    {
        GameSpriteData = data;

        for (int i = 0; i < maxSpritesOnScreen; i++)
        {
            if (GameSpriteData[i].name != null)
            {
                yield return StartCoroutine(PackageConntector.instance.IConnectPackage(GameSpriteData[i].name));
            }
        }
    }

    public IEnumerator IUnloadSprites()
    {
        for (int i = 0; i < 4; i++)
        {
            if (GameSpriteData[i].name != null)
            {
                yield return Addressables.ReleaseInstance(GameSpriteData[i].handles[0]);
                yield return Addressables.ReleaseInstance(GameSpriteData[i].handles[1]);

                GameObject sprite = Sprites.transform.GetChild(i).gameObject;

                sprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                sprite.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                sprite.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            }
        }
    }

    // Загрузка текстур в данный спрайт
    public IEnumerator LoadCurrSprite(GameObject currSprite, int spriteNum, string character, int pose, int emotion)
    {
        yield return StartCoroutine(LoadSpriteByParts(currSprite, spriteNum, character, pose, emotion));
    }

    public IEnumerator LoadSpriteByParts(GameObject spriteToLoad, int spriteNum, string character, int pose, int emotion)
    {
        GameObject Face = spriteToLoad.transform.GetChild(0).gameObject;
        yield return StartCoroutine(CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>
        {
            ILoadSpriteOfSpecificObject(spriteToLoad, spriteNum, character, pose, 0, SpritePart.BASE),
            ILoadSpriteOfSpecificObject(Face, spriteNum, character, pose, emotion, SpritePart.FACE1)
        }));
    }

    public IEnumerator ILoadSpriteOfSpecificObject(GameObject obj, int spriteNum, string characterName, int pose, int emotion, SpritePart part)
    {
        AssetReference spriteReference = characterAssets[(characterName, pose)][emotion];

        AsyncOperationHandle<Sprite> newHandle = spriteReference.LoadAssetAsync<Sprite>();

        yield return newHandle;

        switch (part)
        {
            case SpritePart.BASE:
                GameSpriteData[spriteNum].handles[0] = newHandle;
                break;
            case SpritePart.FACE1:
                GameSpriteData[spriteNum].handles[1] = newHandle;
                break;
            default:
                Debug.Log($"Error in sprite {characterName} loading!");
                break;
        }

        obj.GetComponent<SpriteRenderer>().sprite = newHandle.Result;
    }
}
