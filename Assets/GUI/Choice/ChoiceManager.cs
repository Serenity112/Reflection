using System.Collections.Generic;
using UnityEngine;
using System;
using Fungus;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Collections;

public class ChoiceManager : MonoBehaviour
{
    public static ChoiceManager instance = null;

    [SerializeField]
    private GameObject TextBox;

    private AsyncOperationHandle<GameObject> _options_handler;

    private static string _optionsPrefabName = "OptionsBox";

    private float _fadeSpeed = 5f;

    private ChoiceArr[] _currentChoices;

    private string _currentChoiceCode;

    private string _savedChoicesName = "SavedChoices";
    private string _saveFileName = "SaveFiles.es3";

    private Dictionary<string, int> _saveFileChoices;

    [Serializable]
    public struct ChoiceArr
    {
        public string Message;
        public string BlockName;
    }

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

        _saveFileChoices = new Dictionary<string, int>();
    }

    public void CreateChoice(ChoiceArr[] choices, string choiceCode)
    {
        StartCoroutine(ICreateChoice(choices, choiceCode));
    }

    private IEnumerator ICreateChoice(ChoiceArr[] choices, string choiceCode)
    {
       Typewriter.Instance.denyNextDialog = true;

        _currentChoices = choices;
        _currentChoiceCode = choiceCode;

        _options_handler = Addressables.InstantiateAsync(_optionsPrefabName, gameObject.GetComponent<RectTransform>(), false, true);
        yield return _options_handler;

        if (_options_handler.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Error");
        }

        GameObject result = _options_handler.Result;

        // Пока работает только для 2х выборов, исправить в дальнейших эпизодах
        for (int i = 0; i < 2; i++)
        {
            ChoiceButton choiceButton = result.transform.GetChild(i).GetComponent<ChoiceButton>();
            choiceButton.SetOptions(choices[i].Message, choices[i].BlockName);
        }

        yield return StartCoroutine(ShowOptionsBox(_fadeSpeed));
    }

    public IEnumerator LoadChoise(string blockName)
    {
        TextBoxController.instance.SetStoryText("");

        yield return StartCoroutine(HideOptionsBox(_fadeSpeed));
        ReleaseChoiceBox();

        for (int i = 0; i < _currentChoices.Length; i++)
        {
            if (_currentChoices[i].BlockName == blockName)
            {
                if (_saveFileChoices.ContainsKey(blockName))
                {
                    _saveFileChoices[_currentChoiceCode] = i;
                }
                else
                {
                    // Пофиксить )))
                    //_saveFileChoices.Add(_currentChoiceCode, i);
                }
            }
        }

        LoadBlock(blockName);

       Typewriter.Instance.denyNextDialog = false;
    }

    public IEnumerator ShowOptionsBox(float speed)
    {
        yield return CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            GameButtonsManager.instance.HideTextBoxButtons(speed),
            FadeManager.FadeOnly(gameObject, true, speed),
            FadeManager.FadeObject(TextBox, false, speed),
        });
    }

    public IEnumerator HideOptionsBox(float speed)
    {
        yield return CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            GameButtonsManager.instance.ShowTextBoxButtons(speed),
            FadeManager.FadeOnly(gameObject, false, speed),
            FadeManager.FadeObject(TextBox, true, speed)
        });
    }

    public void ReleaseChoiceBox()
    {
        if (_options_handler.IsValid())
        {
            Addressables.ReleaseInstance(_options_handler);
        }
    }

    private void LoadBlock(string blockName)
    {
        string currentBlock = UserData.instance.CurrentBlock;
        if (currentBlock != null)
        {
            Block activeBlock = PanelsManager.instance.flowchart.FindBlock(currentBlock);
            if (activeBlock != null)
            {
                activeBlock.Stop();
            }
        }

        Block targetBlock = PanelsManager.instance.flowchart.FindBlock(blockName);
        UserData.instance.CurrentBlock = blockName;
        if (targetBlock != null)
        {
            targetBlock.Stop();
            PanelsManager.instance.flowchart.ExecuteBlock(targetBlock, 0);
        }
    }

    public void SaveChoices(int saveNum)
    {
        string savedChoicesName = $"{_savedChoicesName}{saveNum}";
        if (ES3.FileExists(_saveFileName))
        {
            ES3.Save<Dictionary<string, int>>(savedChoicesName, _saveFileChoices, _saveFileName);
        }
    }

    public void LoadSavedChoices(int saveNum)
    {
        string savedChoicesName = $"{_savedChoicesName}{saveNum}";
        if (ES3.FileExists(_saveFileName) && ES3.KeyExists(savedChoicesName, _saveFileName))
        {
            _saveFileChoices = ES3.Load<Dictionary<string, int>>(savedChoicesName, _saveFileName);
        }
    }
}
