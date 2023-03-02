using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ISettingsManager
{
    void OpenSettings();
    void CloseSettings();
    void ApplySetting(Settings setting, SettingsOptions value, float data);
}