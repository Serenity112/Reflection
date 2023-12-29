using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Игровые кнопки - пауза, хайд, скип, лог
public class GameButtonsManager : IButtonManager
{
    public static GameButtonsManager instance = null;

    public void Awake()
    {
        instance = this;
    }

    // Флаг для запрета нажатия на кнопки
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
