using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Интерфейс для всех элементов настроек, визуал которых надо сбрасывать или обновлять
public interface ISettingsOptions
{
    public void UpdateVisuals();

    public void InitialUpdateVisuals();
}
