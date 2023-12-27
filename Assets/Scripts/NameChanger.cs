using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameChanger : MonoBehaviour
{
    public static NameChanger instance = null;

    private Dictionary<Character, string> charactersLocalization = new()
    {
        {Character.Sergey, "������"},
        {Character.Pasha, "����"},

        {Character.Katya, "����"},
        {Character.Nastya, "�����"},
        {Character.Tanya, "����"},
        {Character.Evelina, "�������"},

        {Character.Raketnikov, "����������"},
        {Character.Tumanov, "�������"},

        {Character.Neznakomka, "����������"},
        {Character.Stranger, "����������"},
        {Character.Speakers, "�����������"},
        {Character.Students, "��������"},
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
