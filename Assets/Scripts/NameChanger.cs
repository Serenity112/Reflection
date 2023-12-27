using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameChanger : MonoBehaviour
{
    public static NameChanger instance = null;

    private Dictionary<Character, string> charactersLocalization = new()
    {
        {Character.Sergey, "Сергей"},
        {Character.Pasha, "Паша"},

        {Character.Katya, "Катя"},
        {Character.Nastya, "Настя"},
        {Character.Tanya, "Таня"},
        {Character.Evelina, "Эвелина"},

        {Character.Raketnikov, "Ракетников"},
        {Character.Tumanov, "Туманов"},

        {Character.Neznakomka, "Незнакомка"},
        {Character.Stranger, "Незнакомка"},
        {Character.Speakers, "Выступающие"},
        {Character.Students, "Студенты"},
    };

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

    public void SetName(Character name)
    {
        if (!charactersLocalization.ContainsKey(name))
        {
            SetNameText("");
        }
        else
        {
            SetNameText(charactersLocalization[name]);
        }
    }

    private void SetNameText(string text)
    {
        foreach (Transform child in gameObject.transform)
        {
            child.gameObject.GetComponent<Text>().text = text;
        }
    }
}
