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
        RecoverOldText();

        yield return StartCoroutine(OpenChoice());
    }

    private void RecoverOldText()
    {
        Block curr_block = PanelsManager.instance.flowchart.ActiveBlock;
        var prev_command = curr_block.commandList[curr_block.GetCurrentIndex() - 1];
        if (prev_command.GetType() == typeof(FungusSayDialog))
        {
            FungusSayDialog say = (FungusSayDialog)prev_command;
            Typewriter.Instance.SetText(say.storyText);
        }
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
        yield return CoroutineUtils.WaitForAll(new List<IEnumerator>()
        {
            FadeManager.FadeObject(ChoiceBox, true, _fadeSpeed),
        });
    }

    public IEnumerator CloseChoice()
    {
        yield return CoroutineUtils.WaitForAll(new List<IEnumerator>()
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

    private void LoadBlock(string new_block)
    {
        Flowchart flowchart = PanelsManager.instance.flowchart;
        Block curr_block = flowchart.ActiveBlock;
        curr_block.Stop();

        Block target_block = flowchart.FindBlock(new_block);
        flowchart.ExecuteBlock(target_block, 0);
    }
}
