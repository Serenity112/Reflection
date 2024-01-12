using System.Collections.Generic;
using UnityEngine;
using System;
using Fungus;
using System.Collections;

public class ChoiceManager : MonoBehaviour
{
    public static ChoiceManager instance = null;

    public static bool CHOICE_IS_ACTIVE { get; set; } = false;

    private float _fadeSpeed = 5f;

    private Dictionary<string, int> SavedChoices = new Dictionary<string, int>();

    private GameObject ChoiceBox;

    private string CurrentCode = null;

    [Serializable]
    public struct ChoiceArr
    {
        public string Message;
        public string BlockName;
    }

    private void Awake()
    {
        instance = this;
        ChoiceBox = transform.GetChild(0).gameObject;
    }

    public IEnumerator CreateChoice(ChoiceArr[] choices, string choiceCode)
    {
        CHOICE_IS_ACTIVE = true;
        CurrentCode = choiceCode;

        // Пока работает только для 2х выборов, исправить в дальнейших эпизодах
        for (int i = 0; i < 2; i++)
        {
            ChoiceButton choiceButton = ChoiceBox.transform.GetChild(i).GetComponent<ChoiceButton>();
            choiceButton.Awake();
            choiceButton.SetOptions(choices[i].Message, choices[i].BlockName, i);
            choiceButton.ResetButtonState();
        }

        yield return StartCoroutine(OpenChoice());
    }

    public IEnumerator LoadChoise(string blockName, int number)
    {
        Typewriter.Instance.SetText("");

        yield return StartCoroutine(CloseChoice());

        if (SavedChoices.ContainsKey(CurrentCode))
        {
            SavedChoices[CurrentCode] = number;
        }
        else
        {
            SavedChoices.Add(CurrentCode, number);
        }
        LoadBlock(blockName);

        FadeManager.FadeObject(ChoiceBox, false);
        CHOICE_IS_ACTIVE = false;
    }

    private IEnumerator OpenChoice()
    {
        yield return CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            FadeManager.FadeObject(ChoiceBox, true, _fadeSpeed),
        });
    }

    public IEnumerator CloseChoice()
    {
        GameObject gameGui = PanelsManager.instance.GameGuiPanel;
        GameObject gameButtons = PanelsManager.instance.GameButtons;

        yield return CoroutineWaitForAll.instance.WaitForAll(new List<IEnumerator>()
        {
            FadeManager.FadeOnly(ChoiceBox, false, _fadeSpeed),
        });
    }

    // Для сейв системы
    public void HideChoiceBox()
    {
        FadeManager.FadeObject(ChoiceBox, false);
        CHOICE_IS_ACTIVE = false;
    }

    public void UploadChoices(Dictionary<string, int> choices)
    {
        SavedChoices = choices;
    }

    public Dictionary<string, int> GetSavedChoices()
    {
        return SavedChoices;
    }

    private void LoadBlock(string blockName)
    {
        Flowchart flowchart = PanelsManager.instance.flowchart;
        string currentBlock = UserData.instance.CurrentBlock;

        if (currentBlock != null)
        {
            Block activeBlock = flowchart.FindBlock(currentBlock);
            if (activeBlock != null)
            {
                activeBlock.Stop();
            }
        }

        Block targetBlock =flowchart.FindBlock(blockName);
        UserData.instance.CurrentBlock = blockName;
        flowchart.ExecuteBlock(targetBlock, 0);
    }
}
