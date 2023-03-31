using System;
using System.Collections;
using System.Collections.Generic;
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

    [SerializeField] private Dictionary<Tuple<string, int>, List<AssetReference>> characterAssets = new Dictionary<Tuple<string, int>, List<AssetReference>>();

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

    [SerializeField] private List<AssetReference> Raketnikov;

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

        CharactersScales.Add("Pasha", new Vector3(27f, 27f, 0f));
        CharactersScales.Add("Nastya", new Vector3(37f, 37f, 0f));
        CharactersScales.Add("Evelina", new Vector3(32f, 32f, 0f));
        CharactersScales.Add("Tanya", new Vector3(35f, 35f, 0f));
        CharactersScales.Add("Katya", new Vector3(29f, 29f, 0f));
        CharactersScales.Add("Raketnikov", new Vector3(29f, 29f, 0f));
        CharactersScales.Add("Tumanov", new Vector3(26f, 26f, 0f));

        // Assets
        characterAssets.Add(new Tuple<string, int>("Pasha", 1), Pasha1);
        characterAssets.Add(new Tuple<string, int>("Pasha", 2), Pasha2);
        characterAssets.Add(new Tuple<string, int>("Pasha", 3), Pasha3);
        characterAssets.Add(new Tuple<string, int>("Pasha", 4), Pasha4);
        characterAssets.Add(new Tuple<string, int>("Pasha", 5), Pasha5);
        characterAssets.Add(new Tuple<string, int>("Pasha", 6), Pasha6);
        characterAssets.Add(new Tuple<string, int>("Pasha", 7), Pasha7);
        characterAssets.Add(new Tuple<string, int>("Pasha", 8), Pasha8);
        characterAssets.Add(new Tuple<string, int>("Pasha", 9), Pasha9);

        characterAssets.Add(new Tuple<string, int>("Katya", 1), Katya1);
        characterAssets.Add(new Tuple<string, int>("Katya", 2), Katya2);
        characterAssets.Add(new Tuple<string, int>("Katya", 3), Katya3);
        characterAssets.Add(new Tuple<string, int>("Katya", 4), Katya4);
        characterAssets.Add(new Tuple<string, int>("Katya", 5), Katya5);
        characterAssets.Add(new Tuple<string, int>("Katya", 6), Katya6);
        characterAssets.Add(new Tuple<string, int>("Katya", 7), Katya7);
        characterAssets.Add(new Tuple<string, int>("Katya", 8), Katya8);
        characterAssets.Add(new Tuple<string, int>("Katya", 9), Katya9);

        characterAssets.Add(new Tuple<string, int>("Nastya", 1), Nastya1);
        characterAssets.Add(new Tuple<string, int>("Nastya", 2), Nastya2);
        characterAssets.Add(new Tuple<string, int>("Nastya", 3), Nastya3);

        characterAssets.Add(new Tuple<string, int>("Evelina", 1), Evelina1);
        characterAssets.Add(new Tuple<string, int>("Evelina", 2), Evelina2);
        characterAssets.Add(new Tuple<string, int>("Evelina", 3), Evelina3);

        characterAssets.Add(new Tuple<string, int>("Tanya", 1), Tanya1);
        characterAssets.Add(new Tuple<string, int>("Tanya", 2), Tanya2);
        characterAssets.Add(new Tuple<string, int>("Tanya", 3), Tanya3);

        characterAssets.Add(new Tuple<string, int>("Raketnikov", 1), Raketnikov);

        characterAssets.Add(new Tuple<string, int>("Tumanov", 1), Tumanov1);
        characterAssets.Add(new Tuple<string, int>("Tumanov", 2), Tumanov2);
        characterAssets.Add(new Tuple<string, int>("Tumanov", 3), Tumanov3);
        characterAssets.Add(new Tuple<string, int>("Tumanov", 4), Tumanov4);
    }

    public void printData()
    {
        for (int i = 0; i < 4; i++)
        {
            if (GameSpriteData[i].name == null)
            {
                Debug.Log(i + " Null" + " " + GameSpriteData[i].pose + " " + GameSpriteData[i].emotion + " a: " + GameSpriteData[i].alpha);
            }
            else
            {
                Debug.Log(i + " " + GameSpriteData[i].name + " " + GameSpriteData[i].pose + " " + GameSpriteData[i].emotion + " a: " + GameSpriteData[i].alpha);
            }

        }
        // Debug.Log("_____________________");
    }

    // SaveSpriteData
    public void SaveSpriteData(int Sprite, string name, int pose, int emotion, Vector3 position, float alpha, bool expanded) // Full
    {
        GameSpriteData[Sprite].name = name;
        GameSpriteData[Sprite].pose = pose;
        GameSpriteData[Sprite].emotion = emotion;
        GameSpriteData[Sprite].postion = position;
        GameSpriteData[Sprite].alpha = alpha;
        GameSpriteData[Sprite].expanded = expanded;
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
    public int GetActivityByName(string name)
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

    public int GetNewActivity(string name)
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

    public void SkipSpriteActions_Expand()
    {
        for (int i = 0; i < maxSpritesOnScreen; i++)
        {
            GameObject sprite = GetSprite(i);

            if (GameSpriteData[i].name != null)
            {
                Vector3 scale = CharactersScales[GameSpriteData[i].name];

                if (GameSpriteData[i].expanded)
                {
                    scale = scale * SpriteExpand.instance.expand_coefficient;
                }
                
                sprite.transform.localScale = scale;
            }
        }
    }

    public void SkipSpriteActions()
    {
        for (int i = 0; i < 4; i++)
        {
            GameObject sprite = Sprites.transform.GetChild(i).gameObject;

            if (GameSpriteData[i].name != null)
            {
                sprite.transform.localPosition = GameSpriteData[i].postion;

                Vector3 scale = CharactersScales[GameSpriteData[i].name];

                if (GameSpriteData[i].expanded)
                {
                    scale = scale * SpriteExpand.instance.expand_coefficient;
                }

                sprite.transform.localScale = scale;
                sprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, GameSpriteData[i].alpha);

                sprite.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, GameSpriteData[i].alpha);
                sprite.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

            }
            else
            {
                if (GameSpriteData[i].alpha == 0f)
                {
                    sprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0f);
                    sprite.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0f);
                    sprite.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0f);
                }
            }
        }
    }

    public void LoadSprites()
    {
        //Debug.Log("Called LOAD_SPRITES");
        for (int i = 0; i < 4; i++)
        {
            var data = GameSpriteData[i];

            if (data.name != null)
            {
                GameObject sprite = Sprites.transform.GetChild(i).gameObject;

                // Position
                sprite.transform.localPosition = data.postion;

                // Color
                float alpha = data.alpha;
                sprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
                sprite.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, alpha);
                sprite.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);

                // Scale
                Vector3 scale = CharactersScales[data.name];
                if (GameSpriteData[i].expanded)
                {
                    float coefficient = 1.035f;
                    float x = scale.x * coefficient;
                    float y = scale.y * coefficient;
                    float z = scale.z * coefficient;
                    scale = new Vector3(x, y, z);
                }
                sprite.transform.localScale = scale;

                // Pose, emo, name
                int pose = data.pose;
                int emotion = data.emotion;
                string name = data.name;

                // Loading
                LoadCurrSprite(sprite, name, pose, emotion);
            }
        }
    }

    public void AutoConnectPackages()
    {
        for (int i = 0; i < 4; i++)
        {
            if (GameSpriteData[i].name != null)
            {
                PackageConntector.instance.ConnectPackage(GameSpriteData[i].name);
            }
        }
    }
    public void UnloadSprites()
    {
        for (int i = 0; i < 4; i++)
        {
            if (GameSpriteData[i].name != null)
            {
                Addressables.Release(GameSpriteData[i].handles[0]);
                Addressables.Release(GameSpriteData[i].handles[1]);

                GameObject sprite = Sprites.transform.GetChild(i).gameObject;

                sprite.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                sprite.transform.GetChild(0).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
                sprite.transform.GetChild(1).gameObject.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
            }
        }
    }

    public void LoadCurrSprite(GameObject currSprite, string character, int pose, int emotion)
    {
        StartCoroutine(LoadSpriteByParts(currSprite, character, pose, emotion));
    }

    public IEnumerator LoadSpriteByParts(GameObject currSprite, string character, int pose, int emotion)
    {
        StartCoroutine(ILoadSpriteOfSpecificObject(currSprite, character, pose, 0, SpritePart.BASE));

        GameObject Face = currSprite.transform.GetChild(0).gameObject;

        yield return StartCoroutine(ILoadSpriteOfSpecificObject(Face, character, pose, emotion, SpritePart.FACE1));
    }

    public IEnumerator ILoadSpriteOfSpecificObject(GameObject obj, string characterName, int pose, int emotion, SpritePart part)
    {
        AssetReference spriteReference = characterAssets[new Tuple<string, int>(characterName, pose)][emotion];

        int spriteNum = GetActivityByName(characterName);

        AsyncOperationHandle<Sprite> newHandle = spriteReference.LoadAssetAsync<Sprite>();

        yield return newHandle;

        switch (part)
        {
            case SpritePart.BASE:
                GameSpriteData[spriteNum].handles[0] = newHandle;
                obj.GetComponent<SpriteRenderer>().sprite = newHandle.Result;
                break;
            case SpritePart.FACE1:
                GameSpriteData[spriteNum].handles[1] = newHandle;
                obj.GetComponent<SpriteRenderer>().sprite = newHandle.Result;
                break;
            default:
                Debug.Log("Error in Sprite loading!");
                break;
        }
    }
}