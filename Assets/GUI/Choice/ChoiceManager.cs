using System.Collections.Generic;
using UnityEngine;
using System;
using Fungus;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.AddressableAssets;
using System.Collections;
using System.Linq;

public class ChoiceManager : MonoBehaviour
{
    public static ChoiceManager instance = null;

    public GameObject TextBox;

    private AsyncOperationHandle<GameObject> _options_handler;

    private static string _optionsPrefabName = "OptionsBox";

    private float _fadeSpeed = 5f;

    private ChoiceArr[] _currentChoices;

    private string _currentChoiceCode;

    private string _saveFileName = "SaveFiles.es3";

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
    }

    void Start()
    {

    }


    public void CreateChoice(ChoiceArr[] choices, string choiceCode)
    {
        StartCoroutine(ICreateChoice(choices, choiceCode));
    }

    private IEnumerator ICreateChoice(ChoiceArr[] choices, string choiceCode)
    {
        DialogMod.denyNextDialog = true;

        _currentChoices = choices;
        _currentChoiceCode = choiceCode;

        _options_handler = Addressables.InstantiateAsync(_optionsPrefabName, gameObject.GetComponent<RectTransform>(), false, true);
        yield return _options_handler;

        if (_options_handler.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Error");
        }

        GameObject result = _options_handler.Result;

        // ���� �������� ������ ��� 2� �������, ��������� � ���������� ��������
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
                ES3.Save<int>($"Choice_{_currentChoiceCode}", i, _saveFileName);
            }
        }

        LoadBlock(blockName);

        DialogMod.denyNextDialog = false;
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
}
