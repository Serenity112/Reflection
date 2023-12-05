using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��������� ����� ������ ����� ��� ����� �������� � �������� ������
public abstract class IButtonManager : MonoBehaviour
{
    public bool ButtonSelected = false;

    public List<GameObject> GameButtons = new List<GameObject>();

    public void SubscribeButton(GameObject button)
    {
        GameButtons.Add(button);
    }

    /// <summary>
    /// �������� ��������� ���� ������
    /// </summary>
    public abstract void ResetAllButtonsState();

    /// <summary>
    /// �������� ����������
    /// </summary>
    public abstract void EnableButtons();

    /// <summary>
    /// ��������� ����������
    /// </summary>
    public abstract void DisableButtons();

    /// <summary>
    /// �������� �� �� ���������
    /// </summary>
    public abstract void ResetManager();
}
