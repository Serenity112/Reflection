using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// ��������� ��� ���� ��������� ��������, ������ ������� ���� ���������� ��� ���������
public interface ISettingsOptions
{
    public abstract void UpdateVisuals();

    public abstract void InitialUpdateVisuals();
}
