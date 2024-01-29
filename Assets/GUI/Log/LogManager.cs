using Fungus;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : IButtonManager
{
    public static LogManager instance = null;

    public static bool LOG_PANEL_ACTIVE { get; set; } = false;
    public static bool LOG_PANEL_ACTIVE_POST { get; set; } = false;

    private Dictionary<Character, string> NamesColors;
    private string DefaultColor = "#C8C8C8";

    private int counter = 0;
    private int maxMessages = 300;

    public LogButton logOpenButton;
    public LogBack logBackButton;

    [HideInInspector]
    public GameObject LogPanel;

    public GameObject Content;
    public Slider Slider;

    // Изменнить, когда будет >1 текста
    private Text text;

    private Character _currentCharacter = Character.None;

    private void Awake()
    {
        instance = this;

        LogPanel = transform.GetChild(0).gameObject;

        NamesColors = new Dictionary<Character, string>
        {
            { Character.Pasha, "#D35400" }, //orange
            { Character.Katya, "#E74C3C" }, //red
            { Character.Nastya, "#2ECC71" }, //green
            { Character.Evelina, "#7D3C98" }, //magenta
            { Character.Tanya, "#2980B9" }, //blue
            { Character.Sergey, "#7D3C98" } //blue
        };

        GameObject component = Content.transform.GetChild(0).gameObject;
        text = component.transform.GetChild(0).GetComponent<Text>();
    }

    public void CreateMessage(Character character, string message)
    {
        counter++;
        if (counter > maxMessages || text.text.Length > 16000)
        {
            ClearLog();
        }

        StringBuilder stringBuilder = new StringBuilder();
        if (character != _currentCharacter)
        {
            stringBuilder.Append("\n");

            if (character != Character.None)
            {
                string characterLocalizedName = NameChanger.instance.charactersLocalization[character];
                string color = DefaultColor;

                if (NamesColors.ContainsKey(character))
                {
                    color = NamesColors[character];
                }

                stringBuilder.Append($"<color={color}>{characterLocalizedName}</color>\n");
            }
        }

        stringBuilder.Append($"{message}\n");

        text.text += stringBuilder.ToString();

        _currentCharacter = character;
    }

    public void ClearLog()
    {
        counter = 0;
        text.text = "\n";
    }

    public void SetSliderToEnd()
    {
        Slider.value = 1;
        Slider.onValueChanged.Invoke(1);
    }

    public override void ResetManager()
    {
        ResetAllButtonsState();
        SetSliderToEnd();
    }

    public override void ResetAllButtonsState()
    {
        foreach (var button in GameButtons)
        {
            button.ResetButtonState();
        }
    }

    public void RestoreLogByBlock(Block block, int targetIndex)
    {
        int restoreCount = 100;
        int startIndex = targetIndex - restoreCount > 0 ? targetIndex - restoreCount : 0;

        try
        {
            for (int i = startIndex; i < targetIndex; i++)
            {
                var command = block.commandList[i];
                if (command.GetType() == typeof(FungusSayDialog))
                {
                    FungusSayDialog say = (FungusSayDialog)command;
                    bool parsed = Enum.TryParse<Character>(say.speaker, true, out Character character);
                    Character character_input = parsed ? character : Character.None;

                    string story_text = say.storyText;

                    CreateMessage(character_input, story_text);
                }
            }
        }
        catch (Exception e)
        {
            Debug.Log(e.Message);
        }
    }
}
