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

    public GameObject TextBox;

    private AsyncOperationHandle<GameObject> _options_handler;

    private GameObject result = null;

    private static string _optionsPrefabName = "OptionsBox";

    private float _fadeSpeed = 5f;

    private string _currentChoice;

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


    public void CreateChoice(ChoiceArr[] choices, string choiceName)
    {
        StartCoroutine(ICreateChoice(choices, choiceName));
    }

    private IEnumerator ICreateChoice(ChoiceArr[] choices, string choiceName)
    {
        DialogMod.denyNextDialog = true;

        _currentChoice = choiceName;

        _options_handler = Addressables.InstantiateAsync(_optionsPrefabName, gameObject.GetComponent<RectTransform>(), false, true);
        yield return _options_handler;

        if (_options_handler.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.Log("Error");
        }

        result = _options_handler.Result;

        // Пока работает только для 2х выборов, исправить в дальнейших эпизодах
        for (int i = 0; i < 2; i++)
        {
            ChoiceButton choiceButton = result.transform.GetChild(i).GetComponent<ChoiceButton>();
            choiceButton.SetOptions(choices[i].Message, choices[i].BlockName);
        }

        StartCoroutine(FadeManager.FadeObject(GameButtonsManager.instance.SkipButton, false, _fadeSpeed));
        StartCoroutine(FadeManager.FadeObject(GameButtonsManager.instance.HideButton, false, _fadeSpeed));
        StartCoroutine(FadeManager.FadeObject(GameButtonsManager.instance.LogButton, false, _fadeSpeed));
        StartCoroutine(FadeManager.FadeObject(GameButtonsManager.instance.ContinueGame, false, _fadeSpeed));

        StartCoroutine(FadeManager.FadeOnly(gameObject, true, _fadeSpeed));
        yield return StartCoroutine(FadeManager.FadeObject(TextBox, false, _fadeSpeed));
    }

    public IEnumerator LoadChoise(string blockName)
    {
        TextBoxController.instance.SetStoryText("");

        StartCoroutine(FadeManager.FadeObject(GameButtonsManager.instance.SkipButton, true, _fadeSpeed));
        StartCoroutine(FadeManager.FadeObject(GameButtonsManager.instance.HideButton, true, _fadeSpeed));
        StartCoroutine(FadeManager.FadeObject(GameButtonsManager.instance.LogButton, true, _fadeSpeed));
        StartCoroutine(FadeManager.FadeObject(GameButtonsManager.instance.ContinueGame, true, _fadeSpeed));

        StartCoroutine(FadeManager.FadeOnly(gameObject, false, _fadeSpeed));
        yield return StartCoroutine(FadeManager.FadeObject(TextBox, true, _fadeSpeed));

        if (_options_handler.IsValid())
        {
            Addressables.ReleaseInstance(_options_handler);
        }

        DialogMod.denyNextDialog = false;

        LoadBlock(blockName);
    }

    private void LoadBlock(string blockName)
    {
        string currentBlock = UserData.instance.CurrentBlock;
        Debug.Log("currentBlock: " + currentBlock);
        if (currentBlock != null)
        {
            Block activeBlock = PanelsManager.instance.flowchart.FindBlock(currentBlock);
            if (activeBlock != null)
            {
                activeBlock.Stop();
            }
        }

        Block targetBlock = PanelsManager.instance.flowchart.FindBlock(blockName);
        Debug.Log("targetBlock: " + blockName);
        UserData.instance.CurrentBlock = blockName;
        if (targetBlock != null)
        {
            targetBlock.Stop();
            PanelsManager.instance.flowchart.ExecuteBlock(targetBlock, 0);
        }
    }
}
