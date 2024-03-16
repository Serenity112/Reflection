using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Интерфейс для всех элементов настроек, визуал которых надо сбрасывать или обновлять
public interface ISettingsOptions
{
    public abstract void UpdateVisuals();

    public abstract void InitialUpdateVisuals();
}
