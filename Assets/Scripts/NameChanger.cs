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

        // ��������� �������. ��� ���������� ����������� ��������
        _localizedNames.Add("Katya", "����");
        _localizedNames.Add("Nastya", "�����");
        _localizedNames.Add("Tanya", "����");
        _localizedNames.Add("Eveline", "�������");
        _localizedNames.Add("Pasha", "����");
        _localizedNames.Add("Tumanov", "�������");
        _localizedNames.Add("Raketnikov", "����������");
        _localizedNames.Add("Sergey", "������");
        _localizedNames.Add("Stranger", "����������");
        _localizedNames.Add("Speakers", "�����������");
        _localizedNames.Add("Students", "��������");
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
