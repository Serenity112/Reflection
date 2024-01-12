using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class LogManager : IButtonManager
{
    public static LogManager instance = null;

    public static bool LOG_PANEL_ACTIVE { get; set; } = false;
    public static bool ANIMATION_ENDED { get; set; } = false;

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
        if (counter > maxMessages)
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

        // Лимит вертексов
        try
        {
            text.text += stringBuilder.ToString();
        }
        catch
        {
            ClearLog();
        }

        _currentCharacter = character;
    }

    public void ClearLog()
    {
        text.text = "";
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
}
