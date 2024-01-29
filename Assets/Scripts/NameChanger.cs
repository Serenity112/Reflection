using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameChanger : MonoBehaviour
{
    public static NameChanger instance = null;

    // Заменить, когда будет локализация
    public Dictionary<Character, string> charactersLocalization = new()
    {
        {Character.Sergey, "Сергей"},
        {Character.Pasha, "Паша"},

        {Character.Katya, "Катя"},
        {Character.Nastya, "Настя"},
        {Character.Tanya, "Таня"},
        {Character.Evelina, "Эвелина"},

        {Character.Raketnikov, "Ракетников"},
        {Character.Tumanov, "Туманов"},

        {Character.Stranger, "Незнакомка"},
        {Character.Speakers, "Выступающие"},
        {Character.Student, "Студент"},
        {Character.Zriteli, "Зрители"},
    };

    private void Awake()
    {
        instance = this;
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
        gameObject.transform.GetChild(0).GetComponent<Text>().text = text;
        gameObject.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = text;
    }
}
