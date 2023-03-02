using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

using TMPro;

public class LogManager : MonoBehaviour
{
    public static LogManager instance = null;

    [SerializeField] AssetReference messagePrefab;
    [SerializeField] AssetReference namePrefab;
    [SerializeField] AssetReference spacePrefab;


    [SerializeField] GameObject messagePrefab2;
    [SerializeField] GameObject namePrefab2;
    [SerializeField] GameObject spacePrefab2;

    public RectTransform messageParent;

    Dictionary<string, string> NamesColors;
    Dictionary<string, string> ShortNames;

    public int counter = -1;
    public Button button;

    public List<AsyncOperationHandle<GameObject>> handles;
    public List<GameObject> handles2;


    private string currName;
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

        currName = string.Empty;

        handles = new List<AsyncOperationHandle<GameObject>>();
        handles2 = new List<GameObject>();

        NamesColors = new Dictionary<string, string>();
        NamesColors.Add("pas", "#D35400"); //orange
        NamesColors.Add("kat", "#E74C3C"); //red
        NamesColors.Add("nas", "#2ECC71"); //green
        NamesColors.Add("eve", "#7D3C98"); //magenta
        NamesColors.Add("tan", "#2980B9"); //blue
        NamesColors.Add("ser", "#7D3C98"); //blue

        ShortNames = new Dictionary<string, string>();
        ShortNames.Add("pas", "Паша");
        ShortNames.Add("kat", "Катя");
        ShortNames.Add("nas", "Настя");
        ShortNames.Add("eve", "Эвелина");
        ShortNames.Add("tan", "Таня");
        ShortNames.Add("ser", "Сергей");
    }

    public void NewMessage(string MyMessage, string speakerName)
    {
        counter++;

        if (ShortNames.ContainsKey(speakerName)) // Значит говорит перс
        {
            if (currName != speakerName)
            {
                CreateSpace();
                CreateName(speakerName);
            }

           CreateMessage(MyMessage);

            currName = speakerName;
        }
        else // Значит кусок истории
        {
            if (currName != string.Empty)
            {
                CreateSpace();
            }

            currName = string.Empty;

            CreateMessage(MyMessage);
        }


    }
    void setResultToScroll(GameObject result)
    {
        result.transform.SetParent(messageParent);

        result.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);

        result.GetComponent<RectTransform>().localPosition = new Vector3(result.transform.localPosition.x, result.transform.localPosition.y, 0);
    }
    void CreateName(string name)
    {
        var result = Instantiate(namePrefab2, new Vector3(0, 0, 0), Quaternion.identity);
        handles2.Add(result);

        setResultToScroll(result);

        string nameBack = $"{ShortNames[name]}: ";
        //string NameFront = $"{ShortNames[name]}: ";

        GameObject textBack = result.transform.GetChild(0).gameObject;
        //GameObject textFront = textBack.transform.GetChild(0).gameObject;

        textBack.GetComponent<Text>().text = nameBack;
        //textFront.GetComponent<Text>().text = NameFront;
    }
    void CreateSpace() 
    {
        var result = Instantiate(spacePrefab2, new Vector3(0, 0, 0), Quaternion.identity);
        handles2.Add(result);

        setResultToScroll(result);
    }
    void CreateMessage(string message)
    {
        var result = Instantiate(messagePrefab2, new Vector3(0, 0, 0), Quaternion.identity);
        handles2.Add(result);

        setResultToScroll(result);

        result.transform.GetChild(0).GetComponent<Text>().text = message;
    }
    public void OpenLog()
    {
        gameObject.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>().alpha = 1; //ScrollView
        gameObject.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>().blocksRaycasts = true;

        //gameObject.transform.GetChild(1).gameObject.SetActive(true);
        //gameObject.transform.GetChild(2).gameObject.SetActive(false);

    }
    public void CloseLog()
    {
        gameObject.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>().alpha = 0; //ScrollView
        gameObject.transform.GetChild(0).gameObject.GetComponent<CanvasGroup>().blocksRaycasts = false;


       // gameObject.transform.GetChild(1).gameObject.SetActive(false);
       // gameObject.transform.GetChild(2).gameObject.SetActive(true);
    }
    public void DelLog()
    {
        foreach (Transform child in messageParent)
        {
            Destroy(child.gameObject, 0f);
        }

        handles2 = new List<GameObject>();

        Resources.UnloadUnusedAssets();

    }

    // using Ienumerator______________________________________________________________________________________________________________
    public void NewMessage2(string MyMessage, string speakerName)
    {
        counter++;

        StartCoroutine(IMessage(MyMessage, speakerName));
    }
    IEnumerator IMessage(string MyMessage, string speakerName)
    {
        if (ShortNames.ContainsKey(speakerName)) // Значит говорит перс
        {
            if (currName != speakerName)
            {
                yield return StartCoroutine(ICreateSpace());
                yield return StartCoroutine(ICreateName(speakerName));
            }

            yield return StartCoroutine(ICreateMessage(MyMessage));

            currName = speakerName;
        }
        else // Значит кусок истории
        {
            if (currName != string.Empty)
            {
                yield return StartCoroutine(ICreateSpace());
            }

            currName = string.Empty;

            yield return StartCoroutine(ICreateMessage(MyMessage));
        }
    }
    IEnumerator ICreateName(string name)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(namePrefab);
        yield return handle;
        handles.Add(handle);

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject result = handle.Result;

            setResultToScroll(result);

            string nameBack = $"<color={NamesColors[name]}>{ShortNames[name]}</color>: ";
            string NameFront = $"<color={NamesColors[name]}>{ShortNames[name]}</color>: ";

            GameObject textBack = result.transform.GetChild(0).gameObject;
            GameObject textFront = textBack.transform.GetChild(0).gameObject;

            textBack.GetComponent<Text>().text = nameBack;
            textFront.GetComponent<Text>().text = NameFront;
        }
        else
        {
            Debug.Log("error");
        }

        yield return null;
    }
    IEnumerator ICreateSpace()
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(spacePrefab);
        yield return handle;
        handles.Add(handle);

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject result = handle.Result;

            setResultToScroll(result);
        }
        else
        {
            Debug.Log("error");
        }

        yield return null;
    }
    IEnumerator ICreateMessage(string message)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(messagePrefab);
        yield return handle;
        handles.Add(handle);

        if (handle.Status == AsyncOperationStatus.Succeeded)
        {
            GameObject result = handle.Result;

            setResultToScroll(result);

            handle.Result.transform.GetChild(0).GetComponent<Text>().text = message;
        }
        else
        {
            Debug.Log("error");
        }

        yield return null;
    }
    public IEnumerator IDelLog()
    {
        foreach (AsyncOperationHandle<GameObject> hand in handles)
        {
            Addressables.ReleaseInstance(hand);
        }

        foreach (Transform child in messageParent)
        {
            Destroy(child.gameObject, 0f);

        }

        handles = new List<AsyncOperationHandle<GameObject>>();

        Resources.UnloadUnusedAssets();
        yield return null;

    }
}