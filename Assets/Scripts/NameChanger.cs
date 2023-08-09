using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameChanger : MonoBehaviour
{
    public static NameChanger instance = null;

    private Dictionary<string, string> _localizedNames = new Dictionary<string, string>();

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

        // Временное решение. При добавлении локализации изменить
        _localizedNames.Add("Katya", "Катя");
        _localizedNames.Add("Nastya", "Настя");
        _localizedNames.Add("Tanya", "Таня");
        _localizedNames.Add("Eveline", "Эвелина");
        _localizedNames.Add("Pasha", "Паша");
        _localizedNames.Add("Tumanov", "Туманов");
        _localizedNames.Add("Raketnikov", "Ракетников");
        _localizedNames.Add("Sergey", "Сергей");
        _localizedNames.Add("Stranger", "Незнакомка");
        _localizedNames.Add("Speakers", "Выступающие");
        _localizedNames.Add("Students", "Студенты");
    }

    public void SetName(string name)
    {
        if (name == null || !_localizedNames.ContainsKey(name))
        {
            SetNameText("");
        }
        else
        {
            SetNameText(_localizedNames[name]);
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
