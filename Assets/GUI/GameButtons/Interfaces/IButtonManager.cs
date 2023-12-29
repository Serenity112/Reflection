using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ��������� ����� ������ ����� ��� ����� �������� � �������� ������
public abstract class IButtonManager : MonoBehaviour
{
    protected List<IDraggableButton> GameButtons = new List<IDraggableButton>();

    public void SubscribeButton(IDraggableButton button)
    {
        GameButtons.Add(button);
    }

    /// <summary>
    /// �������� ��������� ���� ������
    /// </summary>
    public abstract void ResetAllButtonsState();

    /// <summary>
    /// �������� �� �� ���������
    /// </summary>
    public abstract void ResetManager();
}
