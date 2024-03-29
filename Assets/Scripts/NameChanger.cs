using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NameChanger : MonoBehaviour
{
    public static NameChanger instance = null;

    // ��������, ����� ����� �����������
    public Dictionary<Character, string> charactersLocalization = new()
    {
        {Character.Sergey, "������"},
        {Character.Pasha, "����"},

        {Character.Katya, "����"},
        {Character.Nastya, "�����"},
        {Character.Tanya, "����"},
        {Character.Evelina, "�������"},

        {Character.Raketnikov, "����������"},
        {Character.Tumanov, "�������"},

        {Character.Stranger, "����������"},
        {Character.Speakers, "�����������"},
        {Character.Student, "�������"},
        {Character.Zriteli, "�������"},
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
