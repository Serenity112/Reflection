using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// ������� ������ - �����, ����, ����, ���
public class GameButtonsManager : IButtonManager
{
    public static GameButtonsManager instance = null;

    public void Awake()
    {
        instance = this;
    }

    // ���� ��� ������� ������� �� ������
    public bool BlockButtonsClick { get; set; } = false;

    public override void ResetAllButtonsState()
    {
        foreach (var button in GameButtons)
        {
            button.ResetButtonState();
        }
    }

    public override void ResetManager()
    {
        BlockButtonsClick = false;
        ResetAllButtonsState();
    }
}
